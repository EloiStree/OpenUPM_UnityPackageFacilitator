using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityPackageAutoBuild : MonoBehaviour
{
    public string m_gitLink;
    [Header("Package Info")]
    public string country="be";
    public string company="eloiexperiments";
    public string[] m_directoriesStructure;
    public PackageJson m_packageJson;
    [Header("Semi-automatic")]
    public AssemblyJson m_assemblyRuntime;
    public AssemblyJson m_assemblyEditor = new AssemblyJson() { m_isEditorAssembly = true };
    public string m_projectPath;

    public void Reset()
    {

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
        m_packageJson.m_packageIdName = string.Format("{0}.{1}.{2}", CleanForNameSpace(country), CleanForNameSpace(company), CleanForNameSpace( Application.productName));
        m_packageJson.m_displayName = Application.productName;
        m_assemblyRuntime.m_packageIdName = m_packageJson.m_packageIdName.ToLower();
        m_assemblyEditor.m_packageIdName = m_packageJson.m_packageIdName.ToLower() + "editor";
        OnValidate();


    }

    private string CleanForNameSpace(string value)
    {
        return value.ToLower().Replace(" ", "").Replace(".", "");
    }

    public void OnValidate()
    {

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