using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System;

public class RegisterDialog : BasePage {

	private InputField m_registerUserName;
	private InputField m_registerPassword;
	private InputField m_registerPassword2;

	static RegisterDialog m_instance;
	public static RegisterDialog Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = ResourceManager.LoadGameObject("Prefabs/Login/RegisterDialog").GetComponent<RegisterDialog>();
			}
			return m_instance;
		}
	}

	void Awake()
	{
		m_registerUserName = GameObject.Find("register_name").GetComponent<InputField>();
		m_registerPassword = GameObject.Find("register_password1").GetComponent<InputField>();
		m_registerPassword2 = GameObject.Find("register_password2").GetComponent<InputField>();

		//m_socketClient = GameObject.Find("Network").GetComponent<SocketClient>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnRegisterCallBack(string respone)
	{
		NETWORK_DATA_PROTOCOL.Protocol protocol 
		= JsonConvert.DeserializeObject<NETWORK_DATA_PROTOCOL.Protocol>(respone);

		if (protocol.ret == 0)
		{
			LoginDialog.Instance.SetTips("注册成功");
		}
		else
		{
			LoginDialog.Instance.SetTips(protocol.msg);
		}
	}

	public void OnRegisterOKPress()
	{
		PageManager.Instance.ShowDialog(LoginDialog.Instance, PageManager.AnimationType.MiddleZoomOut);

		if (m_registerPassword.text != m_registerPassword2.text) {
			LoginDialog.Instance.SetTips("两次密码输入不一致");
			return;
		}
			
		//到后台注册用户
		NETWORK_DATA_PROTOCOL.Protocol protocol = new NETWORK_DATA_PROTOCOL.Protocol();
		protocol.seq = TcpManager.Instance.GetConnectSeq ();
		protocol.command = (UInt16) NETWORK_DATA_PROTOCOL.CMD.REGISTER;

		NETWORK_DATA_PROTOCOL.DoRegisterReq request 
		= new NETWORK_DATA_PROTOCOL.DoRegisterReq ();

		request = new NETWORK_DATA_PROTOCOL.DoRegisterReq ();
		request.name = m_registerUserName.text;
		request.password = m_registerPassword.text;

		protocol.request = JsonConvert.SerializeObject (request);

		SignalModule.FuncWithParam1<string> callback = OnRegisterCallBack;
		TcpManager.Instance.SendAndRecieve (JsonConvert.SerializeObject(protocol), callback); 
	}

	public void OnRegisterCancelPress()
	{
		PageManager.Instance.ShowDialog(LoginDialog.Instance, PageManager.AnimationType.MiddleZoomOut);
	}
}
