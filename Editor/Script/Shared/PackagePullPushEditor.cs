using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PackagePullPush))]
public class PackagePullPushEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PackagePullPush myScript = (PackagePullPush) target;
        
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
            UnityPackageUtility.Down(myScript.GetGitLink(), myScript.m_affectPackageManager);
        }
        if (GUILayout.Button("Up", isDirectoryCreated ? enableStyle : disableStyle))
        {
            UnityPackageUtility.Up(myScript.m_namespaceId,myScript.GetGitLink(),  myScript.m_affectPackageManager);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(6);


        GUILayout.BeginHorizontal();
        //if (isLinkValide && !isDirectoryCreated && GUILayout.Button("Clone"))
        //{
        //    myScript.PullProject();
        //}
        if (isLinkValide && isDirectoryCreated && GUILayout.Button("Pull"))
        {
            myScript.UpdateProject();
        }
        if ( isDirectoryCreated && GUILayout.Button("Pull & Push"))
        {
            myScript.PullAndPush();
        }
        if (isDirectoryCreated && GUILayout.Button("Remove"))
        {
            RemoveFolderWithUnityTool(myScript);
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (isDirectoryCreated && GUILayout.Button("Open Folder"))
        {
            myScript.OpenDirectory();
        }
        if (isDirectoryCreated && GUILayout.Button("Open Cmd"))
        {

            myScript.OpenStatusInCommentLine();
        }
        if (isLinkValide && GUILayout.Button("Open Git Server"))
        {
            Application.OpenURL(myScript.GetGitLink());
        }
        GUILayout.EndHorizontal();
     
    }

    private static void RemoveFolderWithUnityTool(PackagePullPush myScript)
    {
        string directory = myScript.GetProjectPathInUnity();
        //myScript.RemoveProject();
        FileUtil.DeleteFileOrDirectory(directory);
        AssetDatabase.Refresh();
    }
}