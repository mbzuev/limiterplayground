using Prometheus;
using LimiterService.LimiterStuff;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DefaultPrometheusMetricsReporter>();
builder.Services.AddSingleton<IStorageMetrics, DefaultPrometheusMetricsReporter>();
builder.Services.AddSingleton<IConcurrencyLimiterFactory, ConcurrencyLimiterFactory>();
builder.Services.AddSingleton<IConcurrencyLimitCalculator, AimdConcurrencyLimitCalculator>();
builder.Services.AddOptions<ConcurrencyLimitSettings>()
    .Bind(builder.Configuration.GetSection("StorageConcurrencyLimit"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.Configure<AimdConcurrencyLimitSettings>(builder.Configuration.GetSection("StorageConcurrencyLimit"));

var app = builder.Build();
app.UseHttpMetrics();
app.UseMetricServer(settings => settings.EnableOpenMetrics = false);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();