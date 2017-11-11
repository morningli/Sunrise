using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Newtonsoft.Json;

public class MainButtonPressEvent : MonoBehaviour {

//    private SocketClient m_socketClient;
	public GameObject m_room;
    // Use this for initialization
    void Start () {
		//拉取房间列表
		NETWORK_DATA_PROTOCOL.Protocol protocol = new NETWORK_DATA_PROTOCOL.Protocol();
		protocol.seq = TcpManager.Instance.GetConnectSeq ();
		protocol.command = (UInt16)NETWORK_DATA_PROTOCOL.CMD.GET_ROOM_LIST;

		SignalModule.FuncWithParam1<string> callback = ShowRoomList;
		TcpManager.Instance.SendAndRecieve (JsonConvert.SerializeObject(protocol), callback);
    }

	public void ShowRoomList(string data)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(data);

		NETWORK_DATA_PROTOCOL.GetRoomListRsp respone 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.GetRoomListRsp>(protocol.request);

		if (protocol.ret == 0)
		{
			foreach (var room in respone.roomlist) 
			{
				GameObject newRoom = MonoBehaviour.Instantiate(m_room, GameObject.Find("Content").transform);
				newRoom.transform.GetComponent<RoomInfo> ().id = room.id;
				newRoom.transform.GetComponent<RoomInfo> ().name = room.name;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
