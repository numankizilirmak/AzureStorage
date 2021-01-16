using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureStorageLibrary.Interfaces;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class QueueStorageService: IQueueStorageRepository
    {
        private readonly QueueClient queueClient;
        public QueueStorageService(string queueName)
        {
            queueClient = new QueueClient(Configurations.Configuration.connectionString, queueName);
            queueClient.CreateIfNotExists();
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            await queueClient.DeleteMessageAsync(messageId, popReceipt);
        }

        public async Task<QueueMessage> RetrieveNextMessageAsync()
        {
            var queueProperties = await queueClient.GetPropertiesAsync();

            if (queueProperties.Value.ApproximateMessagesCount>0)
            {
                QueueMessage queueMessage =await queueClient.ReceiveMessageAsync();
                return queueMessage;
            }
            return null;
        }
        public async Task SendMessageAsync(string message)
        {
            await queueClient.SendMessageAsync(message);
        }
    }
}
