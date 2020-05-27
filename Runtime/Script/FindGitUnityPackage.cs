using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindGitUnityPackage : MonoBehaviour
    {
        public string m_directory;
        public string[] m_gitLinksPath;
        public List<GitLinkOnDisk> m_gitLinks;
        public List<GitUnityPackageLinkOnDisk> m_packageInfo;

        private void Reset()
        {
            if (string.IsNullOrEmpty(m_directory))
                m_directory = Application.dataPath+"/../";
            m_gitLinksPath = QuickGit.GetAllFolders(m_directory, true);
            QuickGit.GetGitsInDirectory(m_directory, out m_gitLinks);
            m_packageInfo = UnityPackageUtility.GetGitUnityPackageInDirectory(m_directory);
        }
    }


