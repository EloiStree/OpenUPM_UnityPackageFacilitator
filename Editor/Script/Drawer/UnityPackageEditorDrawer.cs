using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityPackageEditorDrawer
{


    public static void DrawManifrest(ref UnityPackageManifest manifestInfo, ref string remove, ref string addname, ref string addvalue, bool withButtonToPushAndLoad = true, bool withDirectoryButton = true)
    {
        int buttonlenght = 30;
        int nameLength = 200;
        if (withButtonToPushAndLoad)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                manifestInfo = UnityPackageUtility.GetManifest();
            }
            if (GUILayout.Button("Push"))
            {
                UnityPackageUtility.SetManifest(manifestInfo);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Add & Remove");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(buttonlenght)))
        {
            manifestInfo.Add(addname, addvalue);
        }
        addname = GUILayout.TextField(addname, GUILayout.Width(nameLength));
        addvalue = GUILayout.TextField(addvalue);



        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", GUILayout.Width(buttonlenght)))
        {
            manifestInfo.Remove(remove);
        }
        remove = GUILayout.TextField(remove);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (withDirectoryButton)
        {
            if (GUILayout.Button("Go to package manifest", GUILayout.Width(150)))
            {
                UnityPackageUtility.OpenManifestFile();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Current package");
        List<DependencyJson> m_dependencies = manifestInfo.dependencies;
        for (int i = 0; i < m_dependencies.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(buttonlenght)))
            {
                manifestInfo.Remove(m_dependencies[i].nameId);
            }
            GUILayout.TextField(m_dependencies[i].nameId, GUILayout.Width(nameLength));
            if (m_dependencies[i].value.IndexOf("http") > -1)
            {

                if (GUILayout.Button("Down", GUILayout.Width(40))) {
                    UnityPackageUtility.Down(m_dependencies[i].value);
                }
                //if (GUILayout.Button("Up", GUILayout.Width(40)))
                //{
                //    UnityPackageUtility.Up(directory);
                //}
                if (GUILayout.Button("Go", GUILayout.Width(25)))
                {
                    Application.OpenURL(m_dependencies[i].value);
                }
            }
            GUILayout.TextField(m_dependencies[i].value);



            GUILayout.EndHorizontal();

        }
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
    public static  void DrawPackageDownUpButton(string directoryPath, string gitUrl, bool affectPackage = true)
    {

        
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
        bool downAllow = true;// isDirectoryCreated;
        if (GUILayout.Button("Down", downAllow ? enableStyle : disableStyle))
        {
            //if (downAllow)
                UnityPackageUtility.Down(directoryPath, gitUrl, affectPackage);
        }
        bool upAllow = isDirectoryCreated && isGitFolderPresent;
        if (GUILayout.Button("Up", upAllow ? enableStyle : disableStyle))
        {
            if (upAllow)
                UnityPackageUtility.Up(directoryPath, affectPackage);
        }
        GUILayout.EndHorizontal();
    }

    public static void DrawPackageEditor(ref string relativePathOfProject, PackageBuildInformation package)
    {
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Create Folder"))
        //{
        //    PackageBuilder.CreateFolder(relativePathOfProject);
        //    AssetDatabase.Refresh();
        //}

        if (string.IsNullOrEmpty(package.m_projectName))
            package.m_projectName = AlphaNumeric(Application.productName);

        package.m_projectName = (GUILayout.TextField("" + package.m_projectName));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("Create Folder"))
        //{
        //    PackageBuilder.CreateFolder(relativePathOfProject);
        //    AssetDatabase.Refresh();
        //}

        if (string.IsNullOrEmpty(package.m_projectId))
            package.m_projectId = AlphaNumeric(Application.productName);
        if (string.IsNullOrEmpty(package.m_company))
            package.m_company = AlphaNumeric(Application.companyName);
        package.m_country = AlphaNumeric(GUILayout.TextField("" + package.m_country));
        GUILayout.Label(".", GUILayout.Width(5));
        package.m_company = AlphaNumeric(GUILayout.TextField("" + package.m_company));
        GUILayout.Label(".", GUILayout.Width(5));
        package.m_projectId = AlphaNumeric(GUILayout.TextField("" + package.m_projectId));
        GUILayout.EndHorizontal();
        GUILayout.Label("Namespace ID: " + package.GetProjectNamespaceId());
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create " + package.m_projectName))
        {
            PackageBuilder.CreateUnityPackage(relativePathOfProject, package);
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();


    }

    private static string AlphaNumeric(string text)
    {
        return text.Replace(" ", "");
    }
}
