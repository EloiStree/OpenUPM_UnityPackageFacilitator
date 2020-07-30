using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DocumentationEditor : EditorWindow
{
    public Info m_info;
    public class Info
    {
        public string m_yo;

    }
    [MenuItem("ꬲ🧰/ Package Utility")]
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

    public static void DrawInfoAboutInterface(DocumentationDirectoryStream documentation)
    {
    }
}