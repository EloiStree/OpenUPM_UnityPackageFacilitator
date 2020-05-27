using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccessGitWithPathSelector 
{
    public  static void GetAffectedGit(UnityPathSelectionInfo selector, out GitLinkOnDisk gitAffected)
    {
        string path = selector.GetAbsolutePath(true);
        if (QuickGit.IsPathHasGitRootFolder(path))
        {
            gitAffected = new  GitLinkOnDisk(path);
        }
        QuickGit.GetGitInParents(path, QuickGit.PathReadDirection.LeafToRoot, out gitAffected);

    }
}
