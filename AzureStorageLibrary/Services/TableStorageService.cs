using AzureStorageLibrary.Interfaces;
using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AzureStorageLibrary.Services
{
    public class TableStorageService<TEntity> : INoSqlRepository<TEntity> where TEntity : TableEntity, new()
    {
        private readonly CloudTableClient cloudTableClient;

        private readonly CloudTable cloudTable;

        public TableStorageService()
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(Configurations.Configuration.connectionString);
            cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTable = cloudTableClient.GetTableReference(typeof(TEntity).Name);
            cloudTable.CreateIfNotExists();
        }
        public async Task<TEntity> Add(TEntity entity)
        {
            var tableOperation = TableOperation.InsertOrMerge(entity);
            var execution = await cloudTable.ExecuteAsync(tableOperation);

            return execution.Result as TEntity;
        }

        public IQueryable<TEntity> All()
        {
            return cloudTable.CreateQuery<TEntity>().AsQueryable();
        }
        public async Task<TEntity> Get(string rowKey, string partitionKey)
        {
            var tableOperation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            var execution = await cloudTable.ExecuteAsync(tableOperation);

            return execution.Result as TEntity;
        }
        public async Task Delete(string rowKey, string partitionKey)
        {
            var entity = await Get(rowKey, partitionKey);
            if (entity != null)
            {
                var tableOperation = TableOperation.Delete(entity);
                await cloudTable.ExecuteAsync(tableOperation);
            }
        }

        public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> query)
        {
            return cloudTable.CreateQuery<TEntity>().Where(query);
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            var tableOperation = TableOperation.Replace(entity);
            var execution = await cloudTable.ExecuteAsync(tableOperation);

            return execution.Result as TEntity;
        }
    }
}
