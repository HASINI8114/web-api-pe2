using Asp.Versioning;
using GDB.Api.Application.Services;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using GDB.Api.Common.Constants;
using GDB.Api.Common.ExceptionHandling;
using GDB.Api.Common.Extensions;
using GDB.Api.Data;
using GDB.Api.Domain;
using GDB.Api.Infrastructure.Repositories;
using GDB.Api.Infrastructure.Repositories.Contracts;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddAppCors(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = ApiVersionParser.Default.Parse(ApiConstants.Version1);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddMvc()
.AddApiExplorer(options =>
{
    // Groups endpoints as "v1", "v2" and fills in {version} in the OpenAPI paths
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// One document per API version: /openapi/v1.json, /openapi/v2.json
builder.Services.AddOpenApi("v1");
builder.Services.AddOpenApi("v2");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IGDBInMemoryDB, GDBInMemoryDB>();

builder.Services.AddSingleton<IGDBInMemoryDataStore, GDBInMemoryDataStore>();

// Register AccountService
builder.Services.AddScoped<IAccountService, AccountService>();

// Register AccountFactory
builder.Services.AddScoped<IAccountFactory, AccountFactory>();

// Register AccountRepositoryFactory
builder.Services.AddScoped<IAccountRepositoryFactory, AccountRepositoryFactory>();

// Register TransactionService
builder.Services.AddScoped<ITransactionService, TransactionService>();

// Register TransactionRepositoryFactory
builder.Services.AddScoped<ITransactionRepositoryFactory, TransactionRepositoryFactory>();

// Register TransactionCommandFactory
builder.Services.AddScoped<ITransactionCommandFactory, TransactionCommandFactory>();

// Register DataBaseConnectionManager
builder.Services.AddScoped<IDataBaseConnectionManager, DataBaseConnectionManager>();

//DataBaseConnectionManager.Initialize(builder.Configuration);

var app = builder.Build();

DataBaseProviderRegistration.Register(
    app.Configuration,
    app.Services.GetRequiredService<ILogger<DataBaseProviderRegistration>>());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseRequestResponseLogging();

app.UseExceptionHandler();

app.UseCors(CorsExtensions.DefaultPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();