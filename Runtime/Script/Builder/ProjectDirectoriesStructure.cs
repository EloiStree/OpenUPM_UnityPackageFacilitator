// Shoul belong to an other Unity Package
using System.IO;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ProjectDirectoriesStructure
{

    public string[] m_defaultDirectory;
    public FileFromText[] m_defaultFiles;
    public FileFromweb[] m_defaultFilesFromWeb;

    public void Create(string whereToCreate)
    {
        if (string.IsNullOrWhiteSpace(whereToCreate) || whereToCreate.Length == 0)
            return;

        char lastCharacer = whereToCreate[whereToCreate.Length - 1];
        if (lastCharacer == '/' || lastCharacer == '\\')
        {
            whereToCreate = whereToCreate.Substring(0, whereToCreate.Length - 1);
        }
        Directory.CreateDirectory(whereToCreate);
        whereToCreate += "/";

        for (int i = 0; i < m_defaultDirectory.Length; i++)
        {
            string path = whereToCreate + m_defaultDirectory[i];
            Directory.CreateDirectory(path);
        }
        for (int i = 0; i < m_defaultFiles.Length; i++)
        {
            string path = whereToCreate + m_defaultFiles[i].m_relativePath;

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, m_defaultFiles[i].m_text);
        }
        for (int i = 0; i < m_defaultFilesFromWeb.Length; i++)
        {
            string path = whereToCreate + m_defaultFilesFromWeb[i].m_relativePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            GetTextFromUrl(m_defaultFilesFromWeb[i].m_url, path);
        }
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private string GetTextFromUrl(string url, string where)
    {
        using (var wc = new System.Net.WebClient())
        {
            wc.DownloadFile(url, where);
        }
        return File.ReadAllText(where);
    }
    private string GetTextFromUrl(string url)
    {
        string contents = "";
        using (var wc = new System.Net.WebClient())
            contents = wc.DownloadString(url);
        return contents;
    }
}

[System.Serializable]
public class FileFromweb
{
    public string m_relativePath;
    public string m_url;
}
[System.Serializable]
public class FileFromText
{
    public string m_relativePath;
    [TextArea(0, 10)]
    public string m_text;
}