using Eloi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AllBuilderInOnEditor : EditorWindow
{
    public Info m_info= new Info();
    public class Info
    {
        public string m_focusPath;
        public GitLinkOnDisk m_gitLink;
        public AbstractFileStream m_readMe;
        public SampleDirectoryStream m_sample;
        public DocumentationDirectoryStream m_documentation;
        public string m_tmpReadMeText="";
        public string m_tmpLicenseLink = "";
        public string m_tmpLicenseText = "";
        public string m_tmpLicenseEmbbedImage = "";
        public string m_tmpLogNew = "";
        public string m_tmpLogTitle = "";
        public string m_tmpLogVersion = "";
        public Vector2 m_scollrPackagePosition;
        public ChangeLogFileStream m_changelog;
        public LicenseFileStream m_license;
        public bool m_tmpLogHide;
        public bool tmpLicenseHide;
        public bool m_tmpReadMeHide;
        public bool m_hideHiddenTool;
    }
    [MenuItem("ꬲ🧰/Package Utility/1. Update & Save", false, 10)]
    static void Init()
    {
        AllBuilderInOnEditor window = (AllBuilderInOnEditor)EditorWindow.GetWindow(typeof(AllBuilderInOnEditor));
        window.Show();



        window.titleContent = new GUIContent ("All Package Info");
            window.RefreshAccess();


        try
        {
            string json = WindowPlayerPref.Load("PackageManagerAllBuildInOnEditor");
            Info i = JsonUtility.FromJson<Info>(json);
            if (i != null)
                window.m_info = i;
        }
        catch (Exception) { }
        Debug.Log("Load");
       


    }

    private void OnDestroy()
    {
        Debug.Log("Save");
       WindowPlayerPref.Save("PackageManagerAllBuildInOnEditor", JsonUtility.ToJson(m_info));
    }


    public bool m_lockState;
    public bool m_pathFound;
    public UnityPathSelectionInfo m_selector = new UnityPathSelectionInfo();
    void OnGUI()
    {
        if (GUILayout.Button("Lock Selection: " + m_lockState))
        {
            m_lockState = !m_lockState;
        }
        if (!Directory.Exists(m_selector.GetAbsolutePath(true)))
        {
            m_lockState = false;
        }

        if (!m_lockState)
        {
            UnityPathSelectionInfo.Get(out m_pathFound, out m_selector);
        }
        string previous = m_info.m_focusPath;
        m_info.m_focusPath = m_selector.GetAbsolutePath(true);
        if (previous != m_info.m_focusPath)
        {
            RefreshAccess();
        }


        EditorGUILayout.TextField(m_selector.GetRelativePath(true));
        EditorGUILayout.TextField(m_selector.GetAbsolutePath(true));

        m_info.m_scollrPackagePosition = GUILayout.BeginScrollView(m_info.m_scollrPackagePosition);
        GUILayout.Label("Welcome", EditorStyles.boldLabel);
        GUILayout.Label("This window allow you to read write config package.");
        GUILayout.Space(10);

        m_info.m_hideHiddenTool  = EditorGUILayout.Foldout(m_info.m_hideHiddenTool, m_info.m_hideHiddenTool ? "→ Doc & Sample" : "↓ Doc & Sample", EditorStyles.boldLabel);
        if (!m_info.m_hideHiddenTool)
        {
            ToggleAndCreateHiddenFolder();
        }

        ReadMeEditor.DrawEditorDefaultInterface(m_info.m_readMe, ref m_info.m_gitLink, ref m_info.m_tmpReadMeText, ref m_info.m_tmpReadMeHide);

        ChangeLogEditor.DrawEditorDefaultInterface(m_info.m_changelog, ref m_info.m_tmpLogVersion, ref m_info.m_tmpLogTitle, ref m_info.m_tmpLogNew, ref m_info.m_tmpLogHide);

        LicenseEditor.DrawEditorDefaultInterface(m_info.m_license, ref m_info.m_tmpLicenseLink, ref m_info.m_tmpLicenseText, ref m_info.tmpLicenseHide);




        //CreatePackageDirectories();
        GUILayout.EndScrollView();


    }

    private void ToggleAndCreateHiddenFolder()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Create:", EditorStyles.boldLabel);
        if (GUILayout.Button("Documentation"))
        {
            DocumentationUtility.Create(m_selector, false); Refresh();
        }
        if (GUILayout.Button("Samples"))
        {
            SampleUtility.Create(m_selector, false);
            Refresh();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Toogle:", EditorStyles.boldLabel);
        if (GUILayout.Button("Documentation")) { DocumentationUtility.Toggle(m_selector); Refresh(); }
        if (GUILayout.Button("Samples")) { SampleUtility.Toggle(m_selector); Refresh(); }
        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
        GUILayout.Label("Delete:", EditorStyles.boldLabel);
        if (GUILayout.Button("Documentation")) { m_info.m_documentation.Delete(); Refresh(); }
        if (GUILayout.Button("Samples")) { m_info.m_sample.Delete(); Refresh(); }
        GUILayout.EndHorizontal();
    }

    private void Refresh()
    {
        AssetDatabase.Refresh();
    }


    private void RefreshAccess()
    {
        m_info.m_readMe = ReadMeUtility.GetReadMeFile(m_selector);
        m_info.m_sample = SampleUtility.GetSampleFolder(m_selector);
        m_info.m_documentation = DocumentationUtility.GetDocumentFolder(m_selector);
        m_info.m_changelog = ChangeLogUtility.GetReadMeFile(m_selector);
        m_info.m_license = LicenseUtility.GetReadMeFile(m_selector);
        QuickGit.GetGitInDirectory(m_selector.GetAbsolutePath(true), out m_info.m_gitLink, false);
    }
}