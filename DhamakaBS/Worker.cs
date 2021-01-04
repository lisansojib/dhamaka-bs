using DhamakaBS.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DhamakaBS
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var products = await GetProducts();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Total products: {products.Count}");
                await Task.Delay(60000, stoppingToken);
            }
        }

        private static async Task<List<Product>> GetProducts()
        {
            List<Product> products = await GetMobileCampaignAsync();

            products = products.FindAll(x => x.OldPrice > 0 && x.OldPrice > x.Price 
                && !x.Slug.Contains("mouse") 
                && !x.Slug.Contains("ssd") 
                && !x.Slug.Contains("keyboard") 
                && !x.Slug.Contains("speaker") 
                && !x.Slug.Contains("webcam")
                && !x.Slug.Contains("camera")
                && !x.Slug.Contains("protector")
                && !x.Slug.Contains("router")
                && !x.Slug.Contains("trimmer")
                && !x.Slug.Contains("heater")
                && !x.Slug.Contains("repeater")
                && !x.Slug.Contains("ny-ubFlRjVNT")
                && !x.Slug.Contains("ny-upNr_CyLj")
                && !x.Slug.Contains("ny-Ex1qAhtBDx")
                && !x.Slug.Contains("ipad")
                && !x.Slug.Contains("linnex")
                && !x.Slug.Contains("plextone")
                && !x.Slug.Contains("and")
                && !x.Slug.Contains("wristband")
                && !x.Slug.Contains("kgtel")
                && !x.Slug.Contains("dell-vostro-14-3401-core-i3")
                );
            products.ForEach(x => x.Disc = (x.OldPrice - x.Price) / x.OldPrice * 100);
            products = products.FindAll(x => x.Disc >= 20);

            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PRODUCT_FILE_NAME);

            var text = await File.ReadAllTextAsync(fullPath);
            if (!string.IsNullOrEmpty(text))
            {
                var oldProducts = JsonConvert.DeserializeObject<List<Product>>(text);
                foreach (var item in oldProducts)
                {
                    var newProduct = products.Find(x => x.Id == item.Id && x.Disc <= item.Disc);
                    if (newProduct != null) products.Remove(newProduct);
                }

                if (products.Any())
                {
                    var mailBody = "";
                    foreach (var item in products)
                    {
                        mailBody += $"{Constants.PRODUCT_BASE_URL}{item.Slug} <b>Discount {(int)item.Disc}%<b><br><br>";
                    }

                    await EmailService.SendEmailAsync(Constants.MAIL_ADDR, "New stock arriived!", mailBody);
                }
            }

            await File.WriteAllTextAsync(fullPath, JsonConvert.SerializeObject(products));

            return products;
        }

        private void Alert()
        {
            try
            {
                using var waveOut = new WaveOutEvent();
                using var wavReader = new WaveFileReader(AppDomain.CurrentDomain.BaseDirectory + "/" + Constants.AUDIO_FILE_NAME);
                waveOut.Init(wavReader);
                waveOut.Play();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }            
        }

        private static async Task<List<Product>> GetMobileCampaignAsync()
        {
            var client = new RestClient("https://service.dhamakashopping.com/graphql")
            {
                Timeout = -1
            };
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query getCampaignById($_id: ObjectId, $pagination: PaginationInput, $category: ObjectId) {\\r\\n  campaignById(_id: $_id) {\\r\\n    _id\\r\\n    name\\r\\n    banner\\r\\n    startAt\\r\\n    endAt\\r\\n    categories {\\r\\n      _id\\r\\n      name\\r\\n      __typename\\r\\n    }\\r\\n    products(pagination: $pagination, filter: {category: $category, status: ACTIVE}) {\\r\\n      _id\\r\\n      name\\r\\n      oldPrice\\r\\n      price\\r\\n      campaignPriceTemp\\r\\n      slug\\r\\n      campaign {\\r\\n        _id\\r\\n        __typename\\r\\n      }\\r\\n      stock\\r\\n      status\\r\\n      images {\\r\\n        image\\r\\n        __typename\\r\\n      }\\r\\n      thumnails {\\r\\n        image\\r\\n        __typename\\r\\n      }\\r\\n      shop {\\r\\n        status\\r\\n        __typename\\r\\n      }\\r\\n      category {\\r\\n        _id\\r\\n        name\\r\\n        slug\\r\\n        parentCategories\\r\\n        parent {\\r\\n          _id\\r\\n          parent {\\r\\n            _id\\r\\n            __typename\\r\\n          }\\r\\n          __typename\\r\\n        }\\r\\n        __typename\\r\\n      }\\r\\n      __typename\\r\\n    }\\r\\n    status\\r\\n    __typename\\r\\n  }\\r\\n}\",\"variables\":{\"pagination\":{\"page\":1,\"limit\":1000},\"_id\":\"5feda8afcd6c250b5dad75d4\"}}",
                       ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(JsonConvert.SerializeObject(data.data.campaignById.products));
            products = products.FindAll(x
                => x.Stock > 0
                && CategoryNames.Categories.Contains(x.Category.Name));

            return products;
        }

    }
}
