
namespace ComputerShare.Services.Zoopla
{
    public class ZooplaAverageSoldPricesForArea
    {
        public string name { get; set; }

        public double average_sold_price_1year { get; set; }
        public double average_sold_price_3year { get; set; }
        public double average_sold_price_5year { get; set; }
        public double average_sold_price_7year { get; set; }

        public double number_of_sales_1year { get; set; }
        public double number_of_sales_3year { get; set; }
        public double number_of_sales_5year { get; set; }
        public double number_of_sales_7year { get; set; }

        public double turnover { get; set; }
        public string prices_url { get; set; }
    }

    public class ZooplaAverageSoldPriceResult
    {
        public string country { get; set; }
        public int result_count { get; set; }
        public double longitude { get; set; }
        public double latitude { get; set; }
        public string area_name { get; set; }
        public string street { get; set; }
        public string town { get; set; }
        public string county { get; set; }
        public string postcode { get; set; }

        public ZooplaAverageSoldPricesForArea[] areas { get; set; }
    }
}
