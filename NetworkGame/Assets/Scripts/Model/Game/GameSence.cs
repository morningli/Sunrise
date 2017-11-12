using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class GameSence : MonoBehaviour {
	
	void Awake()
	{
		Global.SceneAwake();
	}

	// Use this for initialization
	void Start () {
		Global.SceneStart();
		PageManager.Instance.ShowPage(WaitPage.Instance);

		//加入游戏时需要判断是否结束
		SignalModule.FuncWithParam1<string> callback = OnGameOver;
		SignalModule.SignalMgr.instance.Subscribe ((long)SignalModule.GAME_EVT.DATA_SYNC, callback);
	}

	// Update is called once per frame
	void Update () {
		Global.Update();
	}

	public void OnGameOver(string data)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol> (data);

		if (protocol.command == (UInt16)NETWORK_DATA_PROTOCOL.CMD.END_ROOM) {
			NETWORK_DATA_PROTOCOL.DoEnd request 
			= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.DoEnd> (protocol.request);

			string end_text = "winer:";
			foreach (var status in request.status)
			{
				end_text += status.Key;
				break;
			}

			PageManager.Instance.ShowPage (EndPage.Instance);
			EndPage.Instance.SetResult (end_text);
		}
	}
}
