using UnityEngine;
using System.Collections;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

public class Util 
{
    public static Transform FindTransform(string name,Transform tran)
    {
        if (tran.name.Equals(name))
        {
            return tran;
        }
        for (int i = 0; i < tran.childCount; i++)
        {
            Transform t = tran.GetChild(i);
            t = FindTransform(name, t);
            if (t!=null)
            {
                return t;
            }
        }
        return null;
    }
    public static GameObject FindChild(string name,GameObject obj)
    {
        Transform t=FindTransform(name,obj.transform);
        if (!t)
	    {
		    return null; 
	    }
        return t.gameObject;
    }
    public static void AddChild(GameObject obj,GameObject parent)
    {
		obj.transform.SetParent(parent.transform);
        obj.transform.localScale = new Vector3(1, 1, 1);
        obj.transform.localPosition = new Vector3(0, 0, 0);
    }
    public static void Unzip(string filepath,string targetDir)
    {
        if (!File.Exists(filepath))
        {
            UnityEngine.Debug.LogWarning("Can't find the file: " + filepath);
            return;
        }

        using (ZipInputStream s = new ZipInputStream(File.OpenRead(filepath)))
        {
            ZipEntry theEntry;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = targetDir + Path.GetDirectoryName(theEntry.Name);
                string fileName = Path.GetFileName(theEntry.Name);
                string fullPath = directoryName + "/" + fileName;

                // create directory
                if (directoryName.Length > 0)
                {
                    Directory.CreateDirectory(directoryName);
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    using (FileStream streamWriter = File.Create(fullPath))
                    {
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    public static string ConvertPath(string path)
    {
        return path.Replace('\\', '/');
    }
}
