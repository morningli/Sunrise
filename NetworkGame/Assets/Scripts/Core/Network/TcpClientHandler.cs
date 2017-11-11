using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;  
using System.Net.Sockets;  
using System.Text;  
using System.Threading;  
using LitJson;

public class TcpClientHandler{
	
	Socket serverSocket; //服务器端socket  
	//private string ClientIP = "192.168.1.139";//IP地址，改成自己的  
	IPAddress ip; //主机ip  
	IPEndPoint ipEnd;  
	string recvStr; //接收的字符串  
	string sendStr; //发送的字符串  
	byte[] recvData=new byte[1024]; //接收的数据，必须为字节  
	byte[] sendData=new byte[1024]; //发送的数据，必须为字节  
	int recvLen = 0; //接收的数据长度  
	Thread connectThread; //连接线程
	bool socket_ready = false;

	private Queue<string> queue = new Queue<string> ();//新建一个队列，用来存储服务器发来的数据  

	//服务器初始化  
	public void InitSocket(string serverIP, int serverPort)  
	{  
		ip=IPAddress.Parse(serverIP);  
		ipEnd=new IPEndPoint(ip, serverPort); //服务器端口号，自己改

		//开启一个线程连接  
		connectThread=new Thread(new ThreadStart(SocketReceive));  
		connectThread.Start();  
	}  

	//接收服务器消息  
	void SocketConnet()  
	{  		
		if (serverSocket != null) {
			serverSocket.Close ();
		}
		//定义套接字类型,必须在子线程中定义  
		serverSocket=new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);  
		//连接  
		serverSocket.Connect(ipEnd);

		Debug.Log ("connect succ.");

		socket_ready = true;

		//接收数据
		recvLen=serverSocket.Receive(recvData);  
		recvStr=Encoding.UTF8.GetString(recvData,0,recvLen);    
		queue.Enqueue (recvStr);
	}  

	public void SocketSend(string sendStr)  
	{  
		while (!socket_ready) {
			Thread.Sleep (0);
		}

		//清空发送缓存  
		sendData=new byte[1024];  
		//数据类型转换  
		sendData=Encoding.UTF8.GetBytes(sendStr);  
		//发送
		serverSocket.Send(sendData,sendData.Length,SocketFlags.None);
	}

	void SocketReceive()  
	{    
		SocketConnet();
		//不断接收服务器发来的数据  
		while(true)  
		{
			recvData = new byte[1024];  
			recvLen = serverSocket.Receive(recvData);  
			if(recvLen == 0)
			{  
				SocketConnet();  
				continue;  
			}  
			recvStr=Encoding.UTF8.GetString(recvData,0,recvLen);  
			queue.Enqueue (recvStr);  

			Debug.Log ("Receive succ");
		}  
	}  

	//返回接收到的字符串  
	public string GetRecvStr()  
	{  
		string returnStr;  
		//加锁防止字符串被改  
		lock(this)  
		{  
			if (queue.Count > 0) {//队列内有数据时，取出  
				returnStr = queue.Dequeue ();  
				return returnStr;  
			}       }      
		return null;  
	}  

	public void SocketQuit()  
	{  
		//关闭线程  
		if(connectThread!=null)  
		{  
			connectThread.Interrupt();  
			connectThread.Abort();  
		}  
		//最后关闭服务器  
		if(serverSocket!=null)  
			serverSocket.Close();  
	}  
}
