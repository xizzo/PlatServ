using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PlatServ
{
    class Room
    {
        public List<Client> connectedClients = new List<Client>();
        Thread handleClientThreads;

        public Room()
        {
            handleClientThreads = new Thread(handleClients);
            handleClientThreads.Start();
        }

        private void handleClients()
        {
            while (true)
            {
                try
                {
                    foreach (Client cl in connectedClients)
                    {
                        if (cl.newUpdate && cl.alive)
                        {
                            TransmitPositionToOthers(cl.clientID, cl.xPos, cl.yPos);
                            cl.newUpdate = false;
                        }
                    }
                }
                catch
                { 

                }

            }
        }

        private void TransmitPositionToOthers(int clientID, string xPos, string yPos)
        {
            foreach(Client client in connectedClients)
            {
                if(client.clientID != clientID && client.alive)
                {
                    client.SendToClient("002%" + clientID + ":" + xPos + ":" + yPos);
                }
            }
        }

        public void CreateNewPlayer(int ClientID)
        {
            Program.ConsoleSay("ROOM", "Announcing a new client with ID " + ClientID.ToString());
            foreach (Client client in connectedClients)
            {
                if (client.clientID != ClientID)
                {
                    client.SendToClient("001%" + ClientID.ToString() + ":0:0");
                }
                else
                {
                    foreach (Client c in connectedClients)
                    {
                        if (c.clientID != ClientID)
                        {
                            client.SendToClient("001%" + c.clientID.ToString() + ":0:0");
                        }
                    }
                }
            }
        }
    }
}
