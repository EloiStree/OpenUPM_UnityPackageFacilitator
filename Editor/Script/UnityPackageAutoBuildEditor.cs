
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
//https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html

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
            QuickGit.Clone(myScript.m_gitLink, whereToCreate);
            CreateStructure(myScript);
        }
        if (GUILayout.Button("Create files & directory"))
        {
            CreateStructure(myScript);
        }
        if (GUILayout.Button("Pull & Push"))
        {
            QuickGit.PullAddCommitAndPush(whereToCreate, DateTime.Now.ToString("yyyy mm dd hh mm"));
        }
        if (GUILayout.Button("CMD"))
        {
            QuickGit.OpenCmd(whereToCreate);
        }
        if (GUILayout.Button("Folder"))
        {
            Application.OpenURL(myScript.GetFolderPath());
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Reminder:Git must be install and Git.exe must be add in System Variable Path.", MessageType.Warning, true);

    }
    public void CreateStructure(UnityPackageAutoBuild myScript)
    {
        string whereToCreate = myScript.m_projectPath + "/" + myScript.m_packageJson.m_folderName;
        CreatePackageJson(myScript.m_packageJson, whereToCreate, myScript.m_gitLink);
        CreateFolders(whereToCreate, myScript.m_directoriesStructure);
        CreateAssembly(myScript.m_assemblyRuntime, whereToCreate);
        CreateAssembly(myScript.m_assemblyEditor, whereToCreate);
        AssetDatabase.Refresh();
    }

    public void CreateAssembly( AssemblyJson assemblyInfo, string whereToCreate)
    {
        string[] dependenciesModificatedForJson = new string[assemblyInfo.m_reference.Length];
        for (int i = 0; i < assemblyInfo.m_reference.Length; i++)
        {
            dependenciesModificatedForJson[i] = "\"" + assemblyInfo.m_reference[i] + "\"";
        }

        string packageJson = "";
        packageJson += "\n{                                                                   ";
        packageJson += "\n            \"name\": \"" + assemblyInfo.m_packageIdName + "\",          ";
        packageJson += "\n    \"references\": [";
        //       packageJson += "\n        \"be.eloiexperiments.randomtool\"                             ";

        packageJson += string.Join(",", dependenciesModificatedForJson);
        packageJson += "\n],                                                              ";
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





    private void CreateFolders(string whereToCreate, string[] structure)
    {
        for (int i = 0; i < structure.Length; i++)
        {
            Directory.CreateDirectory(whereToCreate + "/" + structure[i]);

        }
    }

    private void CreatePackageJson( PackageJson packageInfo, string whereToCreate, string gitLink, string fileName = "package.json")
    {
        string packageJson = "";
        string[] dependenciesModificatedForJson = new string[packageInfo.m_dependencies.Length];
        for (int i = 0; i < packageInfo.m_dependencies.Length; i++)
        {
            dependenciesModificatedForJson[i] = "\"" + packageInfo.m_dependencies[i] + "\": \"0.0.1\"";
        }
        string[] keywordForJson = new string[packageInfo.m_keywords.Length];
        for (int i = 0; i < packageInfo.m_keywords.Length; i++)
        {
            keywordForJson[i] = "\"" + packageInfo.m_keywords[i] + "\"";
        }

        packageJson += "\n{                                                                                ";
        packageJson += "\n  \"name\": \"" + packageInfo.m_packageIdName + "\",                              ";
        packageJson += "\n  \"displayName\": \"" + packageInfo.m_displayName + "\",                        ";
        packageJson += "\n  \"version\": \"" + packageInfo.m_packageVersion + "\",                         ";
        packageJson += "\n  \"unity\": \"" + packageInfo.m_unityVersion + "\",                             ";
        packageJson += "\n  \"description\": \"" + packageInfo.m_description + "\",              ";
        packageJson += "\n  \"keywords\": [" + string.Join(",", keywordForJson) + "],";
        packageJson += "\n  \"category\": \"" + packageInfo.m_category.ToString() + "\",                   ";
        packageJson += "\n  \"dependencies\":{" + string.Join(",", dependenciesModificatedForJson) + "}";
        packageJson += "\n}                                                                                ";
        Directory.CreateDirectory(whereToCreate);
        File.Delete(whereToCreate + "/package.json");
        File.WriteAllText(whereToCreate + "/package.json", packageJson);

        string m_howToUse = "# How to use: " + packageInfo.m_displayName+ "   " ;
        m_howToUse += "\nAdd the following line to the [UnityRoot]/Packages/manifest.json    ";
        m_howToUse += "\n``` json     ";
        m_howToUse += "\n" + string.Format("\"{0}\":\"{1}\",", packageInfo.m_packageIdName, gitLink)+  "    "  ;
        m_howToUse += "\n```    ";
        m_howToUse += "\n--------------------------------------    ";

        File.WriteAllText(whereToCreate + "/readme.md", m_howToUse+ packageJson);

    }
}



public static class UnityPackageManagerUtility
{
    [MenuItem("Window /Package Utility / Update")]
    public static void RemoveLocker()
    {
        string packagePath = GetProjectPackagePath();
        string package = File.ReadAllText(packagePath);
        package = Regex.Replace(package, "(,)[.\\n\\r]*(\"lock\":)[\\S\\r\\n{}]*", "}");
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
    public static void OpenCmd(string gitDirectoryPath)
    {
        if (gitDirectoryPath.Length < 2) return;

        char disk = 'C';
        if (gitDirectoryPath[1] == ':')
            disk = gitDirectoryPath[0];
        //string cmd = disk + ":" + "&" + "cd \"" + gitDirectoryPath + "\"";

        string strCmdText;
        strCmdText = "/K " + disk + ":" + " && cd " + gitDirectoryPath + " && git status";
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = strCmdText;
        process.Start();
    }
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
