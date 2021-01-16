using AzureStorageLibrary.Interfaces;
using AzureStorageLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AzureWebApp.Controllers
{
    public class TableStorageController : Controller
    {
        private readonly INoSqlRepository<Product> noSqlRepository;
        public TableStorageController(INoSqlRepository<Product> noSqlRepository)
        {
            this.noSqlRepository = noSqlRepository;
        }
        public IActionResult Index()
        {
            ViewBag.Products = noSqlRepository.All();
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            product.RowKey = Guid.NewGuid().ToString();
            var randomPartition=new Random(5);
            product.PartitionKey = randomPartition.Next(1,5).ToString();
            await noSqlRepository.Add(product);
            return RedirectToAction("Index");
        }
    }
}