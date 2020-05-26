
using System.IO;
using UnityEngine;

[System.Serializable]
public class ListOfClassicPackages
{
    public ClassicUnityPackageLink[] m_packageLinks = new ClassicUnityPackageLink[] { };

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
public class ClassicUnityPackageLink
{
    public string m_name = "";
    public string m_pathOrLink = "";

    public void CreateWindowLinkFile(string folderPath, bool alphaNumeric)
    {
        string name = m_name;
        if (alphaNumeric)
            AlphaNumeric(m_name);

        Directory.CreateDirectory(folderPath);
        string fileFormat = "[InternetShortcut]\nURL =" + m_pathOrLink;
        File.WriteAllText(folderPath + "/" + name + ".url", fileFormat);
    }

    private string AlphaNumeric(string name)
    {
        return name.Replace(" ", "").Replace(".", "").Replace("-", "");
    }

    public ClassicUnityPackageLink(string name, string pathOrLink)
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



