using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class DownloadInfoFromGitServer
{
    public enum GitServer { GitHub, GitLab, Unknow}

    public static void LoadNamespaceFromProjectGitLink(string url, out bool found, out string namespaceID, string branchName="master") {
        found = false;
        namespaceID = "";
        url = url.ToLower();

        GitServer server = GitServer.Unknow;
        if (url.IndexOf("gitlab.com") > -1) server = GitServer.GitLab;
        if (url.IndexOf("github.com") > -1) server = GitServer.GitHub;

        url = url.Replace(".git", "");
        int startIndex = url.IndexOf(".com/");
        if (startIndex < 0)
            return;
        url = url.Substring(startIndex + 5);

       // Debug.Log("url:" + url);
        string[] tokens = url.Split('/');
        if (tokens.Length <2)
            return;
        string user= tokens[0], project = tokens[1];
       // Debug.Log("keys:" + server + " "+ user + " " + project + " " +branchName);
        LoadNamespaceFromUrl(server, user, project, branchName, out found, out namespaceID);

       // Debug.Log("NP:" + namespaceID);

        //https://gitlab.com/eloistree/2020_05_25_KoFiCount.git
        //https://github.com/EloiStree/2019_07_21_QuickGitUtility
    }

    public static void LoadNamespaceFromUrl(GitServer server,string userName, string projectName, string projectBranch , out bool found, out string namespaceID) {
        string url = "";
        if (server == GitServer. GitLab)
        {
            url = string.Format("https://gitlab.com/{0}/{1}/-/raw/{2}/package.json", userName, projectName, projectBranch);

        }
        else if (server == GitServer.GitHub)
        {
            url = string.Format("https://raw.githubusercontent.com/{0}/{1}/{2}/package.json"
                , userName, projectName, projectBranch);
        }
        LoadNamespaceFromUrl(url, out found, out namespaceID);
    } 

    public static void LoadNamespaceFromUrl(string url, out bool found, out string namespaceID) {
   
        string page = DownloadPage2(url).ToLower();
        LoadNamespaceFromText(page, out found, out namespaceID);
    }

    public static void LoadNamespaceFromText(string text, out bool found, out string namespaceID)
    {
        found = false;
        namespaceID = "";
        string page = text;
        int index = page.IndexOf("\"name\"");
        if (index < 0) return;
        index += 7;

        page = page.Substring(index);
        page = page.Substring(0, page.IndexOf("\n"));
        page = page.Replace(" ", "").Replace(":", "").Replace("\"", "").Replace(",", "");
        page = page.Trim();
        namespaceID = page;
        found = true;
    }
    public static string DownloadPage(string url)
    {
        try
        {
            WebClient client = new WebClient();
            //client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stream data = client.OpenRead(url);
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();
            return s;
        }
        catch (Exception) {
            return "";
        }
    }

    public static string DownloadPage2(string url)
    {
        string data = "";
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        if (response.StatusCode == HttpStatusCode.OK)
        {
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = null;

            if (response.CharacterSet == null)
                readStream = new StreamReader(receiveStream);
            else
                readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

            data = readStream.ReadToEnd();
            response.Close();
            readStream.Close();
        }
        return data;
    }

    //GIT LAB
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount.git
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/blob/master/package.json
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/raw/master/package.json
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/blame/master/package.json
    //https://gitlab.com/eloistree/2020_05_25_KoFiCount/-/commits/master/package.json


    //GIT HUB
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility
    //Info
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility/blob/master/package.json
    //Blame
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility/blame/master/package.json
    //Raw data
    //https://raw.githubusercontent.com/EloiStree/2019_07_21_QuickGitUtility/master/package.json
    //History
    //https://github.com/EloiStree/2019_07_21_QuickGitUtility/commits/master/package.json



}
