using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : BasePage {
	
	static MainPage m_instance;
	public static MainPage Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Main/MainPage").GetComponent<MainPage>();
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
}
