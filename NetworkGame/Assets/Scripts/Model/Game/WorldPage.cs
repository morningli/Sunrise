using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

public class WorldPage : BasePage
{
    private UnityEngine.GameObject terrain = null;
    public UnityEngine.GameObject terrainPerfab;

    public UnityEngine.GameObject foodsPerfab;
    public UnityEngine.GameObject smashsPerfab;
    public UnityEngine.GameObject avatarPerfab;

    public Sprite[] avatarSprites = new Sprite[2];
    public Sprite[] foodsSprites = new Sprite[3];
    public Sprite[] smashsSprites = new Sprite[1];

    public static float GAME_MAP_SIZE = 20.0f;
    public static int ROOM_MAX_PLAYER = 0;
    public static int GAME_ROUND_TIME = 0;
	private int m_survive = 0;
	public bool isdead = false;

	public UInt32 m_time = 0;
	public Dictionary<string, GameEntity> m_players = new Dictionary<string, GameEntity>();

	private bool isMouseDown = false;

	static WorldPage m_instance;
	public static WorldPage Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Game/WorldPage").GetComponent<WorldPage>();
			}
			return m_instance;
		}
	}

    void Awake()
    {
		SignalModule.FuncWithParam1<string> callback = UpdatePlayerStatus;
		SignalModule.SignalMgr.instance.Subscribe ((long)SignalModule.GAME_EVT.DATA_SYNC, callback);
    }

	void OnGUI()
	{
		GUIStyle fontStyle = new GUIStyle();
		fontStyle.normal.background = null;             //设置背景填充  
		       fontStyle.normal.textColor = Color.yellow;      //设置字体颜色  
		        fontStyle.fontSize = (int)(50.0 * gameObject.transform.localScale.x);
		fontStyle.alignment = TextAnchor.MiddleCenter;

		string num_tips = "幸存人数:" + survive.ToString();
		if (isdead) {
			num_tips += " 游戏结束";
		}
		GUI.Label(new Rect(10, 10, 20, 20), num_tips,fontStyle);
	}

	public void UpdatePlayerStatus(Dictionary<string, NETWORK_DATA_PROTOCOL.PlayerStatus> playerStatus)
	{
		foreach (var status in playerStatus)
		{
			if (!m_players.ContainsKey (status.Key)) {
				//加载预载体资源
				GameObject hp_bar;
				if (status.Value.type == 0) {
					hp_bar = (GameObject)Resources.Load ("Prefabs/Game/avatar1");
				} else {
					hp_bar = (GameObject)Resources.Load ("Prefabs/Game/Food");

				}//实例化预设体  
				var player = Instantiate (hp_bar, transform) as GameObject;
				m_players [status.Key] = player.GetComponent<GameEntity> ();

				if (status.Key == Player.instance.UserName) {
					m_players [status.Key].isPlayer = true;

					Camera.main.transform.parent = player.transform;
					Camera.main.transform.localPosition = new Vector3(0.0f, 0.0f, - 10.0f);
				}
				if (status.Value.type == 0) {
					m_players [status.Key].isAvatar = true;
					m_players [status.Key].entity_name = status.Key;
				}
			}

			m_players [status.Key].isAvatar = true;
			m_players [status.Key].isPlayer = true;

			m_players [status.Key].position = status.Value.position.ConvertVector3 ();
			m_players [status.Key].direction = status.Value.direction.ConvertVector3 ();
			m_players [status.Key].set_radius (status.Value.radius);
		}
	}

	public uint time {
		get{
			return m_time;
		}
		set{
			m_time = value;
		}
	}
	public int survive{
		get{
			return m_survive;
		}
		set{
			m_survive = value;
		}
	}
	public void UpdatePlayerStatus(string data)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(data);

		time = protocol.time;
			
		NETWORK_DATA_PROTOCOL.DoSynchronize request 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.DoSynchronize> (protocol.request);

		if (protocol.time < time) {
			return;
		}
			
		UpdatePlayerStatus (request.status);

		if (request.expired != null) {
			foreach (var playerid in request.expired) {
				if (m_players.ContainsKey (playerid)) {
					if (playerid == Player.instance.UserName) {
						//玩家死亡，游戏结束
						isdead = true;
					}
					m_players [playerid].gameObject.SetActive (false);
				} else {
					Debug.Log ("not found:" + playerid);
				}
			}
		}

		//幸存人数
		survive = request.survive;
	}

    // Use this for initialization
    void Start()
    {
    }
		
    void OnDestroy()
    {
    }
    
    // Update is called once per frame
    void Update()
    {
		if (isdead) {
			//玩家死亡不允许控制角色
			return;
		}
		
		if (Input.GetMouseButtonDown(0))
		{
			isMouseDown = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			isMouseDown = false;
		}

		if (isMouseDown) {

			//请求移动
			NETWORK_DATA_PROTOCOL.Protocol protocol = new NETWORK_DATA_PROTOCOL.Protocol();
			protocol.seq = TcpManager.Instance.GetConnectSeq ();
			protocol.command = (UInt16)NETWORK_DATA_PROTOCOL.CMD.MOVE;
			protocol.time = m_time + 1;
			protocol.key = Player.instance.AccountToken;

			var request =  new NETWORK_DATA_PROTOCOL.DoMove ();
			request.name = Player.instance.UserName;

			var position_world = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Debug.Log ("mouse point:" + position_world.ToString());
			var dest = transform.InverseTransformPoint (position_world);
			Debug.Log ("mouse point relative:" + dest.ToString());
			request.destination = new NETWORK_DATA_PROTOCOL.Vector3 ();
			request.destination.CloneVector3(dest);

			TcpManager.Instance.Send (protocol.PackProtocol(request), "sence");
		}
    }
		
	public override void PageDidDisappear()
	{
		Camera.main.transform.parent = null;
		Camera.main.transform.position = new Vector3(0.0f, 0.0f, - 10.0f);
	}
}
