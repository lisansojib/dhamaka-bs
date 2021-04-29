using DhamakaBS.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DhamakaBS
{
    public class ShopWorker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public ShopWorker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var products = await GetProducts();

                _logger.LogInformation($"Total products: {products.Count} {DateTimeOffset.Now}");
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task<List<ShopProduct>> GetProducts()
        {
            List<ShopProduct> products = await GetShopStocksAsync();

            products = products.FindAll(x => !x.Slug.Contains("bird")
                && !x.Slug.Contains("samsung-note-10-lite")
                && !x.Slug.Contains("samsung-guru-music")
                && !x.Slug.Contains("samsung-galaxy-a31")
                && !x.Slug.Contains("samsung-m31")
                && !x.Slug.Contains("samsung-a51")
                && !x.Slug.Contains("samsung-galaxy-a71")
                && !x.Slug.Contains("realme-7pro")
                && !x.Slug.Contains("realme-8"));

            products.ForEach(x => x.Disc = (x.OldPrice - x.Price) / x.OldPrice * 100);
            products = products.FindAll(x => x.Disc >= 10);

            if (products.Any())
            {
                Alert();

                var mailBody = "";
                foreach (var item in products)
                {
                    mailBody += $"{Constants.PRODUCT_BASE_URL}{item.Slug} <b>Discount {(int)item.Disc}%<b> {item.Stock}<br><br>";
                }

                await EmailService.SendEmailAsync(Constants.MAIL_ADDR, "New stock arriived!", mailBody);
            }

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

        private static async Task<List<ShopProduct>> GetShopStocksAsync()
        {
            List<ShopProduct> products = new();
            foreach (var shopUrl in DhamakaShops.ShopUrls)
            {
                var client = new RestClient(shopUrl)
                {
                    Timeout = -1
                };
                var request = new RestRequest(Method.GET);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse response = await client.ExecuteAsync(request);
                dynamic data = JsonConvert.DeserializeObject(response.Content);
                List<ShopProduct> tempProducts = JsonConvert.DeserializeObject<List<ShopProduct>>(JsonConvert.SerializeObject(data.products), new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });
                tempProducts = products.FindAll(x => x.Stock > 0);
                products.AddRange(tempProducts);
            }

            return products;
        }
    }
}
