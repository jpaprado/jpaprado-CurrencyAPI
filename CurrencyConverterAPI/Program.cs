using CurrencyConverterAPI;
using CurrencyConverterAPI.Models;
using CurrencyConverterAPI.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<CurrentDatabaseSettings>(
    builder.Configuration.GetSection("CurrenciesDatabase"));

builder.Services.AddSingleton<DataBase>();
builder.Services.AddSingleton<CurrencyService>();
builder.Services.AddSingleton<ExchangeRatesApiService>();

builder.Services.AddControllers();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Currency Converter API", Version = "v1" });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Converter API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
