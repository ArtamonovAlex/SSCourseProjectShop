using System;
using System.Collections.Generic;
using System.Text;

namespace SSCourseProjectShop
{
    class IO
    {
        public void PrintCustomers(Customer[] customers)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("┌────────┬──────────┬─────────┐");
            Console.WriteLine("│ Number │ Customer │  Money  │");
            for (int i = 0; i < customers.Length; i++)
            {
                Console.WriteLine("├────────┼──────────┼─────────┤");
                Console.WriteLine($"│ {i,-6} │ {customers[i].Name,-8} │  {customers[i].Money,-5}  │");
            }
            Console.WriteLine("└────────┴──────────┴─────────┘");
        }

        public void PrintProducts(List<Product> products)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("┌────────┬───────────┬──────────┬───────────┐");
            Console.WriteLine("│ Number │  Product  │   Price  │   Count   │");
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine("├────────┼───────────┼──────────┼───────────┤");
                Console.WriteLine($"│ {i,-6} │ {products[i].Name,-9} │  {products[i].Price,-6}  │ {products[i].Count,-8}  │");
            }
            Console.WriteLine("└────────┴───────────┴──────────┴───────────┘");
        }
    }
}
