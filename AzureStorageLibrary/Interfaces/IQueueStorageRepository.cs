using Azure.Storage.Queues.Models;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Interfaces
{
    public interface IQueueStorageRepository
    {
        Task SendMessageAsync(string message);

        Task<QueueMessage> RetrieveNextMessageAsync();

        Task DeleteMessageAsync(string messageId,string popReceipt);
    }
}
