using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;

public class LoginDialog : BasePage {

	private Transform m_loginDialog;
	private Text m_loginTips;
	private InputField m_loginUserName;
	private InputField m_loginUserPassword;

	static LoginDialog m_instance;
	public static LoginDialog Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Login/LoginDialog").GetComponent<LoginDialog>();
			}
			return m_instance;
		}
	}

	void Awake()
	{
		m_loginTips = GameObject.Find("login_tips").GetComponent<Text>();
		m_loginUserName = GameObject.Find("login_name").GetComponent<InputField>();
		m_loginUserPassword = GameObject.Find("login_password").GetComponent<InputField>();
	}
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnLoginCallBack(string data)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(data);

		NETWORK_DATA_PROTOCOL.DoLoginRsp respone 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.DoLoginRsp> (protocol.request);

		if (protocol.ret == 0)
		{
			//保存用户信息
			Player.instance.AccountToken = respone.key;
			Player.instance.UserName = m_loginUserName.text;
			//加载下一个场景
			SceneManager.LoadScene("main", LoadSceneMode.Single);
		}
		else
		{
			m_loginTips.text = protocol.msg;
		}
	}

	public void OnLoginPress()
	{
		bool bIsRegisted = false;

		string userName = m_loginUserName.text;
		string password = m_loginUserPassword.text;

		//到后台验证用户名和密码
		NETWORK_DATA_PROTOCOL.Protocol protocol = new NETWORK_DATA_PROTOCOL.Protocol();
		protocol.seq = TcpManager.Instance.GetConnectSeq ();
		protocol.command = (UInt16) NETWORK_DATA_PROTOCOL.CMD.LOGIN;

		NETWORK_DATA_PROTOCOL.DoLoginReq request
		= new NETWORK_DATA_PROTOCOL.DoLoginReq ();

		request = new NETWORK_DATA_PROTOCOL.DoLoginReq ();
		request.name = userName;
		request.password = password;
		protocol.request = JsonConvert.SerializeObject (request);

		SignalModule.FuncWithParam1<string> callback = OnLoginCallBack;
		TcpManager.Instance.SendAndRecieve (JsonConvert.SerializeObject(protocol), callback);
	}

	public void OnRegisterPress()
	{
		PageManager.Instance.ShowDialog(RegisterDialog.Instance, PageManager.AnimationType.MiddleZoomOut);
	}

	//TODO 提示统一做到框架中
	public void SetTips(string text)
	{
		m_loginTips.text = text;
	}
}
