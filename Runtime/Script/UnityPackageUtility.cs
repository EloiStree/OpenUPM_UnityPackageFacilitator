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

    public static string GetManifestPath() { return Application.dataPath + "/../Packages/manifest.json"; }
    public static string GetManifestJson() {
        if (File.Exists(GetManifestPath()))
            return File.ReadAllText(GetManifestPath());
        return "";
    }
    public static UnityPackageManifest GetManifest()
    {
        return UnityPackageManifest.CreateFromUnityEditor();
    }
    public static void SetManifest(UnityPackageManifest manifest) {
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

    public static UnityPackageManifest CreateManifestFrom(string json)
    {
        return UnityPackageManifest.CreateFromJson(json);
    }

    public static GitUnityPackageJson GetPackageInfo(string packagePath)
    {
        return GitUnityPackageJson.CreateFromFile(packagePath);
    }


    public static void SetPackageInFolder(string packageFolderPath, GitUnityPackageJson package)
    {
        File.WriteAllText(GetPackagePath(packageFolderPath), package.ToJson());

    }

    public static void RemovePackage(string url)
    {
        UnityPackageManifest manifest = GetManifest();
        manifest.RemoveFromUrl(url);
        SetManifest(manifest);

    }
    public static void AddPackage(string namespaceId,string url)
    {
        UnityPackageManifest manifest = GetManifest();
        manifest.Add(namespaceId,url);
        SetManifest(manifest);
    }

    public static void SetPackageFile(string packageFilePath , GitUnityPackageJson package)
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


    public static void OpenManifestFile()
    {
        Application.OpenURL(GetManifestPath());
    }

    internal static UnityPackageManifest DownloadPackageManifest(string url, out string received)
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
        return GetGitUnityPackageInDirectory(QuickGit.GetGitsInDirectory(directoryPath));
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
    public static bool GetUnityPackage(string directoryPath, out GitUnityPackageJson package)
    {
        package = null;
        string filePath = GetPackagePath(directoryPath);
        if (File.Exists(filePath))
            package = GitUnityPackageJson.CreateFromFile(filePath);
        return package != null;
    }
    #endregion
}


[System.Serializable]
public class UnityPackageManifest
{
    
     public List<DependencyJson> dependencies;
   
    //{{
    // "dependencies": {
    //        "be.eloistree.randomtool": "https://gitlab.com/eloistree/2019_01_15_randomtool.git",
    //        "be.eloistree.unityprefsthemall": "https://gitlab.com/eloistree/2019_06_10_unityprefsthemall.git",
    //        "com.unity.ads": "2.0.8",
    //        "com.unity.analytics": "3.3.2",
    //        "com.unity.collab-proxy": "1.2.16",
    //        "com.unity.modules.wind": "1.0.0",
    //        "com.unity.modules.xr": "1.0.0"
    //  },
    //  "lock": {
    //    "be.eloistree.randomtool": {
    //      "hash": "39187c85824aa974aa6791fdfe34158989907b7e",
    //      "revision": "HEAD"
    //    },
    //    "be.eloistree.unityprefsthemall": {
    //      "hash": "bc3801d94db295e2c391dfc35b29487d683be7f2",
    //      "revision": "HEAD"
    //    }
    //}}



    



    public void Add(string nameId, int v1, int v2, int v3) {
        RemoveFromName(nameId);
        dependencies.Insert(0, new DependencyJson(nameId, v1, v2, v3));
    }
    public void Add(string nameId, string urlLink)
    {
        RemoveFromName(nameId);
        RemoveFromName(urlLink);
        dependencies.Insert(0, new DependencyJson(nameId, urlLink));

    }

    public void RemoveFromName(string nameId)
    {
        dependencies = dependencies.Where(t => t.nameId.Trim() != nameId.Trim()).ToList();

    }
    public void RemoveFromUrl(string url)
    {
        dependencies = dependencies.Where(t => t.value.Trim() != url.Trim()).ToList();

    }
    


    public string ToJson() {

        string jsonResult = "{ \"dependencies\": {\n";
        if(dependencies!=null )
        for (int i = 0; i < dependencies.Count; i++)
        {
            jsonResult += string.Format("\"{0}\" : \"{1}\"{2}\n", 
                dependencies[i].nameId,
                dependencies[i].value,
                i >= dependencies.Count - 1?' ' :',');
        }


        jsonResult += "\n}}";
        return jsonResult;

    }

    public static UnityPackageManifest CreateFromUnityEditor()
    {
        return CreateFromFile(UnityPackageUtility.GetManifestPath());
    }

    public static UnityPackageManifest CreateFromFile(string filePath)
    {
        string file = File.ReadAllText(filePath);
        return CreateFromJson(file);
    }

    public static UnityPackageManifest CreateFromJson(string json)
    {
        UnityPackageManifest man = new UnityPackageManifest();
        try
        {
            man.dependencies =  DependencyJson.GetDependeciesFromText(json);
           
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning(e);
        }
        return man;
    }

    public void Add(List<DependencyJson> dependencies)
    {
        for (int i = 0; i < dependencies.Count; i++)
        {
            Add(dependencies[i]);

        }
    }
    public void Remove( List<DependencyJson> dependencies)
    {
        for (int i = 0; i < dependencies.Count; i++)
        {
            Remove(dependencies[i]);

        }
    }
    private void Add(DependencyJson dependencies)
    {
        Add(dependencies.nameId, dependencies.value);
    }
    private void Remove(DependencyJson dependencies)
    {
        RemoveFromName(dependencies.nameId);
        RemoveFromUrl(dependencies.value);
    }

    public void Remove(string removeValue)
    {
        RemoveFromName(removeValue);
        RemoveFromUrl(removeValue);
    }
}

[System.Serializable]
public class GitUnityPackageJson
{
    //"name": "be.eloistree.unitypackagefacilitator",                              
    public string name;
    public string GetNamespaceID() { return name; }
    //"displayName": "Unity Package Creator",                        
    public string displayName;
    public string GetDisplayName() { return displayName; }
    //"version": "0.0.1",                         
    public string version;
    public string GetPackageVersion() { return version; }
    //"unity": "2018.1",                             
    public string unity;
    public string GetUnityVersion() { return unity; }
    //"description": "Tools to create the structure of new unity package under 2 minutes.",                         
    public string description;
    public string GetDescriptionVersion() { return description; }
    //"keywords": ["Script","Tool","Productivity","Git","Unity Package"],                       
    public string[] keywords;
    public string[] GetKeywords() { return keywords; }
    //"category": "Script",                   
    public string category;
    public string GetCategory()
    {
        return category;
    }
    //"dependencies":{}   
    
    private List<DependencyJson > dependencies;

    public string ToJson() { return JsonUtility.ToJson(this,true); }
    public static GitUnityPackageJson CreateFromFile(string filePath)
    {
        string file = File.ReadAllText(filePath);
        return CreateFromJson(file);
    }

    public static GitUnityPackageJson CreateFromJson(string json)
    {
        GitUnityPackageJson value = null;
        try
        {
            value =  JsonUtility.FromJson<GitUnityPackageJson>(json);
        }
        catch (Exception)
        {
          //  UnityEngine.Debug.LogWarning(e);
        }
        value.dependencies = DependencyJson.GetDependeciesFromText(json);
      

        return value;
    }
   
}
[System.Serializable]
public class DependencyJson
{
    public static string dependencyRegex = "\"[\\w\\d-]*\\.[\\w\\d-]*\\.[\\w\\d-\\.]*\"\\s*:\\s*\".*\"";
    public static string depBlocRegex = "\"dependencies\"\\s*:\\s*{";
    public static List<DependencyJson> GetDependeciesFromText(string text) {

        int index = text.IndexOf("dependencies");
        if (index >= 0) {
            string t = text.Substring(index);
            int indexEnd = t.IndexOf('}');
            text = t.Substring(0, indexEnd);
        }
        


    List<DependencyJson> result = new List<DependencyJson>();
        Regex rgx = new Regex(dependencyRegex);

        foreach (Match match in rgx.Matches(text))
        {
            string rawLine = match.Value;
            string[] tokens =
            Regex.Split(rawLine, "\"\\s*:\\s*\""); 
            result.Add(new DependencyJson(tokens[0].Trim(' ').Trim('"'), tokens[1].Trim(' ').Trim('"')));
        }
        return result;
    }

    public string nameId;
    public string value;

    public DependencyJson(string nameId, int v1, int v2, int v3)
    {
        this.nameId = nameId;
        value = string.Format("{0}.{1}.{2}", v1, v2, v3);
    }

    public DependencyJson(string nameId, string urlLink)
    {
        this.nameId = nameId;
        value = urlLink;
    }

    public string GetNamespaceId() { return nameId; }
    public bool GetLink(out string url)
    {
        bool isVersion = IsVersionSet();
        url = "";
        if (!isVersion)
        {
            url = value;
        }
        return !isVersion;
    }

    public bool IsVersionSet()
    {
        int v1, v2, v3;
        return GetVersion(out v1, out v2, out v3);
    }
    public bool GetVersion(out int v1, out int v2, out int v3)
    {
        v1 = v2 = v3 = 0;
        string[] tokens = value.Split('.');
        if (tokens.Length == 3)
        {

            try
            {
                v1 = int.Parse(tokens[0]);
                v2 = int.Parse(tokens[1]);
                v3 = int.Parse(tokens[2]);

            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }
}



[System.Serializable]
public class GitUnityPackageLink : GitLink
{
    public GitUnityPackageJson m_packageInfo;
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