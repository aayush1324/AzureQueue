using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UniversityLMS.Application.DTOs;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;

namespace QueueTrigger
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;
        private readonly ISubmissionRepository submissionRepository;
        private readonly IBlobStorageService blobStorageService;

        public Function1(ILogger<Function1> logger, ISubmissionRepository submissionRepository, IBlobStorageService blobStorageService)
        {
            _logger = logger;
            this.submissionRepository = submissionRepository;
            this.blobStorageService = blobStorageService;
        }

        [Function(nameof(Function1))]
        public async Task Run([QueueTrigger("submissionsfiles", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            var messageBody = message.Body.ToString();  // Assuming the body is a string, if it's byte array, you may need to convert it to string first
            OutboxMessage outboxMessage = JsonConvert.DeserializeObject<OutboxMessage>(messageBody);
            var blobName = $"{outboxMessage.UserId}{outboxMessage.AssignmentId}";
            var stream = await blobStorageService.DownloadBlobAsync(blobName);
            //stream.Position = 0; // Ensure we're reading from the beginning
            var codeFile = "";
            using (var reader = new StreamReader(stream))
            {
                codeFile = await reader.ReadToEndAsync();
            }
            var submissionAssignmentDto = new SubmissionAssignmentDTO
            {
                AssignmentId = outboxMessage.AssignmentId,
                FilePath = outboxMessage.FilePath,
                UserId = outboxMessage.UserId,
                SourceCode= codeFile,
                OutboxMessageId = outboxMessage.Id
            };
            await submissionRepository.ProcessQueueData(submissionAssignmentDto);
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
