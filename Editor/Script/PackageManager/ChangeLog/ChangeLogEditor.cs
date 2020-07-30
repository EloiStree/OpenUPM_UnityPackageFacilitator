using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ChangeLogEditor : EditorWindow
{

    private bool m_pathFound;
    private UnityPathSelectionInfo m_selector= new UnityPathSelectionInfo();
    private string m_version="";
    private string m_title = "";
    private string m_logs = "";
    private bool m_hide;

    [MenuItem("ꬲ🧰/Package Utility/Window/Change Log", false, 20)]
    static void Init()
    {
        ChangeLogEditor window = (ChangeLogEditor)EditorWindow.GetWindow(typeof(ChangeLogEditor));
        window.titleContent = new GUIContent("Change Log");
        window.Show();
    }

    void OnGUI()
    {
        UnityPathSelectionInfo.Get(out m_pathFound, out m_selector);
        GUILayout.Label("Focus: "+ m_selector.GetRelativePath(false), EditorStyles.boldLabel);
        DrawEditorDefaultInterface(new ChangeLogFileStream(m_selector.GetAbsolutePath(false)),ref m_version, ref m_title,ref  m_logs,ref m_hide);
    }



    public static void DrawEditorDefaultInterface(ChangeLogFileStream changelog, ref string version, ref string title, ref string logs, ref bool hide) {
        if (changelog == null) return;
        hide = EditorGUILayout.Foldout(hide, hide? "→ Log" : "↓ Log", EditorStyles.boldLabel);
        if (!hide) { 
            GUILayout.BeginHorizontal();
            GUILayout.Label("Version:", GUILayout.MaxWidth(60));
            if (version == null || version.Length <= 0)
                version = ChangeLogUtility.GetLastVersion(changelog); 
            version = ChangeLogUtility.OnlyDigitsAndPoints(
                EditorGUILayout.TextField(version, GUILayout.MaxWidth(80)));
            GUILayout.Label("Title:", GUILayout.MaxWidth(40));
            title = EditorGUILayout.TextField(title, GUILayout.MaxWidth(1000));
            GUILayout.EndHorizontal();
            GUILayout.Label("New(s):", GUILayout.MaxWidth(60));
            logs = ChangeLogUtility.StartWithDash(
                EditorGUILayout.TextArea( logs, GUILayout.MinHeight(80)));
            GUILayout.BeginHorizontal();
            if (!changelog.Exist() &&  GUILayout.Button("Create ChangeLog.md"))
            {
                ChangeLogUtility.Create(changelog, "# Change Log ", "Find here developer log of this package.  ");
            }
            if (changelog.Exist() && GUILayout.Button("Push to log"))
            {

                ChangeLogUtility.AppendLog(changelog, version, title, logs);
            }
            if (changelog.Exist() && GUILayout.Button("Open")) { if (changelog.Exist()) changelog.Open(); }
            GUILayout.EndHorizontal();
        }

    }

}