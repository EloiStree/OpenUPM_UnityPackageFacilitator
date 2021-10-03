using Eloi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ChangeLogUtility 
{
    public static ChangeLogFileStream GetReadMeFile(UnityPathSelectionInfo m_selector)
    {
        return new ChangeLogFileStream(m_selector.GetAbsolutePath(true), ".md");

    }

    public static string OnlyDigitsAndPoints(string text) {
        Regex rgx = new Regex("[^0-9\\.]");
        return  rgx.Replace(text, "");
    }

    public static void Create(ChangeLogFileStream changelog, string title, string description) {
        if (!changelog.Exist())
        {
            changelog.Create("# " + title + "  \n" + description + "  \n\n");
        }
    }

    public static void AppendLog(ChangeLogFileStream changelog, string version, string title, string logs)
    {
        if (!changelog.Exist()) return;

        string text = changelog.Get();
        int firstLogIndex = text.IndexOf("\n## ");
        if (firstLogIndex < 0)
            firstLogIndex = text.IndexOf("\r## ");
        if (firstLogIndex < 0)
            firstLogIndex = text.Length - 1;
        string before = text.Substring(0, firstLogIndex);
        string after = text.Substring(firstLogIndex);

        string toAppend = "";

        toAppend += string.Format("\n\n## {0} - {1}\n", OnlyDigitsAndPoints(version), DateTime.Now.ToString("yyyy-MM-dd"));
        toAppend += "### "+title+ "\n";
        toAppend += StartWithDash(logs);
        toAppend += "\n";

        changelog.Set(before + toAppend + after);
    }

    public static string GetLastVersion(ChangeLogFileStream changelog)
    {
        if (changelog == null || !changelog.Exist()) return "0.0.0";

        string t = changelog.Get();
        Regex regex = new Regex("([\\n\\r]##\\s\\s*[\\d\\.]+)");


        Match match = regex.Match(t);

        if (match.Success)
        {
            return ChangeLogUtility.OnlyDigitsAndPoints( match.Value);
        }

        return "0.0.0";
    }

    public static  string StartWithDash(string text)
    {
        string[] tokens = text.Split('\n');
        for (int i = 0; i < tokens.Length; i++)
        {
            if (tokens[i].Length <= 0)
                tokens[i] = "- ";
            else if (tokens[i][0] != '-')
                tokens[i] = "- " + tokens[i];
        }
        return string.Join("\n", tokens);
    }

}




public class ChangeLogFileStream : AbstractFileStream
{
    public ChangeLogFileStream(string path) : base(path) { }
    public ChangeLogFileStream(string folderPath, string extensionIfNotFound) :
base(folderPath, "ChangeLog", extensionIfNotFound,
new string[] { ".txt", ".md" })
    {


    }
}