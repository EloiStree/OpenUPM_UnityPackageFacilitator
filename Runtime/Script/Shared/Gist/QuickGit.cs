using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

public static class QuickGit
{
    public static void OpenCmd(string gitDirectoryPath)
    {
        if (gitDirectoryPath.Length < 2) return;

        char disk = 'C';
        if (gitDirectoryPath[1] == ':')
            disk = gitDirectoryPath[0];
        //string cmd = disk + ":" + "&" + "cd \"" + gitDirectoryPath + "\"";

        string strCmdText;
        strCmdText = "/K " + disk + ":" + " && cd " + gitDirectoryPath + " && git status";
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = strCmdText;
        process.Start();
    }
    public static void Clone(string gitUrl, string gitDirectoryPath)
    {
        RunCommands(new string[] {
                "git clone "+ gitUrl+ " "+ gitDirectoryPath
          }, gitDirectoryPath);
    }
    public static void Pull(string gitDirectoryPath)
    {
        RunCommands(new string[] {
                "git pull"
          }, gitDirectoryPath);
    }

    public static bool m_debugState = false;
    public static void  SetDebugOn(bool useDebug)
    {
        m_debugState = useDebug;
    }
    public static bool GetDebugState() { return m_debugState; }

    public static void PullAddCommitAndPush(string gitDirectoryPath, string commitDescription = "none")
    {
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"Save: " + commitDescription + "\"",
                "git pull",
                "git add .",
                "git commit -m \"Merge: "+ commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }
    public static void AddCommitAndPush(string gitDirectoryPath, string commitDescription = "none")
    {
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"" + commitDescription + "\"",
                "git push"
          }, gitDirectoryPath);
    }

    static void RunCommands(string[] cmds, string workingDirectory)
    {
        if (workingDirectory.Length < 2) return;

        char disk = 'C';
        if (workingDirectory[1] == ':')
            disk = workingDirectory[0];

        var process = new Process();
        var psi = new ProcessStartInfo();
        psi.FileName = "cmd.exe";
        psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        psi.RedirectStandardInput = true;
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.WorkingDirectory = workingDirectory;
        process.StartInfo = psi;
        process.Start();
        process.OutputDataReceived += (sender, e) => {
            if(GetDebugState())
                UnityEngine.Debug.Log(e.Data); };
        process.ErrorDataReceived += (sender, e) => {
            if (GetDebugState())
                UnityEngine.Debug.Log(e.Data); };
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();



        using (StreamWriter sw = process.StandardInput)
        {
            sw.WriteLine(disk + ":");
            sw.WriteLine("cd " + workingDirectory);
            foreach (var cmd in cmds)
            {
                if (GetDebugState())
                    UnityEngine.Debug.Log("> " + cmd);
                sw.WriteLine(cmd);
            }
        }
        process.WaitForExit();
    }

    public static void CreateLocal(string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);
        File.WriteAllText(directoryPath + "/test.md", "Test");
        RunCommands(new string[] {
                "git init .",
                "git add .",
                "git commit -m \"First commit\"",
          }, directoryPath);
        //$ git push -u origin master

    }
    public static void PushLocalToNewOnline(GitServer server, string directoryPath, string userName, string newRepoName, out string gitCreatedUrl)
    {
        gitCreatedUrl = "";
        switch (server)
        {
            //case GitServer.GitHub:
            //    PushLocalToGitHub(directoryPath, userName, newRepoName, out gitCreatedUrl);
            //    break; 
            case GitServer.GitLab:
                PushLocalToGitLab(directoryPath, userName, newRepoName, out gitCreatedUrl);
                break;
            case GitServer.GitHub:
                PushLocalToGitHub(directoryPath, userName, newRepoName, out gitCreatedUrl);
                break;
            default:
                break;
        }
    }
    public static void PushLocalToGitHub(string directoryPath, string userName, string newRepoName, out string gitCreatedUrl)
    {
       throw new NotImplementedException("Impossible or not in my skills contact me if youknow how to do it. I tried for hours.");
    //    if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(newRepoName))
    //        gitCreatedUrl = "https://github.com/" + userName + "/" + newRepoName + ".git";
    //    else
    //        gitCreatedUrl = "";
    //    //https://kbroman.org/github_tutorial/pages/init.html
    //    RunCommands(new string[] {
    //                "git add .",
    //                "git commit -m \"Local to Remote\"",

    ////                "git remote add origin git@github.com:"+userName+"/"+newRepoName+".git",
    //                "git remote add origin https://github.com/"+userName+"/"+newRepoName+"",
    //                "git push --set-upstream https://github.com/"+userName+"/"+newRepoName+".git master",
    //                "git push -u origin master"
    //          }, directoryPath);
    }
    public static void PushLocalToGitLab(string directoryPath, string userName, string newRepoName, out string gitCreatedUrl)
    {


        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(newRepoName))
            gitCreatedUrl = "https://gitlab.com/" + userName + "/" + newRepoName + ".git";
        else
            gitCreatedUrl = "";

        //https://docs.gitlab.com/ee/gitlab-basics/create-project.html
        //git push --set-upstream https://gitlab.example.com/namespace/nonexistent-project.git master
        //git push --set-upstream address/your-project.git
        RunCommands(new string[] {
                "git add .",
                "git commit -m \"Local to Remote\"",
                "git push --set-upstream https://gitlab.com/"+userName+"/"+newRepoName+".git master",
                "git push -u origin master"
          }, directoryPath);

    }
    public static void RemoveFolder(string directoryPath)
    {
        RemoveFiles(directoryPath);

        RunCommands(new string[] {
                "del /S /F /AH "+directoryPath,
                "rmdir "+directoryPath
          }, directoryPath);
    }
    public static void RemoveFiles(string directoryPath )
    {
        string[] pathfiles = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);
        string[] pathfilesOwn = Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories);

        for (int i = 0; i < pathfilesOwn.Length; i++)
        {
            pathfilesOwn[i] = "takeown / A / F" + pathfilesOwn[i];
        }
        for (int i = 0; i < pathfiles.Length; i++)
        {

            pathfiles[i] = "del /F /AH " + pathfiles[i];
        }
        List<string> files = new List<string>();
        files.AddRange(pathfiles);
        files.AddRange(pathfilesOwn);
        RunCommands(files.ToArray(), directoryPath);
    }

    public enum GitServer { GitHub, GitLab }
}