using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
public class UnityPackageAutoBuild : MonoBehaviour
{
    public string m_gitLink;
 
    public string[] m_directoriesStructure;
    
    public UnityPackageBuilderJson m_packageJson;
    public string m_projectPath;


    [Header("Contact info")]
    public string m_patreonLink = "http://patreon.com/eloistree";
    public string m_contact = "http://eloistree.page.link/discord";
    [Header("Linked")]
    public PackagePullPush m_pullPush;


    [HideInInspector]
    public string m_gitUserName = "eloistree";


    public void Reset()
    {
        m_projectPath = Application.dataPath;
        MakeSureThatPullPushScriptIsAssociatedToThisScript();
        m_directoriesStructure = new string[] {
        "Runtime"
       ,"Runtime/Scene"
       ,"Runtime/Script"
       ,"Runtime/Script/Shared/Gist"
       ,"Runtime/Assets/"
       ,"Runtime/Assets/Shared/"
       ,"Editor"
       ,"Editor/Script"
    };
        m_packageJson.m_projectId.SetProjectName(Application.productName);
        m_packageJson.m_projectId.SetWithTodayDate();
        MakeSureThatTheAssemblyEditorTargetTheRuntimeOne();


    }

    public void OnValidate()
    {
        m_projectPath = Application.dataPath;
        if (m_packageJson == null)
            return;
        if (m_packageJson.m_assemblyRuntime == null)
            return;
        if (m_packageJson.m_assemblyEditor == null)
            return;
        m_pullPush.SetGitLink(m_gitLink);
        SetWithGitLinkName(m_gitLink, ref m_packageJson);
        m_packageJson.m_assemblyRuntime.m_packageName = m_packageJson.GetProjectNameId(true);
        m_packageJson.m_assemblyEditor.m_packageName = m_packageJson.GetProjectNameId(true) + "editor";
        m_packageJson.m_assemblyRuntime.m_packageNamespaceId = m_packageJson.GetProjectNamespaceId(true);
        m_packageJson.m_assemblyEditor.m_packageNamespaceId = m_packageJson.GetProjectNamespaceId(true) + "editor";
        MakeSureThatPullPushScriptIsAssociatedToThisScript();
    }
    public void MakeSureThatTheAssemblyEditorTargetTheRuntimeOne() {


        UnityPackageAssemblyBuilderJson assEditor = m_packageJson.m_assemblyEditor;
        UnityPackageAssemblyBuilderJson assRuntime = m_packageJson.m_assemblyRuntime;

        List<string> editorRef = assEditor.m_reference.ToList();

        editorRef.Remove(assRuntime.m_packageNamespaceId);
        editorRef.Insert(0,assRuntime.m_packageNamespaceId);
        assEditor.m_reference = editorRef.ToArray();



    }


    public void MakeSureThatPullPushScriptIsAssociatedToThisScript()
    {
        if (m_pullPush == null)
            m_pullPush = GetComponent<PackagePullPush>();

        if (m_pullPush == null)
            m_pullPush = gameObject.AddComponent<PackagePullPush>();
        m_pullPush.SetGitLink( m_gitLink);
    }

    public string GetFolderPath()
    {
        return m_projectPath + "/" + m_packageJson.GetProjectDatedId(false);
    }

    private string CleanForNameSpace(string value)
    {
        return value.ToLower().Replace(" ", "").Replace(".", "");
    }

   

    private void SetWithGitLinkName(string gitlink, ref UnityPackageBuilderJson packageJson)
    {
        packageJson.m_projectId.SetWithIdInText(gitlink);

    }
}
[System.Serializable]
public class EloiProjectIdFormat {
    [Range(2018, 2030)]
    [SerializeField] int m_year = 2019;
    [Range(1, 12)]
    [SerializeField] int m_month = 1;
    [Range(1, 31)]
    [SerializeField] int m_day = 1;
    [SerializeField] string m_projectName= "UnnamedPackage";
    public static string m_idRegex = "\\d\\d\\d\\d_\\d\\d_\\d\\d_[\\w\\d_]*";
    public static string m_dateRegex = "\\d\\d\\d\\d_\\d\\d_\\d\\d";
    public static string m_prefixRegex = "\\d\\d\\d\\d_\\d\\d_\\d\\d_";

    public void SetWithTodayDate() {
        String sDate = DateTime.Now.ToString();
        DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
        SetWithDate(datevalue);
    }
    public void SetWithDate(DateTime time ) {
        m_year = time.Year;
        m_month = time.Month;
        m_day = time.Day;
    }

    public void SetWithIdInText(string text) {
        string value = GetFirstProjectInText(text);
        string[] tokens = value.Split('_');
        try
        {
            m_year = int.Parse(tokens[0]);
            m_month = int.Parse(tokens[1]);
            m_day = int.Parse(tokens[2]);
            m_projectName = (tokens[3]);

        }
        catch (Exception) { return; };

    }

    public string GetFirstProjectInText(string text) {
        string[] projects = FindProjectsInText(text);
        if(projects.Length>0)
             return projects[0];
        return "";
    }
    public string[] FindProjectsInText(string text) {
        List<string> result = new List<string>();
        foreach (Match match in Regex.Matches(text, m_idRegex))
        {
            result.Add(match.Value);
        }
        return result.ToArray();
    }

    public void SetProjectName(string name) {
        m_projectName = name;
    }

    public string GetProjectDatedNameId(bool toLower=false) {
        string id = string.Format("{0:0000}_{1:00}_{2:00}_{3}",m_year, m_month, m_day, GetProjectNameWithoutSpace());
        if(toLower)
        id = id.ToLower();
        return id;
    }

    public string GetProjectNameWithoutSpace(bool toLower=false)
    {
        string id = m_projectName.Replace(" ", "");
        if (toLower)
            id = id.ToLower();
        return id;
    }

    public string GetProjectDisplayName()
    {
        return m_projectName;
    }
}

[System.Serializable]
public class UnityPackageAssemblyBuilderJson
{

    [HideInInspector]
    public string m_packageName;
    [HideInInspector]
    public string m_packageNamespaceId;
    public string[] m_reference;
    [HideInInspector]
    public bool m_isEditorAssembly;
}
[System.Serializable]
public class UnityPackageBuilderJson
{
    public EloiProjectIdFormat m_projectId;
    [Header("Project info")]
    public string country = "be";
    public string company = "eloiexperiments";
    [TextArea(1,5)]
    public string m_description = "No description";
    public string[] m_keywords = new string[] { "Script", "Tool" };
    public CatergoryType m_category = CatergoryType.Script;
    public string m_packageVersion = "0.0.1";
    public string m_unityVersion = "2018.1";
    public string[] m_dependencies = new string[] { "be.eloiexperiments.randomtool" };
    public UnityPackageAssemblyBuilderJson m_assemblyRuntime;
    public UnityPackageAssemblyBuilderJson m_assemblyEditor = new UnityPackageAssemblyBuilderJson() { m_isEditorAssembly = true };
    public RequiredClassicPackageJson m_classicUnityPackageRequired = new RequiredClassicPackageJson {
        m_packageLinks = new ClassicPackageLink[] {
            new ClassicPackageLink("Oculus Light","https://gitlab.com/eloistree/2019_07_23_OculusQuestLight/raw/master/OculusIntegrationLight.unitypackage")
        ,
            new ClassicPackageLink("Oculus Official","https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022")
        }
    };


    public string GetProjectDatedId(bool toLower)
    {

        return m_projectId.GetProjectDatedNameId(toLower);
    }

    public string GetProjectNameId()
    {
        return m_projectId.GetProjectNameWithoutSpace();
    }
    public string GetProjectNamespaceId(bool useLower=false)
    {
        string id = string.Format("{0}.{1}.{2}", country, company, GetProjectNameId());
        if (useLower)
            id= id.ToLower();
        return id ;
    }

    internal string GetProjectNameId(bool toLower)
    {
        return m_projectId.GetProjectNameWithoutSpace(toLower);
    }

    public enum CatergoryType { Script }
}

[System.Serializable]
public class RequiredClassicPackageJson
{
    public ClassicPackageLink[] m_packageLinks = new ClassicPackageLink[] { };

    public string ToJson() { return JsonUtility.ToJson(this); }
    public static RequiredClassicPackageJson FromJsonPath(string path) {
        if (File.Exists(path))
            return FromJsonText(File.ReadAllText(path));
        else return new RequiredClassicPackageJson();
    }
    public static RequiredClassicPackageJson FromJsonText(string jsonText) { return JsonUtility.FromJson<RequiredClassicPackageJson>(jsonText); }

}
[System.Serializable]
public class ClassicPackageLink {
    public string m_name="";
    public string m_pathOrLink = "";

    public ClassicPackageLink(string name, string pathOrLink)
    {
        m_name = name;
        m_pathOrLink = pathOrLink;  
    }

    public bool IsRelativePath()
    {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        return !IsWindowPath() && !IsWebPath();
    }

    public bool IsWebPath()
    {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        return m_pathOrLink.ToLower().IndexOf("http://")>-1 || m_pathOrLink.ToLower().IndexOf("https://")>-1;
    }

    public bool IsWindowPath()
    {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        return Path.IsPathRooted(m_pathOrLink) ;
    }

    public bool IsAssetStoreLink() {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        return IsWebPath() && m_pathOrLink.IndexOf("assetstore") > -1;
    }

    public bool IsUnityPackage() {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        int indexOfPackage = m_pathOrLink.ToLower().LastIndexOf(".unitypackage");
        if (indexOfPackage < 0)
            return false;
        return m_pathOrLink.Substring(indexOfPackage) == ".unitypackage";
    }
    
}
