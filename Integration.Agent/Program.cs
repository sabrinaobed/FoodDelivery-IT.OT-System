using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

// Reuse IT project classes via the project reference:
using FoodDelivery_IT.OT_System.Data;      // AppDbContext
using FoodDelivery_IT.OT_System.Models;    // OrderModel
using FoodDelivery_IT.OT_System.Enum;      // OrderStatus

// Our tiny OT HTTP client (in this project)
using Integration.Agent;

class Program
{
    static async Task Main(string[] args)
    {
        // 1) Read configuration from this project's appsettings.json
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        // Agent settings with safe fallbacks
        var otBaseUrl = config["OtBaseUrl"] ?? "http://localhost:5189";
        var apiKey = config["Agent:ApiKey"] ?? "";
        var polls = int.TryParse(config["Agent:PollIntervalSeconds"], out var p) ? p : 3;
        var timeout = int.TryParse(config["Agent:RequestTimeoutSeconds"], out var t) ? t : 5;

        // 2) Build one HttpClient and wrap it in our tiny OT client
        var http = new HttpClient();
        var ot = new OtClient(http, otBaseUrl, apiKey, timeout);

        // Friendlier startup messages
        Console.WriteLine("=== Integration Agent is now running ===");
        Console.WriteLine($"Connected to OT system at: {otBaseUrl}");
        Console.WriteLine($"I’ll check for new orders every {polls} second(s). (Request timeout: {timeout}s)");
        Console.WriteLine("Process: Look for new IT orders → send them to OT → wait for status updates → update IT.\n");

        // 3) Worker loop: do small batches forever
        while (true)
        {
            try
            {
                using var db = new AppDbContext();

                // 3a) Find unsent orders (SentToOT == false)
                var unsent = await db.Orders
                    .Where(o => o.SentToOT == false)
                    .OrderBy(o => o.OrderId)
                    .Take(10)
                    .ToListAsync();

                if (unsent.Count == 0)
                {
                    Console.WriteLine("No new orders found. Waiting...");// did changes here
                    await Task.Delay(TimeSpan.FromSeconds(polls));
                    continue;
                }

                Console.WriteLine($"Found {unsent.Count} new order(s) to send to OT.");

                // 3b) For each unsent order: send → mark Processing → poll for Sent
                foreach (var o in unsent)
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout + 5));

                    // ---- Send to OT ----
                    var sentOk = await ot.SendAsync(o.OrderId, o.CustomerName, o.DishName, o.Quantity, cts.Token);
                    if (!sentOk)
                    {
                        Console.WriteLine($"Could not send order #{o.OrderId} to OT. Will try again later.");
                        continue; // keep SentToOT=false so we retry
                    }

                    // ---- Update IT: mark as sent-to-OT + Processing ----
                    o.SentToOT = true;
                    o.OrderStatus = OrderStatus.Processing;
                    await db.SaveChangesAsync(cts.Token);

                    Console.WriteLine($"Order #{o.OrderId} sent to OT. Status in IT: Processing.");

                    // 3d) Poll OT for status change to "Sent"
                    for (var attempt = 0; attempt < 10; attempt++)
                    {
                        await Task.Delay(1000, cts.Token); // ~1s between polls
                        var status = await ot.GetStatusAsync(o.OrderId, cts.Token);

                        if (status == "Sent")
                        {
                            o.OrderStatus = OrderStatus.Sent;
                            await db.SaveChangesAsync(cts.Token);

                            Console.WriteLine($"Order #{o.OrderId} is now Sent (updated in IT).");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // More human-friendly error message
                Console.WriteLine($"Something went wrong in this cycle: {ex.Message}");
            }

            // Sleep between scans to avoid busy-waiting
            await Task.Delay(TimeSpan.FromSeconds(polls));
        }
    }
}
