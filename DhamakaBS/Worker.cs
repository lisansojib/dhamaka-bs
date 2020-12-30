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

                if (products.Count > 10) Alert();

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Total products: {products.Count}");
                await Task.Delay(60000, stoppingToken);
            }
        }

        private static async Task<List<Product>> GetProducts()
        {
            var client = new RestClient("https://service.dhamakashopping.com/graphql")
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", "{\"query\":\"query getCampaignById($_id: ObjectId, $pagination: PaginationInput, $category: ObjectId) {\\r\\n  campaignById(_id: $_id) {\\r\\n    _id\\r\\n    name\\r\\n    banner\\r\\n    startAt\\r\\n    endAt\\r\\n    categories {\\r\\n      _id\\r\\n      name\\r\\n      __typename\\r\\n    }\\r\\n    products(pagination: $pagination, filter: {category: $category, status: ACTIVE}) {\\r\\n      _id\\r\\n      name\\r\\n      oldPrice\\r\\n      price\\r\\n      campaignPriceTemp\\r\\n      slug\\r\\n      campaign {\\r\\n        _id\\r\\n        __typename\\r\\n      }\\r\\n      stock\\r\\n      status\\r\\n      images {\\r\\n        image\\r\\n        __typename\\r\\n      }\\r\\n      thumnails {\\r\\n        image\\r\\n        __typename\\r\\n      }\\r\\n      shop {\\r\\n        status\\r\\n        __typename\\r\\n      }\\r\\n      category {\\r\\n        _id\\r\\n        name\\r\\n        slug\\r\\n        parentCategories\\r\\n        parent {\\r\\n          _id\\r\\n          parent {\\r\\n            _id\\r\\n            __typename\\r\\n          }\\r\\n          __typename\\r\\n        }\\r\\n        __typename\\r\\n      }\\r\\n      __typename\\r\\n    }\\r\\n    status\\r\\n    __typename\\r\\n  }\\r\\n}\",\"variables\":{\"pagination\":{\"page\":1,\"limit\":1000},\"_id\":\"5fe577b19ba2980b60f68299\"}}",
                       ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic data = JsonConvert.DeserializeObject(response.Content);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(JsonConvert.SerializeObject(data.data.campaignById.products));
            products = products.FindAll(x 
                => x.Stock > 0 
                && CategoryNames.Categories.Contains(x.Category.Name));

            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.PRODUCT_FILE_NAME);

            var text = await File.ReadAllTextAsync(fullPath);
            if (!string.IsNullOrEmpty(text))
            {
                var oldProducts = JsonConvert.DeserializeObject<List<Product>>(text);

                var newStock = products.FindAll(x => !oldProducts.Select(x => x.Id).Contains(x.Id)).Select(x => Constants.PRODUCT_BASE_URL + x.Slug);
                if (newStock.Any())
                {
                    var mailBody = "";
                    foreach (var item in newStock)
                    {
                        mailBody += item + "<br><br>";
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
    }
}
