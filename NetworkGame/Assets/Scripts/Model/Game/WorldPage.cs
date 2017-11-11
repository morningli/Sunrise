//using KBEngine;
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

//    private UnityEngine.GameObject player = null;
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
        //DontDestroyOnLoad(transform.gameObject);
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
				//m_players [status.Key].position = status.Value.position.ConvertVector3();

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
						//transform.DetachChildren()
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
        //installEvents();
    }

	/*
    void installEvents()
    {
        // in world
        KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
        KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
        KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
        KBEngine.Event.registerOut("set_position", this, "set_position");
        KBEngine.Event.registerOut("set_direction", this, "set_direction");
        KBEngine.Event.registerOut("updatePosition", this, "updatePosition");
        KBEngine.Event.registerOut("onControlled", this, "onControlled");
        KBEngine.Event.registerOut("onSetSpaceData", this, "onSetSpaceData");
        KBEngine.Event.registerOut("onDelSpaceData", this, "onDelSpaceData");

        // in world(register by scripts)
        KBEngine.Event.registerOut("onAvatarEnterWorld", this, "onAvatarEnterWorld");
        KBEngine.Event.registerOut("set_name", this, "set_entityName");
        KBEngine.Event.registerOut("set_state", this, "set_state");
        KBEngine.Event.registerOut("set_moveSpeed", this, "set_moveSpeed");
        KBEngine.Event.registerOut("set_modelScale", this, "set_modelScale");
        KBEngine.Event.registerOut("set_modelID", this, "set_modelID");
    }
*/
    void OnDestroy()
    {
        //KBEngine.Event.deregisterOut(this);
		//角色控制
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

			TcpManager.Instance.Send (protocol.PackProtocol(request));
		}
    }

	/*//加载地图。。。
    public void addSpaceGeometryMapping(string respath)
    {
        Debug.Log("loading scene(" + respath + ")...");
        UI.inst.info("");

        if (terrain == null && terrainPerfab != null)
            terrain = Instantiate(terrainPerfab) as UnityEngine.GameObject;

        if (player)
            player.GetComponent<GameEntity>().entityEnable();
    }*/

	//进入场景
    public void onAvatarEnterWorld(UInt64 rndUUID, Int32 eid/*, KBEngine.Avatar avatar*/)
    {
        /*if (!avatar.isPlayer())
        {
            return;
        }*/

        UI.inst.info("loading scene...(加载场景中...)");
        Debug.Log("loading scene...");

        //createPlayer();
    }

	/*
	//创建玩家
    public void createPlayer()
    {
		//玩家已存在，直接退出
        if (player != null)
        {
            player.GetComponent<GameEntity>().entityEnable();
            return;
        }


        if (KBEngineApp.app.entity_type != "Avatar")
        {
            return;
        }

        KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
        if (avatar == null)
        {
            Debug.Log("wait create(palyer)!");
            return;
        }

        // 玩家默认在第0层，越小的应该越在下一层， 大的覆盖小的
        float layer = 0.0f;

        player = Instantiate(avatarPerfab, new Vector3(avatar.position.x, avatar.position.z, layer),
                             Quaternion.Euler(new Vector3(avatar.direction.y, avatar.direction.z, avatar.direction.x))) as UnityEngine.GameObject;

        GameEntity entity = player.GetComponent<GameEntity>();
        entity.entityDisable();
        avatar.renderObj = player;
        entity.isAvatar = true;
        entity.isPlayer = true;

        // 有必要设置一下，由于该接口由Update异步调用，有可能set_position等初始化信息已经先触发了
        // 那么如果不设置renderObj的位置和方向将为0
        set_position(avatar);
        set_direction(avatar);

        Camera.main.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, - 10.0f);
        Camera.main.transform.parent = player.transform;
    }
		
    public void onEnterWorld(KBEngine.Entity entity)
    {
        if (entity.isPlayer())
        {
            createPlayer();
        }
        else
        {
            UnityEngine.GameObject entityPerfab = null;

            float layer = 0.0f;

            if (entity.className == "Food")
            {
                entityPerfab = foodsPerfab;
                layer = 100.0f;
            }
            else if (entity.className == "Smash")
            {
                layer = -9.0f;

                // 粉碎永远都应该在所有avatar和粮食的上面一层
                entityPerfab = smashsPerfab;
            }
            else
            {
                entityPerfab = avatarPerfab;
            }

            entity.renderObj = Instantiate(entityPerfab, new Vector3(entity.position.x, entity.position.z, layer),
                Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;

            ((UnityEngine.GameObject)entity.renderObj).name = entity.className + "_" + entity.id;

            if (entity.className == "Avatar")
            {
                ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().isAvatar = true;
            }
        }
    }

    public void onLeaveWorld(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        UnityEngine.GameObject.Destroy((UnityEngine.GameObject)entity.renderObj);
        entity.renderObj = null;
    }

    public void onSetSpaceData(UInt32 spaceID, string key, string value)
    {
        if ("GAME_MAP_SIZE" == key)
            GAME_MAP_SIZE = float.Parse(value);
        else if("ROOM_MAX_PLAYER" == key)
            ROOM_MAX_PLAYER = int.Parse(value);
        else if("GAME_MAP_SIZE" == key)
            GAME_ROUND_TIME = int.Parse(value);
    }

    public void onDelSpaceData(UInt32 spaceID, string key)
    {

    }

    public void set_position(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        GameObject go = ((UnityEngine.GameObject)entity.renderObj);
        Vector3 currpos = new Vector3(entity.position.x, entity.position.z, go.transform.position.z);
        go.GetComponent<GameEntity>().destPosition = currpos;
        go.GetComponent<GameEntity>().position = currpos;
    }
	
    public void updatePosition(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
        GameObject go = ((UnityEngine.GameObject)entity.renderObj);
        gameEntity.destPosition = new Vector3(entity.position.x, entity.position.z, go.transform.position.z);
        gameEntity.isOnGround = entity.isOnGround;
    }

    public void onControlled(KBEngine.Entity entity, bool isControlled)
    {
        if (entity.renderObj == null)
            return;

        GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
        gameEntity.isControlled = isControlled;
    }

    public void set_direction(KBEngine.Entity entity)
    {
        if (entity.renderObj == null)
            return;

        ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().destDirection =
            new Vector3(entity.direction.y, entity.direction.z, entity.direction.x);
    }

    public void set_entityName(KBEngine.Entity entity, object v)
    {
        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().entity_name = (string)v;
        }
    }

    public void set_state(KBEngine.Entity entity, object v)
    {
        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().set_state((SByte)v);
        }

        if (entity.isPlayer())
        {
            Debug.Log("player->set_state: " + v);

            if (((SByte)v) == 1)
                UI.inst.showReliveGUI = true;
            else
                UI.inst.showReliveGUI = false;

            return;
        }
    }

    public void set_moveSpeed(KBEngine.Entity entity, object v)
    {
        float fspeed = (float)v;

        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().speed = fspeed;
        }
    }

    public void set_modelScale(KBEngine.Entity entity, object v)
    {
        float modelScale = ((float)v);

        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().set_modelScale(modelScale);
        }
    }

    public void set_modelID(KBEngine.Entity entity, object v)
    {
        Byte modelID = ((Byte)v);

        if (entity.renderObj != null)
        {
            ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().set_modelID(modelID);
        }
    }
    */

	public override void PageDidDisappear()
	{
		Camera.main.transform.parent = null;
		Camera.main.transform.position = new Vector3(0.0f, 0.0f, - 10.0f);

		Debug.Log ("PageDidDisappear");
	}
}
