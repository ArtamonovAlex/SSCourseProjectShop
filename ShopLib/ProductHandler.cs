using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ShopLib
{
    public static class ProductHandler
    {
        public static void PrintProductList(List<Product> products)
        {
            Console.WriteLine("┌────┬─────────────────┬─────────────────┬─────────────────┐");
            Console.WriteLine($"│ {"N",-3}│ {"Имя товара",-16}│ {"Цена",-16}│ {"Количество",-16}│");
            int counter = 1;
            foreach (Product product in products)
            {
                Console.WriteLine("├────┼─────────────────┼─────────────────┼─────────────────┤");
                Console.WriteLine($"│{counter++,3} │{product.name,16} │{product.price,16} │{product.quantity,16} │");
            }
            Console.WriteLine("└────┴─────────────────┴─────────────────┴─────────────────┘");
        }

        public static string SerializeProductList(List<Product> products)
        {
            return JsonConvert.SerializeObject(products);
        }

        public static List<Product> DeserializeProductList(string products)
        {
            return JsonConvert.DeserializeObject<List<Product>>(products);
        }

        public static Product FindProduct(List<Product> products, string productName)
        {
            Product foundedProduct = null;
            foreach(Product product in products)
            {
                if (product.name == productName)
                {
                    foundedProduct = product;
                    break;
                }
            }
            return foundedProduct;
        }
    }
}
