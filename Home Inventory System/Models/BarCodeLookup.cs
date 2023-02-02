using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Home_Inventory_System.Models
{
    internal class BarCodeLookup
    {
        public class Store
        {
            public string name { get; set; }
            public string price { get; set; }
            public string link { get; set; }
            public string currency { get; set; }
            public string currency_symbol { get; set; }
        }

        public class Review
        {
            public string name { get; set; }
            public string rating { get; set; }
            public string title { get; set; }
            public string review { get; set; }
            public string date { get; set; }
        }

        public class Contributor
        {
            public string role { get; set; }
            public string name { get; set; }
        }

        public class Product
        {
            public string barcode_number { get; set; }
            public string barcode_formats { get; set; }
            public string mpn { get; set; }
            public string model { get; set; }
            public string asin { get; set; }
            public string title { get; set; }
            public string category { get; set; }
            public string manufacturer
            {
                get; set;
            }
            public string brand { get; set; }
            public List<Contributor> contributors { get; set; }
            public string age_group { get; set; }
            public string ingredients { get; set; }
            public string nutrition_facts { get; set; }
            public string color { get; set; }
            public string format { get; set; }
            public string multipack { get; set; }
            public string size { get; set; }
            public string length { get; set; }
            public string width { get; set; }
            public string height { get; set; }
            public string weight { get; set; }
            public string release_date { get; set; }
            public string description { get; set; }
            public IList<object> features { get; set; }
            public IList<string> images { get; set; }
            public IList<Store> stores { get; set; }
            public IList<Review> reviews { get; set; }
        }

        public class RootObject
        {
            public IList<Product> products { get; set; }
        }
    }
}
