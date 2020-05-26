using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Linq;


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


        GetAssemblyFilepath(aboslutePath, package.m_assemblyEditor, out absFolder, out absFile);
        Directory.CreateDirectory(absFolder);
        File.Delete(absFile);
        File.WriteAllText(absFile, GetAssemblyAsJson( package.m_assemblyEditor));
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
        packageJson += "\n            \"name\": \"" + assemblyInfo.m_packageNamespaceId + "\",          ";
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
        string[] dependenciesModificatedForJson = new string[packageInfo.m_otherPackageDependency.Length];
        for (int i = 0; i < packageInfo.m_otherPackageDependency.Length; i++)
        {
            dependenciesModificatedForJson[i] = "\"" + packageInfo.m_otherPackageDependency[i] + "\": \"0.0.1\"";
        }
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
        packageJson += "\n  \"description\": \"" + packageInfo.m_description + "\",                         ";
        packageJson += "\n  \"keywords\": [" + string.Join(",", keywordForJson) + "],                       ";
        packageJson += "\n  \"category\": \"" + packageInfo.m_category.ToString() + "\",                   ";
        packageJson += "\n  \"dependencies\":{" + string.Join(",", dependenciesModificatedForJson) + "}     ";
        packageJson += "\n} ";

        return packageJson;
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
    public string m_packageNamespaceId="";
    public string[] m_reference = new string [0];
    [HideInInspector]
    public AssemblyType m_assemblyType = AssemblyType.Unity;

    internal void GetRelativePathOfAssembly(out string folder, out string file)
    {
       
            if (m_assemblyType == AssemblyBuildInformation.AssemblyType.Editor)
            {
                folder= "/Editor";
                file =  "/Editor/com.unity." + m_packageNamespaceId + ".Editor.asmdef";
            }
            else
            {
                folder = "/Runtime/";
                file =  "/Runtime/com.unity." + m_packageNamespaceId + ".Runtime.asmdef";
            }
        
    }

    public enum AssemblyType { Unity, Editor, Test }
}
[System.Serializable]
public class PackageBuildInformation
{
    [Header("Project info")]
    public string m_projectId;
    public string m_projectName;
    public string m_country = "com";
    public string m_company = "youcompany";

    [TextArea(1, 5)]
    public string m_description = "You package description";
    public string[] m_keywords = new string[] { "Script", "Tool" };
    public CatergoryType m_category = CatergoryType.Script;
    public string m_packageVersion = "0.0.1";
    public string m_unityVersion = "2018.1";
    [Tooltip("com.yourcompany.example")]
    public PackageDependencyId[] m_otherPackageDependency = new PackageDependencyId[] { };
    public AssemblyBuildInformation m_assemblyRuntime = new AssemblyBuildInformation() { m_assemblyType = AssemblyBuildInformation.AssemblyType.Unity };
    public AssemblyBuildInformation m_assemblyEditor = new AssemblyBuildInformation() { m_assemblyType = AssemblyBuildInformation.AssemblyType.Editor };
    public AuthorInformation m_author;

    public string GetProjectNamespaceId(bool useLower = false)
    {
        string id = string.Format("{0}.{1}.{2}", m_country, m_company, m_projectId);
        if (useLower)
            id = id.ToLower();
        return id;
    }
    

    internal void CheckThatAssemblyAreDefined()
    {
        m_assemblyRuntime.m_packageName = this.m_projectId;
        m_assemblyRuntime.m_packageNamespaceId = this.m_projectId;
        m_assemblyRuntime.m_assemblyType = AssemblyBuildInformation.AssemblyType.Unity;

        m_assemblyEditor.m_packageName = this.m_projectId + "Editor";
        m_assemblyEditor.m_packageNamespaceId = this.m_projectId+"Editor";
        m_assemblyEditor.m_assemblyType = AssemblyBuildInformation.AssemblyType.Editor;
        CheckThatRuntimeIsInEditor(m_assemblyRuntime, m_assemblyEditor);
    }

    internal void CheckThatRuntimeIsInEditor( AssemblyBuildInformation runtime, AssemblyBuildInformation editor)
    {


        List<string> editorRef = editor.m_reference.ToList();
        editorRef.Remove(runtime.m_packageNamespaceId);
        editorRef.Insert(0, runtime.m_packageNamespaceId);
        editor.m_reference = editorRef.ToArray();

    }

    internal string GetProjectNameId(bool toLower)
    {
        string id = m_projectId.Replace(" ", "");
        if (toLower)
            id = id.ToLower();
        return id;

    }

    public enum CatergoryType { Script }
}

[System.Serializable]
public class AuthorInformation {
    public string m_name;
    public string m_url;
    public string m_mail;

}

