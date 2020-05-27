using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WindowPlayerPref 
{

    public static void Save(string id, string text)
    {
        if (string.IsNullOrWhiteSpace(id)) 
            return;
        File.WriteAllText(Application.persistentDataPath + "/" + id + ".txt", text);
    }
    public static string Load(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) 
            return "";
        string path = Application.persistentDataPath + "/" + id + ".txt";
        if (File.Exists(path))
            return  File.ReadAllText(path);
        return "";
    }

    public static bool Has(string id)
    {
        return File.Exists(Application.persistentDataPath + "/" + id + ".txt");

    }
}
