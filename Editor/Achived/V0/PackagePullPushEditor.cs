using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PackagePullPushMono))]
public class PackagePullPushEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PackagePullPushMono myScript = (PackagePullPushMono) target;
        string gitPath = myScript.GetProjectPathInUnity() + "/.git";
        string projectPath = myScript.GetProjectPathInUnity();
        string gitFolderUrl = "";
        QuickGit.GetGitUrl(projectPath, out gitFolderUrl );
        bool isGitFolderDefine = !string.IsNullOrEmpty(gitFolderUrl);
        string projectPathTmp = projectPath + "tmp";
        string requiredPathFile = projectPath + "/requiredpackages.json";
        ListOfClassicPackages package = ListOfClassicPackages.FromJsonPath(requiredPathFile);

        if (string.IsNullOrEmpty(myScript.GetGitLink()))
            return;
        GUILayout.Label("Commands", EditorStyles.boldLabel);
        bool isLinkValide = myScript.IsGitLinkValide();
        bool isDirectoryCreated = myScript.IsDirectoryCreated();
        GUILayout.BeginHorizontal();
        var disableStyle = new GUIStyle(GUI.skin.button);
        disableStyle.normal.textColor = new Color(0.6627451f, 0.6627451f, 0.6627451f);
        var enableStyle = new GUIStyle(GUI.skin.button);
        enableStyle.normal.textColor = new Color(0f, 0.4f, 0f);
        if (GUILayout.Button("Down", isDirectoryCreated?disableStyle: enableStyle))
        {
            if(!isDirectoryCreated)
                UnityPackageUtility.Down(projectPath,myScript.GetGitLink(), myScript.m_affectPackageManager);
        }
        if (GUILayout.Button("Up", (isDirectoryCreated && isGitFolderDefine) ? enableStyle : disableStyle))
        {
            if (isGitFolderDefine)
                UnityPackageUtility.Up(projectPath, myScript.m_namespaceId, myScript.GetGitLink() , myScript.m_affectPackageManager);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(6);


        GUILayout.BeginHorizontal();
        //if (isLinkValide && !isDirectoryCreated && GUILayout.Button("Clone"))
        //{
        //    myScript.PullProject();
        //}
        if (isGitFolderDefine && GUILayout.Button("Pull"))
        {
            myScript.UpdateProject();
        }
        if (isGitFolderDefine && GUILayout.Button("Pull & Push"))
        {
            myScript.PullAndPush();
        }
        if (isDirectoryCreated && GUILayout.Button("Remove Project"))
        {
            RemoveFolderWithUnityTool(myScript);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (isDirectoryCreated && GUILayout.Button("Open Folder"))
        {
            myScript.OpenDirectory();
        }
        if (isDirectoryCreated && isGitFolderDefine  && GUILayout.Button("Open Cmd"))
        {

            myScript.OpenStatusInCommentLine();
        }
        if (isLinkValide && GUILayout.Button("Open Git Server"))
        {
            Application.OpenURL(myScript.GetGitLink());
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
       
        if (isDirectoryCreated && isGitFolderDefine && GUILayout.Button("Remove Git Info"))
        {
                FileUtil.DeleteFileOrDirectory(gitPath);
        }
        if (isDirectoryCreated && !isGitFolderDefine && GUILayout.Button("Add Git Info"))
        {
            Directory.CreateDirectory(projectPathTmp);
            FileUtil.ReplaceDirectory(projectPath, projectPathTmp);
            FileUtil.DeleteFileOrDirectory(projectPath);
            Directory.CreateDirectory(projectPath);

            //QuickGit.CreateLocal(projectPath);
            // QuickGit.PushLocalToGitLab(projectPath, myScript.m_gitUserName, GetProjectDatedNameId(myScript), out gitUrl);
            QuickGit.Clone(myScript.GetGitLink(), projectPath);
            FileUtil.ReplaceDirectory(gitPath, projectPathTmp + "/.git");
            FileUtil.DeleteFileOrDirectory(projectPath);
            FileUtil.ReplaceDirectory(projectPathTmp, projectPath);
            FileUtil.DeleteFileOrDirectory(projectPathTmp);

            //FileUtil.ReplaceDirectory(projectPathTmp, projectPath);
            // FileUtil.DeleteFileOrDirectory(projectPathTmp);
        }
    
        GUILayout.EndHorizontal();


        if(package.m_packageLinks.Length>0)
            GUILayout.Label("Required Unitypackage:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < package.m_packageLinks.Length; i++)
        {
           ClassicUnityPackageLink link = package.m_packageLinks[i];
            string path = package.m_packageLinks[i].m_pathOrLink;
            
           if (link.IsUnityPackage() )
            {
                if (link.IsWebPath() && GUILayout.Button("Web: " + link.m_name)) {
                    string pathUnityPackage = Application.dataPath + "/../Temp/lastpackagedownloaded.unitypackage";
                    using (var client = new WebClient())
                    {
                        File.WriteAllBytes(pathUnityPackage, client.DownloadData(path));

                    }
                    Application.OpenURL(pathUnityPackage);
                   // FileUtil.DeleteFileOrDirectory(pathUnityPackage);
                }
                else if (link.IsWindowPath() && GUILayout.Button("Local: " +link.m_name))
                    Application.OpenURL(path);
            }
            
            if (link.IsAssetStoreLink() && GUILayout.Button("Store: " +link.m_name))
            {
                Application.OpenURL(path);
            }
        }
        GUILayout.EndHorizontal();


    }

    private static void RemoveFolderWithUnityTool(PackagePullPushMono myScript)
    {
        string directory = myScript.GetProjectPathInUnity();
        //myScript.RemoveProject();
        FileUtil.DeleteFileOrDirectory(directory);
        AssetDatabase.Refresh();
    }
}