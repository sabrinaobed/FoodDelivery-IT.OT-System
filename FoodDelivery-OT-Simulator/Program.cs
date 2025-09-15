using FoodDelivery_OT_Simulator.Endpoint;
using FoodDelivery_OT_Simulator.Interface;
using FoodDelivery_OT_Simulator.Service;

var builder = WebApplication.CreateBuilder(args);

// --- QUIETER LOGGING ---
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;       // one line per log
    options.TimestampFormat = "HH:mm:ss ";
});
builder.Logging.SetMinimumLevel(LogLevel.Information);
// Hide most framework chatter:
builder.Logging.AddFilter("Microsoft", LogLevel.Warning);
builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Warning);
builder.Logging.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);

// (optional) allow calls from your IT console while testing
builder.Services.AddCors(p =>
    p.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

// DI: your “machine” implementation
builder.Services.AddSingleton<IOrderProcessor, InMemoryOrderProcessor>();

var app = builder.Build();
app.UseCors();

// set a fixed port if you like (otherwise a random one will be chosen)
app.Urls.Add("http://localhost:5189");

// map your endpoints
app.MapOtEndpoints();

// Clean, single-line startup message
Console.WriteLine("OT (Kitchen) is running at http://localhost:5189 — ready to receive orders.");
app.Run();
