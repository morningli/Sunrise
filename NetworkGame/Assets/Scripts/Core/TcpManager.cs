using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;  
using UnityEngine.SceneManagement;
using System;  

public class TcpManager : MonoBehaviour {

	public static TcpManager _instance;  
	public Dictionary<string, TcpClient> connections = new Dictionary<string, TcpClient>();

	static public TcpManager Instance
	{
		get
		{
			if (_instance == null) {
				_instance = UnityEngine.Object.FindObjectOfType (typeof(TcpManager)) as TcpManager;
				if (_instance == null) {
					GameObject go = new GameObject ("_TcpManager");
					DontDestroyOnLoad (go);
					_instance = go.AddComponent<TcpManager> ();
				}	
			}
			return _instance;
		}
	}

	void Awake(){  
		//初始化网络连接
		connections.Add("default", new TcpClient());
		connections ["default"].init ("127.0.0.1", 8888);
		//connections ["default"].init ("119.29.174.231", 8888);
		connections.Add("sence", new TcpClient());
		connections ["sence"].init ("127.0.0.1", 8888);

		print ("awake.socket init");
	}  

	// Use this for initialization  
	void Start()  
	{  
	}  

	// Update is called once per frame  
	void Update()  
	{  
		foreach (TcpClient client in connections.Values) {
			client.recieve ();
		}
	}  

	void OnApplicationQuit()  
	{  
		//退出时关闭连接  
		close();
		print ("Quit");  
	}  

	public uint GetConnectSeq(string key = "default"){
		return connections [key].get_seq ();
	}

	public void close(){  
		foreach (TcpClient client in connections.Values) {
			client.close ();
		}
		print ("closeC");  
	} 	

	public void Send(string request, string key = "default")
	{
		connections [key].send (request);
		print ("Send data succ.");
	}

	public void SendAndRecieve(string request, Delegate callback, string key = "default")
	{
		connections [key].send (request, callback);
		print ("SendAndRecieve data succ.");
	}

	public void OnDestroy(){
		//退出时关闭连接  
		close();  
		print ("Destroy"); 
	}

}
