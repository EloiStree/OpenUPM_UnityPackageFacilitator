using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SampleUtility : MonoBehaviour
{
    public static SampleDirectoryStream GetSampleFolder(UnityPathSelectionInfo selector)
    {
        return new SampleDirectoryStream(selector.GetAbsolutePath(true));
    }

    public static void Create(UnityPathSelectionInfo selector, bool asHidden)
    {
        SampleDirectoryStream tmp = new SampleDirectoryStream(selector.GetAbsolutePath(true));
        tmp.Create(asHidden);
    }


    public static void Toggle(UnityPathSelectionInfo selector)
    {
        SampleDirectoryStream tmp = new SampleDirectoryStream(selector.GetAbsolutePath(true));
        if (tmp.Exist())
            tmp.ToggleVisiblity();
    }

    public static string[] GetRelativeFoldersIn(SampleDirectoryStream samplesDir)
    {
        if (!samplesDir.Exist())
            return new string[0];
        string path = samplesDir.GetCurrentPath();
        string [] folders =  Directory.GetDirectories(path);
        for (int i = 0; i < folders.Length; i++)
        {
            folders[i] = (samplesDir.IsVisible() ? "Samples/" : "Samples~/")+UnityPaths.GetRelativePath(path, folders[i]);
            
                Debug.Log(">>" + folders[i]);
        }
        return folders;
        


    }
}

public class SampleDirectoryStream : HiddenDirectoryStream
{
    public SampleDirectoryStream(string absoluteFolderPath) : base(absoluteFolderPath, "Samples")
    {

    }
    public void Create(bool asHidden , bool withHelloDefaultOne )
    {
        Create(asHidden);
        if (withHelloDefaultOne)
            CreateOrOverrdieTextFile("HelloSample/ReadMe.md", "# Hello Documenation !  \n If it is your first sample, feel free to learn about how to manage them here:  \nhttps://eloistree.page.link/hellopackage");

    }
    public override void Create(bool asHidden = true)
    {
        base.Create(asHidden);
     }

}