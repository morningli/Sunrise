using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

public class ServerTest
{
    static void Main(string[] args)
    {
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        TcpListener tlistener = new TcpListener(ip, 10001);
        tlistener.Start();
        Console.Write("服务器监听启动......");
        while (true)
        {
            TcpClient remoteClient = tlistener.AcceptTcpClient();
            Console.WriteLine("客户端已连接！local:{0}<---Client:{1}", remoteClient.Client.LocalEndPoint, remoteClient.Client.RemoteEndPoint);
        }
    }
}
