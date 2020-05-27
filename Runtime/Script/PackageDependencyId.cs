
using System;

[System.Serializable]
public class PackageDependencyId
{
    public string m_nameId = "com.company.packageid";
    public string m_versionOrGitUrl = "";

    public string GetVersionOrUrl()
    {
        if(m_versionOrGitUrl==null|| m_versionOrGitUrl.Length<=0 )
            return "0.0.1";
        return m_versionOrGitUrl;

    }
}