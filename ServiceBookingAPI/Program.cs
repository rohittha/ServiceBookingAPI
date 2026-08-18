using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Resources;
using ServiceBooking.Application;
using ServiceBooking.Infrastructure;
using ServiceBooking.WebUI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(); // (Assuming you have an Application/DependencyInjection.cs)
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//// Add OpenTelemetry and configure it to use Azure Monitor.
//builder.Services.AddOpenTelemetry().UseAzureMonitor();
//// Setting role name and role instance

//// Create a dictionary of resource attributes.
//var resourceAttributes = new Dictionary<string, object> {
//    { "service.name", "servicebooking" },
//    { "service.namespace", "servicebooking.api" },
//    { "service.instance.id", "servicebooking" }};

//// Add the OpenTelemetry telemetry service to the application.
//// This service will collect and send telemetry data to Azure Monitor.
//builder.Services.AddOpenTelemetry()
//    .UseAzureMonitor()
//    // Configure the ResourceBuilder to add the custom resource attributes to all signals.
//    // Custom resource attributes should be added AFTER AzureMonitor to override the default ResourceDetectors.
//    .ConfigureResource(resourceBuilder => resourceBuilder.AddAttributes(resourceAttributes));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

