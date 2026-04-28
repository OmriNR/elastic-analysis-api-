using Elastic.Clients.Elasticsearch;
using Listener;

var builder = Host.CreateApplicationBuilder(args);

var elasticUrl = builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200";
builder.Services.AddSingleton(new ElasticsearchClient(new ElasticsearchClientSettings(new Uri(elasticUrl))));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
