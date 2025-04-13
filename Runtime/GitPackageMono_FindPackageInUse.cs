using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eloi.Git;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GitPackageMono_FindPackageInUse : MonoBehaviour
{

    public string m_currentDirectory = Directory.GetCurrentDirectory();
    public MonoBehaviour[] m_allScriptsInSceen;
    public Type[] m_allTypeofScripts ;
    public string[] m_allTypeofScriptsName;

    public string[] m_allSharpPaths;
    public List<PathWithFile> m_allPathsWithFile = new List<PathWithFile>();
    public List<PathWithFile> m_filePresentInScene = new List<PathWithFile>();

    public string[] m_allPackageJsonFiles;
    public string[] m_allPackageDirectories;

    [System.Serializable]
    public class PathWithFile { 
        public string m_fileName;
        public string m_directory;
        public string m_fullPath;

    }

    public List<DirectoryWithFileInIt> m_filesWithInDirectory = new List<DirectoryWithFileInIt>();
    [System.Serializable]
    public class DirectoryWithFileInIt { 
    
        public string m_directory;
        public List<PathWithFile> m_filesInIt = new List<PathWithFile>();
    }


    public List<GitLinkOnDisk> m_gitUseInTheScene = new List<GitLinkOnDisk>();

    [TextArea(1, 6)]
    public string m_gitCloneList;

    public List<GitToPackageJson> m_gitToPackages = new List<GitToPackageJson>();
    [System.Serializable]
    public class GitToPackageJson {
        public string m_gitLink;
        public string m_packageJsonPath;
        public string m_packageJsonDirectory;
        public GitLinkOnDisk m_gitLinkOnDisk;

        public string m_jsonFileContent;
        public string m_namepackage;

        public string m_hashOf_ORIG_HEAD;



    }

    [TextArea(0, 5)]
    public string m_gitCopyPackageJson;


    [TextArea(0, 5)]
    public string m_gitCopyPackageJsonWithHash;

    [TextArea(0,5)]
    public string m_markdownFullPage;

    public List<string> m_assemblyInPackageFolder = new List<string>();
    


    [ContextMenu("Do The Thing")]
    public void DoTheThing() {

        m_currentDirectory = Directory.GetCurrentDirectory() + "/Assets";
        //Fetch all MonoBehaviours in the scene
        m_allScriptsInSceen = GameObject.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        m_allTypeofScripts = m_allScriptsInSceen.Select(k=>k.GetType()).Distinct().ToArray();
        m_allTypeofScriptsName = m_allTypeofScripts.Select(k => k.Name+ ".cs").ToArray();

        m_allSharpPaths = System.IO.Directory.GetFiles(m_currentDirectory, "*.cs", System.IO.SearchOption.AllDirectories);
        m_allPackageJsonFiles = System.IO.Directory.GetFiles(m_currentDirectory, "package.json", System.IO.SearchOption.AllDirectories);
        m_allPackageDirectories = m_allPackageJsonFiles.Select(k => System.IO.Path.GetDirectoryName(k)).Distinct().ToArray();

        m_allPathsWithFile = new List<PathWithFile>();
        foreach (var path in m_allSharpPaths)
        {
            PathWithFile pathWithFile = new PathWithFile();
            pathWithFile.m_fileName = System.IO.Path.GetFileName(path);
            pathWithFile.m_directory = System.IO.Path.GetDirectoryName(path);
            pathWithFile.m_fullPath = path;
            m_allPathsWithFile.Add(pathWithFile);
        }

        m_filePresentInScene = new List<PathWithFile>();
        foreach (var path in m_allPathsWithFile)
        {
            foreach (var typeName in m_allTypeofScriptsName)
            {
                if (path.m_fileName == typeName)
                {
                    m_filePresentInScene.Add(path);
                }
            }
        }

        m_filesWithInDirectory.Clear();
        foreach (var path in m_allPackageDirectories)
        {
                    DirectoryWithFileInIt directoryWithFileInIt = new DirectoryWithFileInIt();
                    directoryWithFileInIt.m_directory = path;
            foreach (var file in m_filePresentInScene)
            {
                if (file.m_directory.StartsWith(path))
                {
                    directoryWithFileInIt.m_filesInIt.Add(file);
                }
            }
                    m_filesWithInDirectory.Add(directoryWithFileInIt);

        }
        // remove form m_filesWithInDirectory all the directory that hs not files in it
        m_filesWithInDirectory.RemoveAll(k => k.m_filesInIt.Count == 0);

        string assetPath = UnityPaths.GetUnityAssetsPath();
        m_gitUseInTheScene.Clear();
        foreach (var path in m_filesWithInDirectory)
        {
            QuickGit.GetGitsInParents(path.m_directory, out List<GitLinkOnDisk> gitFound);
            m_gitUseInTheScene.AddRange(gitFound);
        }
        m_gitCloneList = "```\n";
        for (int i = 0; i < m_gitUseInTheScene.Count; i++)
        {
            m_gitCloneList += m_gitUseInTheScene[i].m_gitLink + "\n";
        }
        m_gitCloneList += "```\n";


        m_gitToPackages.Clear();
        foreach (var gitLink in m_gitUseInTheScene)
        {
            GitToPackageJson gitToPackageJson = new GitToPackageJson();
            gitToPackageJson.m_gitLink = gitLink.m_gitLink;
            gitToPackageJson.m_packageJsonPath = System.IO.Path.Combine(gitLink.m_projectDirectoryPath, "package.json");
            gitToPackageJson.m_packageJsonDirectory = gitLink.m_projectDirectoryPath;
            gitToPackageJson.m_gitLinkOnDisk = gitLink;
            m_gitToPackages.Add(gitToPackageJson);
        }

        foreach (var gitToPackage in m_gitToPackages)
        {
            if (File.Exists(gitToPackage.m_packageJsonPath))
            {
                gitToPackage.m_jsonFileContent = File.ReadAllText(gitToPackage.m_packageJsonPath);
                string[] lines = gitToPackage.m_jsonFileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("\"name\""))
                    {
                        string[] splitLine = line.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitLine.Length > 1)
                        {
                            string name = splitLine[1].Trim().Replace("\"", "");
                            // name must be in namespace format aa.bb.cc
                            string[] nameSplit = name.Split('.');
                            if (nameSplit.Length >=3)
                            {
                                gitToPackage.m_namepackage = name;
                            }
                        }
                    }
                }


            }
            string fileOrigin = System.IO.Path.Combine(gitToPackage.m_gitLinkOnDisk.m_projectDirectoryPath,".git", "ORIG_HEAD");
            if (File.Exists(fileOrigin))
            {
                gitToPackage.m_hashOf_ORIG_HEAD = File.ReadAllText(fileOrigin);
            }

           
        }

        m_gitCopyPackageJson = "```\n";
        foreach (var gitToPackage in m_gitToPackages)
        {
            if (!string.IsNullOrEmpty(gitToPackage.m_namepackage))
            {
                m_gitCopyPackageJson += "  \"" + gitToPackage.m_namepackage + "\": \"" + gitToPackage.m_gitLink + "\",\n";
            }
        }
        m_gitCopyPackageJson += "```\n";

        m_gitCopyPackageJsonWithHash = "```\n";
        foreach (var gitToPackage in m_gitToPackages)
        {
            if (!string.IsNullOrEmpty(gitToPackage.m_namepackage))
            {
                m_gitCopyPackageJsonWithHash += "  \"" + gitToPackage.m_namepackage + "\": \"" + gitToPackage.m_gitLink + "#" + gitToPackage.m_hashOf_ORIG_HEAD + "\",\n";
            }
        }
        m_gitCopyPackageJsonWithHash += "```\n";
        m_markdownFullPage = $"\n# Packages used in scene \"{ SceneManager.GetActiveScene().name }\"\n\n";
        m_markdownFullPage += $"\n## As links \n\n";
        m_markdownFullPage += m_gitCloneList;
        m_markdownFullPage += $"\n## As package.json \n\n";
        m_markdownFullPage += m_gitCopyPackageJson;
        m_markdownFullPage += $"\n## As package.json with hash \n\n";
        m_markdownFullPage += m_gitCopyPackageJsonWithHash;



        foreach (var gitToPackage in m_gitToPackages)
        {
            string[] assemblies = System.IO.Directory.GetFiles(gitToPackage.m_packageJsonDirectory, "*.asmdef", System.IO.SearchOption.AllDirectories);
            m_assemblyInPackageFolder.AddRange(assemblies);
        }
        m_assemblyInPackageFolder = m_assemblyInPackageFolder.Distinct().ToList();
        m_assemblyJsons.Clear();
        foreach (var assembly in m_assemblyInPackageFolder)
        {
            string json = File.ReadAllText(assembly);
            AssemblyJson assemblyJson = JsonUtility.FromJson<AssemblyJson>(json);
            m_assemblyJsons.Add(assemblyJson);
        }
        m_assemblyNames.Clear();
        foreach (var assemblyJson in m_assemblyJsons)
        {
            m_assemblyNames.Add(assemblyJson.name);
            foreach (var reference in assemblyJson.references)
            {
                m_assemblyNames.Add(reference);
            }
        }
        m_assemblyNames = m_assemblyNames.Distinct().ToList();

        m_allAssemblyInProject = System.IO.Directory.GetFiles(m_currentDirectory, "*.asmdef", System.IO.SearchOption.AllDirectories);
        m_allFilePathOfAssemblyName.Clear();
        foreach (string assemblyName in m_assemblyNames)
        {
            foreach (string assemblyPath in m_allAssemblyInProject)
            {
               if (assemblyPath.Contains(assemblyName))
                {
                    m_allFilePathOfAssemblyName.Add(assemblyPath);
                }
            }
        }

        m_gitOfAssembly.Clear();
        foreach (var assemblyPath in m_allFilePathOfAssemblyName)
        {
            string directory = System.IO.Path.GetDirectoryName(assemblyPath);
            QuickGit.GetGitsInParents(directory, out List<GitLinkOnDisk> gitsAssembly);
            m_gitOfAssembly.AddRange(gitsAssembly);
        }
        m_gitOfAssembly = m_gitOfAssembly
            .GroupBy(a => a.m_gitLink)
            .Select(g => g.First())
            .ToList();

        m_gitToPackagesFromAssembly.Clear();
        foreach (var gitLink in m_gitOfAssembly)
        {
            GitToPackageJson gitToPackageJson = new GitToPackageJson();
            gitToPackageJson.m_gitLink = gitLink.m_gitLink;
            gitToPackageJson.m_packageJsonPath = System.IO.Path.Combine(gitLink.m_projectDirectoryPath, "package.json");
            gitToPackageJson.m_packageJsonDirectory = gitLink.m_projectDirectoryPath;
            gitToPackageJson.m_gitLinkOnDisk = gitLink;
            m_gitToPackagesFromAssembly.Add(gitToPackageJson);
        }

        foreach (var gitToPackage in m_gitToPackagesFromAssembly)
        {
            if (File.Exists(gitToPackage.m_packageJsonPath))
            {
                gitToPackage.m_jsonFileContent = File.ReadAllText(gitToPackage.m_packageJsonPath);
                string[] lines = gitToPackage.m_jsonFileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    if (line.Contains("\"name\""))
                    {
                        string[] splitLine = line.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (splitLine.Length > 1)
                        {
                            string name = splitLine[1].Trim().Replace("\"", "");
                            // name must be in namespace format aa.bb.cc
                            string[] nameSplit = name.Split('.');
                            if (nameSplit.Length >= 3)
                            {
                                gitToPackage.m_namepackage = name;
                            }
                        }
                    }
                }


            }
            string fileOrigin = System.IO.Path.Combine(gitToPackage.m_gitLinkOnDisk.m_projectDirectoryPath, ".git", "ORIG_HEAD");
            if (File.Exists(fileOrigin))
            {
                gitToPackage.m_hashOf_ORIG_HEAD = File.ReadAllText(fileOrigin);
            }


        }

        m_magicCopyPastGit = "## Pakcage with hash\n";
        m_magicCopyPastGit += "```\n";
        foreach (var gitToPackage in m_gitToPackagesFromAssembly)
        {
            if (!string.IsNullOrEmpty(gitToPackage.m_namepackage))
            {
                m_magicCopyPastGit += "  \"" + gitToPackage.m_namepackage + "\": \"" + gitToPackage.m_gitLink + "#" + gitToPackage.m_hashOf_ORIG_HEAD.Trim() + "\",\n";
            }
        }
        m_magicCopyPastGit += "```\n\n";
        m_magicCopyPastGit += "## Package without hash\n\n";
        m_magicCopyPastGit += "```\n\n";
        foreach (var gitToPackage in m_gitToPackagesFromAssembly)
        {
            if (!string.IsNullOrEmpty(gitToPackage.m_namepackage))
            {
                m_magicCopyPastGit += "  \"" + gitToPackage.m_namepackage + "\": \"" + gitToPackage.m_gitLink +"\",\n";
            }
        }
        m_magicCopyPastGit += "```\n";



    }
    public List<AssemblyJson > m_assemblyJsons = new List<AssemblyJson>();
    public List<string> m_assemblyNames = new List<string>();

    public string[] m_allAssemblyInProject;
    public List<string> m_allFilePathOfAssemblyName = new List<string>();

    public List<GitLinkOnDisk> m_gitOfAssembly = new List<GitLinkOnDisk>();

    public List<GitToPackageJson> m_gitToPackagesFromAssembly = new List<GitToPackageJson>();


    public string m_magicCopyPastGit = string.Empty;
}


/**
 
 
{
    "name": "be.elab.unitypackagefacilitator",
    "references": [
        "be.elab.quickgitutility"
    ]
}
 */

[System.Serializable]
public class AssemblyJson {

    public string name;
    public string[] references;
}
