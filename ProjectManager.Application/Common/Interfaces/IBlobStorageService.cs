using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManager.Application.Common.Interfaces
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(string containerName, string fileName, Stream fileStream, CancellationToken cancellationToken = default);
        Task<Stream> DownloadFileAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
        Task DeleteFileAsync(string containerName, string fileName, CancellationToken cancellationToken = default);
    }
}
