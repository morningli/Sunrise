using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSence : MonoBehaviour {

	void Awake()
	{
		Global.SceneAwake();
	}

	// Use this for initialization
	void Start () {
		Global.SceneStart();
		PageManager.Instance.ShowPage(MainPage.Instance);
	}
	
	// Update is called once per frame
	void Update () {
		Global.Update();
	}

}
