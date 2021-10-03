using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Eloi;
public class PackageJsonEditor : EditorWindow
{

    private bool m_pathFound;
    private UnityPathSelectionInfo m_selector;
    private GitLinkOnDisk m_gitLink;
    private PackageBuildInformation m_builder= new PackageBuildInformation();
    private bool m_hide=false;
    private bool m_raw;
    private string m_jsonProposition;

    [MenuItem("ꬲ🧰/Package Utility/Window/Package Json", false, 20)]
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
        DrawEditorDefaultInterface(m_selector,f, ref m_builder,ref m_jsonProposition,ref m_raw, ref m_hide);

    }

    public static void DrawEditorDefaultInterface(UnityPathSelectionInfo selection, PackageJsonFileStream package, ref PackageBuildInformation builder,ref string proposeJson,ref bool rawDisplay,  ref bool hide)
    {
        hide = EditorGUILayout.Foldout(hide, hide ? "→ Package.json" : "↓ Package.json", EditorStyles.boldLabel);
        if (!hide)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Json: " + package.Exist(), EditorStyles.boldLabel);
            GUILayout.Label("Found Git: " + package.GetLinkedGit().Exist(), EditorStyles.boldLabel);
            GUILayout.EndHorizontal();

            rawDisplay = GUILayout.Toggle(rawDisplay, rawDisplay ? "Use Editor" : "Use Raw");
            if (rawDisplay)
            {
                DrawPackageRawEditor( package, ref proposeJson);
            }
            else { 
                DrawPackageEditor(selection,package, builder) ;
            }
          
        }
    }

    private static void DrawPackageRawEditor(PackageJsonFileStream package, ref string rawProposition)
    {
       
        GUILayout.BeginHorizontal();


        if (GUILayout.Button("Empty Shell"))
        {
            TextAsset t = Resources.Load<TextAsset>("PackageJson/EmptyShell");
            if(t!=null)
                rawProposition = t.text;
        }
        if (GUILayout.Button("Default"))
        {
            TextAsset t = Resources.Load<TextAsset>("PackageJson/Default");
            if (t != null)
                rawProposition = t.text;
        }
        if (GUILayout.Button("Full"))
        {
            TextAsset t = Resources.Load<TextAsset>("PackageJson/Full");
            if (t != null)
                rawProposition = t.text;
        }



        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();


        if (GUILayout.Button("Load"))
        {
            if (package.Exist())
            {
                rawProposition = package.Get();
            }
        }
       
        if (GUILayout.Button("Push"))
        {
            if (package.Exist())
            { 
                package.Set(rawProposition,true);
                
            }
        }

        GUILayout.EndHorizontal();
        rawProposition= GUILayout.TextArea(rawProposition, GUILayout.MinHeight(500));
    }

    public static void DrawPackageEditor(UnityPathSelectionInfo selection, PackageJsonFileStream packageTarget, PackageBuildInformation package)
    {

        string path = packageTarget.GetAbsolutePath();
        GitLinkOnDisk gitLinked = packageTarget.GetLinkedGit();

        GUILayout.BeginHorizontal();
      
            if (GUILayout.Button("Create package.json"))
            {

                LoadSamplesFromDirectoryToPackage(selection, ref package); 
                string json = PackageBuilder.GetPackageAsJson(package);
                packageTarget.Set(json, true);
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("Create Files & Folders"))
            {
              PackageBuilder.CreateUnityPackage(packageTarget.GetPackageProjectRoot(), package);
                AssetDatabase.Refresh();
            }
        
        
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (string.IsNullOrEmpty(package.m_projectName))
            package.m_projectName = UnityPaths.AlphaNumeric(Application.productName);

        package.m_projectName = (GUILayout.TextField("" + package.m_projectName));
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();


        if (string.IsNullOrEmpty(package.m_projectAlphNumId)) {
            // package.m_projectAlphNumId = UnityPaths.AlphaNumeric(Application.productName, true);
            package.m_projectAlphNumId= selection.GetSelectName(true);
        }
        if (string.IsNullOrEmpty(package.m_company))
            package.m_company = UnityPaths.AlphaNumeric(Application.companyName);

        package.m_country = UnityPaths.AlphaNumeric(GUILayout.TextField("" + package.m_country));
        GUILayout.Label(".", GUILayout.Width(5));
        package.m_company = UnityPaths.AlphaNumeric(GUILayout.TextField("" + package.m_company));
        GUILayout.Label(".", GUILayout.Width(5));
        package.m_projectAlphNumId = UnityPaths.AlphaNumeric(GUILayout.TextField("" + package.m_projectAlphNumId));
        GUILayout.EndHorizontal();
        GUILayout.Label("Namespace ID: " + package.GetProjectNamespaceId());

        GUILayout.Label("Description");
        package.m_description = GUILayout.TextArea(package.m_description, GUILayout.MinHeight(100));
        GUILayout.BeginHorizontal();
        GUILayout.Label("Tags:", GUILayout.MaxWidth(60));
        package.m_keywords = GUILayout.TextField(string.Join(",",package.m_keywords)).Split(',');
        GUILayout.EndHorizontal();

        GUILayout.Label("Author");
        GUILayout.BeginHorizontal();

        GUILayout.Label("Name: ", GUILayout.MaxWidth(60));
        package.m_author.m_name = GUILayout.TextField(package.m_author.m_name);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Mail: ", GUILayout.MaxWidth(60));
        package.m_author.m_mail = GUILayout.TextField(package.m_author.m_mail);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Url: ", GUILayout.MaxWidth(60));
        package.m_author.m_url = GUILayout.TextField(package.m_author.m_url);
        GUILayout.EndHorizontal();


        GUILayout.Label("Repository Info");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Git Url: " , GUILayout.MaxWidth(60));
        
    
        package.m_repositoryLink.m_url = GUILayout.TextField(gitLinked.GetUrl());
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Revision: " , GUILayout.MaxWidth(60));
        package.m_repositoryLink.m_revision = GUILayout.TextField(gitLinked.GetLastRevision());
      
        GUILayout.EndHorizontal();


        GUILayout.Space(10);
        GUILayout.Label("Direct dependence");
        DrawEditableDependency(ref package.m_otherPackageDependency);
        GUILayout.Space(10);
        GUILayout.Label("Relative dependence");
        DrawEditableDependency(ref package.m_otherPackageRelation);


        SampleDirectoryStream sample = SampleUtility.GetSampleFolder(selection);
        SampleEditor.DrawInfoAboutInterface(sample);

        DocumentationDirectoryStream documentation = DocumentationUtility.GetDocumentFolder(selection);
        DocumentationEditor.DrawInfoAboutInterface(documentation);

    }

    private static void LoadSamplesFromDirectoryToPackage(UnityPathSelectionInfo selection, ref PackageBuildInformation package)
    {

        SampleDirectoryStream samplesDir = SampleUtility.GetSampleFolder(selection);
        string[] folders = SampleUtility.GetRelativeFoldersIn(samplesDir);
        for (int i = 0; i < folders.Length; i++)
        {
            string name = folders[i];
            name = UnityPaths.GetLastPartOfPath(name);
            package.m_samples.m_samples.Add(new SampleInfo()
            {
                m_displayName = name,
                m_assetRelativePath = folders[i],
                m_description = "" 
            }) ;

        }

    }

    private static void DrawEditableDependency(ref PackageDependencyId [] dependencies)
    {
       
        for (int i = 0; i < dependencies.Length; i++)
        {
            GUILayout.BeginHorizontal();
            dependencies[i].m_nameId = GUILayout.TextField(dependencies[i].m_nameId);
            dependencies[i].m_nameId =
                UnityPaths.NamespaceTrim(dependencies[i].m_nameId);

            dependencies[i].m_versionOrGitUrl = GUILayout.TextField(dependencies[i].m_versionOrGitUrl);
            if (string.IsNullOrEmpty(dependencies[i].m_versionOrGitUrl))
                dependencies[i].m_versionOrGitUrl = "0.0.0";
            GUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Dependency"))
        {
            List<PackageDependencyId> d = dependencies.ToList();
            d.Add(new PackageDependencyId());
            dependencies = d.ToArray();
        }
        if (GUILayout.Button("Remove Empty"))
        {
            List<PackageDependencyId> d = dependencies.ToList();
            
            dependencies =d.Where(k => k.m_nameId != null && k.m_nameId.Length > 0).ToArray();
        }
    }




}