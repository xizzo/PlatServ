using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PlatServ
{
    class Client
    {
        public string IPAdress;
        public TcpClient clientSocket;
        public Boolean alive;
        public string userName;
        public int clientID;
        public Boolean newUpdate = false;

        Thread handleClientThread;

        public string xPos;
        public string yPos;


        public Client(TcpClient socket, int ClientID)
        {
            this.clientSocket = socket;
            this.IPAdress = socket.Client.RemoteEndPoint.ToString();
            Program.ConsoleSay("SERVER","Client ip " + this.IPAdress);
            this.alive = true;
            this.clientID = ClientID;

            handleClientThread = new Thread(handleClient);
            handleClientThread.Start();
        }

        public void SendToClient(string s)
        {
            if (this.alive)
            {
                try
                {
                    NetworkStream networkStream = this.clientSocket.GetStream();
                    Byte[] broadcastBytes = null;

                    broadcastBytes = Encoding.ASCII.GetBytes(s + "$");
                    networkStream.Write(broadcastBytes, 0, broadcastBytes.Length);
                    networkStream.Flush();
                }
                catch
                {

                }
            }
        }

        string lastData = "";
        private void handleClient()
        {
            if (!this.clientSocket.Connected)
            {
                KillConnection();
            }
            byte[] bytesFrom = new byte[100025];
            string dataFromClient = null;
            while (this.alive)
            {
                try
                {
                    NetworkStream networkStream = clientSocket.GetStream();
                    networkStream.Read(bytesFrom, 0, (int)clientSocket.ReceiveBufferSize);
                    dataFromClient = System.Text.Encoding.ASCII.GetString(bytesFrom);
                    string validation = dataFromClient.Substring(dataFromClient.IndexOf("$") + 1, dataFromClient.IndexOf("^") - dataFromClient.IndexOf("$") - 1);
                    dataFromClient = dataFromClient.Substring(0, dataFromClient.IndexOf("$"));

                    if (lastData == validation)
                    {
                        KillConnection();
                    }
                    else
                        lastData = validation;

                    ReadData(dataFromClient);

                }
                catch (Exception)
                {
                    if (this.alive)
                    {
                        //endConnection();
                    }
                }
            }
        }

        private void ReadData(string s)
        {
            int code = int.Parse(s.Substring(0, 3));
            string val = GetDataFromCode(s);
            switch (code)
            {
                case 000:
                    this.userName = val;
                    Program.ConsoleSay(this.IPAdress, "Username is : " + this.userName);
                    break;
                case 001:
                    xPos = val.Substring(0, val.IndexOf(":"));
                    yPos = val.Substring(val.IndexOf(":") + 1, val.Length - val.IndexOf(":") - 1);
                    Program.ConsoleSay(this.IPAdress, "Pos update: " + xPos + " : " + yPos);
                    newUpdate = true;
                    break;
                case 100:
                    Program.RemoveClientID(this.clientID);
                    KillConnection();
                    break;

            }
        }

        private string GetDataFromCode(string s)
        {
            s = s.Substring(s.IndexOf("%") + 1, s.Length - s.IndexOf("%") - 1);
            return s;
        }

        private void KillConnection()
        {
            endConnection();
            alive = false;
        }

        private void endConnection()
        {
            Program.ConsoleSay("Server", "Client(" + this.IPAdress + ") disconnected");
            this.alive = false;
            this.clientSocket.Close();
            this.handleClientThread.Abort();
        }
    }
}
