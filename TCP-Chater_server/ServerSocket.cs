using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TCP_Chater
{
    public class ServerSocket
    {
        private Socket serverSocket;
        private List<ClientSocket> clientLists = new List<ClientSocket>();
        private bool isRunning;

        public void Start()
        {
            try
            {
                isRunning = true;
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(10);
                ThreadPool.QueueUserWorkItem(Accept, null);
                ThreadPool.QueueUserWorkItem(Receive, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable Initialize"+e.Message);
                serverSocket.Close();
            }
        }

        public void Send(string msg)
        {
            for (int i = 0; i < clientLists.Count; i++)
            {
                clientLists[i].Send(msg);
            }
        }

        public void Accept(Object obj)
        {
            while (isRunning)
            {
                try
                {
                    Socket socket = serverSocket.Accept();
                    ClientSocket clientSocket = new ClientSocket(socket);
                    clientLists.Add(clientSocket);
                    clientSocket.Send("Welcome to Server!");
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Accpet Method is forced to Close");
                    Console.WriteLine(e.Message);
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine("wa2");
                }
                catch (Exception e)
                {
                    Console.WriteLine("wa3");
                }
            }
        }

        public void Receive(Object obj)
        {
            while (isRunning)
            {
                for (int i = 0; i < clientLists.Count; i++)
                {
                    clientLists[i].Received();
                }
            }
        }

        public void Close()
        {
            isRunning = false;
            for (int i = 0; i < clientLists.Count; i++)
            {
                clientLists[i].Close();
            }
            clientLists.Clear();
            if (serverSocket != null)
            {
                serverSocket.Close();
                serverSocket = null;
            }
        }
    }
}
