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

    public static PackagePullPushObject m_pushPullInfo;
    public static PackagePullPush m_pushPull;

    void OnGUI()
    {
        m_pushPullInfo = (PackagePullPushObject)EditorGUILayout.ObjectField(m_pushPullInfo, typeof(PackagePullPushObject));
        if (m_pushPullInfo == null)
            return;
        m_pushPull = m_pushPullInfo.m_data;
        if (m_pushPull != null)
        {

            GUILayout.BeginHorizontal();
            GUILayout.Label("Namespace", GUILayout.Width(70));
            m_pushPull.m_packageNamespaceId = GUILayout.TextArea(m_pushPull.m_packageNamespaceId);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Git", GUILayout.Width(40));
            m_pushPull.m_gitUrl = GUILayout.TextArea(m_pushPull.m_gitUrl);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Share", GUILayout.Width(40));
            GUILayout.TextArea(string.Format("\"{0}\":\"{1}\",", m_pushPull.m_packageNamespaceId, m_pushPull.m_gitUrl));
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Folder", GUILayout.Width(40));
            m_pushPull.m_relativeFolderPath = GUILayout.TextArea(m_pushPull.m_relativeFolderPath);
            GUILayout.EndHorizontal();
            m_folderFoldout = EditorGUILayout.Foldout(m_folderFoldout, "Folder & Git");
            if (m_folderFoldout)
            {

                GUILayout.BeginHorizontal();
                string pathToWork = GetPathOfFolder();
                if (!Directory.Exists(pathToWork) && GUILayout.Button("Create folder"))
                {
                    Directory.CreateDirectory(GetPathOfFolder());
                    RefreshDataBase();
                }
                else if (Directory.Exists(pathToWork) && GUILayout.Button("Remove Folder"))
                {
                    FileUtil.DeleteFileOrDirectory(GetPathOfFolder());
                    RefreshDataBase();
                }

                if (!(Directory.Exists(pathToWork) && Directory.GetFiles(pathToWork).Length > 0) && GUILayout.Button("Clone Git"))
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

            CreateDownUpButton();
            GUILayout.Space(6);


            GUILayout.BeginHorizontal();


        }
    }

    int affectPackageManager = 0;
    private  void CreateDownUpButton()
    {


        string link = m_pushPull.m_gitUrl;
        string directoryPath = GetPathOfFolder();
        string namespaceid = m_pushPull.m_packageNamespaceId;
        bool affectPackage = true;
        bool isDirectoryCreated = Directory.Exists(directoryPath);
        string folderUrl = "";
        bool isGitFolderPresent = QuickGit.GetGitUrl(directoryPath, out folderUrl);
        GUIStyle disableStyle = GetDisableStyle();
        GUIStyle enableStyle = GetEnableStyle();

        string[] options = new string[]
        {
     "Affect package", "Just Down or Upload"
        };
        affectPackageManager = EditorGUILayout.Popup( affectPackageManager, options, GUILayout.Width(200));
        affectPackage = affectPackageManager == 0;
        
        GUILayout.BeginHorizontal();
        bool downAllow = isDirectoryCreated;
        if (GUILayout.Button("Down", downAllow ? enableStyle: disableStyle, GUILayout.Width(100)))
        {
            if (downAllow)
                UnityPackageUtility.Down(directoryPath, link, affectPackage);
        }
        bool upAllow= isDirectoryCreated && isGitFolderPresent ;
        if (GUILayout.Button("Up", upAllow ? enableStyle : disableStyle, GUILayout.Width(100)))
        {
            if (upAllow)
                UnityPackageUtility.Up(directoryPath,namespaceid, link, affectPackage);
        }
        GUILayout.EndHorizontal();
    }

    private static GUIStyle GetDisableStyle()
    {
        var disableStyle = new GUIStyle(GUI.skin.button);
        disableStyle.normal.textColor = new Color(0.6627451f, 0.6627451f, 0.6627451f);
        return disableStyle;
    }

    private static GUIStyle GetEnableStyle()
    {
        var enableStyle = new GUIStyle(GUI.skin.button);
        enableStyle.normal.textColor = new Color(0f, 0.4f, 0f);
        return enableStyle;
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
