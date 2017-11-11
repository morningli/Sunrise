using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipDialog : BasePage {

	private Text m_tip;

	static TipDialog m_instance;
	public static TipDialog Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Common/TipDialog").GetComponent<TipDialog>();
			}
			return m_instance;
		}
	}

	void Awake()
	{
		m_tip = GameObject.Find ("Text").GetComponent<Text> ();
	}

	public string tip {
		get{
			return m_tip.text;
		}
		set{
			m_tip.text = value;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
