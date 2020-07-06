using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class UnityPackageEditorDrawer
{



    public static void DrawManifrest(ref Utility_ManifestJson manifestInfo, ref string addname, ref string addvalue, ref Vector2 scrollValue, bool withButtonToPushAndLoad = true)
    {
        int buttonlenght = 30;
        int nameLength = 250;
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
            if (GUILayout.Button("Go 2 Manifest"))
            {
                UnityPackageUtility.OpenManifestFile();
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("", GUILayout.Width(buttonlenght));
        GUILayout.Label("Namespace Id", GUILayout.Width(nameLength));
        GUILayout.Label("https://server.com/user/project.git#branch");


        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(buttonlenght)))
        {
            manifestInfo.Add(addname, addvalue);
        }
        addname = GUILayout.TextField(addname, GUILayout.Width(nameLength));
        if (addvalue.Length > 0 && GUILayout.Button("<o")) {

            bool found;
            string nameId;
            DownloadInfoFromGitServer.LoadNamespaceFromProjectGitLink(addvalue, out found, out nameId);
            addname = nameId;
        }

        addvalue = GUILayout.TextField(addvalue);




        GUILayout.EndHorizontal();
        if (addvalue.Length > 0 && addvalue.ToLower().IndexOf(".git") < 0)
        {
            EditorGUILayout.HelpBox("Are you sure it is a git link ?", MessageType.Warning);
            if (GUILayout.Button("Add .git at the end of the link"))
            {
                addvalue += ".git";
            }
        }

        scrollValue = GUILayout.BeginScrollView(scrollValue, false, true);
        List<DependencyJson> m_dependencies = manifestInfo.dependencies;
        if (m_dependencies != null && m_dependencies.Count>0)
        {

        GUILayout.Label("Current package");
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
                    if (GUILayout.Button("Update", GUILayout.Width(60)))
                    {
                        manifestInfo.RemoveLocker(m_dependencies[i].nameId);
                        AssetDatabase.Refresh();
                    }
                    if (GUILayout.Button("Down", GUILayout.Width(50))) {
                        UnityPackageUtility.Down(m_dependencies[i].value);
                        AssetDatabase.Refresh();
                    }
            
                if (GUILayout.Button("Go", GUILayout.Width(30)))
                {
                    Application.OpenURL(m_dependencies[i].value);
                }
            }
            GUILayout.TextField(m_dependencies[i].value);



            GUILayout.EndHorizontal();

            }
        }
        GUILayout.EndScrollView();
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

    public static  void DrawPackageDownUpButton(GitLinkOnDisk disk, bool affectPackage = true)
    {
        if (!disk.HasUrl()) return;
        
        bool isDirectoryCreated = Directory.Exists(disk.GetDirectoryPath());
        bool isGitFolderPresent = disk.HasUrl();
        GUIStyle disableStyle = GetDisableStyle();
        GUIStyle enableStyle = GetEnableStyle();


        GUILayout.BeginHorizontal();
        bool downAllow = true;
        if (GUILayout.Button("Down", downAllow ? enableStyle : disableStyle))
        {
             UnityPackageUtility.Down(disk.GetDirectoryPath(), disk.GetUrl(), affectPackage);
        }
        bool upAllow = isDirectoryCreated && isGitFolderPresent;
        if (GUILayout.Button("Up", upAllow ? enableStyle : disableStyle))
        {
            if (upAllow)
              UnityPackageUtility.Up(disk.GetDirectoryPath(), affectPackage);
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

        if (string.IsNullOrEmpty(package.m_projectAlphNumId))
            package.m_projectAlphNumId = AlphaNumeric(Application.productName);
        if (string.IsNullOrEmpty(package.m_company))
            package.m_company = AlphaNumeric(Application.companyName);
        package.m_country = AlphaNumeric(GUILayout.TextField("" + package.m_country));
        GUILayout.Label(".", GUILayout.Width(5));
        package.m_company = AlphaNumeric(GUILayout.TextField("" + package.m_company));
        GUILayout.Label(".", GUILayout.Width(5));
        package.m_projectAlphNumId = AlphaNumeric(GUILayout.TextField("" + package.m_projectAlphNumId));
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
