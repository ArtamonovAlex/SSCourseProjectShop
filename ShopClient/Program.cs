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
            TcpClient client = new TcpClient();
            try
            {
                client.Connect("127.0.0.1", 80);
            }
            catch (SocketException ex) when (ex.ErrorCode == 10061)
            {
                Console.WriteLine("Магазин закрыт. Приходите позже.");
                return;
            }

            Console.WriteLine("Представьтесь:");
            string customerName = Console.ReadLine();

            Console.WriteLine("Сколько у вас денег?");
            long customerBalance;
            while (!long.TryParse(Console.ReadLine(), out customerBalance))
            {
                Console.WriteLine("Ожидалось целое число\nСколько у вас денег?");
            }

            NetworkStream ns = client.GetStream();
            byte[] clearBuffer = new byte[256];
            byte[] Buffer = new byte[256];
            ns.Write(Encoding.UTF8.GetBytes($"{customerName}:{customerBalance}"));
            Console.WriteLine("Ждём очередь...");
            ns.Read(Buffer, 0, Buffer.Length);
            if (Encoding.UTF8.GetString(Buffer).Trim((char)0) == "Ok")
            {
                Console.WriteLine("Вы в магазине. Чтобы посмотреть список товаров, наберите \"список\"");
            }

            string command;
            do
            {
                command = Console.ReadLine();
                switch (command)
                {
                    case "список":
                        ns.Write(Encoding.UTF8.GetBytes("список:0"));
                        clearBuffer.CopyTo(Buffer,0);
                        ns.Read(Buffer, 0, Buffer.Length);
                        List<Product> products = ProductHandler.DeserializeProductList(Encoding.UTF8.GetString(Buffer));
                        ProductHandler.PrintProductList(products);
                        break;
                    case "купить":
                        Console.WriteLine("Что хотите приобрести?");
                        string productName = Console.ReadLine();
                        Console.WriteLine("Сколько товара хотите приобрести?");
                        int productQuantity;
                        while(!int.TryParse(Console.ReadLine(), out productQuantity))
                        {
                            Console.WriteLine("Ожидалось целое число\nСколько товара хотите приобрести?");
                        }
                        ns.Write(Encoding.UTF8.GetBytes($"{productName}:{productQuantity}"));
                        Console.WriteLine("Совершаем покупочки...");
                        clearBuffer.CopyTo(Buffer, 0);
                        ns.Read(Buffer,0,Buffer.Length);
                        string[] response = Encoding.UTF8.GetString(Buffer).Split(':');
                        Console.Write($"Вы купили {response[0]} из {productQuantity} желаемых единиц");
                        if (response[1].Trim((char)0) == "Ok")
                        {
                            Console.WriteLine(".");
                        } else
                        {
                            Console.WriteLine($", потому что {response[1].Trim((char)0)}.");
                        }
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
