using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Eloi
{
    public class PackageBasicBuilder : EditorWindow
    {



        [MenuItem("ꬲ🧰/Package Utility/0. Init. & Struct", false, 10)]
        public static void ShowWindow()
        {
            PackageBasicBuilder window = (PackageBasicBuilder)EditorWindow.GetWindow(typeof(PackageBasicBuilder));
            window.Show();

            window.name = "Package Init Builder";
            window.titleContent.text = "Package Init Builder";
            window.RefreshAccess();
            try
            {
                string json = WindowPlayerPref.Load("PackageBasicBuilder");
                Info i = JsonUtility.FromJson<Info>(json);
                if (i != null)
                    m_info = i;
            }
            catch (Exception) { }
            Debug.Log("Load");
        }

        private void RefreshAccess()
        {
            m_info.m_selector = new UnityPathSelectionInfo();
            m_info.m_packageBuilder = new PackageBuildInformation();
        }

        private void OnDestroy()
        {
            Debug.Log("Save");
            WindowPlayerPref.Save("PackageBasicBuilder", JsonUtility.ToJson(m_info));
        }

        private static void SelectAsset(PackageBuildInformationObject package)
        {
            UnityPackageBuilderWindow.m_packageInformation = package;
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

        public string m_packageNamespaceId;

        public string m_proposeCreateFolderField;
        public bool m_lockSelection;


        /// WINDOW EDITOR VALUE ///
        bool m_createPackageFoldout;
        Vector2 scollrPackagePosition;
        string m_whereToCreateScritpable = "Facilitator";


        public static Info m_info = new Info();
        public class Info
        {
            public bool m_selectorpathFound;
            public UnityPathSelectionInfo m_selector;
            public PackageJsonFileStream m_packageTargeted;
            public GitLinkOnDisk m_targetedGit;
            public PackageBuildInformation m_packageBuilder = new PackageBuildInformation();
            public bool m_hidePackageBuilder;
            public bool m_hideGitUtilitary;
            public int m_dropDownSelectionServer = 0;
            public string m_projectNameToCreate = "";
            public string m_userNameToCreateGit = "";
            public string m_tmpFolderToCreate = "";
            public string m_tmpCloneProposed = "";
            public bool m_tmp_rawDisplayJsonPackage = true;
            public string m_tmpPackageJsonProposition = "";
        }

        public void ResetInfo()
        {
            m_absolutPathOfFolderToWorkOn = "";
        }

        void OnGUI()
        {


            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Back-up:", EditorStyles.boldLabel);
            if (GUILayout.Button("Save"))
            {
                WindowPlayerPref.Save("PackageBasicBuilderBackup", JsonUtility.ToJson(m_info));
            }
            if (WindowPlayerPref.Has("PackageBasicBuilderBackup") && GUILayout.Button("Load"))
            {
                try
                {
                    string json = WindowPlayerPref.Load("PackageBasicBuilderBackup");
                    Info i = JsonUtility.FromJson<Info>(json);
                    if (i != null)
                        m_info = i;
                }
                catch (Exception) { }
            }

            EditorGUILayout.EndHorizontal();

            m_info.m_selector = null;
            m_info.m_targetedGit = null;
            UnityPathSelectionInfo.Get(out m_info.m_selectorpathFound, out m_info.m_selector);
            AccessGitWithPathSelector.GetAffectedGit(m_info.m_selector, out m_info.m_targetedGit);

            if (m_info.m_targetedGit != null)
            {
                string p = m_info.m_targetedGit.GetRelativeDirectoryPath();
                m_info.m_selector = new UnityPathSelectionInfo(p);
            }

            EditorGUILayout.HelpBox("Reminder: Git must be install and Git.exe must be add in System Variable Path.", MessageType.Warning, true);
            m_info.m_packageTargeted = PackageJsonUtility.GetPackageFile(m_info.m_selector);



            if (GUILayout.Button("Select: " + m_info.m_selector.GetSelectName(false)))
            {
                m_info.m_selector.Open();
            }
            if (m_info.m_targetedGit == null)
            {
                string path = m_info.m_selector.GetAbsolutePath(true);

                if (GUILayout.Button("Git Init. in " + m_info.m_selector.GetSelectName(true)))
                {
                    QuickGit.CreateLocal(path);
                }
                GitForFacilitationEditor.ProposeCloneProject(m_info.m_selector, ref m_info.m_tmpCloneProposed);
                GitForFacilitationEditor.ProposeToCreateFolder(m_info.m_selector, ref m_info.m_tmpFolderToCreate);

            }
            else
            {

                if (!m_info.m_targetedGit.IsHosted())
                {
                    GitForFacilitationEditor.PushLocalGitToOnlineAccount(m_info.m_targetedGit,
                        ref m_info.m_userNameToCreateGit,
                        ref m_info.m_projectNameToCreate,
                        ref m_info.m_dropDownSelectionServer,
                        ref m_info.m_hideGitUtilitary);
                }
                GUILayout.Space(20);
                if (GUILayout.Button("Git: " + m_info.m_selector.GetSelectName(true), EditorStyles.boldLabel))
                {
                    Application.OpenURL(m_info.m_selector.GetAbsolutePath(false));
                }
                if (QuickGit.IsPathHasGitRootFolder(m_info.m_selector.GetAbsolutePath(true)))
                {
                    GitEditorDrawer.DisplayGitCommands(m_info.m_targetedGit);
                    UnityPackageEditorDrawer.DrawPackageDownUpButton(m_info.m_targetedGit, true);
                }
                PackageJsonEditor.DrawEditorDefaultInterface(m_info.m_selector, m_info.m_packageTargeted, ref m_info.m_packageBuilder, ref m_info.m_tmpPackageJsonProposition, ref m_info.m_tmp_rawDisplayJsonPackage, ref m_info.m_hidePackageBuilder); ;



            }

        }



        private void DrawGitIniCreateAndPush()
        {
            ResetInfo();

            if (!m_lockSelection)
                m_relativeSelection = GetFolderSelectedInUnity();
            m_absoluteSelection = Application.dataPath + "/" + m_relativeSelection;

            // propose to create folder if none are selected
            if (m_relativeSelection == "")
            {
                GitForFacilitationEditor.ProposeToCreateFolder(m_info.m_selector, ref m_info.m_tmpFolderToCreate);
                GitForFacilitationEditor.DisplayMessageToHelp("Please select or create a empty folder");

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


            UnityPackageUtility.TryToAccessPackageNamespaceIdFromFolder(m_absoluteSelection, out m_packageNamespaceId);
            DisplayPackageInformation();


            if (string.IsNullOrEmpty(m_linkedGitUrl))
            {
                //PushLocalGitToOnlineAccount(ref m_info.m_hideGitUtilitary);
            }
            if (!string.IsNullOrEmpty(m_linkedGitUrl))
            {
                GitLinkOnDisk gd = new GitLinkOnDisk(m_gitLinkedToSelectedAsset);
                GitEditorDrawer.DisplayGitCommands(gd);
                //UnityPackageEditorDrawer.DrawPackageDownUpButton(m_absoluteSelection, m_gitLinkedToSelectedAsset, true);
            }


            if (m_gitLinkedToSelectedAsset == "")
                return;


            m_absolutPathOfFolderToWorkOn = m_gitLinkedToSelectedAsset;

            m_createPackageFoldout = EditorGUILayout.Foldout(m_createPackageFoldout, "Structure package");
            if (m_createPackageFoldout)
            {
                CreatePackageStructure();
            }

            m_dangerousButton = EditorGUILayout.Foldout(m_dangerousButton, "Dangerous Option");
            if (m_dangerousButton)
            {

                if (GUILayout.Button("Remove Repository"))
                {
                    FileUtil.DeleteFileOrDirectory(m_gitLinkedToSelectedAsset);

                }
                if (GUILayout.Button("Remove .git"))
                {
                    FileUtil.DeleteFileOrDirectory(m_gitLinkedToSelectedAsset + "/.git");
                }
            }


        }

        public static string m_manifestaddNamespace;
        public static string m_manifestaddgitlink;
        public static Utility_ManifestJson m_manifestInfo = new Utility_ManifestJson();
        public static bool m_currentManifest;


        private void DisplayPackageInformation()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Package ID", GUILayout.Width(120));
            GUILayout.TextField(m_packageNamespaceId);
            GUILayout.EndHorizontal();

        }

        public static bool m_dangerousButton;

        private string GetGitFolderNameFromFolderPathInUnity()
        {
            return m_gitLinkedToSelectedAsset.Replace(Application.dataPath + "/", "").Replace(Application.dataPath, "");
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
            if (GUILayout.Button(m_lockSelection ? "Unlock the selection" : "Lock the selection"))
            {
                m_lockSelection = !m_lockSelection;
            }
            GUILayout.Space(20);
        }

        private void CreatePackageStructure()
        {
            scollrPackagePosition = GUILayout.BeginScrollView(scollrPackagePosition);

            CreatePackageCollectionLink();

            //////////////// Create Package ////////////////////////
            CreatePackageDirectories();
            GUILayout.Space(10);

            //////////////// Folder structure ////////////////////////
            CreateDirectories();
            //////////////// Links ////////////////////////
            CreateLinks();

            GUILayout.Space(10);
            //////////////// COMMUN ////////////////////////
            CreateReadMe();

            GUILayout.EndScrollView();
        }

        private void CreateReadMe()
        {
            GUILayout.Label("> Read Me", EditorStyles.boldLabel);
            m_readMeSpecificInformation = GUILayout.TextArea(m_readMeSpecificInformation, GUILayout.Height(200));


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh Proposition"))
            {


                m_readMeSpecificInformation = "";
                m_readMeSpecificInformation += string.Format("```\"{0}\":\"{1}\",```", m_packageInformation.m_data.GetProjectNamespaceId(), m_linkedGitUrl);

            }
            if (GUILayout.Button("Create ReadMe.md"))
            {
                Directory.CreateDirectory(m_gitLinkedToSelectedAsset);
                File.WriteAllText(m_gitLinkedToSelectedAsset + "/ReadMe.md", m_readMeSpecificInformation);
                //RefreshDatabase();

            }
            GUILayout.EndHorizontal();
        }

        public bool m_directoriesFoldout;
        private void CreateDirectories()
        {
            GUILayout.Label("Folder structure", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            m_folderStructureWanted = (ProjectDirectoriesStructureObject)EditorGUILayout.ObjectField(m_folderStructureWanted, typeof(ProjectDirectoriesStructureObject));
            GUILayout.EndHorizontal();


            if (m_folderStructureWanted == null)
            {


                if (GUILayout.Button("Create Default"))
                {
                    m_folderStructureWanted = (ProjectDirectoriesStructureObject)ScriptableUtility.CreateScritableAsset<ProjectDirectoriesStructureObject>(m_gitfolderName + "/" + m_whereToCreateScritpable, "Folders_" + m_gitfolderName, false);
                    if (m_fullPackage)
                        m_fullPackage.m_structure = m_folderStructureWanted;
                }
            }
            if (m_fullPackage && m_folderStructureWanted)
                m_fullPackage.m_structure = m_folderStructureWanted;

            if (m_folderStructureWanted != null)
            {
                m_directoriesFoldout = EditorGUILayout.Foldout(m_directoriesFoldout, "Files structure");
                if (m_directoriesFoldout)
                {
                    string[] folder = m_folderStructureWanted.m_data.m_defaultDirectory;
                    for (int i = 0; i < folder.Length; i++)
                    {
                        GUILayout.Label("Folder: " + folder[i]);
                    }

                    FileFromText[] file = m_folderStructureWanted.m_data.m_defaultFiles;
                    for (int i = 0; i < file.Length; i++)
                    {
                        GUILayout.Label("File: " + file[i].m_relativePath);
                    }


                    FileFromweb[] weblink = m_folderStructureWanted.m_data.m_defaultFilesFromWeb;
                    for (int i = 0; i < weblink.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(">", GUILayout.Width(20)))
                        {
                            Application.OpenURL(weblink[i].m_url);
                        }
                        GUILayout.Label("Web: " + weblink[i].m_relativePath);

                        GUILayout.EndHorizontal();
                    }
                }
                if (GUILayout.Button("Create files in Runtime"))
                {
                    m_folderStructureWanted.m_data.Create(GetFolderWhereToWorkOn() + "/Runtime");
                    // RefreshDatabase();
                }
            }


        }

        public static bool m_linkFoldout;
        private void CreateLinks()
        {
            //////////////// Linked Assets ////////////////////////
            GUILayout.Label("Linked assets", EditorStyles.boldLabel);
            m_linksAdvice = (ListOfClassicPackagesObject)EditorGUILayout.ObjectField(m_linksAdvice, typeof(ListOfClassicPackagesObject));
            if (m_linksAdvice != null)
            {
                ClassicUnityPackageLink[] links = m_linksAdvice.m_data.m_packageLinks;

                m_linkFoldout = EditorGUILayout.Foldout(m_linkFoldout, "Links");
                if (m_linkFoldout)
                {
                    for (int i = 0; i < links.Length; i++)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(25);
                        if (GUILayout.Button(links[i].m_name))
                        {
                            Application.OpenURL(links[i].m_pathOrLink);
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                if (GUILayout.Button("Create links"))
                {
                    string path = GetFolderWhereToWorkOn() + "/Links";
                    Directory.CreateDirectory(path);
                    for (int i = 0; i < links.Length; i++)
                    {
                        links[i].CreateWindowLinkFile(path, false);
                    }
                    // RefreshDatabase();
                }
            }


            if (m_linksAdvice == null)
            {
                if (GUILayout.Button("Create Default"))
                {
                    m_linksAdvice = (ListOfClassicPackagesObject)ScriptableUtility.CreateScritableAsset<ListOfClassicPackagesObject>(m_gitfolderName + "/" + m_whereToCreateScritpable, "Links_" + m_gitfolderName, false);
                    if (m_fullPackage)
                        m_fullPackage.m_links = m_linksAdvice;
                }
            }
            if (m_fullPackage && m_linksAdvice)
                m_fullPackage.m_links = m_linksAdvice;
        }

        private void CreatePackageDirectories()
        {
            GUILayout.Label("Create a package:", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            m_packageInformation = (PackageBuildInformationObject)EditorGUILayout.ObjectField(m_packageInformation, typeof(PackageBuildInformationObject));

            //        RenameAsset();
            GUILayout.EndHorizontal();
            if (m_packageInformation != null)
            {
                PackageBuildInformation package = m_packageInformation.m_data;
                UnityPackageEditorDrawer.DrawPackageEditor(ref m_gitLinkedToSelectedAsset, package);
            }
            if (m_packageInformation == null)
            {

                if (GUILayout.Button("Create Default"))
                {
                    m_packageInformation = (PackageBuildInformationObject)ScriptableUtility.CreateScritableAsset<PackageBuildInformationObject>(m_gitfolderName + "/" + m_whereToCreateScritpable, "Package_" + m_gitfolderName, false);
                    if (m_fullPackage)
                        m_fullPackage.m_package = m_packageInformation;
                }
            }
            if (m_fullPackage && m_packageInformation)
                m_fullPackage.m_package = m_packageInformation;
        }

        private void CreatePackageCollectionLink()
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
                    m_absolutPathOfFolderToWorkOn = m_fullPackage.m_package.m_data.m_projectAlphNumId;
                //RefreshDatabase();
            }
            m_previousFullPackage = m_fullPackage;
            if (m_fullPackage == null)
            {
                if (GUILayout.Button("Create collection"))
                {
                    m_fullPackage = (FullPackageBuildObject)ScriptableUtility.CreateScritableAsset<FullPackageBuildObject>(m_gitfolderName + "/" + m_whereToCreateScritpable, "Collection_" + m_gitfolderName, false);
                }
            }
        }

        private void DisplayGitPathAndLink()
        {

            GUILayout.BeginHorizontal();

            GUILayout.Label("Git folder", GUILayout.Width(120));
            GUILayout.TextField(m_gitLinkedToSelectedAsset);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Git Link", GUILayout.Width(120));
            GUILayout.TextField(m_linkedGitUrl);

            GUILayout.EndHorizontal();
        }



        private string GetGitFolderAbsolutPath(out bool hasGitInParent)
        {
            string path = "";
            hasGitInParent = false;
            GitLinkOnDisk gd;
            QuickGit.GetGitInParents(m_absoluteSelection, QuickGit.PathReadDirection.LeafToRoot, out gd);
            if (gd != null)
            {
                hasGitInParent = true;
                path = gd.GetDirectoryPath();
            }
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

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("GitLab"))
            {
                Application.OpenURL("https://gitlab.com/dashboard/projects");
            }
            if (GUILayout.Button("GitHub"))
            {
                Application.OpenURL("https://github.com/");
            }

            GUILayout.EndHorizontal();

        }

        private void DisplayButtonOpenAndSelection()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Explorer"))
            {
                Application.OpenURL(GetFolderWhereToWorkOn());
            }
            if (GUILayout.Button("Select in Unity"))
            {
                Ping.PingFolder(m_absolutPathOfFolderToWorkOn, true);
            }
            GUILayout.EndHorizontal();
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
                    AssetDatabase.RenameAsset(path, m_packageInformation.m_data.m_projectAlphNumId);
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




        private string GetFolderWhereToWorkOn()
        {
            if (m_gitLinkedToSelectedAsset == "")
                return "";
            return m_gitLinkedToSelectedAsset;
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
}