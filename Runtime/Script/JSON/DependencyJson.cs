using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[System.Serializable]
public class DependencyJson
{
    public static string dependencyRegex = "\"[\\w\\d-]*\\.[\\w\\d-]*\\.[\\w\\d-\\.]*\"\\s*:\\s*\".*\"";
    public static string depBlocRegex = "\"dependencies\"\\s*:\\s*{";
    public static List<DependencyJson> GetDependeciesFromText(string text)
    {

        int index = text.IndexOf("dependencies");
        if (index >= 0)
        {
            string t = text.Substring(index);
            int indexEnd = t.IndexOf('}');
            text = t.Substring(0, indexEnd);
        }



        List<DependencyJson> result = new List<DependencyJson>();
        Regex rgx = new Regex(dependencyRegex);

        foreach (Match match in rgx.Matches(text))
        {
            string rawLine = match.Value;
            string[] tokens =
            Regex.Split(rawLine, "\"\\s*:\\s*\"");
            result.Add(new DependencyJson(tokens[0].Trim(' ').Trim('"'), tokens[1].Trim(' ').Trim('"')));
        }
        return result;
    }

    public string nameId;
    public string value;

    public DependencyJson(string nameId, int v1, int v2, int v3)
    {
        this.nameId = nameId;
        value = string.Format("{0}.{1}.{2}", v1, v2, v3);
    }

    public DependencyJson(string nameId, string urlLink)
    {
        this.nameId = nameId;
        value = urlLink;
    }

    public string GetNamespaceId() { return nameId; }
    public bool GetLink(out string url)
    {
        bool isVersion = IsVersionSet();
        url = "";
        if (!isVersion)
        {
            url = value;
        }
        return !isVersion;
    }

    public bool IsVersionSet()
    {
        int v1, v2, v3;
        return GetVersion(out v1, out v2, out v3);
    }
    public bool GetVersion(out int v1, out int v2, out int v3)
    {
        v1 = v2 = v3 = 0;
        string[] tokens = value.Split('.');
        if (tokens.Length == 3)
        {

            try
            {
                v1 = int.Parse(tokens[0]);
                v2 = int.Parse(tokens[1]);
                v3 = int.Parse(tokens[2]);

            }
            catch (Exception)
            {
                return false;
            }
        }
        return true;
    }
}
