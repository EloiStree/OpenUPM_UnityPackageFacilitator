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
    //[MenuItem("ꬲ🧰/ Facilitator/Pull Push")]
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
       // DisplayPullPushInProject();

      


        m_pushPullInfo = (PackagePullPushObject)EditorGUILayout.ObjectField(m_pushPullInfo, typeof(PackagePullPushObject));
        if (m_pushPullInfo != null)
            m_pushPull = m_pushPullInfo.m_data;
        else m_pushPull = null;


        if (m_pushPullInfo == null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Name:", GUILayout.Width(50));
            m_folderToCreate = GUILayout.TextArea(m_folderToCreate);
            GUILayout.EndHorizontal();

            DisplayCopyPastField();

            if (GUILayout.Button("Create Default"))
            {
                m_pushPullInfo = GetDefaultPullPushObject(m_folderToCreate);
                m_pushPullInfo.m_data.m_gitUrl = m_copyPastShortCut;
                m_pushPullInfo.m_data.m_packageNamespaceId = m_nameSpaceToCreate;
            }
        }


        if (!string.IsNullOrEmpty(m_copyPastShortCut))
        {
            //\w*\.git
            string isGitLink = "\\w*\\.git";
            //".*"\s*:\s*".*\.git["\s\n\r\z]
            string isPackageManagerLink = "\".*\"\\s*:\\s*\".*\\.git[\"\\s\\n\\r\\a,]";


            //"be.eloistree.overrideandroidvolume":"https://gitlab.com/eloistree/2019_06_30_overrideandroidvolume.git",
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
            //https://gitlab.com/eloistree/HeyMyFriend
            //https://gitlab.com/eloistree/2019_06_30_overrideandroidvolume.git
            else if (MathRegex(m_copyPastShortCut, isGitLink))
            {
                CheckOrCreateDefault();
                m_pushPullInfo.m_data.m_gitUrl = m_copyPastShortCut;
                UnityPackageUtility.TryToAccessPackageNamespaceIdFromGitCloneUrl(m_copyPastShortCut, out  m_pushPullInfo.m_data.m_packageNamespaceId);

                if(string.IsNullOrEmpty(m_pushPullInfo.m_data.m_relativeFolderPath))
                m_pushPullInfo.m_data.m_relativeFolderPath = GetGitProjectName(m_copyPastShortCut);
                m_copyPastShortCut = "";
            }
        }




        scroll = EditorGUILayout.BeginScrollView(scroll);


        if (m_pushPull != null)
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

           // UnityPackageEditorDrawer.DrawPackageDownUpButton(GetPathOfFolder(), m_pushPull.m_gitUrl, true);
            GitLinkOnDisk gd = new GitLinkOnDisk(GetPathOfFolder());
            GitEditorDrawer.DisplayGitCommands(gd);


           m_folderFoldout = EditorGUILayout.Foldout(m_folderFoldout, "Folder & Git");
            if (m_folderFoldout)
            {
                DisplayCopyPastField();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Folder", GUILayout.Width(40));
                m_pushPull.m_relativeFolderPath = GUILayout.TextArea(m_pushPull.m_relativeFolderPath);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Namespace", GUILayout.Width(70));
                m_pushPull.m_packageNamespaceId = GUILayout.TextArea(m_pushPull.m_packageNamespaceId);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Git", GUILayout.Width(40));
                m_pushPull.m_gitUrl = GUILayout.TextArea(m_pushPull.m_gitUrl);
                GUILayout.EndHorizontal();

            }

            //m_upDownFoldout = EditorGUILayout.Foldout(m_upDownFoldout, "Pull Push");
            //if (m_upDownFoldout)
            //{

            //}

            GUILayout.Space(6);

        }
        EditorGUILayout.EndScrollView();
    }

    private void DisplayCopyPastField()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Past .git", GUILayout.Width(50));
        m_copyPastShortCut = GUILayout.TextArea(m_copyPastShortCut);
        m_copyPastShortCut = m_copyPastShortCut.Trim();
        GUILayout.EndHorizontal();
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
        return (PackagePullPushObject)ScriptableUtility.CreateScritableAsset<PackagePullPushObject>("ꬲ🧰/Facilitator/WebLink", name, true);
    }

    private bool MathRegex(string text, string regex)
    {
        return (new Regex(regex)).IsMatch(m_copyPastShortCut);
    }

    private void CheckOrCreateDefault()
    {
        string assetName = m_folderToCreate;
    

        if(m_pushPullInfo == null)
            m_pushPullInfo = GetDefaultPullPushObject(assetName); 
    }

    int affectPackageManager = 0;
   


    public static bool m_folderFoldout=true;
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
