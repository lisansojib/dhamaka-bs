using DhamakaBS.Core;
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
                        mailBody += $"{item.Url} <br> {item.Status} <br><br>";
                    }

                    await EmailService.SendEmailAsync(Constants.MAIL_ADDR, "New stock arriived!", mailBody);
                }

                _logger.LogInformation("Individual Product running at: {time}", DateTimeOffset.Now);
                _logger.LogInformation($"Stock Arrived: {newStockList.Count}");

                await Task.Delay(300000, stoppingToken);
            }
        }

        private async Task<List<Product>> GetProductStockAsync()
        {
            List<Product> newStockList = new();

            var httpClient = new HttpClient();

            //foreach (var productUrl in ProductSlugs.DUrls)
            //{
            //    using var response = await httpClient.GetAsync(productUrl);
            //    using var content = response.Content;
            //    var result = await content.ReadAsStringAsync();
            //    var document = new HtmlDocument();
            //    document.LoadHtml(result);

            //    int stock = 0;
            //    try
            //    {
            //        var vvd = document.DocumentNode.Descendants("div");
            //        var vvc = document.DocumentNode.Descendants("div").Where(d => d.Attributes["class"].Value.Contains("product-view-single-product-area"));
            //        var productInfo = document.GetElementbyId("__NEXT_DATA__").InnerText;

            //        var startIndex = productInfo.IndexOf(":", productInfo.IndexOf("stock")) + 1;
            //        var length = productInfo.IndexOf(",", startIndex) - startIndex;

            //        int.TryParse(productInfo.Substring(startIndex, length), out stock);

            //        if (stock > 0) newStockList.Add(productUrl);
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex.Message);

            //        result.Contains("In Stock", StringComparison.OrdinalIgnoreCase);
            //    }
            //}

            foreach (var productUrl in ProductSlugs.SUrls)
            {
                using var response = await httpClient.GetAsync(productUrl);
                using var content = response.Content;
                var result = await content.ReadAsStringAsync();
                var document = new HtmlDocument();
                document.LoadHtml(result);

                var productList = document.DocumentNode.Descendants().Where(x => x.HasClass("product-box-2")).ToList();
                foreach (var item in productList)
                {
                    var inStock = item.Descendants().FirstOrDefault(x => x.HasClass("my-stock-l")).InnerText;
                    var hasDiscount = item.Descendants().FirstOrDefault(x => x.HasClass("my-dis"))?.InnerText ?? "";
                    if (inStock.Contains("In stock", StringComparison.OrdinalIgnoreCase) && hasDiscount.Contains("Off", StringComparison.OrdinalIgnoreCase))
                    {
                        hasDiscount = hasDiscount.Replace("\r\n", string.Empty).Replace("\t", string.Empty).Trim();

                        var productLink = item.Descendants().FirstOrDefault(x => x.HasClass("product-title")).Descendants("a").FirstOrDefault().Attributes["href"].Value;
                        newStockList.Add(new Product
                        {
                            Status = hasDiscount,
                            Url = productLink
                        });
                    }                    
                }

                //var productInfo = document.DocumentNode.Descendants().Where(x => x.HasClass("product-description-wrapper")).FirstOrDefault().InnerText;

                //if (productInfo.Contains("In Stock", StringComparison.OrdinalIgnoreCase)) newStockList.Add(productUrl);
            }

            return newStockList;
        }
    }
}
