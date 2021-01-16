using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using AzureStorageLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class BlobStorageService : IBlobRepository
    {
        private readonly BlobServiceClient blobServiceClient;

        public BlobStorageService()
        {
            blobServiceClient = new BlobServiceClient(Configurations.Configuration.connectionString);
        }
        public string BlobUrl { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public async Task DeleteAsync(string filename, string containername)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containername);
            var blobClient = containerClient.GetBlobClient(filename);
            await blobClient.DeleteAsync();
        }

        public async Task<Stream> DownloadAsync(string filename, string containername)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containername);
            var blobClient = containerClient.GetBlobClient(filename);
            var file= await blobClient.DownloadAsync();

            return file.Value.Content;
        }

        public List<string> GetContainerFiles(string containername)
        {
            List<string> fileList = new List<string>();
            var containerClient = blobServiceClient.GetBlobContainerClient(containername);
            var blobs = containerClient.GetBlobs();
            foreach (var item in blobs)
            {
                fileList.Add(item.Name);
            }
            return fileList;
        }

        public async Task<List<string>> GetLogAsync(string fileName)
        {
            var logList = new List<string>();
            var containerClient = blobServiceClient.GetBlobContainerClient("Logs");
            var blobClient = containerClient.GetBlobClient(fileName);
            await containerClient.CreateIfNotExistsAsync();
            var file = await blobClient.DownloadAsync();

            using (var stream =new StreamReader(file.Value.Content))
            {
                string line = string.Empty;
                while((line=stream.ReadLine())!=null)
                {
                    logList.Add(line);
                }
            }
            return logList;
        }

        public async Task UploadAsync(Stream stream, string filename, string containername)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containername);
            await containerClient.CreateIfNotExistsAsync();
            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            var blobClient = containerClient.GetBlobClient(filename);
            await blobClient.UploadAsync(stream);
        }

        public async Task WriteLogAsync(string text)
        {
            var containerClient = blobServiceClient.GetBlobContainerClient("Logs");
            var fileName = GetLogFileName();
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetAppendBlobClient(fileName);

            using (var memoryStream=new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    streamWriter.Write($"{DateTime.Now} : {text}");
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    await blobClient.AppendBlockAsync(memoryStream);
                }
            }
        }
        private string GetLogFileName()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.Year);
            stringBuilder.Append(DateTime.Now.Month);
            stringBuilder.Append(DateTime.Now.Day);
            stringBuilder.Append(DateTime.Now.Hour);
            return stringBuilder.ToString();
        }
    }
}
