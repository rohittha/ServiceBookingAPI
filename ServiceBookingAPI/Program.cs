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

// 1. ADD CORS SERVICE
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization(); app.UseCors("AllowAll");

app.MapControllers();

app.Run();

