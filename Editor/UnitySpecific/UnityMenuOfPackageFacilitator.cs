using UnityEditor;
using UnityEngine;

public static class UnityMenuOfPackageFacilitator
{
    [MenuItem("Window /Package Utility/Utils/Remove Locker", false, 90)]
    public static void RemoveLocker()
    {
        UnityPackageUtility.RemoveLocker();
        AssetDatabase.Refresh();
    }
    [MenuItem("Window /Package Utility/Utils/Open Manifest.json", false, 90)]
    public static void OpenManifestJson()
    {
        UnityPackageUtility.OpenManifestFile();
    }
    [MenuItem("Window /Package Utility/What? Help !?", false, 0)]
    public static void OpenHelloPackageTutorial()
    {
        Application.OpenURL("http://eloistree.page.link/hellopackage");
    }

    

}