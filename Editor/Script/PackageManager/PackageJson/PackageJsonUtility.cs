using Eloi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Eloi
{

    public class PackageJsonUtility
    {
        public static PackageJsonFileStream GetPackageFile(UnityPathSelectionInfo selector)
        {
            return new PackageJsonFileStream(selector.GetAbsolutePath(true));
        }

    }

    public class PackageJsonFileStream : AbstractFileStream
    {
        string m_rootPath = "";
        public PackageJsonFileStream(string packageAndGitRoot) : base(packageAndGitRoot + "/package.json")
        {
            m_rootPath = packageAndGitRoot;
        }


        public bool HasGitLinked()
        {
            return QuickGit.IsPathHasGitRootFolder(m_rootPath);
        }
        public GitLinkOnDisk GetLinkedGit() { return new GitLinkOnDisk(m_rootPath); }

        public string GetPackageProjectRoot()
        {
            return m_rootPath;
        }

    }
}