using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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


    public string m_copyPastShortCut="";
    public string m_gitLinkToCreate;
    public string m_nameSpaceToCreate;
    public string m_folderToCreate;


    Vector2 scroll;
    void OnGUI()
    {
        DisplayPullPushInProject();

        if (m_pushPullInfo == null)
        {

            if (GUILayout.Button("Create Default"))
            {
                m_pushPullInfo = GetDefaultPullPushObject();
                m_pushPullInfo.m_data.m_gitUrl = m_copyPastShortCut;
                m_pushPullInfo.m_data.m_packageNamespaceId = m_nameSpaceToCreate;
            }
        }


        m_pushPullInfo = (PackagePullPushObject)EditorGUILayout.ObjectField(m_pushPullInfo, typeof(PackagePullPushObject));
        if (m_pushPullInfo != null)
            m_pushPull = m_pushPullInfo.m_data;
        else m_pushPull = null;

        GUILayout.BeginHorizontal();
        GUILayout.Label("Copy>Past Shortcut", GUILayout.Width(100));
        m_copyPastShortCut = GUILayout.TextArea(m_copyPastShortCut);
        m_copyPastShortCut = m_copyPastShortCut.Trim();
        GUILayout.EndHorizontal();

        GUILayout.Button("", GUILayout.Height(1));

        if (!string.IsNullOrEmpty(m_copyPastShortCut))
        {
            //\w*\.git
            string isGitLink = "\\w*\\.git";
            //".*"\s*:\s*".*\.git["\s\n\r\z]
            string isPackageManagerLink = "\".*\"\\s*:\\s*\".*\\.git[\"\\s\\n\\r\\a,]";


            //"be.eloiexperiments.overrideandroidvolume":"https://gitlab.com/eloistree/2019_06_30_overrideandroidvolume.git",
            if (MathRegex(m_copyPastShortCut, isPackageManagerLink))
            {
                string[] tokens = Regex.Split(m_copyPastShortCut, "\"\\s*:\\s*\"");
                tokens[0] = tokens[0].Replace("\"", "");
                tokens[1] = tokens[1].Replace("\"", "");

                CheckOrCreateDefault();
                m_pushPullInfo.m_data.m_gitUrl = tokens[1];
                m_pushPullInfo.m_data.m_packageNamespaceId = tokens[0];
                m_pushPullInfo.m_data.m_relativeFolderPath = GetGitProjectName(m_copyPastShortCut);
                m_copyPastShortCut = "";

            }
            //https://gitlab.com/eloistree/2019_06_30_overrideandroidvolume.git
            else if (MathRegex(m_copyPastShortCut, isGitLink))
            {
                CheckOrCreateDefault();
                m_pushPullInfo.m_data.m_gitUrl = m_copyPastShortCut;
                m_pushPullInfo.m_data.m_packageNamespaceId = TryToGetNameSpaceFromPublicWebPackageInRoot(m_copyPastShortCut);
                m_pushPullInfo.m_data.m_relativeFolderPath = GetGitProjectName(m_copyPastShortCut);
                m_copyPastShortCut = "";
            }
        }




        scroll = EditorGUILayout.BeginScrollView(scroll);


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

                CreateDownUpButton();
                QuickGit.DisplayEditorCommands(GetPathOfFolder());
            }

            //m_upDownFoldout = EditorGUILayout.Foldout(m_upDownFoldout, "Pull Push");
            //if (m_upDownFoldout)
            //{

            //}

            GUILayout.Space(6);

        }
        EditorGUILayout.EndScrollView();
    }

    private static void DisplayPullPushInProject()
    {

       List<ScritableFound<PackagePullPushObject>> objs = ScriptableUtility.GetAllInstancesWithInfo<PackagePullPushObject>();
        //Debug.Log("Objs c: " + objs.Count);
        //Debug.Log("Objs l: " + dd.Length);

        string[] packages = objs.Where(k =>k!=null).Select(k => GetName(k)).ToArray();
        int selected = 0;
        string[] options = packages;
        selected = EditorGUILayout.Popup("", selected, options);
    }

    private static string GetName(ScritableFound<PackagePullPushObject> pullPush)
    {
        if (pullPush == null)
            return "A";
        if (pullPush.m_linked == null)
            return "B";
        if (pullPush.m_linked.m_data == null)
            return "C";
        return pullPush.m_path ;
    }

    private string TryToGetNameSpaceFromPublicWebPackageInRoot(string gitLink)
    {
        string link = gitLink.Replace(".git", "/raw/master/package.json");
        try
        {
            WebClient c = new WebClient();
            string text = c.DownloadString(link);
            //Debug.Log("Yes: " + link);
            //Debug.Log("Text:" + text);
            string namespaceRaw = Regex.Match(text,"\"name\":\\s*\".*\"").Value;
            string[] token = Regex.Split(namespaceRaw, "\"\\s*:\\s*\"");
            if (token.Length == 2)
            {
                return token[1].Trim().Trim('"');
            }

            return "";
        }
        catch (Exception) {
            //Debug.Log("Nope: "+ link);
            return "";
        }
        
    }

    //https://gitlab.com/eloistree/2019_06_30_overrideandroidvolume.git
    private string GetGitProjectName(string text)
    {
        Match m = Regex.Match(text, "\\w*\\.git");
        if (m.Success)
            return m.Value.Substring(0, m.Value.Length-4);
        return "";
    }

    private static PackagePullPushObject GetDefaultPullPushObject(string name="")
    {
        if (name == "")
            name = "Default";
        return (PackagePullPushObject)ScriptableUtility.CreateScritableAsset<PackagePullPushObject>("Facilitator", "Default", true);
    }

    private bool MathRegex(string text, string regex)
    {
        return (new Regex(regex)).IsMatch(m_copyPastShortCut);
    }

    private void CheckOrCreateDefault()
    {
        if(m_pushPullInfo == null)
            m_pushPullInfo = GetDefaultPullPushObject(); 
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

     //   string[] options = new string[]
     //   {
     //"Affect package", "Just Down or Upload"
     //   };
        //affectPackageManager = EditorGUILayout.Popup( affectPackageManager, options, GUILayout.Width(200));
        //affectPackage = affectPackageManager == 0;
        
        GUILayout.BeginHorizontal();
        bool downAllow = isDirectoryCreated;
        if (GUILayout.Button("Down", downAllow ? enableStyle: disableStyle))
        {
            if (downAllow)
                UnityPackageUtility.Down(directoryPath, link, affectPackage);
        }
        bool upAllow= isDirectoryCreated && isGitFolderPresent ;
        if (GUILayout.Button("Up", upAllow ? enableStyle : disableStyle))
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
