using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Ping 
{
    public static void PingFolder(string relativeFolderName, bool fromAssets)
    {
        if (relativeFolderName[relativeFolderName.Length - 1] == '/')
            relativeFolderName = relativeFolderName.Substring(0, relativeFolderName.Length - 1);
        
        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath((fromAssets?"Assets/":"") + relativeFolderName, typeof(UnityEngine.Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);

        UnityEngine.Object[] selection = new UnityEngine.Object[1];
        selection[0] = obj;
        Selection.objects = selection;

    }
}
