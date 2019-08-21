﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

//https://learn.unity.com/tutorial/introduction-to-scriptable-objects#5cf187b7edbc2a31a3b9b123
class UnityPackageBuilderWindow : EditorWindow
{
    [MenuItem("Facilitator/Create Package")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UnityPackageBuilderWindow));
    }

    [MenuItem("CONTEXT/FullPackageBuildObject/Open Editor")]
    public static void ShowWindowd()
    {
        UnityPackageBuilderWindow.ShowWindow();
    }

    public static FullPackageBuildObject m_fullPackage;
    public static FullPackageBuildObject m_previousFullPackage;
    //public static ProjectGitLinkObject m_gitServerLink;
    public static PackageBuildInformationObject m_packageInformation;

    public static ContactInformationObject m_contactInformation;
    public static ProjectDirectoriesStructureObject m_folderStructureWanted;
    public static ListOfClassicPackagesObject m_linksAdvice;



    public static PackageBuildInformationObject m_linkedPackagePrevious;
    public string m_absolutPathOfFolderToWorkOn;
    public string m_readMeSpecificInformation;
    public string m_linkedGitUrl;
    public string m_gitLinkedToSelectedAsset;
    public string m_packageLinkedToSelectedAsset;
    public string m_gitlabprojectId;
    public string m_gitlabprojectAuthor;
    public string m_gitLinkToClone;
    public bool m_foldoutFolders;

    public string m_relativeSelection;
    public string m_absoluteSelection;
    public string m_gitfolderName;

    public string m_proposeCreateFolderField;
    public bool m_lockSelection;

    Vector2 scrollPos;


    public void ResetInfo() {
        m_absolutPathOfFolderToWorkOn = "";
    }

    void OnGUI()
    {

        ResetInfo();

        // Find selected folder
        if (!m_lockSelection)
            m_relativeSelection = GetFolderSelectedInUnity();
        m_absoluteSelection = Application.dataPath + "/" + m_relativeSelection;

        // propose to create folder if none are selected
        if (m_relativeSelection == "")
        {
            ProposeToCreateFolder(ref m_proposeCreateFolderField);
            DisplayMessageToHelp("Please select or create a empty folder");
            return;
        }
        DisplayLocker();
        DisplayFolderSelectionnedInUnity();



        bool hasGitInParent;
        m_gitLinkedToSelectedAsset = GetGitFolderAbsolutPath(out hasGitInParent);
        if (!hasGitInParent)
        {
            ProposeToCreateLocalGit();
            return;
        }



        /// IF as local git but no url;
        /// 

        m_gitfolderName = GetGitFolderNameFromFolderPathInUnity();

        QuickGit.GetGitUrl(m_gitLinkedToSelectedAsset, out m_linkedGitUrl);
        DisplayGitPathAndLink();
        DisplayGitProjectName();
        if (string.IsNullOrEmpty(m_linkedGitUrl))
        {
            PushLocalGitToOnlineAccount();
        }
        if (!string.IsNullOrEmpty(m_linkedGitUrl))
        {
            QuickGit.DisplayEditorCommands(m_gitLinkedToSelectedAsset);

        }


        if (m_gitLinkedToSelectedAsset == "")
            return;

        GUILayout.Label("_______________________");

        m_absolutPathOfFolderToWorkOn = m_gitLinkedToSelectedAsset;





        CreatePackageStructure();

        GUILayout.Label("_______________________");
        //m_relativeFolderToWorkOn = m_gitLinkedToSelectedAsset;

        //DisplayButtonOpenAndSelection();

        //GUILayout.Label("> Git", EditorStyles.boldLabel);
        //DisplayGitInformation();



        //If Not Folder
        //// Propose to create
        //// Return;

        // If not git
        //// propse to create
        //// retrun

        // Display: group affected
        //// If null propose create default
        // // else propose to edit;

        // Display:  define package
        //// If null propose create default
        // // else propose to edit;

        // Display:  folder structure
        //// If null propose create default
        // // else propose to edit;

        // Display:  Link
        //// If null propose create default
        // // else propose to edit;

        // Display:  Contact
        //// If null propose create default
        // // else propose to edit;


        // Get read me proposition
        // Display:  Read Me with proposition
        //// If null propose create default
        // // else propose to edit;




    }

    private string GetGitFolderNameFromFolderPathInUnity()
    {
       return  m_gitLinkedToSelectedAsset.Replace(Application.dataPath + "/", "").Replace(Application.dataPath, "");
    }

    private void DisplayGitProjectName()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Git Folder", GUILayout.Width(120));
        GUILayout.TextField(m_gitfolderName);
        GUILayout.EndHorizontal();
    }

    private void DisplayLocker()
    {
        if (GUILayout.Button(m_lockSelection? "Unlock the selection": "Lock the selection")) {
            m_lockSelection = !m_lockSelection;
        }
        GUILayout.Space(20);
    }

    private void CreatePackageStructure()
    {

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.Space(20);
        DisplayCollectionLink();

   

        //////////////// Create Package ////////////////////////
        GUILayout.Label("Create a package:", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        m_packageInformation = (PackageBuildInformationObject)EditorGUILayout.ObjectField(m_packageInformation, typeof(PackageBuildInformationObject));

        //        RenameAsset();
        GUILayout.EndHorizontal();
        if (m_packageInformation != null)
        {
            PackageBuildInformation package = m_packageInformation.m_data;
            UnityPackageEditorDrawer.DrawPackageEditor(ref m_absolutPathOfFolderToWorkOn, package);
        }
        if (m_packageInformation == null)
        {

            if (GUILayout.Button("Create Default"))
            {
                m_packageInformation = (PackageBuildInformationObject)CreateScritableAsset<PackageBuildInformationObject>(m_gitfolderName, "Package_" + m_gitfolderName, false);
                if (m_fullPackage)
                    m_fullPackage.m_package = m_packageInformation;
            }
        }
        if (m_fullPackage && m_packageInformation)
            m_fullPackage.m_package = m_packageInformation;




        //////////////// Folder structure ////////////////////////
        GUILayout.Label("Folder structure", EditorStyles.boldLabel);


        GUILayout.BeginHorizontal();
        if (m_folderStructureWanted != null)
        {
            if (GUILayout.Button("Create Runtime:"))
            {
                m_folderStructureWanted.m_data.Create(GetFolderWhereToWorkOn() + "/Runtime");
                RefreshDatabase();
            }
        }
        m_folderStructureWanted = (ProjectDirectoriesStructureObject)EditorGUILayout.ObjectField(m_folderStructureWanted, typeof(ProjectDirectoriesStructureObject));
        GUILayout.EndHorizontal();
        if (m_folderStructureWanted == null)
        {
            if (GUILayout.Button("Create Default"))
            {
                m_folderStructureWanted = (ProjectDirectoriesStructureObject)CreateScritableAsset<ProjectDirectoriesStructureObject>(m_gitfolderName, "Folders_" + m_gitfolderName, false);
                if (m_fullPackage)
                    m_fullPackage.m_structure = m_folderStructureWanted;
            }
        }
        if (m_fullPackage && m_folderStructureWanted)
            m_fullPackage.m_structure = m_folderStructureWanted;
        //////////////// Linked Assets ////////////////////////
        GUILayout.Label("Linked assets", EditorStyles.boldLabel);
        m_linksAdvice = (ListOfClassicPackagesObject)EditorGUILayout.ObjectField(m_linksAdvice, typeof(ListOfClassicPackagesObject));
        if (m_linksAdvice != null)
        {
            if (GUILayout.Button("Create links"))
            {
                string path = GetFolderWhereToWorkOn() + "/Links";
                Directory.CreateDirectory(path);
                ClassicPackageLink[] links = m_linksAdvice.m_data.m_packageLinks;
                for (int i = 0; i < links.Length; i++)
                {
                    links[i].CreateWindowLinkFile(path, false);
                }
                RefreshDatabase();
            }
        }
        if (m_linksAdvice == null)
        {
            if (GUILayout.Button("Create Default"))
            {
                m_linksAdvice = (ListOfClassicPackagesObject)CreateScritableAsset<ListOfClassicPackagesObject>(m_gitfolderName, "Links_" + m_gitfolderName, false);
                if (m_fullPackage)
                    m_fullPackage.m_links = m_linksAdvice;
            }
        }
        if (m_fullPackage && m_linksAdvice)
            m_fullPackage.m_links = m_linksAdvice;
        //////////////// Contact information ////////////////////////
        GUILayout.Label("How to contact", EditorStyles.boldLabel);
        m_contactInformation = (ContactInformationObject)EditorGUILayout.ObjectField(m_contactInformation, typeof(ContactInformationObject));
        if (m_contactInformation != null && m_contactInformation.m_data != null)
        {
            GUILayout.BeginHorizontal();


            //if (string.IsNullOrEmpty(package.m_projectName))
            //    package.m_projectName = AlphaNumeric(Application.productName);

            m_contactInformation.m_data.m_firstName = (GUILayout.TextField("" + m_contactInformation.m_data.m_firstName));
            m_contactInformation.m_data.m_lastName = (GUILayout.TextField("" + m_contactInformation.m_data.m_lastName));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Contact:", GUILayout.Width(80));
            m_contactInformation.m_data.m_howToContact = (GUILayout.TextField("" + m_contactInformation.m_data.m_howToContact));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Patreon:", GUILayout.Width(80));
            m_contactInformation.m_data.m_patreonLink = (GUILayout.TextField("" + m_contactInformation.m_data.m_patreonLink));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Paypal:", GUILayout.Width(80));
            m_contactInformation.m_data.m_paypalLink = (GUILayout.TextField("" + m_contactInformation.m_data.m_paypalLink));
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Create Contact.md"))
            {
                string path = GetFolderWhereToWorkOn();
                Directory.CreateDirectory(path);
                File.WriteAllText(path + "/Contact.md", "Hello " + m_contactInformation.m_data.m_firstName);

                RefreshDatabase();
            }
        }
        if (m_contactInformation == null)
        {
            if (GUILayout.Button("Create Default"))
            {
                m_contactInformation = (ContactInformationObject)CreateScritableAsset<ContactInformationObject>(m_gitfolderName, "Contact_" + m_gitfolderName, false);
                if (m_fullPackage)
                    m_fullPackage.m_contact = m_contactInformation;
            }
        }
        if (m_fullPackage && m_contactInformation)
            m_fullPackage.m_contact = m_contactInformation;

        GUILayout.Space(20);
        //////////////// COMMUN ////////////////////////
        GUILayout.Label("> Additional", EditorStyles.boldLabel);
        m_readMeSpecificInformation = GUILayout.TextArea(m_readMeSpecificInformation, GUILayout.Height(200));


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh Proposition"))
        {


            m_readMeSpecificInformation = "";

        }
        if (GUILayout.Button("Create ReadMe.md"))
        {
            string path = GetFolderWhereToWorkOn();
            Directory.CreateDirectory(path);
            File.WriteAllText(path + "/ReadMe.md", m_readMeSpecificInformation);
            RefreshDatabase();

        }
        GUILayout.EndHorizontal();
        GUILayout.EndScrollView();
    }

    private void DisplayCollectionLink()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Project information collection:", EditorStyles.boldLabel);
        m_fullPackage = (FullPackageBuildObject)EditorGUILayout.ObjectField(m_fullPackage, typeof(FullPackageBuildObject));
        GUILayout.EndHorizontal();
        if (m_fullPackage != null && m_previousFullPackage != m_fullPackage)
        {
            if (m_fullPackage.m_package)
                m_packageInformation = m_fullPackage.m_package;
            if (m_fullPackage.m_contact)
                m_contactInformation = m_fullPackage.m_contact;
            if (m_fullPackage.m_structure)
                m_folderStructureWanted = m_fullPackage.m_structure;
            if (m_fullPackage.m_links)
                m_linksAdvice = m_fullPackage.m_links;
            if (m_fullPackage.m_package && m_absolutPathOfFolderToWorkOn == "")
                m_absolutPathOfFolderToWorkOn = m_fullPackage.m_package.m_data.m_projectId;
            RefreshDatabase();
        }
        m_previousFullPackage = m_fullPackage;
        if (m_fullPackage == null)
        {
            if (GUILayout.Button("Create collection"))
            {
                m_fullPackage = (FullPackageBuildObject)CreateScritableAsset<FullPackageBuildObject>(m_gitfolderName, "Collection_"+ m_gitfolderName , false);
            }
        }
    }

    private void DisplayGitPathAndLink()
    {

        GUILayout.BeginHorizontal();

        GUILayout.Label("Git folder", GUILayout.Width(120));
        GUILayout.TextField( m_gitLinkedToSelectedAsset);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Git Link", GUILayout.Width(120));
        GUILayout.TextField(m_linkedGitUrl);

        GUILayout.EndHorizontal();
    }

    

    private string GetGitFolderAbsolutPath(out bool hasGitInParent)
    {
       string path = QuickGit.GetGitRootInParent(m_absoluteSelection, out hasGitInParent);
        if (!hasGitInParent)
            path = "";
        return path;
    }

    private void ProposeToCreateLocalGit()
    {
        if (GUILayout.Button("Create Local"))
        {
            QuickGit.CreateLocal(m_absoluteSelection);
        }
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clone"))
        {

            QuickGit.Clone(m_gitLinkToClone, m_absoluteSelection);
        }
        m_gitLinkToClone = GUILayout.TextField(
                m_gitLinkToClone);


        GUILayout.EndHorizontal();
    }

    private void DisplayButtonOpenAndSelection()
    {
        GUILayout.BeginHorizontal();
        if ( GUILayout.Button("Open Explorer"))
        {

            Application.OpenURL(GetFolderWhereToWorkOn());
        }
        if (GUILayout.Button("Select in Unity"))
        {
            PingFolder(m_absolutPathOfFolderToWorkOn);
        }

        GUILayout.EndHorizontal();
    }

   

    private void PushLocalGitToOnlineAccount()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("User id ");
        m_gitlabprojectAuthor = GUILayout.TextField(m_gitlabprojectAuthor);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Project id ");
        m_gitlabprojectId = GUILayout.TextField(m_gitlabprojectId);
        string urlToCrate = string.Format("https://gitlab.com/{0}/{1}.git", m_gitlabprojectAuthor, m_gitlabprojectId);
       

        GUILayout.EndHorizontal();
        if (m_gitlabprojectAuthor != "" && m_gitlabprojectId != "")

        {
            if (GUILayout.Button("Create/Push Online"))
            {
                string url = "";
                QuickGit.PushLocalToGitLab(m_gitLinkedToSelectedAsset, m_gitlabprojectAuthor, m_gitlabprojectId, out url);
            }

        }
        DisplayMessageToHelp("Please enter your account id and the name of the project in the git link: " + urlToCrate);
    }

    private void DisplayFoldoutWihtGitAndPackage()
    {
        if (m_foldoutFolders = EditorGUILayout.Foldout(m_foldoutFolders, "Folders Information"))
        {




            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.Label("Git", GUILayout.Width(100));
            GUILayout.TextField(m_gitLinkedToSelectedAsset);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Space(30);
            GUILayout.Label("Package", GUILayout.Width(100));
            GUILayout.TextField(m_gitLinkedToSelectedAsset);
            GUILayout.EndHorizontal();


        }
    }

    private void ProposeToCreateFolder(ref string folderName)
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Create folder:",GUILayout.Width(120))) {
            Directory.CreateDirectory(Application.dataPath + "/" + folderName);
            RefreshDatabase();
            PingFolder(folderName);
        }
        folderName = GUILayout.TextField(folderName);
        GUILayout.EndHorizontal();

    }

    private static void DisplayMessageToHelp(string msg)
    {
        EditorGUILayout.HelpBox(msg, MessageType.Info);
    }

    private void DisplayFolderSelectionnedToWorkIn()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Working in:", GUILayout.Width(100));
        GUILayout.TextField(GetFolderWhereToWorkOn());
        GUILayout.EndHorizontal();
    }

    private void DisplayFolderSelectionnedInUnity()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Selection", GUILayout.Width(100));
        GUILayout.TextField(m_relativeSelection);
        GUILayout.EndHorizontal();
    }

    private string GetFolderSelectedInUnity()
    {
        if (Selection.activeObject != null)
        {
            
            return GetSelectedPathOrFallback();
            
        }
        else return "";
    }

    private static void RenameAsset()
    {
        if (m_packageInformation)
        {


            string namePckage = GUILayout.TextField((m_packageInformation.name));
            if (namePckage != m_packageInformation.name)
            {

                string path = AssetDatabase.GetAssetPath(m_packageInformation.GetInstanceID());
                AssetDatabase.RenameAsset(path, m_packageInformation.m_data.m_projectId);
            }
        }
    }

    private string GetRelativePathOfSelectedAsset()
    {
        string path = m_packageLinkedToSelectedAsset;
        if (string.IsNullOrEmpty(path))
        {
            path = m_gitLinkedToSelectedAsset;
        }
        if (!string.IsNullOrEmpty(path))
        {


            path = path.Replace(Application.dataPath + "\\Assets", "");
            path = path.Replace(Application.dataPath + "/Assets", "");
            if (path.IndexOf("Assets/") == 0)
                path = path.Replace("Assets/", "");
            if (path.IndexOf("Assets\\") == 0)
                path = path.Replace("Assets\\", "");
            return path;
        }
        return "";

    }

    private bool IsSelectedFolderHasFilesInIt()
    {
        string path = GetFolderWhereToWorkOn();
        return Directory.Exists(path) && Directory.GetFiles(path).Length > 0;
    }

    private void PingFolder(string relativeFolderName)
    {
        if (relativeFolderName[relativeFolderName.Length - 1] == '/')
            relativeFolderName = relativeFolderName.Substring(0, relativeFolderName.Length - 1);
        UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath("Assets/" + relativeFolderName, typeof(UnityEngine.Object));
        Selection.activeObject = obj;
        EditorGUIUtility.PingObject(obj);

        UnityEngine.Object[] selection = new UnityEngine.Object[1];
        selection[0] = obj;
        Selection.objects = selection;

    }

    private void RefreshDatabase()
    {
        AssetDatabase.Refresh();
    }

    private string GetFolderWhereToWorkOn()
    {
        if (m_absolutPathOfFolderToWorkOn == "")
            return "";
        return Application.dataPath + "/" + m_absolutPathOfFolderToWorkOn;
    }

    

    public static ScriptableObject CreateScritableAsset<T>(string relative, string name, bool selectAsset) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        Directory.CreateDirectory(Application.dataPath + "/Facilitator/" + relative);
        AssetDatabase.CreateAsset(asset, "Assets/Facilitator/" + relative + "/" + name + ".asset");
        AssetDatabase.SaveAssets();

        if (selectAsset)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        AssetDatabase.Refresh();
        return asset;
    }

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }
    void OnSelectionChange()
    {
        //Debug.Log("DD");
        //OnGUI();
        // selectionIDs = Selection.instanceIDs;
    }

    public static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        if (path.Length > 6)
            path = path.Substring(7, path.Length - 7);
        else path = "";
        return path;
    }
}