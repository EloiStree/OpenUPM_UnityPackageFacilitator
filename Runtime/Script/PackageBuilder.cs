using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackageBuilder 
{
    public void CreateTheAssembly(AssemblyBuildInformation assembly) {


    }

    public void CreateThePackage(PackageBuildInformation package) {


    }
}


[CreateAssetMenu(fileName = "PackageBuildInfo", menuName = "Facilitator/Create/Contact Information", order = 1)]
public class ContactInformationObject : ScriptableObject
{
    public ContactInformation m_data;
}


[CreateAssetMenu(fileName = "PackageBuildInfo", menuName = "Facilitator/Create/Package Info", order = 1)]
public class PackageBuildInformationObject : ScriptableObject
{
    public PackageBuildInformation m_data;
}
[CreateAssetMenu(fileName = "ListOfPackages", menuName = "Facilitator/Create/Classic packages", order = 1)]
public class ListOfClassicPackagesObject : ScriptableObject
{
    public ListOfClassicPackages m_data;
}
[CreateAssetMenu(fileName = "ListOfPackages", menuName = "Facilitator/Create/Directories Structure", order = 1)]
public class ProjectDirectoriesStructureObject : ScriptableObject
{
    public ProjectDirectoriesStructure m_data;
}



///////////////////////////////////////////////////////////////
/// <summary>
/// 
/// </summary>///////////////////////////////////////////////////////////////


[System.Serializable]
public class AssemblyBuildInformation
{

    [HideInInspector]
    public string m_packageName;
    [HideInInspector]
    public string m_packageNamespaceId;
    public string[] m_reference;
    [HideInInspector]
    public AssemblyType m_assemblyType;
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
    public string[] m_otherPackageDependency = new string[] { };
    public AssemblyBuildInformation m_assemblyRuntime = new AssemblyBuildInformation() { m_assemblyType = AssemblyBuildInformation.AssemblyType.Unity };
    public AssemblyBuildInformation m_assemblyEditor = new AssemblyBuildInformation() { m_assemblyType = AssemblyBuildInformation.AssemblyType.Editor };

    public ListOfClassicPackages m_requiredAndAdviceClassicPackage = new ListOfClassicPackages
    {
        m_packageLinks = new ClassicPackageLink[] {
           new ClassicPackageLink("Oculus Light","https://gitlab.com/eloistree/2019_07_23_OculusQuestLight/raw/master/OculusIntegrationLight.unitypackage")
        ,  new ClassicPackageLink("Oculus Official","https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022")
        }
    };

    public string GetProjectNamespaceId(bool useLower = false)
    {
        string id = string.Format("{0}.{1}.{2}", m_country, m_company, m_projectId);
        if (useLower)
            id = id.ToLower();
        return id;
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
public class ListOfClassicPackages
{
    public ClassicPackageLink[] m_packageLinks = new ClassicPackageLink[] { };

    public string ToJson() { return JsonUtility.ToJson(this); }
    public static ListOfClassicPackages FromJsonPath(string path)
    {
        if (File.Exists(path))
            return FromJsonText(File.ReadAllText(path));
        else return new ListOfClassicPackages();
    }
    public static ListOfClassicPackages FromJsonText(string jsonText) { return JsonUtility.FromJson<ListOfClassicPackages>(jsonText); }

}

[System.Serializable]
public class ClassicPackageLink
{
    public string m_name = "";
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
        return m_pathOrLink.ToLower().IndexOf("http://") > -1 || m_pathOrLink.ToLower().IndexOf("https://") > -1;
    }

    public bool IsWindowPath()
    {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        return Path.IsPathRooted(m_pathOrLink);
    }

    public bool IsAssetStoreLink()
    {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        return IsWebPath() && m_pathOrLink.IndexOf("assetstore") > -1;
    }

    public bool IsUnityPackage()
    {
        if (string.IsNullOrEmpty(m_pathOrLink))
            return false;
        int indexOfPackage = m_pathOrLink.ToLower().LastIndexOf(".unitypackage");
        if (indexOfPackage < 0)
            return false;
        return m_pathOrLink.Substring(indexOfPackage) == ".unitypackage";
    }
}

[System.Serializable]
public class ContactInformation
{
    [Header("Contact info")]
    public string m_firstName = "Eloi";
    public string m_lastName = "Strée";
    public string m_profilPictureLink = "https://avatars0.githubusercontent.com/u/20149493?s=460&v=4";
    public string m_howToContact = "http://eloistree.page.link/discord";
    public string m_patreonLink = "http://patreon.com/eloistree";
    public string m_paypalLink;
    public List<AdditionalInformation> m_additionalInformations;
}
[System.Serializable]
public class AdditionalInformation
{
    public string m_title;
    public string m_value;
}


// Shoul belong to an other Unity Package
[System.Serializable]
public class ProjectDirectoriesStructure {

    public string[] m_defaultDirectory;
    public FileFromText[] m_defaultFiles;
    public FileFromweb [] m_defaultFilesFromWeb;
}

[System.Serializable]
public class FileFromweb {
    public string m_relativePath;
    public string m_url;
}
[System.Serializable]
public class FileFromText
{
    public string m_relativePath;
    [TextArea(0,10)]
    public string m_text;
}