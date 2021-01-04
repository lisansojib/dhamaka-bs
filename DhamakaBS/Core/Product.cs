using Newtonsoft.Json;

namespace DhamakaBS
{
    public class Product
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("oldPrice")]
        public decimal OldPrice { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("campaignPriceTemp")]
        public decimal CampaignPriceTemp { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("campaign")]
        public Campaign Campaign { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("shop")]
        public Shop Shop { get; set; }

        [JsonProperty("__typename")]
        public string Typename { get; set; }

        [JsonProperty("category")]
        public Category Category { get; set; }

        public string Url { get; set; }

        public decimal Disc { get; set; }
    }

    public class Campaign
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("__typename")]
        public string Typename { get; set; }
    }

    public class Shop
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("__typename")]
        public string Typename { get; set; }
    }

    public class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
