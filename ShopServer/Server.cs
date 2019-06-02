using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ShopLib;

namespace ShopServer
{
    class Server
    {
        private TcpListener Listener;
        private List<Product> Products;
        private static Semaphore sem = new Semaphore(3, 3);

        public Server(int Port)
        {
            Products = new List<Product>();
            Listener = new TcpListener(IPAddress.Any, Port);
            new Thread(ListenerThread).Start(Listener);
            string command;
            do
            {
                command = Console.ReadLine();
                switch (command)
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
                        lock (Products)
                        {
                            if (ProductHandler.FindProduct(Products, productName) == null)
                            {
                                Products.Add(new Product(productName, productPrice, productQuantity));
                                Console.WriteLine($"Товар {productName} успешно добавлен, чтобы увидеть весь список товаров наберите \"список\"");
                            }
                            else
                            {
                                Product product = ProductHandler.FindProduct(Products, productName);
                                product.price = productPrice;
                                product.quantity = productQuantity;
                                Console.WriteLine($"Товар {productName} успешно изменён, чтобы увидеть весь список товаров наберите \"список\"");
                            }
                        }
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
            byte[] clearBuffer = new byte[256];
            sem.WaitOne();
            TcpClient client = (TcpClient)StateInfo;
            NetworkStream ns = client.GetStream();
            byte[] Buffer = new byte[256];
            ns.Read(Buffer, 0, Buffer.Length);
            ns.Write(Encoding.UTF8.GetBytes("Ok"));
            string[] customerInfo = Encoding.UTF8.GetString(Buffer).Split(':');
            string customerName = customerInfo[0];
            long customerBalance = long.Parse(customerInfo[1]);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Пришёл {customerName}, у него {customerBalance} денег.");
            Console.ResetColor();

            while (client.Connected)
            {
                try
                {
                    clearBuffer.CopyTo(Buffer, 0);
                    ns.Read(Buffer, 0, Buffer.Length);
                }
                catch (IOException ex) when (ex.InnerException.GetType().Equals(typeof(SocketException)) && ((SocketException)ex.InnerException).ErrorCode == 10053)
                {
                    break;
                }
                if (Buffer[0] == 0)
                {
                    break;
                }
                string[] customerMessage = Encoding.UTF8.GetString(Buffer).Split(':');
                string customerAction = customerMessage[0];
                int customerQuantity = int.Parse(customerMessage[1]);
                string answer;
                switch (customerAction)
                {
                    case "список":
                        answer = ProductHandler.SerializeProductList(Products);
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"{customerName} желает приобрести {customerAction}, {customerQuantity} штук.");
                        Console.ResetColor();
                        int productsBought = 0;
                        if (ProductHandler.FindProduct(Products, customerAction) == null)
                        {
                            answer = $"{productsBought}:товар отсутствует";
                            break;
                        }
                        string reason = "Ok";
                        lock (Products)
                        {
                            Product product = ProductHandler.FindProduct(Products, customerAction);
                            while (productsBought < customerQuantity)
                            {
                                if (product.price > customerBalance)
                                {
                                    reason = "недостаточно средств";
                                    break;
                                }
                                if (product.quantity == 0)
                                {
                                    reason = "товар закончился";
                                    break;
                                }
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{customerName} начинает покупку {customerAction}.");
                                Console.ResetColor();
                                customerBalance -= product.price;
                                product.quantity--;
                                productsBought++;
                                Thread.Sleep(10000);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine($"{customerName} совершил{(customerName[customerName.Length - 1] == 'а' ? "a" : "")} покупку {customerAction}.\n");
                                Console.ResetColor();
                            }
                        }
                        answer = $"{productsBought}:{reason}";
                        break;
                }
                client.GetStream().Write(Encoding.UTF8.GetBytes(answer));
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{customerName} ушёл.");
            Console.ResetColor();
            client.Close();
            ns.Close();
            sem.Release();
        }

        private void ListenerThread(Object StateInfo)
        {
            TcpListener listener = (TcpListener)StateInfo;
            listener.Start();
            Console.WriteLine("Listener started");
            int counter = 1;
            bool isServerRunning = true;
            while (isServerRunning)
            {
                try
                {
                    sem.WaitOne();
                    new Thread(ClientThread).Start(listener.AcceptTcpClient());
                    sem.Release();
                    Console.WriteLine("Got client " + counter++);
                }
                catch (SocketException ex) when (ex.ErrorCode == 10004)
                {
                    isServerRunning = false;
                }
            }
        }
    }
}
