using UnityEditor;

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
   

}