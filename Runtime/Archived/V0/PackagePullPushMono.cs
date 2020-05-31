using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackagePullPushMono : MonoBehaviour
{
    [SerializeField] string m_gitLink;
    public bool m_affectPackageManager=true;
    
    [Space(10)]
    [Header("Debug information")]
    public string m_projectName;
    public string m_namespaceId;
    public bool m_isValideLink;
    public bool m_useDebug;


    public string GetGitLink() { return m_gitLink; }
    

    public void UpdateProject()
    {
        QuickGit.Pull(GetProjectPathInUnity());
    }

    public bool IsGitLinkValide()
    {
        return UnityPackageUtility.IsGitLinkValide(m_gitLink);
    }

    public void OnValidate()
    {
        RefreshInfo();
        string path = GetProjectPathInUnity() + "/package.json";
        if (File.Exists(path)) {
        Utility_PackageJson pack = UnityPackageUtility.GetPackageInfo(path);
            if (pack != null) {
                m_namespaceId = pack.GetNamespaceID();
            }
        }
    }

    public string GetProjectPathInUnity()
    {
        return UnityPackageUtility.GetGitDirectoryPropositionRootPathInUnity(m_gitLink);
    }

    public void RefreshInfo()
    {
        QuickGit.SetDebugOn(m_useDebug);
        m_useDebug = QuickGit.GetDebugState();

        m_isValideLink = UnityPackageUtility.IsGitLinkValide(m_gitLink);
        m_projectName = UnityPackageUtility. GetProjectNameFromGitLink(m_gitLink);
    }




    public void PullProject()
    {
        Directory.CreateDirectory(GetProjectPathInUnity());
        QuickGit.Clone(m_gitLink, GetProjectPathInUnity());
    }



    public void SetGitLink(string gitLink)
    {
        m_gitLink = gitLink;
        RefreshInfo();
    }

    public void PullAndPush() {

        QuickGit.AddFileInEmptyFolder(GetProjectPathInUnity());
        QuickGit.PullPushWithAddAndCommit(GetProjectPathInUnity(), DateTime.Now.ToString("yyyy/mm/dd -  hh:mm"));
       
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
