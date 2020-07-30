using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SampleEditor : EditorWindow
{
    public  Info m_info;
    public class Info {
        public string m_yo;

    }
    //[MenuItem("ꬲ🧰/Package Utility/Window/Sample", false, 20)]
    static void Init()
    {
        SampleEditor window = (SampleEditor)EditorWindow.GetWindow(typeof(SampleEditor));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Refresh")) { }
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        m_info.m_yo = EditorGUILayout.TextField("Text Field", m_info.m_yo);

    }

    public static void DrawInfoAboutInterface(SampleDirectoryStream sample)
    {



    }
}