using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

//https://learn.unity.com/tutorial/introduction-to-scriptable-objects#5cf187b7edbc2a31a3b9b123
public class PackagePullPushWindow : EditorWindow
{
    [MenuItem("Window / Facilitator/Pull Push")]
    public static void ShowWindow()
    {
        PackagePullPushWindow win = (PackagePullPushWindow)EditorWindow.GetWindow(typeof(PackagePullPushWindow));
        win.name = "Pull Push";
        win.titleContent.text = "Pull Push";
    }

    public PackagePullPushObject m_pushPullInfo;
    public PackagePullPush m_pushPull;

    void OnGUI()
    {
        m_pushPullInfo = (PackagePullPushObject)EditorGUILayout.ObjectField(m_pushPullInfo, typeof(PackagePullPushObject));
        m_pushPull = m_pushPullInfo.m_data;
        if (m_pushPull != null)
        {
           
            GUILayout.BeginHorizontal();
            GUILayout.Label("Namespace", GUILayout.Width(70));
            m_pushPull.m_packageNamespaceId = GUILayout.TextArea(m_pushPull.m_packageNamespaceId);
         
            GUILayout.Label("Git", GUILayout.Width(40));
            m_pushPull.m_gitUrl = GUILayout.TextArea(m_pushPull.m_gitUrl);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Share", GUILayout.Width(40));
            GUILayout.TextArea(string.Format("\"{0}\":\"{1}\",", m_pushPull.m_packageNamespaceId, m_pushPull.m_gitUrl));
            GUILayout.EndHorizontal();
            
            m_folderFoldout = EditorGUILayout.Foldout(m_folderFoldout ,"Folder");
            if (m_folderFoldout) {

                GUILayout.BeginHorizontal();
                string pathToWork = GetPathOfFolder();
                m_pushPull.m_relativeFolderPath = GUILayout.TextArea(m_pushPull.m_relativeFolderPath);
                if (!Directory.Exists(pathToWork) && GUILayout.Button("Create folder"))
                {
                    Directory.CreateDirectory(GetPathOfFolder());
                    RefreshDataBase();
                }
                else if(Directory.Exists(pathToWork) && GUILayout.Button("Remove Folder"))
                {
                    Directory.Delete(GetPathOfFolder());
                    RefreshDataBase();
                }

                if (!(Directory.Exists(pathToWork) &&  Directory.GetFiles(pathToWork).Length>0)  && GUILayout.Button("Clone Git"))
                {
                    Directory.CreateDirectory(GetPathOfFolder());
                    QuickGit.Clone(m_pushPull.m_gitUrl, GetPathOfFolder());
                    RefreshDataBase();
                }

                GUILayout.EndHorizontal();

                QuickGit.DisplayEditorCommands(GetPathOfFolder());
            }

            m_upDownFoldout = EditorGUILayout.Foldout(m_upDownFoldout, "Pull Push");
            if (m_upDownFoldout)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Down"))
                {
                    Debug.Log("Not implemented yet");
                    //QuickGit.
                    RefreshDataBase();
                }
                if (GUILayout.Button("Up"))
                {

                    Debug.Log("Not implemented yet");
                    RefreshDataBase();
                }
                GUILayout.EndHorizontal();
                
            }


            }
    }
    public static bool m_folderFoldout;
    public static bool m_upDownFoldout;

    private void RefreshDataBase()
    {
        AssetDatabase.Refresh();
    }

    private string GetPathOfFolder()
    {
        return Application.dataPath + "/" + m_pushPull.m_relativeFolderPath;
    }
}
