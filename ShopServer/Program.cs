using System;
using System.Threading;

namespace ShopServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(2, 2);
            ThreadPool.SetMaxThreads(12, 12);
            Server MainShop = new Server(80);
        }
    }
}
