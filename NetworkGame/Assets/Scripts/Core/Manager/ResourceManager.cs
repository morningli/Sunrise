using UnityEngine;
using System.Collections;
using System.IO;

public class ResourceManager
{
    public static Object Load(string path)
    {
        return Resources.Load(Util.ConvertPath(path));
    }

    public static GameObject LoadGameObject(string path)
    {
		Debug.Log ("path:" + path);
        Object obj = Load(path);
        return GameObject.Instantiate(obj) as GameObject;
    }

    public static Texture2D LoadTexture(string path)
    {
        FileStream file = File.OpenRead(Util.ConvertPath(path));
        byte[] data = new byte[file.Length];
        file.Read(data, 0, (int)file.Length);
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(data);
        return tex;
    }
}
