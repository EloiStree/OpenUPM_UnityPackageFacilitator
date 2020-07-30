using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;

public class UnityPackageUtility
{
    public static void OpenManifestFile()
    {
        Application.OpenURL(GetProjectPackagePath());
    }

    private static string GetProjectPackagePath()
    {
        return Application.dataPath + "/../Packages/manifest.json";
    }
    public static string GetManifestPath() { return Application.dataPath + "/../Packages/manifest.json"; }
    public static string GetManifestJson() {
        if (File.Exists(GetManifestPath()))
            return File.ReadAllText(GetManifestPath());
        return "";
    }

    public static void OpenPackageHiddenFolder()
    {
        Application.OpenURL(GetPackageHiddenFolder());
    }
    public static string GetPackageHiddenFolder()
    {
        return Directory.GetCurrentDirectory() + "\\Library\\PackageCache";
    }

    public static Utility_ManifestJson GetManifest()
    {
        return Utility_ManifestJson.CreateFromUnityEditor();
    }
    public static void SetManifest(Utility_ManifestJson manifest) {
        File.WriteAllText(GetManifestPath(), manifest.ToJson());
    }

    public static string GetPackagePath(string directoryPath) { return directoryPath + "/package.json"; }


    public static void DeleteManifest()
    {
        File.Delete(GetManifestPath());
    }
    public static void RemoveLocker()
    {
        SetManifest(GetManifest());
    }

    public static Utility_ManifestJson CreateManifestFrom(string json)
    {
        return Utility_ManifestJson.CreateFromJson(json);
    }

    public static Utility_PackageJson GetPackageInfo(string packagePath)
    {
        return Utility_PackageJson.CreateFromFile(packagePath);
    }


    public static void SetPackageInFolder(string packageFolderPath, Utility_PackageJson package)
    {
        File.WriteAllText(GetPackagePath(packageFolderPath), package.ToJson());

    }

    public static void RemovePackage(string url)
    {
        Utility_ManifestJson manifest = GetManifest();
        manifest.RemoveFromUrl(url);
        SetManifest(manifest);

    }
    public static void AddPackage(string namespaceId,string url)
    {
        Utility_ManifestJson manifest = GetManifest();
        manifest.Add(namespaceId,url);
        SetManifest(manifest);
    }

    public static void SetPackageFile(string packageFilePath , Utility_PackageJson package)
    {
        File.Delete(packageFilePath);
        File.WriteAllText(packageFilePath, package.ToJson());

    }

    public static string GetPackageRootInParent(string currentPath)
    {
        while (!string.IsNullOrEmpty(currentPath))
        {
            if (IsFolderContainRootOfPackage(currentPath))
                return currentPath;
            currentPath = GoUpInPath(currentPath);
        }
        return null;
    }

    private static bool IsFolderContainRootOfPackage(string currentPath)
    {
        if (!Directory.Exists(currentPath))
            return false;
       return  Directory.GetFiles(currentPath, "package.json").Length>0;
    }

    private static string GoUpInPath(string currentPath)
    {
        int lastIndex = currentPath.LastIndexOf('/');
        if (lastIndex < 0)
            lastIndex = currentPath.LastIndexOf('\\');
        if (lastIndex < 0)
            return "";
        return currentPath.Substring(0, lastIndex);
    }

 
    //public static void RemoveLocker(string nameId)
    //{
    //    throw new NotImplementedException();
    //}

    public static Utility_ManifestJson DownloadPackageManifest(string url, out string received)
    {
        WebClient client = new WebClient();
        received = client.DownloadString(url);
       return UnityPackageUtility.CreateManifestFrom(received);
    }


    public static bool TryToAccessPackageNamespaceIdFrom(TextAsset textAsset, out string namespaceID)
    {
        if (textAsset == null) {
            namespaceID = "";
            return false;
        }
        return TryToAccessPackageNamespaceIdFromText(textAsset.text, out namespaceID);
    }


    public static bool TryToAccessPackageNamespaceIdFromFolder(string projectRootPath, out string namespaceID)
    {
        namespaceID = "";
        if (!Directory.Exists(projectRootPath))
            return false;
        if (!File.Exists(projectRootPath+"/package.json"))
            return false;

        string text = File.ReadAllText(projectRootPath + "/package.json"); 
        return TryToAccessPackageNamespaceIdFromText(text, out namespaceID);
    }

    public static bool TryToAccessPackageNamespaceIdFromGitCloneUrl(string gitCloneUrl, out string namespaceID)
    {
        namespaceID = "";
        string link = gitCloneUrl;
        if (gitCloneUrl.ToLower().IndexOf("gitlab.com") > -1) {

            //https://gitlab.com/eloistree/2019_07_21_UnityPackageFacilitator.git
            //https://gitlab.com/eloistree/2019_07_21_UnityPackageFacilitator/raw/master/package.json
            link = gitCloneUrl.Replace(".git", "/raw/master/package.json");
        }
        if (gitCloneUrl.ToLower().IndexOf("github.com") > -1)
        {
        //https://github.com/EloiStree/2019_07_21_UnityPackageFacilitator.git
        //https://raw.githubusercontent.com/EloiStree/2019_07_21_UnityPackageFacilitator/master/README.md
          link = "https://raw.githubusercontent" + gitCloneUrl.Substring(gitCloneUrl.IndexOf(".com"));
            link = link.Replace(".git", "/master/package.json");
        }
        
        try
        {
            WebClient c = new WebClient();
            return TryToAccessPackageNamespaceIdFromText(c.DownloadString(link), out namespaceID);

        }
        catch (Exception)
        {
            //Debug.Log("Nope: "+ link);
        }
        namespaceID = "";
        return false;

    }
    public static bool TryToAccessPackageNamespaceIdFromText(string text, out string namespaceID)
    {
        string packagejonAsText = Regex.Match(text, "\"name\":\\s*\".*\"").Value;
        string[] token = Regex.Split(packagejonAsText, "\"\\s*:\\s*\"");
        if (token.Length == 2)
        {
            namespaceID = token[1].Trim().Trim('"');
            return true;
        }

        namespaceID = "";
        return false;
    }




    #region GIT PACKAGE ACCESS 

    public static List<GitUnityPackageLinkOnDisk> GetGitUnityPackageInDirectory(string directoryPath)
    {
        List<GitLinkOnDisk> links;
        QuickGit.GetGitsInDirectory(directoryPath, out links);
        return GetGitUnityPackageInDirectory(links);
    }
    public static void Down(string gitUrl, bool affectManifest = true)
    {
        Down(GetGitDirectoryPropositionRootPathInUnity(gitUrl), gitUrl, affectManifest);
     }
    public static void Down(string directory, string gitUrl, bool affectManifest=true)
    {

        string directoryPath = directory;
        Directory.CreateDirectory(directoryPath);
        if (!Directory.Exists(directoryPath+"/.git"))
            QuickGit.Clone(gitUrl, directoryPath);
        else QuickGit.Pull(directoryPath);

        if (affectManifest && !string.IsNullOrEmpty(gitUrl))
            UnityPackageUtility.RemovePackage(gitUrl);
    }
    public static void Up(string directory, bool affectManifest= true)
    {
        string namespaceId="", gitUrl="";

        TryToAccessPackageNamespaceIdFromFolder(directory, out namespaceId);
        QuickGit.GetGitUrl(directory, out gitUrl);
        Up(directory, namespaceId, gitUrl, affectManifest);
    }

    
    public static void Up(string directory, string namespaceId, string gitUrl, bool affectManifest=true)
    {
        string directoryPath = directory;
        QuickGit.AddFileInEmptyFolder(directoryPath);
        QuickGit.PullPushWithAddAndCommit(directoryPath, "Update: "+DateTime.Now.ToString("yyyy/mm/dd -  hh:mm"));
       
        #if UNITY_EDITOR

                UnityEditor.FileUtil.DeleteFileOrDirectory(directoryPath);
                UnityEditor.AssetDatabase.Refresh();
        #endif
        if (affectManifest && !string.IsNullOrEmpty(namespaceId) && !string.IsNullOrEmpty(gitUrl))
            UnityPackageUtility.AddPackage(namespaceId, gitUrl);
    }


    public static  string GetGitDirectoryPropositionRootPathInUnity(string gitLink)
    {
        return Application.dataPath + "/" + UnityPackageUtility.GetProjectNameFromGitLink(gitLink);
    }
    public static bool IsGitLinkValide(string gitLink)
    {
        if (string.IsNullOrEmpty(gitLink))
            return false;
        if (gitLink.LastIndexOf('/') < 0) return false;
        if (gitLink.ToLower().LastIndexOf(".git") < 0) return false;
        string name = GetProjectNameFromGitLink(gitLink);
        if (name == null || string.IsNullOrWhiteSpace(name))
            return false;
        return true;
    }
    public static string GetProjectNameFromGitLink(string gitLinkFormated)
    {
        if (gitLinkFormated == null)
            return "";
        // https://gitlab.com/eloistree/2019_07_22_oculusguardianidentity.git
        // https://github.com/EloiStree/CodeAndQuestsEveryDay.git
        int startProjectName = gitLinkFormated.LastIndexOf('/');
        if (startProjectName < 0)
            return "";
        string projectName = gitLinkFormated.Substring(startProjectName + 1).Replace(".git", "").Replace(".GIT", "");
        return projectName;
    }
    public static string RemoveWhiteSpace(string gitLinkFormated, string by = "_")
    {
        if (!string.IsNullOrEmpty(gitLinkFormated)
            ) return "";
        return gitLinkFormated.Replace(" ", by);
    }

    public static List<GitUnityPackageLinkOnDisk> GetGitUnityPackageInDirectory(string[] directoriesPath)
    {
        return GetGitUnityPackageInDirectory(QuickGit.GetGitsInGivenDirectories(directoriesPath));
    }
    public static List<GitUnityPackageLinkOnDisk> GetGitUnityPackageInDirectory(List<GitLinkOnDisk> packages)
    {
        List<GitUnityPackageLinkOnDisk> p = new List<GitUnityPackageLinkOnDisk>();
        for (int i = 0; i < packages.Count; i++)
        {
            p.Add(new GitUnityPackageLinkOnDisk(packages[i].m_projectDirectoryPath));

        }
        return p;
    }
    public static bool GetUnityPackage(string directoryPath, out Utility_PackageJson package)
    {
        package = null;
        string filePath = GetPackagePath(directoryPath);
        if (File.Exists(filePath))
            package = Utility_PackageJson.CreateFromFile(filePath);
        return package != null;
    }
    #endregion
}




[System.Serializable]
public class GitUnityPackageLink : GitLink
{
    public Utility_PackageJson m_packageInfo;
    public bool IsPackageDefined() { return m_packageInfo != null && !string.IsNullOrWhiteSpace(m_packageInfo.GetNamespaceID()); }
}
[System.Serializable]
public class GitUnityPackageLinkOnDisk : GitUnityPackageLink
{

    public string m_projectDirectoryPath;

    public GitUnityPackageLinkOnDisk(string directoryPath)
    {

        UnityPackageUtility.GetUnityPackage(directoryPath, out m_packageInfo);
        QuickGit.GetGitUrl(directoryPath, out m_gitLink);
        this.m_projectDirectoryPath = directoryPath;


    }
    public bool IsPathDefined() { return !string.IsNullOrWhiteSpace(m_projectDirectoryPath); }

}