using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public class SettingReader
{
    Dictionary<string, string> m_valueDic = new Dictionary<string, string>();
    string m_path;
    string m_text;
    public void LoadFromFile(string path)
    {
        m_text = File.OpenText(path).ReadToEnd();
        ParseSetting();
    }
    public void LoadFromString(string setting)
    {
        m_path = null;
        m_text = setting;
        ParseSetting();
    }

    void ParseSetting()
    {
        int lineID = 0;
        StringReader reader = new StringReader(m_text);
        while (true)
        {
            lineID++;
            string line = reader.ReadLine();
            if (line == null)
            {
                break;
            }
            line = line.Trim();
            if (line.Length==0)
            {
                continue;
            }
            if (line.Substring(0, 2) == "//")
            {
                continue;
            }
            string[] kv = line.Split(new char[] { '=' }, StringSplitOptions.None);
            if (kv.Length != 2)
            {
                Debug.LogError("Expression Error->File-" + m_path + ",line-" + lineID);
                continue;
            }
            if (m_valueDic.ContainsKey(kv[0]))
            {
                Debug.LogError("Key Repeat->File-" + m_path + ",line-" + lineID);
                continue;
            }
            m_valueDic.Add(kv[0], kv[1]);
        }
    }

    bool ContainsKey(string key)
    {
        return m_valueDic.ContainsKey(key);
    }

    public string GetString(string key)
    {
        string value;
        if (!m_valueDic.TryGetValue(key, out value))
        {
            Debug.LogError("Key is not exist->key:" + key);
            return "";
        }
        return value;
    }

    public int GetInt(string key)
    {
        string value = GetString(key);
        int result;
        if (!int.TryParse(value, out result))
        {
            Debug.LogError("Value is not int->key:" + key);
            return 0;
        }
        return result;
    }

    public float GetFloat(string key)
    {
        string value = GetString(key);
        float result;
        if (!float.TryParse(value, out result))
        {
            Debug.LogError("Value is not float->key:" + key);
            return 0;
        }
        return result;
    }

    public double GetDouble(string key)
    {
        string value = GetString(key);
        double result;
        if (!double.TryParse(value, out result))
        {
            Debug.LogError("Value is not double->key:" + key);
            return 0;
        }
        return result;
    }


}


