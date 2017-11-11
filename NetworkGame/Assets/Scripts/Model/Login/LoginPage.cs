using UnityEngine;
using System.Collections;
//using game_proto;

public class LoginPage : BasePage 
{
    static LoginPage m_instance;
    public static LoginPage Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = ResourceManager.LoadGameObject("Prefabs/Login/LoginPage").GetComponent<LoginPage>();
            }
            return m_instance;
        }
    }

    void Awake()
    {
//        UIEventListener.Get(this.gameObject.FindChild("QQLoginButton")).onClick = OnQQLoginButtonClick;
//        UIEventListener.Get(this.gameObject.FindChild("WXLoginButton")).onClick = OnWXLoginButtonClick;
    }

	public override void PageDidAppear()
    {
 	    base.PageDidAppear();
		PageManager.Instance.ShowDialog(LoginDialog.Instance, PageManager.AnimationType.MiddleZoomOut);
    }

	// Event
    void OnQQLoginButtonClick(GameObject go)
    {	
//		LoginModel.Instance.LoginToServer();
    }

    void OnWXLoginButtonClick(GameObject go)
    {
//		LoginReq req = new LoginReq ();
//		req.token = "test";
//		req.channel = (int)LoginChannel.kChannelTypeQQ;
//		NetworkManager.Instance.Send(req);
        //Application.LoadLevel("Menu");
    }
}
