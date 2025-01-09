using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Application.Services;
using UniversityLMS.Infrastructure.DbContexts;
using UniversityLMS.Infrastructure.Repositories;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Configure BlobServiceClient
        string blobConnectionString = context.Configuration.GetConnectionString("AzureBlobStorage");
        services.AddSingleton(new BlobServiceClient(blobConnectionString));

        // Configure QueueClient
        string queueConnectionString = context.Configuration.GetConnectionString("AzureQueueStorage");
        string queueName = context.Configuration["QueueStorage:QueueName"];
        services.AddSingleton(new QueueClient(queueConnectionString, queueName));
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IJudgeService, Judge0Service>();
        services.AddSingleton<IPlagiarism, PlagiarismService>();
        services.AddSingleton<IBlobStorageService, BlobStorageService>();
        services.AddSingleton<IUnitofWorkRepository, UnitofWorkRepository>();
        services.AddSingleton<IOutboxMessageRepository, OutboxMessageRepository>();

        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer("Server=QB-NOI-1120;Database=UniversityProjectDbOutboxVersion;Trusted_Connection=True;TrustServerCertificate=True"));
    })
    .Build();

host.Run();