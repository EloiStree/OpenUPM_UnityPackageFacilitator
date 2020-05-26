using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ReadMeEditor : EditorWindow
{
    public  Info m_info;
    public class Info {
        public string m_yo;

    }
    [MenuItem("Window/Package Utility/Read Me")]
    static void Init()
    {
        SampleEditor window = (SampleEditor)EditorWindow.GetWindow(typeof(SampleEditor));
        window.Show();
    }


    public UnityPathSelectionInfo m_selector;
    void OnGUI()
    {
      ReadMeFileStream f = ReadMeUtility.GetReadMeFile(m_selector);
      
        if (GUILayout.Button("Refresh")) { }
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        m_info.m_yo = EditorGUILayout.TextField("Read Me Content", m_info.m_yo, GUILayout.MinHeight(100)) ;

    }

  
}