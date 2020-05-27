using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScriptableUtility 
{

    public static ScriptableObject CreateScritableAsset<T>(string relative, string name, bool selectAsset) where T : ScriptableObject
    {
        if (relative == null)
            relative = "";
        if (name == null)
            name = "Default";

        bool hasRelativePath = !string.IsNullOrEmpty(relative);
        T asset = ScriptableObject.CreateInstance<T>();
        Directory.CreateDirectory(Application.dataPath + "/" + relative);
        string t = "Assets/" + (hasRelativePath ? (relative + "/") : "") + name + ".asset";
      
        AssetDatabase.CreateAsset(asset,t );
        AssetDatabase.SaveAssets();

        if (selectAsset)    
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        AssetDatabase.Refresh();
        return asset;
    }

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("" + typeof(T).Name);  
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)  
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }

    public static List<ScritableFound<T>> GetAllInstancesWithInfo<T>() where T : ScriptableObject
    {

        string[] guids = AssetDatabase.FindAssets("" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];

        List<ScritableFound<T>> list = new List<ScritableFound<T>>();
        for (int i = 0; i < guids.Length; i++)     
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            ScritableFound<T> result = new ScritableFound<T>();
            result.m_linked = a[i];
            result.m_path = path;
            list.Add(result);
        }
        return list;
    }

}

public class ScritableFound<G> where G : ScriptableObject
{
    public G m_linked;
    public string m_path;
}