namespace DhamakaBS.Core
{
    public static class Constants
    {
        public const string AUDIO_FILE_NAME = "alert.wav";
        public const string PRODUCT_FILE_NAME = "product.json";
        public const string PRODUCT_BASE_URL = "https://dhamakashopping.com/products/";
        public const string MAIL_ADDR = "md.sojibmiya@gmail.com"; //;ranahamid007@gmail.com
    }

    public static class CategoryNames
    {
        public const string SMART_PHONES = "Smart Phones";
        public const string FEATURE_PHONES = "Feature Phones";
        public const string TABLETS = "Tablets";
        public const string PHONES_N_TABLETS = "Phones & Tablets";
        public const string LAPTOP_N_COMPUTER = "Laptop & Computer";
        public const string LAPTOPS = "Laptops";
        public const string Accessories = "Computer & Laptop Accessories";

        public static string[] Categories = new string[] { 
            SMART_PHONES
            , FEATURE_PHONES
            , LAPTOPS
            , TABLETS
            , PHONES_N_TABLETS
            , LAPTOP_N_COMPUTER
            , Accessories
        };
    }

    public static class ProductSlugs
    {
        public static string[] DUrls = new string[]
        {
            "https://dhamakashopping.com/products/samsung-galaxy-m01s-3-32gb-marr21",
            "https://dhamakashopping.com/products/redmi-9-4-64-smartphone-mar21",
            "https://dhamakashopping.com/products/redmi-9a-2-32-smartphone-mar21"
        };

        public static string[] SUrls = new string[]
        {
            "https://sirajganjshop.com/search?subcategory=mobile"
        };
    }

    public static class DhamakaShops
    {
        public static string[] ShopUrls = new string[]
        {
            "https://catalog-reader.dhamakashopping.com/api/products?page=0&limit=100&shop=606d45295aee430006b93e6b",
            "https://catalog-reader.dhamakashopping.com/api/products?page=0&limit=100&shop=606d4477ea64290006d7ad82",
            "https://catalog-reader.dhamakashopping.com/api/products?page=0&limit=100&shop=606d43fb0ba8d80006b7a7a6"
        };
    }
}
