using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageJsonUtility 
{
    public static PackageJsonFileStream GetPackageFile(UnityPathSelectionInfo selector)
    {
        GitLinkOnDisk git;
        QuickGit.GetGitInParents(selector.GetAbsolutePath(true), QuickGit.PathReadDirection.LeafToRoot, out git);
        return new PackageJsonFileStream(git.GetDirectoryPath());
    }
}

public class PackageJsonFileStream : FileStream {
    public PackageJsonFileStream(string packageAndGitRoot) : base(packageAndGitRoot+"/package.json") { }
   
}
