using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Chater_client;

class Program
{
    // 1. 创建套接字Socket
    static Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    static bool isClose = false;
    static void Main(string[] args)
    {
        // 2. 用Connect方法与服务器相连接
        try
        {
            Console.WriteLine("请输入IP(回车则使用默认值):");
            string ip = Console.ReadLine();
            if (string.IsNullOrEmpty(ip))
            {
                ip = "127.0.0.1";
            }
            Console.WriteLine("请输入Port:(回车则使用默认值)");
            string port = Console.ReadLine();
            if (string.IsNullOrEmpty(port))
            {
                port = "8080";
            }
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(ip),int.Parse(port));
            socketTcp.Connect(ipEndPoint);
        }
        catch (SocketException e)
        {
            if (e.ErrorCode == 10061)
            {
                Console.WriteLine("服务器拒绝连接");
            }
            else
            {
                Console.WriteLine("连接失败" + e.ErrorCode);
            }
            socketTcp.Close();
            return;
        }
        Console.WriteLine("------ TcpSocket聊天程序 ------\n" + "   tips:输入 command:Quit 即可退出程序\n");
        // 3. 用Send和Receive相关方法收发数据
        Thread threadReceiveMsg = new Thread(ReceiveMsg);
        threadReceiveMsg.Start();
        socketTcp.Send(Encoding.UTF8.GetBytes("Hello i'm client!"));
        string sendMsg = Console.ReadLine();
        while (!isClose)
        {
            if (sendMsg == "command:Quit")
            {
                isClose = true;
                break;
            }
            socketTcp.Send(Encoding.UTF8.GetBytes(sendMsg));
            sendMsg = Console.ReadLine();
        }
        //关闭线程
        if (isClose)
        {
            threadReceiveMsg.Join();
        }
        //threadSendMsg.Join();
        // 4. 用Shutdown方法释放连接
        socketTcp.Shutdown(SocketShutdown.Both);
        // 5. 关闭套接字
        socketTcp.Close();
        Console.WriteLine("客户端已关闭");
    }
    static void ReceiveMsg()
    {
        byte[] result = new byte[1024];
        while (!isClose)
        {
            if (socketTcp.Available > 0)
            {
                int receiveNum = socketTcp.Receive(result);
                Console.WriteLine($"Received Message From {socketTcp.RemoteEndPoint.ToString()}:{Encoding.UTF8.GetString(result, 0, receiveNum)}");
            } 
        }
    }
}
