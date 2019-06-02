using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSCourseProjectShop
{
    public class Customer
    {
        private Random _random;
        public Customer(string name, int money)
        {
            Name = name;
            Money = money;
            _random = new Random(Guid.NewGuid().GetHashCode());
        }

        public string Name { get; set; }
        public int Money { get; set; }

        public void Buy(Product product, int cost)
        {
            if (Money < cost)
                throw new ArgumentOutOfRangeException("Customer hasn't enough money");
            Money -= cost;

        }
        public int GetRandom()
        {
            return _random.Next(0, 2);
        }
    }
}
