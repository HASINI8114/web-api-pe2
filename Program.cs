using Asp.Versioning;
using GDB.Api.Application.Services;
using GDB.Api.Application.Services.Contracts;
using GDB.Api.Application.Services.Implementations;
using GDB.Api.Common.Constants;
using GDB.Api.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

DataBaseProviderRegistration.Register(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddScoped<IAccountService>(_ => AccountServiceFactory.Create());

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