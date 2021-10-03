using Eloi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LicenseUtility 
{
    public static LicenseFileStream GetReadMeFile(UnityPathSelectionInfo m_selector)
    {
        return new LicenseFileStream(m_selector.GetAbsolutePath(true), ".md");

    }
}

public class LicenseFileStream : AbstractFileStream
{
    public LicenseFileStream(string path) : base(path) { }
    public LicenseFileStream(string folderPath, string extensionIfNotFound) :
       base(folderPath, "License", extensionIfNotFound,
       new string[] { ".txt", ".md" })
    {


    }

}