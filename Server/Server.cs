using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class Server
    {
        private Socket socket;
        private int maxClients = int.Parse(ConfigurationManager.AppSettings["max_broj_klijenata"]);
        private bool isRunning;
        public Server() 
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ConfigurationManager.AppSettings["ip"]), int.Parse(ConfigurationManager.AppSettings["port"]));

            socket.Bind(endPoint);
            socket.Listen(maxClients);
            isRunning = true;
            Thread thread = new Thread(AcceptClient);
            thread.Start();
        }

        public void AcceptClient()
        {
            try 
            {
                while (isRunning)
                {
                    Socket clientSocket = socket.Accept();
                    ClientHandler handler = new ClientHandler(clientSocket);
                    Controller.Instance.Clients.Add(handler);
                    Thread clientThread = new Thread(handler.HandleRequest); 
                    clientThread.Start();
                }

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        public void Stop()
        {
            isRunning = false;
            socket.Close();
        }
    }
}
