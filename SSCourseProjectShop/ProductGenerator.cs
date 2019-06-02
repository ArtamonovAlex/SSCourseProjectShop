using System.Collections.Generic;

namespace SSCourseProjectShop
{
    public class ProductGenerator
    {
        public static int Count = 2;
        public List<Product> Generate()
        {
            var milk = new Product("Молоко", 20)
            {
                Count = 10000
            };
            var bread = new Product("Хлеб", 50)
            {
                Count = 10000
            };
            return new List<Product>
            {
                milk,
                bread
            };
        }
    }
}
