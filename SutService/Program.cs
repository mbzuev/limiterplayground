using Prometheus;
using SutService.LimiterStuff;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<DefaultPrometheusMetricsReporter>();
builder.Services.AddSingleton<IStorageMetrics, DefaultPrometheusMetricsReporter>();
builder.Services.AddSingleton<IConcurrencyLimiterFactory, ConcurrencyLimiterFactory>();
builder.Services.AddSingleton<IConcurrencyLimitCalculator, AimdConcurrencyLimitCalculator>();
builder.Services.AddOptions<ConcurrencyLimitSettings>()
    .Bind(builder.Configuration.GetSection("RecoStorageConcurrencyLimit"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.Configure<AimdConcurrencyLimitSettings>(builder.Configuration.GetSection("RecoStorageConcurrencyLimit"));

var app = builder.Build();
app.UseHttpMetrics(); // should be registered before ErrorHandling to proper error codes in metrics
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