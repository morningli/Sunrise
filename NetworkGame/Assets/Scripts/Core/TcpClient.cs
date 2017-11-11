using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

//单个链接
using System;


public class TcpClient{

	public int level = 0;
	public uint seq = 0;

	private TcpClientHandler client = new TcpClientHandler ();
	private Queue<string> recieve_data = new Queue<string>();

	public void init(string ip, int port)
	{
		client.InitSocket (ip, port);
	}

	public void send(string data){
		client.SocketSend (protocol_begin.ToString()); 
		client.SocketSend (data); 
		client.SocketSend (protocol_end.ToString());
	}
		
	public uint get_seq()
	{
		return seq++;
	}

	private ulong get_signal(ulong seq)
	{
		ulong result = (ulong)SignalModule.GAME_EVT.SIGNAL_CUSTOM_MIN + seq;
		Debug.Log ("get signal:" + result.ToString());
		return result;
	}

	public void send(string request, Delegate callback)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(request);

		//注册回调
		SignalModule.SignalMgr.instance.Subscribe (get_signal((ulong)protocol.seq), callback, true);
		send (request);

		Debug.Log ("send data." + request);
	}
		
	private const char protocol_begin = (char)2;
	private const char protocol_end = (char)3;
	private string buf_temp;

	public void recieve()
	{  
		string returnData = client.GetRecvStr ();

		if (returnData != null) 
		{
			foreach (char ch in returnData)
			{
				switch (ch) 
				{
				case protocol_begin:
					buf_temp = "";
					break;
				case protocol_end:
//					Debug.Log (buf_temp);
					recieve_data.Enqueue (buf_temp);
					break;
				default:
					buf_temp += ch;
					break;
				}
			}

			while (recieve_data.Count > 0) {
				string respone = recieve_data.Dequeue ();

				Debug.Log ("recieve data." + respone);

				NETWORK_DATA_PROTOCOL.Protocol protocol 
				= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(respone);

				if (protocol.respone) {
					//事件分发
					SignalModule.SignalMgr.instance.Raise (get_signal ((ulong)protocol.seq), respone);
				} else {
					SignalModule.SignalMgr.instance.Raise ((ulong)SignalModule.GAME_EVT.DATA_SYNC, respone);
				}
			}
		}
	}

	public void close()
	{
		client.SocketQuit();
	}
}
