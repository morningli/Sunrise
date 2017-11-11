using UnityEngine;
using System.Collections;
using System.IO;

public class Setting
{
    static Setting m_instance;
    public static Setting Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new Setting();
            }
            return m_instance;
        }
    }
    Setting()
    {
        TextAsset defaultSettingAsset = ResourceManager.Load("Text/DefaultSetting") as TextAsset;
        defaultSetting = new SettingReader();
        currentSetting = new SettingReader();
        if (defaultSettingAsset==null)
        {
            Debug.LogError("defaultSetting is not exist");
        }
        else
        {
            defaultSetting.LoadFromString(defaultSettingAsset.text);
        }
        if (File.Exists(Application.persistentDataPath + "/CurrentSetting.txt"))
        {
            currentSetting.LoadFromFile(Application.persistentDataPath + "/CurrentSetting.txt");
        }
        else
        {
            currentSetting.LoadFromString(defaultSettingAsset.text);
        }
        
    }

    public SettingReader defaultSetting;
    public SettingReader currentSetting;
}
