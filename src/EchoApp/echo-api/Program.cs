using System.Net;
using Microsoft.Extensions.Logging.Console;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddSimpleConsole(opts =>
    {
        opts.IncludeScopes = false;
        opts.TimestampFormat = "[yyyy-MM-dd HH:mm:ss] ";
        opts.ColorBehavior = LoggerColorBehavior.Disabled;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/echo/{message}", (string message) =>
{
    app.Logger.LogInformation("Echoing message: {Message}", message);
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