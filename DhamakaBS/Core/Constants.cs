namespace DhamakaBS.Core
{
    public static class Constants
    {
        public const string AUDIO_FILE_NAME = "alert.wav";
        public const string PRODUCT_FILE_NAME = "product.json";
        public const string PRODUCT_BASE_URL = "https://www.dhamakashopping.com/products/";
        public const string MAIL_ADDR = "md.sojibmiya@gmail.com;ranahamid007@gmail.com";
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
            "https://www.dhamakashopping.com/products/galaxy-m20-3gb-rom-32-gb-rom-13-0-mp-5-0-mp-camera-with-6-3-display-smartphone",
            "https://www.dhamakashopping.com/products/realme-c17-6gb-ram-128gb-rom-f7yCR2u8QK",
            "https://www.dhamakashopping.com/products/realme-7i-8gb-ram-128gb-rom-sOrfVYokd"
        };

        public static string[] SUrls = new string[]
        {
            "https://www.sirajganjshop.com/product/Realme-Narzo-20-3ajoc"
        };
    }
}
