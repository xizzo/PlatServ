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

        static Room room1;
        static TcpListener serverSocket;
        static TcpClient clientSocket;

        static Boolean isRunning = false;

        static void Main(string[] args)
        {
            isRunning = true;
            serverSocket = new TcpListener(new IPEndPoint(IPAddress.Parse("192.168.0.142"), 7777));
            clientSocket = default(TcpClient);

            readConsoleThread = new Thread(readConsole);
            readConsoleThread.Start();

            serverSocket.Start();
            updateGameThread.Start();
            ConsoleSay("SERVER", "Started on " + serverSocket.LocalEndpoint);
            room1 = new Room();
            int count = 0;

            while (isRunning)
            {

                try
                {
                    clientSocket = serverSocket.AcceptTcpClient();
                    ConsoleSay("SERVER", "Client(" + count + ") connect: " + clientSocket.Client.RemoteEndPoint.ToString());
                    Client newClient = new Client(clientSocket, count);
                    clientsList.Add(newClient);
                    room1.connectedClients.Add(newClient);
                    room1.CreateNewPlayer(count);
                    count++;
                }
                catch
                {
                    // server shutting down
                    isRunning = false;
                }

            }
        }

        private static void ExitServer()
        {
            try
            {
                isRunning = false;
                serverSocket.Stop();
                Environment.Exit(1);
            }
            catch(Exception ex)
            {
                ConsoleSay("SERVER", ex.Message);
            }
        }

        public static void UpdateGames()
        {
            while (isRunning)
            {
                //do code
            }
        }

        public static void readConsole()
        {
            string command = null;
            while (isRunning)
            {
                command = Console.ReadLine();
                List<string> paramValues = new List<string>();
                paramValues = command.Split(' ').ToList<string>();
                if (paramValues.Count > 1)
                    command = paramValues[0];
                switch(command)
                {
                    case "help" :
                        ConsoleSay("RESPONSE", "Available commands:");
                        ConsoleSay("RESPONSE", "CMD: clear");
                        ConsoleSay("RESPONSE", "CMD: clients");
                        break;
                    case "clear" : 
                        clientsList.Clear();
                        room1.connectedClients.Clear();
                        ConsoleSay("RESPONSE", "Cleared connections.");
                        break;
                    case "kick":
                        int clientID;
                        if (paramValues.Count == 2)
                        {
                            if (int.TryParse(paramValues[1], out clientID) && ClientIDExists(clientID))
                            {
                                RemoveClientID(clientID);
                                ConsoleSay("RESPONSE", "Kicking client with id: " + paramValues[1]);
                            }
                            else
                            {
                                ConsoleSay("RESPONSE", "Please usa a valid ID");
                            }
                        }
                        else
                        {
                            ConsoleSay("RESPONSE", "Please usa a valid ID");
                        }
                        break;
                    case "clients":
                        Boolean hasClients = false;
                        foreach(Client cl in clientsList)
                        {
                            ConsoleSay("RESPONSE", "[" + cl.clientID + "] " + cl.IPAdress);
                            hasClients = true;
                        }
                        if (!hasClients)
                            ConsoleSay("RESPONSE", "No clients connected");
                        break; 
                    case "exit":
                        ExitServer();
                        break;
                    default :
                        ConsoleSay("RESPONSE", "Invalid command!");
                        break;
                }
            }
        }

        public static bool ClientIDExists(int clientID)
        {
            bool val = false;
            foreach (Client cl in clientsList)
            {
                if (cl.clientID == clientID)
                {
                    val = true;
                    return val;
                }
            }
            return val;
        }

        public static void ConsoleSay(string from, string s)
        {
            Console.WriteLine("[" + from + "] " + s);
        }

        public static void RemoveClientID(int clientID)
        {
            foreach(Client cl in clientsList)
            {
                if(cl.clientID == clientID)
                {
                    clientsList.Remove(cl);
                    room1.RemoveClientIDFromRoom(clientID);
                    ConsoleSay("SERVER", "Client [" + cl.clientID + "] " + cl.IPAdress + " disconnected");
                    return;
                }
            }
        }

    }
}
