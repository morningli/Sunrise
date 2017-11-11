
using UnityEngine;
//using KBEngine;
using System.Collections;
using System;
using System.Xml;
using System.Collections.Generic;

public class GameEntity : MonoBehaviour
{
    private bool isMouseDown = false;

	//是否本地玩家
	public bool isPlayer = false;
	//是否玩家
    public bool isAvatar = false;

    private Vector3 _position = Vector3.zero;
    private Vector3 _eulerAngles = Vector3.zero;
    private Vector3 _scale = Vector3.zero;

    //public Vector3 destPosition = Vector3.zero;
    //public Vector3 destDirection = Vector3.zero;

	//public float _speed = 0f;

    public string entity_name = "";

    //public bool isOnGround = true;

    //public bool isControlled = false;

    //public bool entityEnabled = true;

    //private Vector3 lastMoveDir = Vector3.zero;

    private static GameObject directionObj = null;
    private static GameObject directionObj_sprite = null;

    void Awake()
    {
    }

    void Start()
    {
    }

    void OnDestroy()
    {
    }

    void OnGUI()
    {
        if (Camera.main == null || entity_name == "")
            return;

        //根据NPC头顶的3D坐标换算成它在2D屏幕中的坐标     
        Vector2 uiposition = Camera.main.WorldToScreenPoint(transform.position);

        //得到真实NPC头顶的2D坐标
		uiposition = new Vector2(uiposition.x, Screen.height - uiposition.y);

        //计算NPC名称的宽高
        Vector2 nameSize = GUI.skin.label.CalcSize(new GUIContent(entity_name));


        GUIStyle fontStyle = new GUIStyle();
        fontStyle.normal.background = null;             //设置背景填充  
        fontStyle.normal.textColor = Color.yellow;      //设置字体颜色  
        fontStyle.fontSize = (int)(50.0 * gameObject.transform.localScale.x);
        fontStyle.alignment = TextAnchor.MiddleCenter;

        //绘制NPC名称
        GUI.Label(new Rect(uiposition.x - (nameSize.x / 2), uiposition.y - nameSize.y, nameSize.x, nameSize.y), entity_name, fontStyle);
    }

    public Vector3 position
    {
        get
        {
            return _position;
        }

        set
        {
			Debug.Log ("position:" + value.ToString());

            _position = value;

            if (gameObject != null)
				gameObject.transform.localPosition = _position;
        }
    }

    public Vector3 eulerAngles
    {
        get
        {
            return _eulerAngles;
        }

        set
        {
            _eulerAngles = value;

            if (directionObj != null)
            {
				directionObj.transform.localEulerAngles = _eulerAngles;
            }
        }
    }

    public Quaternion rotation
    {
        get
        {
            return Quaternion.Euler(_eulerAngles);
        }

        set
        {
            eulerAngles = value.eulerAngles;
        }
    }

	public Vector3 direction
	{
		set{
			float angle = Vector3.Angle(Vector3.left, value); //求出两向量之间的夹角 
			Vector3 angles = eulerAngles;
			angles.z = angle;
			eulerAngles = angles;
		}
	}

    public Vector3 scale
    {
        get
        {
            return _scale;
        }

        set
        {
			Debug.Log ("scale:" + value.ToString());

            _scale = value;

            if (gameObject != null)
                gameObject.transform.localScale = _scale;
        }
    }

	/*
    public float speed
    {
        get
        {
            return _speed;
        }

        set
        {
			Debug.Log ("speed:" + value.ToString());
            _speed = value;
        }
    }*/

	//物体图片
    /*public void set_modelID(int modelID)
    {
        World worldScript = GameObject.Find("game_render").GetComponent<World>();
        Sprite sp = null;

        if(this.transform.gameObject.name.ToLower().Contains("avatar"))
        {
            sp = worldScript.avatarSprites[modelID];
        }
        else if (this.transform.gameObject.name.ToLower().Contains("food"))
        {
            sp = worldScript.foodsSprites[modelID];
        }
        else if (this.transform.gameObject.name.ToLower().Contains("smash"))
        {
            sp = worldScript.smashsSprites[modelID];
        }

        GetComponent<SpriteRenderer>().sprite = sp;
    }*/

    public void set_modelScale(float scale)
    {
		gameObject.transform.localScale = new Vector3(scale, scale, 1f);
        //UpdateDirection(Vector3.zero);
    }

	public void set_radius(float radius)
	{
		//float x = transform.localScale.x * GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		float x = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		set_modelScale (radius / x);
	}

    /*public void entityEnable()
    {
        entityEnabled = true;
    }

    public void entityDisable()
    {
        entityEnabled = false;
    }

    public void set_state(sbyte v)
    {
    }
*/
    void updatePos(Vector3 targetPos)
    {
		Debug.LogWarning ("update targetPos:" + targetPos.ToString());

        // 球身不允许超出边界
        /*Vector3 size = GetComponent<SpriteRenderer>().sprite.bounds.size;
        size *= this.transform.localScale.x;

        float x_half = (size.x / 2);
        float y_half = (size.y / 2);

        if (targetPos.x < x_half)
			Debug.LogWarning ("modify targetPos.x:" + targetPos.x.ToString() + ",x_half:" + x_half.ToString());
            targetPos.x = x_half;

        if (targetPos.x > World.GAME_MAP_SIZE - x_half)
			Debug.LogWarning ("modify targetPos.x:" + targetPos.x.ToString() + ",GAME_MAP_SIZE - x_half:" + (World.GAME_MAP_SIZE - x_half).ToString());
            targetPos.x = World.GAME_MAP_SIZE - x_half;

        if (targetPos.y < y_half)
			Debug.LogWarning ("modify targetPos.y:" + targetPos.y.ToString() + ",x_half:" + y_half.ToString());
            targetPos.y = y_half;

        if (targetPos.y > World.GAME_MAP_SIZE - y_half)
			Debug.LogWarning ("modify targetPos.y:" + targetPos.y.ToString() + ",GAME_MAP_SIZE - y_half:" + (World.GAME_MAP_SIZE - y_half).ToString());
            targetPos.y = World.GAME_MAP_SIZE - y_half;

		if (this.transform.position != targetPos) {
			Debug.LogWarning ("update pos:" + targetPos.ToString());
		}*/

        //UpdateDirection(targetPos);

		this.transform.localPosition = targetPos;
    }

    void FixedUpdate()
    {
        if (!isAvatar)
            return;

      //  if (!entityEnabled)
       //     return;

        //if (isPlayer == isControlled)
        //    return;
		
		//TODO 更新位置信息
        //KBEngine.Event.fireIn("updatePlayer", gameObject.transform.position.x, gameObject.transform.position.z, gameObject.transform.position.y, gameObject.transform.rotation.eulerAngles.y);

    }

	/*void UpdatePosition(Vector3 pos)
	{
		gameObject.transform.position = pos;
	}

	void UpdateDirection(double angle)
	{
		//gameObject.transform.rotation.eulerAngles.y = angle;
	}

    void UpdateDirection(Vector3 targetPos)
    {
        if(directionObj == null)
        {
			directionObj = (GameObject)Resources.Load ("Prefabs/Direction");
			if (directionObj) {
				Instantiate (directionObj, transform);
				directionObj_sprite = GameObject.Find ("Direction/sprite");
			}
        }

        // 更新距离
        if(targetPos == Vector3.zero)
        {
            if(directionObj_sprite)
            {
                // 球身边界
                Vector3 size = GetComponent<SpriteRenderer>().sprite.bounds.size;
                size *= this.transform.localScale.x;

                float x_half = (size.x / 2);

                directionObj_sprite.transform.localPosition = new Vector3(
					-(x_half + 1.2f), 
					directionObj_sprite.transform.localPosition.y, 
                    directionObj_sprite.transform.localPosition.z);
            }

            return;
        }

        Vector3 dir = targetPos - transform.position;
        dir.z = 0f;
        dir = dir.normalized;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotation = Quaternion.Slerp(rotation, Quaternion.Euler(0, 0, targetAngle - 180.0f), 8f * Time.deltaTime);
    }*/

    void Update()
    {
       /* if (!isAvatar)
            return;

        if (!entityEnabled)
        {
            position = destPosition;
            return;
        }

	    float deltaSpeed = (speed * Time.deltaTime);
		//Debug.LogWarning ("mouse down.speed:" + speed.ToString ());
		//Debug.LogWarning ("mouse down.deltaTime:" + Time.deltaTime.ToString ());
	
        if (isPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMouseDown = true;
            }
            if (Input.GetMouseButtonUp(0))
            {
                isMouseDown = false;
            }

            if (isMouseDown)
            {

				Debug.LogWarning ("mouse down.speed:" + speed.ToString ());
				Debug.LogWarning ("mouse down.deltaTime:" + Time.deltaTime.ToString ());

                Vector3 movement = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
                movement.z = 0.0f;

				Debug.LogWarning ("mouse down.mouseposition:" + Input.mousePosition.ToString());//(297.0, 254.0, 0.0)
				Debug.LogWarning ("mouse down.mouseposition world:" + Camera.main.ScreenToWorldPoint(Input.mousePosition).ToString());//(-0.1, 1.7, -10.0)
				Debug.LogWarning ("mouse down.position:" + this.transform.position.ToString());//(-2.3, 1.2, 0.0)
				Debug.LogWarning ("mouse down.movement:" + movement.ToString());//(2.2, 0.5, 0.0)
				                
                if (movement.magnitude <= 1.0)
                {
                    lastMoveDir = Vector3.zero;
                    return;
                }

                movement.Normalize();
                lastMoveDir = movement;
				Debug.LogWarning ("mouse down.lastMoveDir:" + lastMoveDir.ToString());//
				Debug.LogWarning ("mouse down.deltaSpeed:" + deltaSpeed.ToString());

				//移到某位置
                updatePos(this.transform.position + (movement * deltaSpeed));
            }
            else
            {
                if (lastMoveDir != Vector3.zero)
                    updatePos(this.transform.position + (lastMoveDir * deltaSpeed));
            }
        }*/

        /*else
        {
            // 如果是其他玩家移动
            float dist = Vector3.Distance(new Vector3(destPosition.x, destPosition.y, 0f),
                    new Vector3(position.x, position.y, 0f));

            if (dist > 0.01f)
            {
                Vector3 pos = position;

                Vector3 movement = destPosition - pos;
                movement.z = 0f;
                movement.Normalize();

                movement *= deltaSpeed;

                if (dist > deltaSpeed || movement.magnitude > deltaSpeed)
                    pos += movement;
                else
                    pos = destPosition;

                position = pos;
            }
            else
            {
                position = destPosition;
            }
        }*/
    }
}

