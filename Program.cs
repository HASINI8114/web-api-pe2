using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using GDB.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// Register TransactionService
builder.Services.AddScoped<ITransactionService, TransactionService>();

DataBaseConnectionManager.Initialize(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();