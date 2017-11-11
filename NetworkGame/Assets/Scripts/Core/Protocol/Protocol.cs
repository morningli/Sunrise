using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace NETWORK_DATA_PROTOCOL {

	enum CMD{
	HERTBEAT,
	LOGIN,
	REGISTER,
	MOVE,
	FIRE,
	EXIT,
	ENTER_ROOM,
	SYNCHRONIZE,
	GET_ROOM_LIST,
	GAME_START,
	END_ROOM,
	};

	[System.Serializable]
	public class Protocol
	{
		public UInt32          	seq;
		public UInt32          	time;
		public string          	key;
		public UInt16           command;
		public string 		    request;
		public bool            	respone;
		public Int16           	ret;
		public string          	msg;

		public string PackProtocol<T> (T data)
		{
			this.request = JsonConvert.SerializeObject (data);
			return JsonConvert.SerializeObject (this);
		}
	}

	[System.Serializable]
	public class Vector3
	{
		public float x;
		public float y;
		public float z;

		public void CloneVector3(UnityEngine.Vector3 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
		}

		public UnityEngine.Vector3 ConvertVector3()
		{
			UnityEngine.Vector3 v = new UnityEngine.Vector3 ();
			v.x = x;
			v.y = y;
			v.z = z;
			return v;
		}
	}

	//登录
	[System.Serializable]
	public class DoLoginReq 
	{
		public string name;
		public string password; 
	}

	[System.Serializable]
	public class DoLoginRsp 
	{
		public string key;
	}

	//注册
	[System.Serializable]
	public class DoRegisterReq 
	{
		public string name;
		public string password;
	}

	//拉取房间信息
	[System.Serializable]
	public class Room
	{
		public string id;
		public string name;
	}

	//[System.Serializable]
	//public class GetRoomListReq
	//{
	//}

	[System.Serializable]
	public class GetRoomListRsp
	{
		public Room[] roomlist;
	}

	//进入房间
	[System.Serializable]
	public class DoEnterRoomReq
	{
		public string name;
		public string room;
	}
		
	//同步
	[System.Serializable]
	public class PlayerStatus 
	{
		public Vector3  position;
		public Vector3 	direction;
		public float 	radius;
		public int 		type;
		public int 		experience;
	}

	[System.Serializable]
	public class DoSynchronize
	{
		public string                  				room;
		public Dictionary<string, PlayerStatus> 	status;
		public string[] 							expired;
		public int 									survive;
	}

	[System.Serializable]
	public class DoStart {
		public string room;
		public Dictionary<string, PlayerStatus> status;
		public int survive;
	}

	[System.Serializable]
	public class DoMove
	{
		public string name;
		public Vector3 destination;
	}

	[System.Serializable]
	public class DoEnd {
		public string room;
		public Dictionary<string, PlayerStatus> status;
	}
}
