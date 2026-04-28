using Serilog;
using Serilog.Sinks.Elasticsearch;
using Worker;

var builder = Host.CreateApplicationBuilder(args);

var elasticsearchUrl = builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "orders-worker-logs-{0:yyyy.MM.dd}",
        NumberOfReplicas = 1,
        NumberOfShards = 2
    }).CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
