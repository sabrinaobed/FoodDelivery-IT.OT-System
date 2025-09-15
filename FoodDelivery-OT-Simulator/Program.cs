
using FoodDelivery_OT_Simulator.Endpoint;
using FoodDelivery_OT_Simulator.Interface;
using FoodDelivery_OT_Simulator.Service;

var builder = WebApplication.CreateBuilder(args);

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

Console.WriteLine("Kitchen System (OT) running at http://localhost:5189");
app.Run();
