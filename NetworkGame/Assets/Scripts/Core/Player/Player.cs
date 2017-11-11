using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {
	public static Player instance = new Player();

	public Player()
	{
	}

	public string AccountToken
	{
		get {
			return PlayerPrefs.GetString ("AccountToken");
		}
		set {
			PlayerPrefs.SetString("AccountToken", value);
		}
	}

	public string UserName
	{
		get {
			return PlayerPrefs.GetString ("UserName");
		}
		set {
			PlayerPrefs.SetString("UserName", value);
		}
	}
}
