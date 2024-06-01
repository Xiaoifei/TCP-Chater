using System.Net.Sockets;
using System.Text;

namespace TCP_Chater;

public class ClientSocket
{
    private static int CLINET_COUNTER = 0; 
    private int clientID;
    public Socket socket;

    public ClientSocket(Socket socket)
    {
        CLINET_COUNTER++;
        clientID = CLINET_COUNTER;
        this.socket = socket;
    }

    public void Close()
    {
        if (socket != null)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            socket = null;
        }
    }

    public void Send(String msg)
    {
        if (socket != null)
        {
            try
            {
                socket.Send(Encoding.UTF8.GetBytes(msg));
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch (ObjectDisposedException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
        }
    }

    public void Received()
    {
        if (socket != null)
        {
            if (socket.Available > 0)
            {
                byte[] bytes = new byte[1024];
                socket.Receive(bytes);
                ThreadPool.QueueUserWorkItem(HandleMsg,Encoding.UTF8.GetString(bytes));
            }
        }
    }

    public void HandleMsg(Object obj)
    {
        String msg = obj as string;
        Console.WriteLine(msg);
    }
}