using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;
using UnityEditor.PackageManager;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class PackageBuilder 
{
    

    public static void CreateFolder(string relativePathFolder)
    {
        Directory.CreateDirectory(GetAbsolutPathInProject(relativePathFolder));
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

    }

   
    public static void CreateUnityPackage(string aboslutePath, PackageBuildInformation package)
    {

        package.CheckThatAssemblyAreDefined();
        Directory.CreateDirectory(aboslutePath);

        string packageJson = GetPackageAsJson(package);
        File.WriteAllText(aboslutePath + "/" + "package.json", packageJson);
        string path = "";
        string absFile, absFolder;

        GetAssemblyFilepath(aboslutePath, package.m_assemblyRuntime,out absFolder, out absFile);

        Directory.CreateDirectory(absFolder);
        File.Delete(absFile);
        File.WriteAllText(absFile, GetAssemblyAsJson( package.m_assemblyRuntime));
        File.WriteAllText(absFolder + "/" + UnityPaths.AlphaNumeric(package.GetProjectNamespaceId(false) )+ ".cs", "//Empty Script");

        GetAssemblyFilepath(aboslutePath, package.m_assemblyEditor, out absFolder, out absFile);
        Directory.CreateDirectory(absFolder);
        File.Delete(absFile);
        File.WriteAllText(absFile, GetAssemblyAsJson( package.m_assemblyEditor));
        File.WriteAllText(absFolder + "/" + UnityPaths.AlphaNumeric(package.GetProjectNamespaceId(false)) + ".cs", "//Empty Script");
#if UNITY_EDITOR

        AssetDatabase.Refresh();
#endif

    }

    private static string GetAbsolutPathInProject(string relativePathFolder)
    {
        return Application.dataPath + "/" + relativePathFolder;
    }

    public static void GetAssemblyFilepath(string absolutePath, AssemblyBuildInformation assembly, out string absFolder, out string absFile) {
        string file, folder;
        assembly.GetRelativePathOfAssembly(out folder, out file);
        absFolder = absolutePath + "/" + folder;
        absFile = absolutePath + "/" + file;
    }

    public static string GetAssemblyAsJson( AssemblyBuildInformation assemblyInfo)
    {
        string[] dependenciesModificatedForJson = new string[assemblyInfo.m_reference.Length];
        for (int i = 0; i < assemblyInfo.m_reference.Length; i++)
        {
            dependenciesModificatedForJson[i] = "\"" + assemblyInfo.m_reference[i] + "\"";
        }

        string packageJson = "";
        packageJson += "\n{                                                                   ";
        packageJson += "\n            \"name\": \"" + assemblyInfo.GetNameSpace() + "\",          ";
        packageJson += "\n    \"references\": [";
        packageJson += string.Join(",", dependenciesModificatedForJson);
        packageJson += "\n],                                                              ";
        packageJson += "\n    \"optionalUnityReferences\": [],                                  ";
        if (assemblyInfo.m_assemblyType == AssemblyBuildInformation.AssemblyType.Editor)
        {
            packageJson += "\n    \"includePlatforms\": [                                           ";
            packageJson += "\n        \"Editor\"                                                    ";
            packageJson += "\n    ],                                                              ";
        }
        else
        {
            packageJson += "\n    \"includePlatforms\": [],                                                  ";
        }
        packageJson += "\n    \"excludePlatforms\": [],                                         ";
        packageJson += "\n    \"allowUnsafeCode\": false,                                       ";
        packageJson += "\n    \"overrideReferences\": false,                                    ";
        packageJson += "\n    \"precompiledReferences\": [],                                    ";
        packageJson += "\n    \"autoReferenced\": true,                                         ";
        packageJson += "\n    \"defineConstraints\": []                                         ";
        packageJson += "\n}                                                                   ";

        return packageJson;
    }





    private void CreateFolders(string whereToCreate, ProjectDirectoriesStructure structure)
    {
        structure.Create(whereToCreate);
    }


    public static void CreatePackageJson(string absolutePath, string packageJson) {

        Directory.CreateDirectory(absolutePath);
        File.Delete(absolutePath + "/package.json");
        File.WriteAllText(absolutePath + "/package.json", packageJson);
    }

    public  static string GetPackageAsJson( PackageBuildInformation packageInfo,  string fileName = "package.json")
    {
        string packageJson = "";
        string[] dependenciesModificatedForJson = CompressDependenciesToString(packageInfo.m_otherPackageDependency);
        string[] dependenciesRelationModificatedForJson = CompressDependenciesToString(packageInfo.m_otherPackageRelation);

        string[] keywordForJson = new string[packageInfo.m_keywords.Length];
        for (int i = 0; i < packageInfo.m_keywords.Length; i++)
        {
            keywordForJson[i] = "\"" + packageInfo.m_keywords[i] + "\"";
        }

        packageJson += "\n{                                                                                ";
        packageJson += "\n  \"name\": \"" + packageInfo.GetProjectNamespaceId(true) + "\",                              ";
        packageJson += "\n  \"displayName\": \"" + packageInfo.m_projectName + "\",                        ";
        packageJson += "\n  \"version\": \"" + packageInfo.m_packageVersion + "\",                         ";
        packageJson += "\n  \"unity\": \"" + packageInfo.m_unityVersion + "\",                             ";
        packageJson += "\n  \"unityRelease\": \"" + packageInfo.m_unityVersionRelease + "\",                             ";
        packageJson += "\n  \"description\": \"" + packageInfo.m_description + "\",                         ";
        packageJson += "\n  \"keywords\": [" + string.Join(",", keywordForJson) + "],                       ";
        packageJson += "\n  \"dependencies\":{" + string.Join(",", dependenciesModificatedForJson) + "},     ";
        packageJson += "\n  \"relatedPackages\":{" + string.Join(",", dependenciesRelationModificatedForJson) + "},     ";
        packageJson += "\n  \"samples\" : [" + GetSamplesCompress(packageInfo.m_samples.m_samples.ToArray()) + "],     ";
        packageJson += "\n  \"author\" : {" +
            "\n\"name\":\"" + packageInfo.m_author.m_name + "\"," +
            "\n\"mail\":\"" + packageInfo.m_author.m_mail + "\"," +
            "\n\"url\":\"" + packageInfo.m_author.m_url + "\"" + "},     "; 
        
        packageJson += "\n  \"repository\":{" +
           "\n\"type\":\"" + packageInfo.m_repositoryLink.m_type + "\"," +
           "\n\"url\":\"" + packageInfo.m_repositoryLink.m_url + "\"," +
           "\n\"footprint\":\"" + packageInfo.m_repositoryLink.m_footprint + "\"," +
           "\n\"revision\":\"" + packageInfo.m_repositoryLink.m_revision + "\"" + "},     ";


        packageJson += "\n  \"category\": \"" + packageInfo.m_category.ToString().Replace("_", " ") + "\"                   ";
        packageJson += "\n} ";

        return packageJson;
    }

   
    private static string GetSamplesCompress(SampleInfo[] samples)
    {
        string[] tokens = new string[samples.Length];
        string result = "";
        for (int i = 0; i < samples.Length; i++)
        {
            result = "";
            result += "\n{";
            result += string.Format("\n\"displayName\":\"{0}\" ,", samples[i].m_displayName);
            result += string.Format("\n\"description\":\"{0}\" ,", samples[i].m_description);
            result += string.Format("\n\"path\":\"{0}\"", UnityPaths.ReplaceByBackslash( samples[i].m_assetRelativePath));
            result += "\n}";
            tokens[i] = result;
        }
        return string.Join(",\n", tokens);
    }

    private static string[] CompressDependenciesToString(PackageDependencyId[] dependencies)
    {
        string[] dependenciesModificatedForJson = new string[dependencies.Length];
        for (int i = 0; i < dependencies.Length; i++)
        {
            dependenciesModificatedForJson[i] =
                "\"" + dependencies[i].m_nameId +
                "\": \"" + dependencies[i].GetVersionOrUrl() + "\"";
        }

        return dependenciesModificatedForJson;
    }
}




///////////////////////////////////////////////////////////////
/// <summary>
/// 
/// </summary>///////////////////////////////////////////////////////////////


[System.Serializable]
public class AssemblyBuildInformation
{

    [HideInInspector]
    public string m_packageName="";
    [HideInInspector]
    
    [SerializeField] string m_packageNamespaceId="";
    public string[] m_reference = new string [0];
    [HideInInspector]
    public AssemblyType m_assemblyType = AssemblyType.Unity;

    public void SetNameSpace(string namespaceID){
        m_packageNamespaceId = UnityPaths.NamespaceTrim(namespaceID);
    }
    public string GetNameSpace() { return UnityPaths.NamespaceTrim(m_packageNamespaceId); }

    public void GetRelativePathOfAssembly(out string folder, out string file)
    {
       
            if (m_assemblyType == AssemblyBuildInformation.AssemblyType.Editor)
            {
                folder= "/Editor";
                file =  "/Editor/" + m_packageNamespaceId + ".Editor.asmdef";
            }
            else
            {
                folder = "/Runtime/";
                file =  "/Runtime/" + m_packageNamespaceId + ".Runtime.asmdef";
            }
        
    }

    public enum AssemblyType { Unity, Editor, Test }
}
[System.Serializable]
public class PackageBuildInformation
{
    [Header("Project info")]
    public string m_projectAlphNumId;
    public string m_projectName;
    public string m_country = "com";
    public string m_company = "youcompany";

    [TextArea(1, 5)]
    public string m_description = "You package description";
    public string[] m_keywords = new string[] { "Script", "Tool" };
    public CatergoryType m_category = CatergoryType.Script;
    public string m_packageVersion = "0.0.1";
    public string m_unityVersion = "2018.1";

    public PackageDependencyId[] m_otherPackageDependency = new PackageDependencyId[] { };
    public PackageDependencyId[] m_otherPackageRelation = new PackageDependencyId[] { };
    public AssemblyBuildInformation m_assemblyRuntime = new AssemblyBuildInformation() { m_assemblyType = AssemblyBuildInformation.AssemblyType.Unity };
    public AssemblyBuildInformation m_assemblyEditor = new AssemblyBuildInformation() { m_assemblyType = AssemblyBuildInformation.AssemblyType.Editor };
    public AuthorInformation m_author = new AuthorInformation();
    public GitRepository m_repositoryLink = new GitRepository();
    public SamplesInfo m_samples= new SamplesInfo();
    public string m_unityVersionRelease;

    public string GetProjectNamespaceId(bool useLower = false)
    {
        string id =string.Format("{0}.{1}.{2}",
            UnityPaths.AlphaNumeric(m_country),
            UnityPaths.AlphaNumeric(m_company),
            UnityPaths.AlphaNumeric(m_projectAlphNumId));
        if (useLower)
            id = id.ToLower();
        return id;
    }
    

    public void CheckThatAssemblyAreDefined()
    {
        m_assemblyRuntime.m_packageName = this.m_projectAlphNumId;
        m_assemblyRuntime.SetNameSpace( this.GetProjectNamespaceId());
        m_assemblyRuntime.m_assemblyType = AssemblyBuildInformation.AssemblyType.Unity;

        m_assemblyEditor.m_packageName = this.m_projectAlphNumId + "Editor";
        m_assemblyEditor.SetNameSpace(this.GetProjectNamespaceId() +"Editor");
        m_assemblyEditor.m_assemblyType = AssemblyBuildInformation.AssemblyType.Editor;
        CheckThatRuntimeIsInEditor(m_assemblyRuntime, m_assemblyEditor);
    }

    public void CheckThatRuntimeIsInEditor( AssemblyBuildInformation runtime, AssemblyBuildInformation editor)
    {


        List<string> editorRef = editor.m_reference.ToList();
        editorRef.Remove(runtime.GetNameSpace());
        editorRef.Insert(0, runtime.GetNameSpace());
        editor.m_reference = editorRef.ToArray();

    }

    public string GetProjectNameId(bool toLower)
    {
        string id = m_projectAlphNumId.Replace(" ", "");
        if (toLower)
            id = id.ToLower();
        return id;

    }

    public enum CatergoryType { Script, Libraries,Cinematography,Text_Rendering,Unity_Test_Framework, Other }
}

[System.Serializable]
public class AuthorInformation
{
    public string m_name="";
    public string m_url="";
    public string m_mail="";

}
[System.Serializable]
public class GitRepository
{
    public string m_url = "";
    public string m_type = "Git";
    public string m_revision = "";
    public string m_footprint = "";

}

[System.Serializable]
public class SamplesInfo {
    public List<SampleInfo> m_samples = new List<SampleInfo>();
}

[System.Serializable]
public class SampleInfo
{
    public string m_displayName = "";
    public string m_description = "Git";
    public string m_assetRelativePath = "Samples~/";

    public void ResetDefaultSamplesPath() { 
      m_assetRelativePath= "Samples~/";
    }
}

