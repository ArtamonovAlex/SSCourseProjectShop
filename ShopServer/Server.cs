using ShopLib;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ShopServer
{
    class Server
    {
        private TcpListener Listener;
        private List<Product> Products;

        public Server(int Port)
        {
            Products = new List<Product>();
            Listener = new TcpListener(IPAddress.Any, Port);
            ThreadPool.QueueUserWorkItem(new WaitCallback(ListenerThread), Listener);
            string command;
            do
            {
                command = Console.ReadLine();
                switch(command)
                {
                    case "добавить":
                        Console.WriteLine("Введите наименование товара:");
                        string productName = Console.ReadLine();
                        Console.WriteLine($"Введите цену товара {productName}:");
                        int productPrice;
                        while (!int.TryParse(Console.ReadLine(), out productPrice))
                        {
                            Console.WriteLine($"Ожидалось целое число\nВведите цену товара {productName}:");
                        }
                        Console.WriteLine($"Введите количество товара {productName}:");
                        int productQuantity;
                        while (!int.TryParse(Console.ReadLine(), out productQuantity))
                        {
                            Console.WriteLine($"Ожидалось целое число\nВведите количество товара {productName}:");
                        }
                        Products.Add(new Product(productName, productPrice, productQuantity));
                        Console.WriteLine($"Товар {productName} успешно добавлен, чтобы увидеть весь список товаров наберите \"список\"");
                        break;
                    case "список":
                        ProductHandler.PrintProductList(Products);
                        break;
                    case "shutdown":
                        break;
                    default:
                        Console.WriteLine("Неизвестная команда");
                        break;
                }
            } while (command != "shutdown");
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        private void ClientThread(Object StateInfo)
        {
            TcpClient client = (TcpClient)StateInfo;
            NetworkStream ns = client.GetStream();
            byte[] Buffer = new byte[256];
            ns.Read(Buffer, 0, Buffer.Length);
            string[] customerInfo = Encoding.UTF8.GetString(Buffer).Split(':');
            string customerName = customerInfo[0];
            long customerBalance = long.Parse(customerInfo[1]);
            Console.WriteLine($"Пришёл {customerName}, у него {customerBalance} денег.");

            while (client.Connected)
            {
                if (ns.DataAvailable)
                {
                    ns.Read(Buffer, 0, Buffer.Length);
                    string[] customerMessage = Encoding.UTF8.GetString(Buffer).Split(':');
                    string customerAction = customerMessage[0];
                    long customerQuantity = long.Parse(customerMessage[1]);
                    string answer;
                    switch (customerAction)
                    {
                        case "список":
                            answer = ProductHandler.SerializeProductList(Products);
                            break;
                        default:
                            answer = "Неизвестная команда";
                            break;
                    }
                    client.GetStream().Write(Encoding.UTF8.GetBytes(answer));
                }
            }
            client.Close();
            ns.Close();
        }

        private void ListenerThread(Object StateInfo)
        {
            TcpListener listener = (TcpListener)StateInfo;
            listener.Start();
            Console.WriteLine("Listener started");
            int counter = 1;

            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), listener.AcceptTcpClient());
                Console.WriteLine("Got client " + counter++);
            }
        }
    }
}
