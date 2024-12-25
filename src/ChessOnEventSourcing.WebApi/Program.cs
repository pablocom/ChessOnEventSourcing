using ChessOnEventSourcing.Application;
using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore;
using ChessOnEventSourcing.EventStore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddScoped<IEventStore, NpgsqlEventStore>();
builder.Services.AddScoped<NpgsqlUnitOfWork>();
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<NpgsqlUnitOfWork>());
builder.Services.AddScoped<IDbTransactionProvider>(sp => sp.GetRequiredService<NpgsqlUnitOfWork>());
builder.Services.AddScoped<IChessboardRepository, ChessboardRepository>();
builder.Services.AddScoped<IEventStore, NpgsqlEventStore>();
builder.Services.AddScoped<ChessboardService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost("/chessboards/{id:guid}", async (Guid id, [FromServices] ChessboardService chessboardService) =>
{
    await chessboardService.CreateChessboard(id);
    return Results.Ok();
});

app.MapGet("/chessboards/{id:guid}", async (Guid id, [FromServices] IChessboardRepository chessboards) =>
{
    var chessboard = await chessboards.GetBy(id);
    return Results.Ok(chessboard);
});

app.MapPost("/chessboards/{id:guid}/move", async (Guid id, [FromServices] ChessboardService chessboardService) =>
{
    await chessboardService.Move(id);
    return Results.Ok();
});

await app.RunAsync();