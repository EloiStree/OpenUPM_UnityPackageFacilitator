using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GitForFacilitationEditor : MonoBehaviour
{

    public static void PushLocalGitToOnlineAccount(GitLinkOnDisk gitLink, ref string userName, ref string projectNameId, ref int dropDownSelectionServer, ref bool compact)
    {
        compact = EditorGUILayout.Foldout(compact, "→ Push repository online", EditorStyles.boldLabel);
        if (!compact)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("User id ");
            userName = GUILayout.TextField(userName, GUILayout.MaxWidth(500));

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Project id ");
            projectNameId = GUILayout.TextField(projectNameId, GUILayout.MaxWidth(500));




            GUILayout.EndHorizontal();
            dropDownSelectionServer = EditorGUILayout.Popup(dropDownSelectionServer, new string[] { "GitLab", "GitHub" });

            string urlToCreate = "";
            if (dropDownSelectionServer == 0)
            {
                urlToCreate = string.Format("https://gitlab.com/{0}/{1}.git", userName, projectNameId);

            }
            if (dropDownSelectionServer == 1)
            {
                urlToCreate = "https://github.com?name=" + projectNameId;

            }


            if (userName != "" && projectNameId != "")
            {

                string url = "";
                if (dropDownSelectionServer == 0)
                {
                    if (GUILayout.Button("Create/Push Online"))
                    {
                        QuickGit.PushLocalToGitLab(gitLink.GetDirectoryPath(), userName, projectNameId, out url);
                        urlToCreate = string.Format("https://gitlab.com/{0}/{1}.git", userName, projectNameId);

                        Application.OpenURL(urlToCreate);
                    }

                }
                if (dropDownSelectionServer == 1)
                {
                    if (GUILayout.Button("Go to Github"))
                    {
                        //QuickGit.PushLocalToGitHub(gitLink.GetDirectoryPath(), userName, projectNameId, out url);
                        urlToCreate = "https://github.com?name=" + projectNameId;
                        Application.OpenURL(url);
                    }
                }

            }
            DisplayDeleteRepositoryOptions(gitLink);
            DisplayMessageToHelp("Please enter your account id and the name of the project in the git link: " + urlToCreate);
        }
    }
    public static void DisplayMessageToHelp(string msg)
    {
        EditorGUILayout.HelpBox(msg, MessageType.Info);
    }

    public static void DisplayDeleteRepositoryOptions(GitLinkOnDisk gitLink)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Remove Repository"))
        {
            FileUtil.DeleteFileOrDirectory(gitLink.GetDirectoryPath());
            RefreshDatabase();
        }
        if (GUILayout.Button("Remove .git"))
        {
            FileUtil.DeleteFileOrDirectory(gitLink.GetDirectoryPath() + "/.git");
            RefreshDatabase();
        }
        EditorGUILayout.EndHorizontal();

    }
   

    public static void ProposeToCreateFolder(UnityPathSelectionInfo selector,ref string folderName)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create folder:", GUILayout.Width(120)))
        {
            if (!string.IsNullOrEmpty(folderName))
            {
                string full = selector.GetAbsolutePath(true);
                string relative = selector.GetRelativePath(true);
                if (Directory.Exists(full)) { 
                
                    Directory.CreateDirectory(full + "/" + folderName);
                    RefreshDatabase();
                   
                    Ping.PingFolder(relative+"/"+folderName, false);
                }
            }
        }
        folderName = GUILayout.TextField(folderName);
        GUILayout.EndHorizontal();
        DisplayMessageToHelp("Please select or create a empty folder");


    }

    public static void ProposeCloneProject(UnityPathSelectionInfo selector, ref string cloneProposed)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clone"))
        {

            QuickGit.Clone(cloneProposed, selector.GetAbsolutePath(true) );
        }
        cloneProposed = GUILayout.TextField(
                cloneProposed);


        GUILayout.EndHorizontal();
    }

    public static void RefreshDatabase()
    {
        AssetDatabase.Refresh();
    }

}
