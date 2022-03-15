using System.Net;
using echo_api.Models;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddDbContext<EchoHistoryDb>(opt => opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseCors("CorsPolicy");

using (var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<EchoHistoryDb>())
{
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.MapGet("/echo/{message}", (string message) =>
// {
//     string logMessage = string.Format("Echoing message: {0}", message);
//     app.Logger.LogInformation(logMessage);
//     return Results.Ok(message);
// })
// .WithName("Echo");

app.MapGet("/echo/{message}", (string message, EchoHistoryDb db) =>
{
    string logMessage = string.Format("Echoing message: {0}", message);
    app.Logger.LogInformation(logMessage);
    db.EchoLogs.Add(new EchoHistory { Message = logMessage });
    db.SaveChanges();
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

app.MapGet("/logs", (EchoHistoryDb db) =>
{
    return db.EchoLogs.ToList();
})
.WithName("GetLog");

app.Run();