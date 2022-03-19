using System.Net;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddSimpleConsole(opts =>
    {
        opts.IncludeScopes = false;
        opts.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        opts.ColorBehavior = LoggerColorBehavior.Disabled;
    });

builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
    });

var app = builder.Build();

app.UseCors("CorsPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/echo/{message}", (string message) =>
{
    string logMessage = string.Format("Echoing message: {0}", message);
    app.Logger.LogInformation(logMessage);
    return Results.Ok(message);
})
.WithName("Echo");

app.MapGet("/hostname", () =>
{
    var hostname = Dns.GetHostName();
    app.Logger.LogInformation("Getting hostname: {Hostname}", hostname);
    return Results.Ok(hostname);
})
.WithName("Hostname");

app.Run();