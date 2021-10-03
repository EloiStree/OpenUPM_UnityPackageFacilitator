using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Eloi
{

public class ReadMeUtility 
{
    public static ReadMeFileStream GetReadMeFile(UnityPathSelectionInfo m_selector)
    {
        return new ReadMeFileStream(m_selector.GetAbsolutePath(true), ".md");

    }

    public static string CreateBasicDefaultOnFrom(Utility_PackageJson packageInfo, GitLinkOnDisk gitLink)
    {
        string text = "# Welcome to: " + packageInfo.displayName + "  \n";
        text += "Never used a package before ?  \n";
        text += "You don't understand how to use this git repository ?  \n";
        text += "May be you should stop now and learn the basic:  \n";
        text += "https://eloistree.page.link/hellopackage  \n";
        text += "  \n";
        text += "You know how the music work ?  \n";
        text += "Here is the copy past link of this project:  \n";
        text += "`\"" + packageInfo.GetNamespaceID() + "\":\"" + gitLink.GetUrl() + "\"`  \n";
        text += "  \n";
        text += "In hope that this code help you.  \n";
        text += "Kind regards,  \n";
        text += Application.companyName;
        return text;
    }
    public static string CreateBasicDefaultOnFrom( GitLinkOnDisk gitLink)
    {
        
        string text = "# Welcome to: " + gitLink.GetUrl() + "  \n";
        text += "Never used a package before ?  \n";
        text += "You don't understand how to use this git repository ?  \n";
        text += "May be you should stop now and learn the basic:  \n";
        text += "https://eloistree.page.link/hellopackage  \n";
        text += "  \n";
        text += "You know how the music work ?  \n";
        text += "Here is the copy past link of this package manager:  \n";
        text += "`"+gitLink.GetUrl()+ "`  \n";
        text += "  \n";
        text += "In hope that this code help you.  \n";
        text += "Kind regards,  \n";
        text += Application.companyName;
        return text;
    }




}

public class AbstractFileStream {

    public string m_pathToFile;
    public AbstractFileStream(string absoluteFilePath)
    {
        m_pathToFile = absoluteFilePath;
    }

    public AbstractFileStream(string folderPath, string fileName, string defaultIfNonExisting, params string[] extension)
    {
        bool foundOne;
        string filePathFound;
        defaultIfNonExisting = UnityPaths.StartByPoint(defaultIfNonExisting);
        UnityPaths.GetPathOf(folderPath, fileName, extension, out foundOne, out filePathFound);
        if (foundOne)
            m_pathToFile=filePathFound;
        else m_pathToFile= (folderPath + "/" + fileName + defaultIfNonExisting);

    }

    public bool Exist() { return File.Exists(m_pathToFile); }
    public void Delete() { 
        if(File.Exists(m_pathToFile))
            File.Delete(m_pathToFile);
    }

    public void Open()
    {
        Application.OpenURL(m_pathToFile);
    }

    public virtual void Create(string text = "Yeah, you read me :)") { Set(text); }
    public string Get() { return File.ReadAllText(m_pathToFile); }
    public void Set(string text, bool refreshDataBaseAfter=false) {
       if (File.Exists(m_pathToFile))
            File.Delete(m_pathToFile);
       File.WriteAllText(m_pathToFile, text);
        if(refreshDataBaseAfter)    
            AssetDatabase.Refresh();
    }

    public string GetAbsolutePath() { return m_pathToFile; }
}

public class ReadMeFileStream : AbstractFileStream
{
    public ReadMeFileStream(string directPath) : base(directPath) { }
    public ReadMeFileStream(string folderPath, string extensionIfNotFound) :
        base(folderPath, "ReadMe", extensionIfNotFound, 
        new string[] { ".txt", ".md" })
    {
    
    
    }
    }
}