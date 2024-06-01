using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Chater;

class Program
{
    private static void Main(string[] args)
    {
        ServerSocket serverSocket = new ServerSocket();
        serverSocket.Start();
        Console.WriteLine("输入 closeme 即可退出程序");
        String str = "";
        while (true)
        {
            str = Console.ReadLine();
            if (string.IsNullOrEmpty(str))continue;
            if (str.Equals("closeme"))
            {
                Console.WriteLine("closing...");
                serverSocket.Close();
                break;
            }
            serverSocket.Send(str);
        }
        Console.WriteLine("closed");
    }
}