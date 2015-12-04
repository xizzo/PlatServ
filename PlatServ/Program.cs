using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;

namespace PlatServ
{
    class Program
    {
        static Thread readConsoleThread;
        static Thread updateGameThread = new Thread(new ThreadStart(UpdateGames));
        static List<Client> clientsList = new List<Client>();

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(new IPEndPoint(IPAddress.Parse("192.168.0.142"), 7777));
            TcpClient clientSocket = default(TcpClient);

            readConsoleThread = new Thread(readConsole);
            readConsoleThread.Start();

            serverSocket.Start();
            updateGameThread.Start();
            ConsoleSay("Server", "Started on " + serverSocket.LocalEndpoint);

            Room room1 = new Room();
            int count = 0;
            while (true)
            {

                clientSocket = serverSocket.AcceptTcpClient();

                ConsoleSay("Server", "Client(" + count + ") connect: " + clientSocket.Client.RemoteEndPoint.ToString());
                Client newClient = new Client(clientSocket, count);
                clientsList.Add(newClient);
                room1.connectedClients.Add(newClient);
                room1.CreateNewPlayer(count);
                count++;

            }
        }

        public static void UpdateGames()
        {
            while (true)
            {
                //do code
            }
        }

        public static void readConsole()
        {
            string command = null;
            while (true)
            {
                command = Console.ReadLine();
                /*if (command == "send")
                    broadcast("Hello Client!");
                else
                    ConsoleSay("Server", "Unknown command: "+command);*/
            }
        }

        public static void ConsoleSay(string from, string s)
        {
            Console.WriteLine("[" + from + "] " + s);
        }
    }
}
