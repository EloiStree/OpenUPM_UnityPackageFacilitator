using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PackageJsonEditor : EditorWindow
{

    private bool m_pathFound;
    private UnityPathSelectionInfo m_selector;
    private GitLinkOnDisk m_gitLink;
    private PackageBuildInformation m_builder= new PackageBuildInformation();
    private bool m_hide=false;

    [MenuItem("Window/Package Utility/Package Json")]
    static void Init()
    {
        PackageJsonEditor window = (PackageJsonEditor)EditorWindow.GetWindow(typeof(PackageJsonEditor));
        window.titleContent = new GUIContent("Package.json");
        window.Show();
    }

    void OnGUI()
    {
        UnityPathSelectionInfo.Get(out m_pathFound, out m_selector);
        GUILayout.Label("Package.json: " + m_selector.GetRelativePath(false), EditorStyles.boldLabel);
        PackageJsonFileStream f = PackageJsonUtility.GetPackageFile(m_selector);
        QuickGit.GetGitInParents(m_selector.GetAbsolutePath(false), QuickGit.PathReadDirection.LeafToRoot, 
            out m_gitLink);
        DrawEditorDefaultInterface(f, ref m_gitLink, ref m_builder, ref m_hide);

    }

    public static void DrawEditorDefaultInterface(PackageJsonFileStream package, ref GitLinkOnDisk gitLink, ref PackageBuildInformation builder,  ref bool hide)
    {
        hide = EditorGUILayout.Foldout(hide, hide ? "→ Package.json" : "↓ Package.json", EditorStyles.boldLabel);
        if (!hide)
        {
            GUILayout.Label("Found Pack: " + package.Exist(), EditorStyles.boldLabel);
            GUILayout.Label("Found Git: " + package.GetLinkedGit().Exist(), EditorStyles.boldLabel);
            //GUILayout.Label("Read Me:", EditorStyles.boldLabel);
            //readMeText = EditorGUILayout.TextArea(readMeText, GUILayout.MinHeight(100));
            //GUILayout.BeginHorizontal();
            //if (GUILayout.Button("Create Default"))
            //{
            //    if (gitLink != null)
            //        readme.Create(ReadMeUtility.CreateBasicDefaultOnFrom(gitLink));
            //    else readme.Create("# Read Me  \n Hey buddy!  \nWhat 's up ?");
            //    readMeText = readme.Get();
            //}
            //if (GUILayout.Button("Load")) { readMeText = readme.Get(); }
            //if (GUILayout.Button("Override"))
            //{
            //    readme.Set(readMeText);
            //}
            //if (GUILayout.Button("Open"))
            //{
            //    readme.Open();
            //}
            //GUILayout.EndHorizontal();
        }
    }

    internal static void DrawEditorDefaultInterface(PackageJsonFileStream m_packageTargeted, ref GitLinkOnDisk m_targetedGit, ref object m_packageBuilder, ref object m_hidePackageBuilder)
    {
        throw new NotImplementedException();
    }

    internal static void DrawEditorDefaultInterface(PackageJsonFileStream m_packageTargeted, ref GitLinkOnDisk m_targetedGit, ref PackageBuildInformation m_packageBuilder, ref object m_hidePackageBuilder)
    {
        throw new NotImplementedException();
    }
}