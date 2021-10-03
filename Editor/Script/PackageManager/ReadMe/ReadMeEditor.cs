using Eloi;
using System;
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
    [MenuItem("ꬲ🧰/Package Utility/Window/Read Me", false, 20)]
    static void Init()
    {
        ReadMeEditor window = (ReadMeEditor)EditorWindow.GetWindow(typeof(ReadMeEditor));
        window.titleContent = new GUIContent("Read Me");
        window.Show();
    }


    public UnityPathSelectionInfo m_selector;
    private bool m_pathFound;
    private GitLinkOnDisk m_gitLink;
    private bool m_hide;
    private string m_text;

    void OnGUI()
    {

        UnityPathSelectionInfo.Get(out m_pathFound, out m_selector);
        GUILayout.Label("Read Me: " + m_selector.GetRelativePath(false), EditorStyles.boldLabel);
        ReadMeFileStream f = ReadMeUtility.GetReadMeFile(m_selector);
        QuickGit.GetGitInParents(m_selector.GetAbsolutePath(false),QuickGit.PathReadDirection.LeafToRoot, out m_gitLink);

        //QuickGit.GetGitInDirectory(m_selector.GetAbsolutePath(false), out m_gitLink, true);
        DrawEditorDefaultInterface(f, ref m_gitLink, ref m_text, ref m_hide) ;

    }

    public  static void DrawEditorDefaultInterface(AbstractFileStream readme,ref  GitLinkOnDisk gitLink, ref string readMeText, ref bool hide)
    {
        hide = EditorGUILayout.Foldout(hide, hide ? "→ Read Me" : "↓ Read Me", EditorStyles.boldLabel);
        if (!hide)
        {
            GUILayout.Label("Read Me:", EditorStyles.boldLabel);
            GUILayout.Label("Linked git:" +(gitLink==null?"None":gitLink.GetName()), EditorStyles.boldLabel); ;
            readMeText = EditorGUILayout.TextArea(readMeText, GUILayout.MinHeight(100));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Create Default"))
            {
                if (gitLink != null)
                    readme.Create(ReadMeUtility.CreateBasicDefaultOnFrom(gitLink));
                else readme.Create("# Read Me  \n Hey buddy!  \nWhat 's up ?");
                readMeText = readme.Get();
            }
            if (GUILayout.Button("Load")) {readMeText = readme.Get(); }
            if (GUILayout.Button("Override"))
            {
                readme.Set(readMeText);
            }
            if (GUILayout.Button("Open"))
            {
                readme.Open();
            }
            GUILayout.EndHorizontal();
        }
    }

}