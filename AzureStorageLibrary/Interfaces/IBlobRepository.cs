using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Interfaces
{
    public interface IBlobRepository
    {
        public string BlobUrl { get; set; }
        Task UploadAsync(Stream stream, string filename, string containername);

        Task<Stream> DownloadAsync(string filename, string containername);

        Task DeleteAsync(string filename, string containername);

        Task WriteLogAsync(string text);

        Task<List<string>> GetLogAsync(string fileName);

        List<string> GetContainerFiles(string containername);
    }
}
