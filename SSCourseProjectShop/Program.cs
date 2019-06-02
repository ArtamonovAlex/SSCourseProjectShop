using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SSCourseProjectShop
{
    class Program
    {
        static void Main(string[] args)
        {
            var customers = new Customer[]{
                new Customer("Петя", 10000),
                new Customer("Катя", 10000)
            };
            var io = new IO();
            io.PrintCustomers(customers);

            var shop = new Shop();
            io.PrintProducts(shop._products);
            Thread[] threads = new Thread[2];
            threads[0] = new Thread(
                () => shop.StartSession(customers[0])
            );
            threads[1] = new Thread(
                () => shop.StartSession(customers[1])
            );

            for (int i = 0; i < 2; i++)
            {
                threads[i].Start();
            }
            for (int i = 0; i < 2; i++)
            {
                threads[i].Join();
            }

            io.PrintCustomers(customers);
            io.PrintProducts(shop._products);
            Console.ReadLine();
        }
    }
}
