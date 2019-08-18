using UnityEngine;
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
    //public static ProjectGitLinkObject m_gitServerLink;
    public static PackageBuildInformationObject m_packageInformation;
    public static ContactInformationObject           m_contactInformation;
    public static ProjectDirectoriesStructureObject  m_folderStructureWanted;
    public static ListOfClassicPackagesObject        m_linksAdvice;



    public static PackageBuildInformationObject m_linkedPackagePrevious;
    public string m_folderName;
    public string m_readMeSpecificInformation;
    public string m_linkedGitUrl;
    public string m_gitLinkedToSelectedAsset;
    public string m_packageLinkedToSelectedAsset;
    public string m_gitlabprojectId;
    public string m_gitlabprojectAuthor;
    public string m_gitLinkToClone;

    Vector2 scrollPos;
    void OnGUI()
    {
        string relativePath = GetRelativePathOfSelectedAsset();
        if (!string.IsNullOrEmpty(relativePath)) {
           
            m_folderName = relativePath;
        }

       
        
        scrollPos = GUILayout.BeginScrollView(scrollPos);


        if (Selection.activeObject != null)
        {
            FullPackageBuildObject full = Selection.activeObject as FullPackageBuildObject;
            if (full != null)
            {
            m_fullPackage = full;
            }
            string currentSelectedPath = GetSelectedPathOrFallback();

            m_gitLinkedToSelectedAsset = QuickGit.GetGitRootInParent(currentSelectedPath);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Git", EditorStyles.boldLabel);
            GUILayout.TextField(m_gitLinkedToSelectedAsset);
            GUILayout.EndHorizontal();

            m_packageLinkedToSelectedAsset = UnityPackageUtility.GetPackageRootInParent(currentSelectedPath);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Package", EditorStyles.boldLabel);
            GUILayout.TextField(m_gitLinkedToSelectedAsset);
            GUILayout.EndHorizontal();
        }
     


        GUILayout.Label("> Set", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Folder Name:");
        m_folderName = GUILayout.TextField(m_folderName);
        if (m_packageInformation!=null && m_folderName=="" )
        {
//            if (GUILayout.Button("Refresh")) { }
            m_folderName = m_packageInformation.m_data.m_projectId;
        }
        bool isFolderCreated = Directory.Exists(GetFolderTargetOfUnityPackage());

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (!isFolderCreated && GUILayout.Button("Create Folder"))
        {
            Directory.CreateDirectory(GetFolderTargetOfUnityPackage());
        }

        if (isFolderCreated && GUILayout.Button("Remove Folder"))
        {
            FileUtil.DeleteFileOrDirectory(GetFolderTargetOfUnityPackage());
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        if (isFolderCreated && GUILayout.Button("Open Explorer"))
        {
            Application.OpenURL(GetFolderTargetOfUnityPackage());
        }
        if (isFolderCreated && IsSelectedFolderHasFilesInIt() && GUILayout.Button("Select in Unity"))
        {
            PingFolder(m_folderName);
        }

        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Drag and drop:", EditorStyles.boldLabel);
        m_fullPackage = (FullPackageBuildObject)EditorGUILayout.ObjectField(m_fullPackage, typeof(FullPackageBuildObject));
        GUILayout.EndHorizontal();
        if (m_fullPackage != null)
        {
           // m_gitServerLink = m_fullPackage.m_gitLink;
            m_packageInformation = m_fullPackage.m_package;
            m_contactInformation = m_fullPackage.m_contact;
            m_folderStructureWanted = m_fullPackage.m_structure;
            m_linksAdvice = m_fullPackage.m_links;
            if(m_fullPackage.m_package && m_folderName=="")
              m_folderName = m_fullPackage.m_package.m_data.m_projectId;
            m_fullPackage = null;
            RefreshDatabase();
        }
        if (m_fullPackage == null)
        {
            if (GUILayout.Button("Create Default"))
            {
                m_fullPackage = (FullPackageBuildObject)CreateScritableAsset<FullPackageBuildObject>(m_folderName + "Builder/DefaultFullPackage");
            }
        }

        GUILayout.Space(20);

        GUILayout.Label("> Git", EditorStyles.boldLabel);

        string whereGitIs = GetFolderTargetOfUnityPackage();
        m_linkedGitUrl = "";
        QuickGit.GetGitUrl(GetFolderTargetOfUnityPackage(), out m_linkedGitUrl);
        
        //m_gitServerLink = (ProjectGitLinkObject)EditorGUILayout.ObjectField(m_gitServerLink, typeof(ProjectGitLinkObject));
        //if (m_gitServerLink == null) { 
        //    if (GUILayout.Button("Create Default")) {
        //        m_gitServerLink = (ProjectGitLinkObject) CreateScritableAsset<ProjectGitLinkObject>(m_folderName+"Builder/GitLink");
        //    }
        //}
       

        if (!Directory.Exists(whereGitIs))
        {
            //            EditorGUILayout.HelpBox("Git need a directory", MessageType.Info);

            if (GUILayout.Button("Create directory:" + m_folderName))
            {
                Directory.CreateDirectory(whereGitIs);
            }


        }

        else
        {

            if (!string.IsNullOrEmpty(m_linkedGitUrl))
            {
                m_linkedGitUrl = GUILayout.TextField(m_linkedGitUrl);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add -a"))
                {
                    QuickGit.Add(whereGitIs);
                }
                if (GUILayout.Button("Commit"))
                {
                    QuickGit.Commit(whereGitIs);
                }
                if (GUILayout.Button("Pull"))
                {
                    QuickGit.Pull(whereGitIs);
                }
                if (GUILayout.Button("Push"))
                {
                    QuickGit.Push(whereGitIs);
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add>Commit>Pull"))
                {
                    QuickGit.AddCommitAndPush(whereGitIs);
                }
                if (GUILayout.Button("A>C>Pull + A>C>push"))
                {
                    QuickGit.PullPushWithAddAndCommit(whereGitIs);
                }

                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Status"))
                {
                    QuickGit.OpenCmd(whereGitIs);
                }
                if (GUILayout.Button("Server"))
                {
                    Application.OpenURL(m_linkedGitUrl);
                }

                GUILayout.EndHorizontal();

            }
            else
            {
                bool isSelectionHasGit = QuickGit.IsGitFolder(whereGitIs);
                bool isSeleectionHasGitAndUrl = QuickGit.IsGitFolderWihtUrl(whereGitIs);
                bool isFolderEmpty = QuickGit.IsFolderEmpty(whereGitIs);

                //if (m_gitServerLink != null)
                {
                    //GUILayout.BeginHorizontal();
                    //GUILayout.Label("User:");
                    //m_gitServerLink.m_data.m_urlAccount = GUILayout.TextField(m_gitServerLink.m_data.m_urlAccount);
                    //GUILayout.EndHorizontal();
                    //GUILayout.BeginHorizontal();
                    //GUILayout.Label("Url:");
                    //m_gitServerLink.m_data.m_projectGitUrl = GUILayout.TextField(m_gitServerLink.m_data.m_projectGitUrl);
                    //GUILayout.EndHorizontal();

                    if (!isSelectionHasGit)
                    {
                        if (isFolderEmpty)
                        {
                            if (GUILayout.Button("Create Local"))
                            {
                                QuickGit.CreateLocal(whereGitIs);
                            }
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Clone"))
                            {

                                QuickGit.Clone(m_gitLinkToClone, whereGitIs);
                            }
                            m_gitLinkToClone = GUILayout.TextField(
                                    m_gitLinkToClone);


                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            EditorGUILayout.HelpBox("to create or clone a project the folder need to be empty. Please move folders to continue.", MessageType.Warning);

                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Open explorer"))
                            {
                                Application.OpenURL(whereGitIs);
                            }
                            if (GUILayout.Button("Try to move"))
                            {
                                //                            Application.OpenURL(whereGitIs);
                            }
                            GUILayout.EndHorizontal();
                        }
                    }
                    else if( m_linkedGitUrl=="")
                    {
                        GUILayout.Label("Local Git to GitLab:");
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("User id ");
                        m_gitlabprojectAuthor = GUILayout.TextField(m_gitlabprojectAuthor);
                        GUILayout.EndHorizontal();


                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Project id ");
                        m_gitlabprojectId = GUILayout.TextField(m_gitlabprojectId);
                        GUILayout.EndHorizontal();

                        if (
                            m_gitlabprojectAuthor != "" &&
                            m_gitlabprojectId != "" &&
                            GUILayout.Button("Push"))
                        {
                            string url = "";
                            QuickGit.PushLocalToGitLab(whereGitIs, m_gitlabprojectAuthor, m_gitlabprojectId, out url);
                         }
                    }
                }
            }

        }




        GUILayout.Space(20);
        GUILayout.Label("> Create", EditorStyles.boldLabel);

        //////////////// Create Package ////////////////////////
        GUILayout.Label("Create a package:", EditorStyles.boldLabel);
        m_packageInformation = (PackageBuildInformationObject)EditorGUILayout.ObjectField(m_packageInformation, typeof(PackageBuildInformationObject));
        if (m_packageInformation != null)
        {
            PackageBuildInformation package = m_packageInformation.m_data;
            UnityPackageEditorDrawer.DrawPackageEditor(ref m_folderName, package);
        }
        if (m_packageInformation == null) { 
            if (GUILayout.Button("Create Default")) {
                m_packageInformation = (PackageBuildInformationObject) CreateScritableAsset<PackageBuildInformationObject>(m_folderName+"Builder/DefaultPackage");
            }
        }

        //////////////// Folder structure ////////////////////////
        GUILayout.Label("Folder structure", EditorStyles.boldLabel);


        GUILayout.BeginHorizontal();
        if (m_folderStructureWanted != null)
        {
            if (GUILayout.Button("Create Runtime:"))
            {
                m_folderStructureWanted.m_data.Create(GetFolderTargetOfUnityPackage() + "/Runtime");
                RefreshDatabase();
            }
        }
        m_folderStructureWanted = (ProjectDirectoriesStructureObject)EditorGUILayout.ObjectField(m_folderStructureWanted, typeof(ProjectDirectoriesStructureObject));
        GUILayout.EndHorizontal();
        if (m_folderStructureWanted == null)
        {
            if (GUILayout.Button("Create Default"))
            {
                m_folderStructureWanted = (ProjectDirectoriesStructureObject)CreateScritableAsset<ProjectDirectoriesStructureObject>(m_folderName + "Builder/DefaultStructure");
            }
        }
        //////////////// Linked Assets ////////////////////////
        GUILayout.Label("Linked assets", EditorStyles.boldLabel);
        m_linksAdvice = (ListOfClassicPackagesObject)EditorGUILayout.ObjectField(m_linksAdvice, typeof(ListOfClassicPackagesObject));
        if (m_linksAdvice != null)
        {
            if (GUILayout.Button("Create links"))
            {
                string path = GetFolderTargetOfUnityPackage() + "/Links";
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
                m_linksAdvice = (ListOfClassicPackagesObject)CreateScritableAsset<ListOfClassicPackagesObject>(m_folderName + "Builder/DefaultLinkPackage");
            }
        }
        //////////////// Contact information ////////////////////////
        GUILayout.Label("How to contact", EditorStyles.boldLabel);
        m_contactInformation = (ContactInformationObject)EditorGUILayout.ObjectField(m_contactInformation, typeof(ContactInformationObject));
        if (m_contactInformation != null)
        {
            if (GUILayout.Button("Create Contact.md"))
            {
                string path = GetFolderTargetOfUnityPackage();
                Directory.CreateDirectory(path);
                File.WriteAllText(path + "/Contact.md", "Hello " + m_contactInformation.m_data.m_firstName);

                RefreshDatabase();
            }
        }
        if (m_contactInformation == null)
        {
            if (GUILayout.Button("Create Default"))
            {
                m_contactInformation = (ContactInformationObject)CreateScritableAsset<ContactInformationObject>(m_folderName + "Builder/DefaultContact");
            }
        }

        GUILayout.Space(20);
        //////////////// COMMUN ////////////////////////
        GUILayout.Label("> Additional", EditorStyles.boldLabel);
        m_readMeSpecificInformation = GUILayout.TextArea(m_readMeSpecificInformation, GUILayout.Height(200));
        if (GUILayout.Button("Create ReadMe.md"))
        {
            string path = GetFolderTargetOfUnityPackage();
            Directory.CreateDirectory(path);
            File.WriteAllText(path + "/ReadMe.md", "Hey hey");
            RefreshDatabase();

        }
        GUILayout.EndScrollView();
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
        string path = GetFolderTargetOfUnityPackage();
        return Directory.Exists(path) &&   Directory.GetFiles(path).Length > 0;
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

    private string GetFolderTargetOfUnityPackage()
    {
        return Application.dataPath + "/" + m_folderName;
    }

    public string GetUnityFolderPath(string relativePath) {
        return Application.dataPath + "/" + relativePath;



    }
    /* FROM THE WEB */


    public void CreateDefault(string where) {

    }


    public static ScriptableObject CreateScritableAsset<T>(string name) where T:ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        string relative = name ;
        Directory.CreateDirectory(Application.dataPath + "/" + relative);
        AssetDatabase.CreateAsset(asset, "Assets/"+ relative + ".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
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
        return path;
    }
}