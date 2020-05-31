using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

public class DocumentationUtility : MonoBehaviour
{
    public static DocumentationDirectoryStream GetDocumentFolder(UnityPathSelectionInfo selector)
    {
        return new DocumentationDirectoryStream(selector.GetAbsolutePath(true));
    }

    public static void Create(UnityPathSelectionInfo selector, bool asHidden)
    {
        DocumentationDirectoryStream tmp = new DocumentationDirectoryStream(selector.GetAbsolutePath(true));
        tmp.Create(asHidden);
    }
    public static void Toggle(UnityPathSelectionInfo selector)
    {
        Debug.Log("Test>:" + selector.GetAbsolutePath(true));
        DocumentationDirectoryStream tmp = new DocumentationDirectoryStream(selector.GetAbsolutePath(true));
        if(tmp.Exist())
        tmp.ToggleVisiblity(); 
    }

}
public class DocumentationDirectoryStream : HiddenDirectoryStream
{
    public DocumentationDirectoryStream(string absoluteFolderPath): base(absoluteFolderPath,  "Documentation")
    { 
    
    }
    public override void Create(bool asHidden = true)
    {
        base.Create(asHidden);
        CreateOrOverrdieTextFile("Home.md", "# Hello Documenation !");
    }

   
}


public class HiddenDirectoryStream
{
    public string m_folderName;

    public string m_pathRoot;
    public string m_pathToFolder;
    public string m_pathToFolderHidden;
    public HiddenDirectoryStream(string absoluteFolderPath, string folderName)
    {
        m_folderName = folderName;
        m_pathRoot = absoluteFolderPath;
        m_pathToFolder = absoluteFolderPath+"/"+ folderName;
        m_pathToFolderHidden = absoluteFolderPath + "/" + folderName + "~";
    }
    public bool Exist() { return IsHidden() || IsVisible();    }
    public virtual void Create(bool asHidden=true) {
        Directory.CreateDirectory(asHidden?m_pathToFolderHidden: m_pathToFolder);
    }
    public string GetCurrentPath() {
        if (IsHidden()) return m_pathToFolderHidden;
        else return m_pathToFolder;
            }
    public void Set(string text) {Directory.CreateDirectory(m_pathToFolder); }
    public void SetHidden(bool hiddenValue) {
        if (!Exist()) throw new Exception("Documention don't existe, create it first");
        if (IsHidden() && !hiddenValue) { 
            Directory.Move(m_pathToFolderHidden, m_pathToFolder);
            AssetDatabase.Refresh();
        }
        if (!IsHidden() && hiddenValue)
        { 
            Directory.Move(m_pathToFolder,m_pathToFolderHidden);
            AssetDatabase.Refresh();
        }
    }
    public bool IsHidden()
    {
        return Directory.Exists(m_pathToFolderHidden);
    }
    public bool IsVisible()
    {
        return Directory.Exists(m_pathToFolder);
    }
    public void Delete()
    {
        string p = GetCurrentPath();
        if(Directory.Exists(p))
             Directory.Delete(p,true);
    }


    public void CreateOrOverrdieTextFile(string relativeFilePath, string text)
    {
        string path = GetCurrentPath();
        File.WriteAllText(path+"/"+relativeFilePath, text);
    }

    public void ToggleVisiblity()
    {
        Debug.Log("Test:" + IsHidden());
        SetHidden(!IsHidden());
    }
}