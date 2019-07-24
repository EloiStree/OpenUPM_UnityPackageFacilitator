using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnityPackageAutoBuild : MonoBehaviour
{
    public string m_gitLink;
    [Header("Contact info")]
    public string m_patreonLink = "http://patreon.com/eloistree";
    public string m_contact = "http://eloistree.page.link/discord";
    [Header("Package Info")]
    public string country="be";
    public string company="eloiexperiments";
    public string[] m_directoriesStructure;
    public PackageJson m_packageJson;
    [Header("Semi-automatic")]
    public AssemblyJson m_assemblyRuntime;
    public AssemblyJson m_assemblyEditor = new AssemblyJson() { m_isEditorAssembly = true };
    public string m_projectPath;
    [Header("Linked")]
    public PackagePullPush m_pullPush;
    [HideInInspector]
    public string m_gitUserName = "eloistree";
    public void Reset()
    {
        RefreshToAccessPullPushScript();
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
        m_projectPath = Application.dataPath;
        String sDate = DateTime.Now.ToString();
        DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
        m_packageJson.m_year = datevalue.Year;
        m_packageJson.m_month = datevalue.Month;
        m_packageJson.m_day = datevalue.Day;
        m_packageJson.m_projectIdName = Application.productName.Replace(" ", "").ToLower();
        m_packageJson.m_packageIdName = string.Format("{0}.{1}.{2}", CleanForNameSpace(country), CleanForNameSpace(company), CleanForNameSpace(Application.productName));
        m_packageJson.m_displayName = Application.productName;
        m_assemblyRuntime.m_packageIdName = m_packageJson.m_packageIdName.ToLower();
        m_assemblyEditor.m_packageIdName = m_packageJson.m_packageIdName.ToLower() + "editor";
        OnValidate();


    }

    public void RefreshToAccessPullPushScript()
    {
        if (m_pullPush == null)
            m_pullPush = GetComponent<PackagePullPush>();

        if (m_pullPush == null)
            m_pullPush = gameObject.AddComponent<PackagePullPush>();
        m_pullPush.SetGitLink( m_gitLink);
    }

    public string GetFolderPath()
    {
        return m_projectPath + "/" + m_packageJson.m_folderName;
    }

    private string CleanForNameSpace(string value)
    {
        return value.ToLower().Replace(" ", "").Replace(".", "");
    }

    public void OnValidate()
    {

        if (m_packageJson == null)
            return;
        if (m_assemblyRuntime == null)
            return;
        if (m_assemblyEditor == null)
            return;

        RefreshToAccessPullPushScript();


        m_pullPush.SetGitLink(m_gitLink);
        m_projectPath = Application.dataPath;
        m_packageJson.RefreshFolderName();
        m_packageJson.m_packageIdName = string.Format("{0}.{1}.{2}", CleanForNameSpace(country), CleanForNameSpace(company), CleanForNameSpace(m_packageJson.m_projectIdName));
        m_assemblyRuntime.m_packageName = m_packageJson.m_projectIdName.ToLower();
        m_assemblyEditor.m_packageName = m_packageJson.m_projectIdName.ToLower() + "editor";
        m_assemblyRuntime.m_packageIdName = m_packageJson.m_packageIdName.ToLower();
        m_assemblyEditor.m_packageIdName = m_packageJson.m_packageIdName.ToLower() + "editor";

        List<string> editorRef = m_assemblyEditor.m_reference.ToList();
        editorRef.Remove(m_assemblyRuntime.m_packageIdName);
        editorRef.Add(m_assemblyRuntime.m_packageIdName);
        m_assemblyEditor.m_reference = editorRef.ToArray();
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
    [TextArea(1,5)]
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
    public enum CatergoryType { Script }
}