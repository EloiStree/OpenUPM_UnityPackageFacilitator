using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackagePullPush : MonoBehaviour
{
    public string m_gitLink;
    [Space(10)]
    [Header("Debug information")]
    public string m_projectName;
    public bool m_isValideLink;
    public JsonUnityPackageMirror m_packageMirror;
    public bool m_useDebug;

    public string GetProjectNameFromGitLink(string gitLinkFormated) {
        // https://gitlab.com/eloistree/2019_07_22_oculusguardianidentity.git
        // https://github.com/EloiStree/CodeAndQuestsEveryDay.git

        gitLinkFormated = RemoveWhiteSpace(gitLinkFormated);
        int startProjectName = m_gitLink.LastIndexOf('/');
        if (startProjectName < 0)
            return "";
        string projectName = m_gitLink.Substring(startProjectName + 1).Replace(".git", "").Replace(".GIT", "");
        return projectName;
    }

    public void UpdateProject()
    {
        QuickGit.Pull(GetProjectPathInUnity());
    }

    public bool IsGitLinkValide()
    {
        return IsGitLinkValide(m_gitLink);
    }

    public void OnValidate()
    {
        QuickGit.SetDebugOn(m_useDebug);
        m_useDebug = QuickGit.GetDebugState();

           m_isValideLink = IsGitLinkValide(m_gitLink);
        m_projectName = GetProjectNameFromGitLink(m_gitLink);
    }

    public bool IsGitLinkValide(string gitLink)
    {
        if (string.IsNullOrEmpty(gitLink))
            return false;
        if (gitLink.LastIndexOf('/') < 0) return false;
        if (gitLink.ToLower().LastIndexOf(".git") < 0) return false;
        string name = GetProjectNameFromGitLink(gitLink);
        if (name == null || string.IsNullOrWhiteSpace(name))
            return false;
        return true;
    }

    public string RemoveWhiteSpace(string gitLinkFormated, string by="_")
    {
        return gitLinkFormated.Replace(" ", by);
    }

    public void PullProject()
    {
        Directory.CreateDirectory(GetProjectPathInUnity());
        QuickGit.Clone(m_gitLink, GetProjectPathInUnity());
    }

    public string GetProjectPathInUnity()
    {
        return Application.dataPath + "/" + GetProjectNameFromGitLink(m_gitLink);
    }

    public void PullAndPush() {


        QuickGit.PullAddCommitAndPush(GetProjectPathInUnity());
    }
    
    public void RemoveProject() {
        if(IsDirectoryCreated())
            QuickGit.RemoveFolder(GetProjectPathInUnity());
    }
    public void OpenDirectory() {
        Application.OpenURL(GetProjectPathInUnity());
    }
    public void OpenStatusInCommentLine() {
        QuickGit.OpenCmd(GetProjectPathInUnity());
    }
    public bool IsDirectoryCreated() {
        return Directory.Exists(GetProjectPathInUnity());
    }
}
public class JsonUnityPackageMirror
{

}