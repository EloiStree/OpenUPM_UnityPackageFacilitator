using UnityEngine;
using UnityEditor;
using System.Collections;

//https://learn.unity.com/tutorial/introduction-to-scriptable-objects#5cf187b7edbc2a31a3b9b123
class UnityPackageBuilderWindow : EditorWindow
{
    [MenuItem("Facilitator/Create Package")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(UnityPackageBuilderWindow));
    }

    public static PackageBuildInformationObject linkedPackage;
    public static PackageBuildInformationObject linkedPackagePrevious;
    public string m_folderName;
    public string m_test;

  

    void OnGUI()
    {
        bool packageChange =false;
        if (linkedPackagePrevious != linkedPackage) {
            packageChange = true;
            linkedPackagePrevious = linkedPackage;
        }

        GUILayout.Label("Create a package", EditorStyles.boldLabel);
        linkedPackage = (PackageBuildInformationObject) EditorGUILayout.ObjectField(linkedPackage, typeof(PackageBuildInformationObject));
        if (linkedPackage != null) {
            PackageBuildInformation package = linkedPackage.m_data;

            if (packageChange && package != null) {
                m_folderName = package.m_projectId;
            }
            UnityPackageEditorDrawer.DrawPackageEditor( ref m_folderName, package); 
            
        }
    }
    public string GetUnityFolderPath(string relativePath) {
        return Application.dataPath + "/" + relativePath;
    }
}