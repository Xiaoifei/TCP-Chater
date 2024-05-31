using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TCP_Chater;

class Program
{
    private static bool isClose = false;
    private static Socket socket;
    private static List<Socket> socketClientsList = new List<Socket>();
    private static int socketClientNum = 0;
    private static void Main(string[] args)
    {
        Console.WriteLine("输入 command:Quit 即可退出程序");
        Console.WriteLine("输入 command:Switch 可切换对象");
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        try
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8080);
            socket.Bind(ipEndPoint);
        }
        catch (Exception e)
        {
            Console.WriteLine("绑定失败");
            Console.WriteLine(e.Message);
            throw;
        }

        socket.Listen(1024);
        Console.WriteLine("等待客户端连入");

        Thread threadAcceptConnect = new Thread(AcceptClientConnect);
        threadAcceptConnect.Start();
        threadAcceptConnect.IsBackground = true;

        Thread threadReceiveMsg = new Thread(ReceiveMsg);
        threadReceiveMsg.Start();
        threadReceiveMsg.IsBackground = true;

        while (!isClose)
        {
            string sendMsg = Console.ReadLine();
            if (string.IsNullOrEmpty(sendMsg)) continue;
            if (sendMsg.StartsWith("command:"))
            {
                sendMsg = sendMsg.Substring(8);
                if (sendMsg == "Quit")
                {
                    Console.WriteLine("closing...");
                    isClose = true;
                    break;
                }
                else if (sendMsg == "Switch")
                {
                    Console.WriteLine("选择聊天对象:(输入序号即可)");
                    for (int i = 0; i < socketClientsList.Count; i++)
                    {
                        Console.WriteLine(socketClientsList[i].RemoteEndPoint.ToString() + " 序号:" + i);
                    }
                    string tempstr = Console.ReadLine();
                    if (!string.IsNullOrEmpty(tempstr) && int.Parse(tempstr)>=0 && int.Parse(tempstr)<= socketClientsList.Count)
                    {
                        socketClientNum = int.Parse(tempstr);
                        Console.WriteLine("已切换至："+socketClientNum);
                    }
                }
            }
            else
            {
                Console.WriteLine("发送给："+socketClientNum);
                try
                {
                    socketClientsList[socketClientNum].Send(Encoding.UTF8.GetBytes(sendMsg));
                }
                catch (Exception e)
                {
                    Console.Write("发送失败，原因：");
                    Console.WriteLine(e);
                }
            }
        }
        if (isClose)
        {
            threadReceiveMsg.Join();
            //threadAcceptConnect.Join();
            for (int i = 0; i < socketClientsList.Count; i++)
            {
                socketClientsList[i].Shutdown(SocketShutdown.Both);
                socketClientsList[i].Close();
            } 
            socketClientsList.Clear();
        }
        Console.WriteLine("服务端已关闭");
    }
    static void ReceiveMsg()
    {
        byte[] result = new byte[1024];
        Socket socketClient;
        while (!isClose)
        {
            for (int i = 0; i < socketClientsList.Count; i++)
            {
                socketClient = socketClientsList[i];
                if (socketClient != null && socketClient.Available > 0)
                {
                    int receiveNum = socketClient.Receive(result);
                    Console.WriteLine("Received Message From {0}:{1}",socketClient.RemoteEndPoint.ToString(),Encoding.UTF8.GetString(result,0,receiveNum));
                }
            }
        }
    }

    static void AcceptClientConnect()
    {
        Socket socketClient;
        while (!isClose)
        {
            socketClient = socket.Accept();
            socketClientsList.Add(socketClient);
            socketClient.Send(Encoding.UTF8.GetBytes("Welcome to Connect to Server!"));
            Console.WriteLine("与 " +socketClient.RemoteEndPoint.ToString() + " 连接建立成功！");
        }
    }
}