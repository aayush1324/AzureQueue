using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
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

        //services.AddSingleton<IQueueService>(provider => new QueueService("UseDevelopmentStorage=true"));

        services.AddSingleton(new QueueClient(queueConnectionString, "submissionfiles"));
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        services.AddSingleton<IJudgeService, Judge0Service>();
        services.AddSingleton<IPlagiarism, PlagiarismService>();
        services.AddSingleton<IBlobStorageService, BlobStorageService>();
        services.AddSingleton<IUnitofWorkRepository, UnitofWorkRepository>();
        services.AddSingleton<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddSingleton<ISubmissionRepository, SubmissionRepository>();

        services.AddHttpClient();
        services.AddHttpContextAccessor();

        services.AddSingleton(x =>
            new BlobServiceClient("UseDevelopmentStorage=true"));


        //services.AddSingleton(x =>
        //    new QueueServiceClient("UseDevelopmentStorage=true"));

        services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer("Server=QB-NOI-1120;Database=UniversityProjectDbOutboxVersion;Trusted_Connection=True;TrustServerCertificate=True"));

        // Register QueueServiceClient without a name to make it the default
        services.AddSingleton(provider =>
        {
            string queueConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")!;
            if (string.IsNullOrEmpty(queueConnectionString))
            {
                throw new InvalidOperationException("AzureWebJobsStorage connection string is missing.");
            }
            return new QueueServiceClient(queueConnectionString);
        });
    })
    .Build();

host.Run();