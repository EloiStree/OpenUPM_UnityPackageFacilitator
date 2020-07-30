using UnityEditor;
using UnityEngine;

public static class UnityMenuOfPackageFacilitator
{
    [MenuItem("ꬲ🧰/Package Utility/Utils/Remove All Locker ⚠️", false, 90)]
    public static void RemoveLocker()
    {
        UnityPackageUtility.RemoveLocker();
        AssetDatabase.Refresh();
    }
    [MenuItem("ꬲ🧰/Package Utility/Utils/Go to Manifest.json", false, 90)]
    public static void OpenManifestJson()
    {
        UnityPackageUtility.OpenManifestFile();
    }
    [MenuItem("ꬲ🧰/Package Utility/Utils/Go to Packages Hidden Directory", false, 90)]
    public static void OpenPackageHiddenFolder()
    {
        UnityPackageUtility.OpenPackageHiddenFolder();
    }
    [MenuItem("ꬲ🧰/Package Utility/📕 What is a Unity Package ?", false, 0)]
    public static void OpenHelloPackageTutorial()
    {
        Application.OpenURL("http://eloistree.page.link/hellopackage");
    }

    

}