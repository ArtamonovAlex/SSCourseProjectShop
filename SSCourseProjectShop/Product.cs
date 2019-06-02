using System;

namespace SSCourseProjectShop
{
    public class Product
    {
        private int _count;
        private readonly object syncObj;

        public int Count
        {
            get
            {
                lock (syncObj)
                {
                    return _count;
                }

            }
            set
            {
                lock (syncObj)
                {
                    _count = value;
                }
            }
        }

        public string Name { get; }
        public int Price { get; }

        public Product(string name, int price)
        {
            syncObj = new Object();
            Name = name;
            Price = price;
        }

        public bool DecreaseCount(int amount)
        {
            lock (syncObj)
            {
                if (_count >= amount)
                {
                    _count -= amount;
                    return true;
                }
                return false;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Product product &&
                   this.Name == product.Name &&
                   this.Price == product.Price;
        }


        public override string ToString()
        {
            return $"Продукт: {this.Name}\nЦена: {this.Price}\nКоличество: {this.Count}";
        }
    }
}
