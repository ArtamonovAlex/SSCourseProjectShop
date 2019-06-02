using System;
using ShopLib;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ShopClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Представьтесь:");
            string customerName = Console.ReadLine();

            Console.WriteLine("Сколько у вас денег?");
            long customerBalance;
            while(!long.TryParse(Console.ReadLine(), out customerBalance)) {
                Console.WriteLine("Ожидалось целое число\nСколько у вас денег?");
            }

            TcpClient client = new TcpClient();
            client.Connect("127.0.0.1", 80);
            NetworkStream ns = client.GetStream();
            byte[] Buffer = new byte[256];
            ns.Write(Encoding.UTF8.GetBytes($"{customerName}:{customerBalance}"));

            string command;
            do
            {
                command = Console.ReadLine();
                switch(command)
                {
                    case "список":
                        ns.Write(Encoding.UTF8.GetBytes("список:0"));
                        ns.Read(Buffer, 0, Buffer.Length);
                        List<Product> products = ProductHandler.DeserializeProductList(Encoding.UTF8.GetString(Buffer));
                        ProductHandler.PrintProductList(products);
                        break;
                    case "exit":
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            } while (command != "exit");
            client.Close();
            ns.Close();
        }
    }
}
