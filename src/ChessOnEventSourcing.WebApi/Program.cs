using ChessOnEventSourcing.Application;
using ChessOnEventSourcing.Domain;
using ChessOnEventSourcing.EventStore;
using ChessOnEventSourcing.EventStore.Repositories;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDbConnectionFactory>(_ =>
    new NpgsqlConnectionFactory(builder.Configuration.GetConnectionString("Database")!));
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
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/chessboards", async ([FromServices] ChessboardService chessboardService) =>
{
    await chessboardService.CreateChessboard();
    return Results.Ok();
})
.WithOpenApi();

app.MapGet("/chessboards/{id:guid}", async (Guid id, [FromServices] IChessboardRepository chessboards) =>
{
    var chessboard = await chessboards.GetBy(id);
    return Results.Ok(chessboard);
})
.WithOpenApi();

await app.RunAsync();