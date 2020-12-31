﻿using DhamakaBS.Core;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DhamakaBS
{
    public class ProductWorker : BackgroundService
    {
        private readonly ILogger<ProductWorker> _logger;

        public ProductWorker(ILogger<ProductWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var newStockList = await GetProductStockAsync();

                if (newStockList.Any())
                {
                    var mailBody = "";
                    foreach (var item in newStockList)
                    {
                        mailBody += item + "<br><br>";
                    }

                    await EmailService.SendEmailAsync(Constants.MAIL_ADDR, "New stock arriived!", mailBody);
                }

                _logger.LogInformation("Individual Product running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Stock Arrived: {newStockList.Count}");

                await Task.Delay(60000, stoppingToken);
            }
        }

        private static async Task<List<string>> GetProductStockAsync()
        {
            List<string> newStockList = new List<string>();

            var httpClient = new HttpClient();

            foreach (var item in ProductSlugs.Slugs)
            {
                var productUrl = $"{Constants.PRODUCT_BASE_URL}{item}";
                using var response = await httpClient.GetAsync(productUrl);
                using var content = response.Content;
                var result = await content.ReadAsStringAsync();
                var document = new HtmlDocument();
                document.LoadHtml(result);
                var productInfo = document.GetElementbyId("__NEXT_DATA__").InnerText;

                var startIndex = productInfo.IndexOf(":", productInfo.IndexOf("stock")) + 1;
                var length = productInfo.IndexOf(",", startIndex) - startIndex;

                int.TryParse(productInfo.Substring(startIndex, length), out int stock);

                if (stock > 0) newStockList.Add(productUrl);
            }

            return newStockList;
        }
    }
}