using UnityEngine;
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

    public string entity_name = "";

    private static GameObject directionObj = null;
    private static GameObject directionObj_sprite = null;

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
		fontStyle.normal.background = null;             //设置背景填充  
		        fontStyle.normal.textColor = Color.yellow;      //设置字体颜色  
		        fontStyle.fontSize = (int)(50.0 * gameObject.transform.localScale.x);
		fontStyle.alignment = TextAnchor.MiddleCenter;

		//绘制NPC名称
		GUI.Label(new Rect(uiposition.x - (nameSize.x / 2), uiposition.y - nameSize.y, nameSize.x, nameSize.y), entity_name, fontStyle);
	}

    void Awake()
    {
    }

    void Start()
    {
    }

	void Update()
	{
	}

    void OnDestroy()
    {
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

    public void set_modelScale(float scale)
    {
		gameObject.transform.localScale = new Vector3(scale, scale, 1f);
    }

	public void set_radius(float radius)
	{
		float x = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
		set_modelScale (radius / x);
	}
}

