using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;


public class Global
{
    
    public static bool alreadySetupWhenAwake = false;
    public static bool alreadySetupWhenStart = false;


    //Path
    public static string StorageDataPath;
    public static string DownloadPath;
    public static string WarriorPath;
    public static string ImagePath;
    public static string MapPath;
    public static string UserDataPath;
    public static string TempPath;

    public static Font Arial;
    ///////////////////////////////////////////////////////////////////////////////////////////
    public static void SceneAwake()
    {
        if (!alreadySetupWhenAwake)
        {
            SetupOnceWhenAwake();
            alreadySetupWhenAwake = true;
        }
        SetupEverySceneWhenAwake();
    }
    public static void SceneStart()
    {
        if (!alreadySetupWhenStart)
        {
            SetupOnceWhenStart();
            alreadySetupWhenStart = true;
        }
        SetupEverySceneWhenStart();
    }

    static void SetupOnceWhenAwake()
    {
        if (Config.userLocalResource)
        {
            StorageDataPath = Application.dataPath + "/../../UserStorage/";
        }
        else
        {
            StorageDataPath = Application.persistentDataPath + "/";
        }
        DownloadPath = StorageDataPath + "Download/";
        WarriorPath = DownloadPath + "Config/Warrior/";
        ImagePath = DownloadPath + "Resources/";
        MapPath = DownloadPath + "Config/Map/";

        UserDataPath = StorageDataPath + "UserData/";
        TempPath = Application.temporaryCachePath + "/";

        DOTween.Init();

        //NetworkManager.Instance.useFakeData = false;
        
        //Text label=ResourceManager.LoadGameObject("Prefabs/Common/BaseLabel").GetComponent<Text>();
		//Arial = label.font;
        //GameObject.Destroy(label.gameObject);

        //WarriorTemplateManager.Instance.path = Global.WarriorPath;
    }

    static void SetupEverySceneWhenAwake()
    {
        
    }

    static void SetupOnceWhenStart()
    {
        
    }

    static void SetupEverySceneWhenStart()
    {
        /*if (Config.showDebugButton)
        {
            DebugPage.CreateDebugButton();
        }*/
        if (Config.showLogOnScreen)
        {
            GameObject go = new GameObject("OutLog");
            go.AddComponent<OutLog>();
        }
    }

    public static void Update()
    {
//        EventManager.Instance.Update();
 //       NetworkManager.Instance.Update();
    }



}
