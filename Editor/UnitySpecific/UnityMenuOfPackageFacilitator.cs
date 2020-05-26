using UnityEditor;

public static class UnityMenuOfPackageFacilitator
{
    [MenuItem("Window /Package Utility/Utils/Remove Locker")]
    public static void RemoveLocker()
    {
        UnityPackageUtility.RemoveLocker();
    }
    [MenuItem("Window /Package Utility/Utils/Open Manifest.json")]
    public static void OpenManifestJson()
    {
        UnityPackageUtility.OpenManifestFile();
    }
   

}