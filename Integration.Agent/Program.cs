using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

// Reuse your IT project classes via the project reference:
using FoodDelivery_IT.OT_System.Data;      // AppDbContext
using FoodDelivery_IT.OT_System.Models;    // OrderModel
using FoodDelivery_IT.OT_System.Enum;    // OrderStatus


// Our tiny OT HTTP client
using Integration.Agent;


class Program
{
    static async Task Main(string[] args)
    {
        // 1) Read configuration (same file name that AppDbContext expects)
        //    Make sure appsettings.json in Integration.Agent is set to "Copy if newer".
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var otBaseUrl = config["OtBaseUrl"] ?? "http://localhost:5189";
        var apiKey = config["Agent:ApiKey"] ?? "";
        var polls = int.TryParse(config["Agent:PollIntervalSeconds"], out var p) ? p : 3;
        var timeout = int.TryParse(config["Agent:RequestTimeoutSeconds"], out var t) ? t : 5;

        // 2) Build a single HttpClient + our wrapper that talks to OT
        var http = new HttpClient();
        var ot = new OtClient(http, otBaseUrl, apiKey, timeout);

        Console.WriteLine("=== Integration.Agent started ===");
        Console.WriteLine($"OT: {otBaseUrl} | Poll interval: {polls}s | Timeout: {timeout}s");
        Console.WriteLine("This agent will: find new IT orders, POST to OT, poll status, update IT.\n");

        // 3) Main loop: forever do small batches
        while (true)
        {
            try
            {
                using var db = new AppDbContext(); // uses IT appsettings.json (same filename)

                // 3a) Find unsent orders (SentToOT == false)
                var unsent = await db.Orders
                    .AsNoTracking()
                    .Where(o => o.SentToOT == false)
                    .OrderBy(o => o.OrderId)
                    .Take(10)
                    .ToListAsync();

                if (unsent.Count == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(polls));
                    continue;
                }

                Console.WriteLine($"Found {unsent.Count} order(s) to send to OT.");

                // 3b) Send each unsent order to OT
                foreach (var o in unsent)
                {
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout + 5));

                    var sentOk = await ot.SendAsync(o.OrderId, o.CustomerName, o.DishName, o.Quantity, cts.Token);
                    if (!sentOk)
                    {
                        Console.WriteLine($"WARN: Failed to send order #{o.OrderId} to OT. Will retry later.");
                        continue; // leave SentToOT=false so we retry next loop
                    }

                    // 3c) Mark IT order as sent-to-OT and Processing (stub update: no SELECT needed)
                    var markProcessing = new OrderModel { OrderId = o.OrderId, SentToOT = true, OrderStatus = OrderStatus.Processing };
                    db.Attach(markProcessing);
                    db.Entry(markProcessing).Property(x => x.SentToOT).IsModified = true;
                    db.Entry(markProcessing).Property(x => x.OrderStatus).IsModified = true;
                    await db.SaveChangesAsync(cts.Token);

                    Console.WriteLine($"Sent order #{o.OrderId} → OT accepted (Processing).");

                    // 3d) Poll OT for status change to "Sent" (bounded attempts)
                    for (var attempt = 0; attempt < 10; attempt++)
                    {
                        await Task.Delay(1000, cts.Token); // wait 1s between polls
                        var status = await ot.GetStatusAsync(o.OrderId, cts.Token);

                        if (status == "Sent")
                        {
                            var done = new OrderModel { OrderId = o.OrderId, OrderStatus = OrderStatus.Sent };
                            db.Attach(done);
                            db.Entry(done).Property(x => x.OrderStatus).IsModified = true;
                            await db.SaveChangesAsync(cts.Token);

                            Console.WriteLine($"Order #{o.OrderId} → Sent (updated in IT).");
                            break; // stop polling this order
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 3e) Never crash the agent—log and retry next tick.
                Console.WriteLine($"ERROR(loop): {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromSeconds(polls));
        }
    }
}


