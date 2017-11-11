using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class WaitPage : BasePage {

	static WaitPage m_instance;
	public static WaitPage Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Game/WaitPage").GetComponent<WaitPage>();
			}
			return m_instance;
		}
	}

	// Use this for initialization
	void Start () {
		SignalModule.FuncWithParam1<string> callback = OnGameStart;
		SignalModule.SignalMgr.instance.Subscribe ((long)SignalModule.GAME_EVT.DATA_SYNC, callback, true);
	 }
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnGameStart(string data)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol> (data);

		NETWORK_DATA_PROTOCOL.DoStart request 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.DoStart> (protocol.request);

		WorldPage.Instance.UpdatePlayerStatus (request.status);
		WorldPage.Instance.survive = request.survive;
		PageManager.Instance.ShowPage (WorldPage.Instance);
	}


}
