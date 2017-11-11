using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndPage : BasePage {

	public GameObject result;

	static EndPage m_instance;
	public static EndPage Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Game/EndPage").GetComponent<EndPage>();
			}
			return m_instance;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetResult(string text)
	{
		result.GetComponent<Text> ().text = text;
	}
}
