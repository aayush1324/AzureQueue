using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityLMS.Domain.Entities;

namespace UniversityLMS.Application.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadBlobAsync(string blobName, Stream content);
        Task<Stream> DownloadBlobAsync(string blobName);

        Task<bool> PushToQueue(OutboxMessage message);
    }
}
