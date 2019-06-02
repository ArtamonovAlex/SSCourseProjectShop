using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSCourseProjectShop
{
    public class Shop
    {
        private static object sync = new Object();
        private static Shop _instance;
        public List<Product> _products;

        private int _minIndex = 0;
        public Shop()
        {
            var generator = new ProductGenerator();
            _products = generator.Generate();
            MinPrice();
        }

        public static Shop Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (sync)
                    {
                        if (_instance == null)
                            _instance = new Shop();
                    }
                }
                return _instance;
            }

        }

        public void StartSession(Customer customer)
        {

            while (customer.Money >= MinPrice())
            {

                int number = customer.GetRandom();
                int price = _products[number].Price;
                if (customer.Money >= price
                     && _products[number].DecreaseCount(1))
                {
                    Console.WriteLine($"{customer.Name} покупает {_products[number].Name}");
                    customer.Money -= price;
                    Console.WriteLine($"Закончил покупку {customer.Name}");
                }

            }
        }

        private int MinPrice()
        {
            if (_products[_minIndex].Count <= 0)
            {
                int minPrice = 0;
                bool isFound = false;
                for (int i = 0; i < _products.Count && _products[i].Count > 0; i++)
                {
                    if (!isFound)
                    {
                        isFound = true;
                        minPrice = _products[i].Price;
                        _minIndex = i;
                    }
                    else
                    {
                        if (_products[i].Price < minPrice)
                        {
                            minPrice = _products[i].Price;
                            _minIndex = i;
                        }
                    }
                }
                if (!isFound) return Int32.MaxValue;

            }

            return _products[_minIndex].Price;

        }
    }
}
