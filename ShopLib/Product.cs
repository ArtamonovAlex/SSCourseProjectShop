using System;
using System.Collections.Generic;
using System.Text;

namespace ShopLib
{
    public class Product
    {
        public string name;
        public int price;
        public int quantity;

        public Product(string name, int price, int quantity)
        {
            this.name = name;
            this.price = price;
            this.quantity = quantity;
        }
    }
}
