using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PackageJsonUtility 
{
    public static PackageJsonFileStream GetPackageFile(UnityPathSelectionInfo selector)
    {
        return new PackageJsonFileStream(selector.GetAbsolutePath(true));
    }
}

public class PackageJsonFileStream : FileStream {
    public string m_rootPath="";
    public PackageJsonFileStream(string packageAndGitRoot) : base(packageAndGitRoot+"/package.json") {
        m_rootPath = packageAndGitRoot;
    }


    public GitLinkOnDisk GetLinkedGit() { return new GitLinkOnDisk(m_rootPath); }   
}
