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
        public ReadMeFileStream m_readMe;
        public SampleDirectoryStream m_sample;
        public DocumentDirectoryStream m_documentation;
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
    }
    [MenuItem("Window/Package Utility/All")]
    static void Init()
    {
        AllBuilderInOnEditor window = (AllBuilderInOnEditor)EditorWindow.GetWindow(typeof(AllBuilderInOnEditor));
        window.Show();


        try
        {
        string json =WindowPlayerPref.Load("PackageManagerAllBuildInOnEditor");
            Info i = JsonUtility.FromJson<Info>(json);
            if (i != null) 
                window.m_info = i;
            window.RefreshAccess();
            Debug.Log("Load");
        }
        catch (Exception) { }


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
        if (GUILayout.Button("Lock Selection: "+m_lockState)) {
            m_lockState = !m_lockState;
        }
        if ( ! Directory.Exists(m_selector.GetAbsolutePath(true) )) {
            m_lockState = false;
        }

        if (!m_lockState) { 
            UnityPathSelectionInfo.Get(out m_pathFound, out m_selector);
        }
        string previous = m_info.m_focusPath;
        m_info.m_focusPath = m_selector.GetAbsolutePath(true);
        if (previous != m_info.m_focusPath) {
            RefreshAccess();
        }


        EditorGUILayout.TextField(m_selector.GetRelativePath(true));
        EditorGUILayout.TextField(m_selector.GetAbsolutePath(true));

        m_info.m_scollrPackagePosition = GUILayout.BeginScrollView(m_info.m_scollrPackagePosition);
        GUILayout.Label("Welcome", EditorStyles.boldLabel);
        GUILayout.Label("This window allow you to read write config package.");
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Create:", EditorStyles.boldLabel);
        if (GUILayout.Button("Documentation")) { 
            DocumentationUtility.Create(m_selector,false); Refresh(); }
        if (GUILayout.Button("Sample")) { SampleUtility.Create(m_selector,false);
            Refresh();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Toogle:", EditorStyles.boldLabel);
        if (GUILayout.Button("Documentation")) { DocumentationUtility.Toggle(m_selector); Refresh(); }
        if (GUILayout.Button("Sample")) { SampleUtility.Toggle(m_selector); Refresh(); }
        GUILayout.EndHorizontal(); GUILayout.BeginHorizontal();
        GUILayout.Label("Delete:", EditorStyles.boldLabel);
        if (GUILayout.Button("Documentation")) { m_info.m_documentation.Delete(); Refresh(); }
        if (GUILayout.Button("Sample")) { m_info.m_sample.Delete(); Refresh(); }
        GUILayout.EndHorizontal();

        GUILayout.Label("Read Me:", EditorStyles.boldLabel);
        m_info.m_tmpReadMeText = EditorGUILayout.TextArea( m_info.m_tmpReadMeText, GUILayout.MinHeight(100));
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Default")) {
            if (m_info.m_gitLink != null)
                m_info.m_readMe.Create(ReadMeUtility.CreateBasicDefaultOnFrom(m_info.m_gitLink));
            else m_info.m_readMe.Create("# Read Me  \n Hey buddy!  \nWhat 's up ?");
            LoadReadMe();
        }
        if (GUILayout.Button("Load")) { LoadReadMe(); }
        if (GUILayout.Button("Override")) { OverrideReadMe(); }
        if (GUILayout.Button("Open")) { OpenReadMeExplorer(); }
        GUILayout.EndHorizontal();


        GUILayout.Space(10);

        ChangeLogEditor.DrawEditorDefaultInterface(m_info.m_changelog,ref  m_info.m_tmpLogVersion,ref m_info.m_tmpLogTitle,ref m_info.m_tmpLogNew);
        GUILayout.Space(10);

        LicenseEditor.DrawEditorDefaultInterface(m_info.m_license,ref  m_info.m_tmpLicenseLink, ref m_info.m_tmpLicenseText);

        GUILayout.Space(10);



        //CreatePackageDirectories();
        GUILayout.EndScrollView();


    }

   
    private void Refresh()
    {
        AssetDatabase.Refresh();
    }

    private void OpenReadMeExplorer()
    {
        m_info.m_readMe.Open();
    }

    private void OverrideReadMe()
    {
        m_info.m_readMe.Set(m_info.m_tmpReadMeText);
    }

    private void LoadReadMe()
    {
        m_info.m_tmpReadMeText = m_info.m_readMe.Get();
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