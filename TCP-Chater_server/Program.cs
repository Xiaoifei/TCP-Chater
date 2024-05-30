using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Chater;

class Program
{
private static Socket socketClient;
    private static bool isClose = false;
    private static void Main(string[] args)
    {
        Console.WriteLine("输入 command:Quit 即可退出程序");
        // 1. 创建套接字Socket
        Socket socketTcp = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        // 2. 用Bind方法将套接字与本地地址绑定
        try
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            socketTcp.Bind(ipEndPoint);
        }
        catch (Exception e)
        {
            Console.WriteLine("绑定失败"+e.Message);
            throw;
        }
        // 3. 用Listen方法监听
        socketTcp.Listen(1);
        Console.WriteLine("等待客户端连入");
        // 4. 用Accept方法等待客户端连接
        // 5. 建立连接，Accept返回新套接字
        socketClient = socketTcp.Accept();
        Console.WriteLine("连接建立成功！");
        // 6. 用Send和Receive相关方法收发数据
        Thread threadReceiveMsg = new Thread(ReceiveMsg);
        threadReceiveMsg.Start();
        socketClient.Send(Encoding.UTF8.GetBytes("Welcome to Connect to Server!"));
        string sendMsg = Console.ReadLine();
        while (!isClose)
        {
            if (sendMsg == "command:Quit")
            {
                isClose = true;
                break;
            }
            socketClient.Send(Encoding.UTF8.GetBytes(sendMsg));
            sendMsg = Console.ReadLine();
        }
        //关闭线程
        if (isClose)
        {
            threadReceiveMsg.Join();
        }        
        // 7. 用Shutdown方法释放连接
        socketClient.Shutdown(SocketShutdown.Both);
        // 8. 关闭套接字
        socketClient.Close();
        Console.WriteLine("服务端已关闭");
    }
    static void ReceiveMsg()
    {
        byte[] result = new byte[1024];
        while (!isClose)
        {
            if (socketClient.Available > 0)
            {
                int receiveNum = socketClient.Receive(result);
                Console.WriteLine("Received Message From {0}:{1}",socketClient.RemoteEndPoint.ToString(),Encoding.UTF8.GetString(result,0,receiveNum));
            }
        }
    }
}