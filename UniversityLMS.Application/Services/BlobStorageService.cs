 using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Application.Interfaces;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly QueueClient _queueClient;
        private readonly IConfiguration _configuration;

        public BlobStorageService(BlobServiceClient blobServiceClient, IConfiguration configuration, QueueServiceClient queueClient)
        {
            _configuration = configuration;
            string containerName = _configuration["BlobStorage:ContainerName"]!;
            _containerClient = blobServiceClient.GetBlobContainerClient("submissions");

            _containerClient.CreateIfNotExistsAsync();
            string queueContainer = _configuration["QueueStorage:ContainerName"]!;
            _queueClient = queueClient.GetQueueClient("submissionsfiles");
            _queueClient.CreateIfNotExistsAsync();
        }

        public async Task<string> UploadBlobAsync(string blobName, Stream content)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, overwrite: true);
            return blobClient.Uri.ToString();
        }

        public async Task<Stream> DownloadBlobAsync(string blobName)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            var download = await blobClient.DownloadAsync();
            return download.Value.Content;
        }

        public async Task<bool> PushToQueue(OutboxMessage message)
        {
            try
            {

                // Ensure the queue exists
                await _queueClient.CreateIfNotExistsAsync();

                var messagePayload = JsonConvert.SerializeObject(message);

                // Send the message to the queue
                await _queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(messagePayload)));

                return true;
            }
            catch
            {
                return false;

            }
        }
    }
}

