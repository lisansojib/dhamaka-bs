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

        public static string[] Categories = new string[] { 
            SMART_PHONES
            , FEATURE_PHONES
            , LAPTOPS
            , TABLETS
            , PHONES_N_TABLETS
            , LAPTOP_N_COMPUTER
        };
    }
}
