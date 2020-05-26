
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
        if (m_seeDefaultOne)
            DrawDefaultInspector();
    }
    
}




