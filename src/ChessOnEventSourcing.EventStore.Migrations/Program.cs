using ChessOnEventSourcing.EventStore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddTransient<DbConnectionFactory>();
builder.Services.AddTransient<EventStoreDbMigrator>();
builder.Services.AddHostedService<EventStoreDbMigratorHostedService>();

var app = builder.Build();

await app.RunAsync();
