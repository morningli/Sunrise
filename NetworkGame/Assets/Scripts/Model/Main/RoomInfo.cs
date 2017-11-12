using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System;

public class RoomInfo : MonoBehaviour {

	public string id;
	public string name;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Find ("Text").GetComponent<Text> ().text = name;
	}

	public void OnOpenRoom()
	{
		//获取房间号
		int iRoomID = int.Parse(id);
		//保存数据
		PlayerPrefs.SetInt("roomid", iRoomID);
		//请求加入场景
		NETWORK_DATA_PROTOCOL.Protocol protocol = new NETWORK_DATA_PROTOCOL.Protocol();
		protocol.seq = TcpManager.Instance.GetConnectSeq ();
		protocol.command = (UInt16)NETWORK_DATA_PROTOCOL.CMD.ENTER_ROOM;

		NETWORK_DATA_PROTOCOL.DoEnterRoomReq request = new NETWORK_DATA_PROTOCOL.DoEnterRoomReq ();
		request.name = Player.instance.UserName;
		request.room = iRoomID.ToString();
		protocol.request = JsonConvert.SerializeObject (request);
					
		SignalModule.FuncWithParam1<string> callback = EnterSence;
		TcpManager.Instance.SendAndRecieve (JsonConvert.SerializeObject(protocol), callback, "sence");
	}

	public void EnterSence(string respone)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(respone);

		if (protocol.ret == 0) {
			//加载下一个场景
			SceneManager.LoadScene ("game", LoadSceneMode.Single);
		} else {
			Debug.LogError (protocol.msg);
		}
	}
}
