#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
//https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html
public class UnityPackageAutoBuild : MonoBehaviour
{
    public string m_gitLink;
    public string m_projectPath;
    public PackageJson m_packageJson;
    public AssemblyJson m_assemblyRuntime;
    public AssemblyJson m_assemblyEditor = new AssemblyJson() { m_isEditorAssembly =true};
    public string[] m_directoriesStructure = new string[] {
        "Runtime"
       ,"Runtime/Scene"
       ,"Runtime/Script"
       ,"Runtime/Script/Shared/Gist"
       ,"Runtime/Assets/"
       ,"Runtime/Assets/Shared/"
       ,"Editor"
       ,"Editor/Script"
    };

    public void Reset()
    {
       

        m_projectPath = Application.dataPath;
        String sDate = DateTime.Now.ToString();
        DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
        m_packageJson.m_year = datevalue.Year;
        m_packageJson.m_month = datevalue.Month;
        m_packageJson.m_day = datevalue.Day;
        m_packageJson.m_projectIdName = Application.productName.Replace(" ", "");
        m_packageJson.m_packageIdName = "be.eloiexperiments." + Application.productName.Replace(" ", "");
        m_packageJson.m_displayName = Application.productName;
        m_assemblyRuntime.m_packageIdName = m_packageJson.m_packageIdName;
        m_assemblyEditor.m_packageIdName = m_packageJson.m_packageIdName+"Editor";
        OnValidate();
        
     
    }
    public void OnValidate()
    {

        m_projectPath = Application.dataPath;
        m_packageJson.RefreshFolderName();
        m_packageJson.m_packageIdName = "be.eloiexperiments." + m_packageJson.m_projectIdName.Replace(" ", "");
        m_assemblyRuntime.m_packageName = m_packageJson.m_projectIdName;
        m_assemblyEditor.m_packageName = m_packageJson.m_projectIdName + "Editor";
        m_assemblyRuntime.m_packageIdName = m_packageJson.m_packageIdName;
        m_assemblyEditor.m_packageIdName = m_packageJson.m_packageIdName + "Editor";
    }

    public void CreateStructure()
    {
        string whereToCreate = m_projectPath+"/"+m_packageJson.m_folderName;
        CreatePackageJson(m_packageJson,whereToCreate );
        CreateFolders(whereToCreate, m_directoriesStructure);
        CreateAssembly(m_assemblyRuntime, whereToCreate);
        CreateAssembly(m_assemblyEditor, whereToCreate);
        AssetDatabase.Refresh();
    }

    public void CreateAssembly(AssemblyJson assemblyInfo, string whereToCreate)
    {
        string[] dependenciesModificatedForJson = new string[assemblyInfo.m_reference.Length];
        for (int i = 0; i < assemblyInfo.m_reference.Length; i++)
        {
            dependenciesModificatedForJson[i] = "\"" + assemblyInfo.m_reference[i] + "\"";
        }

        string packageJson = "";
        packageJson += "\n{                                                                   ";
       packageJson += "\n            \"name\": \""+ assemblyInfo .m_packageIdName+ "\",          ";
       packageJson += "\n    \"references\": [                                                 ";
//       packageJson += "\n        \"be.eloiexperiments.randomtool\"                             ";

        packageJson += string.Join(",", dependenciesModificatedForJson);
        packageJson += "\n    ],                                                              ";
       packageJson += "\n    \"optionalUnityReferences\": [],                                  ";
        if (assemblyInfo.m_isEditorAssembly)
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

        if (assemblyInfo.m_isEditorAssembly)
        {
            Directory.CreateDirectory(whereToCreate + "/Editor");
            string name = whereToCreate + "/Editor/com.unity." + assemblyInfo.m_packageName + ".Editor.asmdef";
            File.Delete(name);
            File.WriteAllText(name, packageJson);

        }
       else
        {
            Directory.CreateDirectory(whereToCreate + "/Runtime/");
            string name = whereToCreate + "/Runtime/com.unity." + assemblyInfo.m_packageName + ".Runtime.asmdef";
            File.Delete(name);
            File.WriteAllText(name, packageJson);

        }



    }

   



    private void CreateFolders(string whereToCreate,string [] structure)
    {
        for (int i = 0; i < structure.Length; i++)
        {
            Directory.CreateDirectory(whereToCreate +"/"+ structure[i]);

        }
    }

    private void CreatePackageJson( PackageJson packageInfo, string whereToCreate, string fileName = "package.json")
    {
        string packageJson="";
        string[] dependenciesModificatedForJson = new string[packageInfo.m_dependencies.Length];
        for (int i = 0; i < packageInfo.m_dependencies.Length; i++)
        {
            dependenciesModificatedForJson[i] = "\"" + packageInfo.m_dependencies[i] + "\": \"0.0.1\"";
        }
        packageJson += "\n{";

        packageJson += "\n{                                                                                           ";
        packageJson += "\n  \"name\": \""+ packageInfo.m_packageIdName + "\",                                           ";
        packageJson += "\n  \"displayName\": \"" + packageInfo.m_displayName + "\",                                                    ";
        packageJson += "\n  \"version\": \"" + packageInfo.m_packageVersion + "\",                                                                       ";
        packageJson += "\n  \"unity\": \"" + packageInfo.m_unityVersion + "\",                                                                        ";
        packageJson += "\n  \"description\": \"" +packageInfo.m_description + "\",              ";
        packageJson += "\n  \"keywords\": [                                                                             ";
        packageJson += string.Join(",", packageInfo.m_keywords);
        packageJson += "\n  ],                                                                                        ";
        packageJson += "\n  \"category\": \"" + packageInfo.m_category.ToString() + "\",                                                                     ";
        packageJson += "\n  \"dependencies\":                                                                             ";
        packageJson += "\n      {                                                                                     ";
        packageJson += string.Join(",", dependenciesModificatedForJson);
        packageJson += "\n      }                                                                                     ";
        packageJson += "\n}                                                                                           ";
        Directory.CreateDirectory(whereToCreate);
        File.Delete(whereToCreate + "/package.json");
        File.WriteAllText(whereToCreate + "/package.json", packageJson);
        File.WriteAllText(whereToCreate + "/readme.md", packageJson);
    }
}
    public enum CatergoryType { Script}
[CustomEditor(typeof(UnityPackageAutoBuild))]
public class UnityPackageAutoBuildEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        UnityPackageAutoBuild myScript = (UnityPackageAutoBuild)target;

        string whereToCreate = myScript.m_projectPath + "/" + myScript.m_packageJson.m_folderName;

      
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clone"))
        {
            Directory.CreateDirectory(whereToCreate);
            QuickGit.Clone( myScript.m_gitLink, whereToCreate);
            myScript.CreateStructure();
        }
        if (GUILayout.Button("Create files & directory"))
        {
            myScript.CreateStructure();
        }
        if (GUILayout.Button("Pull & Push"))
        {
            QuickGit.PullAddCommitAndPush(whereToCreate, DateTime.Now.ToString("yyyy mm dd hh mm"));
        }
        GUILayout.EndHorizontal();



        //if (!string.IsNullOrEmpty(m_gitLink))
        //{
        //    if (!Directory.Exists(whereToCreate) || (Directory.Exists(whereToCreate) && !Directory.Exists(whereToCreate + "/.git")))
        //    {
                
        //        UnityEngine.Debug.Log("Clone...");
        //        UnityEngine.Debug.Log("Where ... " + whereToCreate);
        //        QuickGit.Clone(m_gitLink, whereToCreate);
        //    }
        //}
    }
}

[System.Serializable]
public class PackageJson
{
    [Header("Project id")]
    [Range(2018, 2030)]
    public int m_year = 2019;
    [Range(1, 12)]
    public int m_month = 1;
    [Range(1, 31)]
    public int m_day = 1;
    public string m_projectIdName = "ProjectName";
    [Header("Project info")]
    public string m_folderName = "2019_01_01_ProjectName";
    public string m_packageIdName = "be.eloiexperiments.unnamed";
    public string m_displayName = "Unnamed package";
    public string m_description = "No description";
    public string[] m_keywords = new string[] { "Script", "Tool" };
    public CatergoryType m_category = CatergoryType.Script;
    public string m_packageVersion = "0.0.1";
    public string m_unityVersion = "2018.1";
    public string[] m_dependencies = new string[] { "be.eloiexperiments.randomtool" };

    internal void RefreshFolderName()
    {
        m_projectIdName = m_projectIdName.Replace(" ", "");
           m_folderName = string.Format("{0:00}_{1:00}_{2:00}_{3}", m_year, m_month, m_day, m_projectIdName);
    }
}

[System.Serializable]
public class AssemblyJson
{

    public string m_packageName;
    public string m_packageIdName;
    public string[] m_reference;
    public bool m_isEditorAssembly;
}

public static class UnityPackageManagerUtility {
    [MenuItem("Window /Package Utility / Update")]
    public static void RemoveLocker() {
        string packagePath = GetProjectPackagePath();
        string package = File.ReadAllText(packagePath);
        package = Regex.Replace(package, "(,)[.\\n\\r]*(\"lock\":)[\\S\\r\\n{}]*","}" );
        File.WriteAllText(packagePath, package);
    }

    private static string GetProjectPackagePath()
    {
        return Application.dataPath + "/../Packages/manifest.json";
    }
}


//// com.unity.unityprefsthemall.Runtime
public static class QuickGit
{

    public static void Clone(string gitUrl, string gitDirectoryPath)
    {
        RunCommands(new string[] {
                "git clone "+ gitUrl+ " "+ gitDirectoryPath
          }, gitDirectoryPath);
    }
    public static void Pull(string gitDirectoryPath)
    {
        RunCommands(new string[] {
                "git pull"
          }, gitDirectoryPath);
    }
    public static void PullAddCommitAndPush(string gitDirectoryPath, string commitDescription = "none")
    {
        RunCommands(new string[] {
                "git pull",
                "git add .",
                "git commit -m \"" + commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }
    public static void AddCommitAndPush(string gitDirectoryPath, string commitDescription = "none")
    {
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"" + commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }

    static void RunCommands(string[] cmds, string workingDirectory)
    {
        if (workingDirectory.Length < 2) return;

        char disk = 'C';
        if (workingDirectory[1] == ':')
            disk = workingDirectory[0];

        var process = new Process();
        var psi = new ProcessStartInfo();
        psi.FileName = "cmd.exe";
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.WorkingDirectory = workingDirectory;
        process.StartInfo = psi;
        process.Start();
        process.OutputDataReceived += (sender, e) => { Console.WriteLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { Console.WriteLine(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();



        using (StreamWriter sw = process.StandardInput)
        {
            sw.WriteLine(disk + ":");
            sw.WriteLine("cd " + workingDirectory);
            foreach (var cmd in cmds)
            {
                UnityEngine.Debug.Log("> " + cmd);
                sw.WriteLine(cmd);
            }
        }
        process.WaitForExit();
    }

}

#endif


