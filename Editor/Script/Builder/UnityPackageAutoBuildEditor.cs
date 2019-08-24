
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
//https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html

[CustomEditor(typeof(UnityPackageAutoBuild))]
public class UnityPackageAutoBuildEditor : Editor
{
    public static string m_gitUserName;
    public static bool m_seeDefaultOne=true;

    public override void OnInspectorGUI()
    {
        //if (GUILayout.Button("Default"))
        //{
        //    m_seeDefaultOne = !m_seeDefaultOne;
        //}
        if (m_seeDefaultOne)
            DrawDefaultInspector();
   
        UnityPackageAutoBuild myScript = (UnityPackageAutoBuild)target;

        if (GUILayout.Button("Open window")) {
            UnityPackageBuilderWindow.ShowWindow(myScript.m_packageInfo);
        }
       
       
     
    }
    
}



public static class UnityPackageManagerUtility
{
    [MenuItem("Window /Package Utility / Remove Locker")]
    public static void RemoveLocker()
    {
        //string packagePath = GetProjectPackagePath();
        //string package = File.ReadAllText(packagePath);
        //package = Regex.Replace(package, "(,)[. \\n \\r]*(\"lock\":)[\\S \\r \\n { }]*", "}");
        // File.WriteAllText(packagePath, package);
        //AssetDatabase.Refresh();
        UnityPackageUtility.RemoveLocker();
    }

    private static string GetProjectPackagePath()
    {
        return Application.dataPath + "/../Packages/manifest.json";
    }
}
