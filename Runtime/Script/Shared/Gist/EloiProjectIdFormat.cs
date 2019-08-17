using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class EloiProjectIdFormat
{
    [Range(2018, 2030)]
    [SerializeField] int m_year = 2019;
    [Range(1, 12)]
    [SerializeField] int m_month = 1;
    [Range(1, 31)]
    [SerializeField] int m_day = 1;
    [SerializeField] string m_projectName = "UnnamedPackage";
    public static string m_idRegex = "\\d\\d\\d\\d_\\d\\d_\\d\\d_[\\w\\d_]*";
    public static string m_dateRegex = "\\d\\d\\d\\d_\\d\\d_\\d\\d";
    public static string m_prefixRegex = "\\d\\d\\d\\d_\\d\\d_\\d\\d_";

    public void SetWithTodayDate()
    {
        String sDate = DateTime.Now.ToString();
        DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
        SetWithDate(datevalue);
    }
    public void SetWithDate(DateTime time)
    {
        m_year = time.Year;
        m_month = time.Month;
        m_day = time.Day;
    }

    public void AddDateInFrontOf(string text)
    {
        string value = GetFirstProjectInText(text);
        string[] tokens = value.Split('_');
        try
        {
            m_year = int.Parse(tokens[0]);
            m_month = int.Parse(tokens[1]);
            m_day = int.Parse(tokens[2]);
            m_projectName = (tokens[3]);

        }
        catch (Exception) { return; };

    }

   

    public string GetFirstProjectInText(string text)
    {
        string[] projects = FindProjectsInText(text);
        if (projects.Length > 0)
            return projects[0];
        return "";
    }
    public string[] FindProjectsInText(string text)
    {
        List<string> result = new List<string>();
        foreach (Match match in Regex.Matches(text, m_idRegex))
        {
            result.Add(match.Value);
        }
        return result.ToArray();
    }

    public void SetProjectName(string name)
    {
        m_projectName = name;
    }
    public static string GetProjectDateFormatFor(int year, int month, int day, string projectName = "")
    {

        return string.Format("{0:0000}_{1:00}_{2:00}_{3}",
            year,
            month,
            day,
            projectName);
    }
    public static string GetProjectDateFormatFor(string projectName = "") {
        String sDate = DateTime.Now.ToString();
        DateTime datevalue = (Convert.ToDateTime(sDate.ToString()));
        return string.Format("{0:0000}_{1:00}_{2:00}_{3}",
            datevalue.Year,
            datevalue.Month,
            datevalue.Day,
            projectName);
    }

    public string GetProjectDatedNameId(bool toLower = false)
    {
        string id = GetProjectDateFormatFor(GetProjectNameWithoutSpace());
        if (toLower)
            id = id.ToLower();
        return id;
    }

    public string GetProjectNameWithoutSpace(bool toLower = false)
    {
        string id = m_projectName.Replace(" ", "");
        if (toLower)
            id = id.ToLower();
        return id;
    }

    public string GetProjectDisplayName()
    {
        return m_projectName;
    }
}
