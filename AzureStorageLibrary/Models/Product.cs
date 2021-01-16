using Microsoft.Azure.Cosmos.Table;

namespace AzureStorageLibrary.Models
{
    public class Product:TableEntity
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Price { get; set; }
    }
}
