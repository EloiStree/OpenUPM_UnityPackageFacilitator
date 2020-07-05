using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(ManifestFromWeb))]
public class ManifestFromWebEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        ManifestFromWeb myScript = (ManifestFromWeb)target;


        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Reload"))
        {
            myScript.DownloadWebManifest();
        }
      
        if (GUILayout.Button("Merge"))
        {
            Utility_ManifestJson manifest = UnityPackageUtility.GetManifest();
            manifest.Add(myScript.m_manifest.dependencies);
             UnityPackageUtility.SetManifest(manifest);
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Override"))
        {
            UnityPackageUtility.SetManifest(myScript.m_manifest);
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();
        UnityPackageEditorDrawer.DrawManifrest(ref myScript.m_manifest, ref myScript.m_toAddName, ref myScript.m_toAddValue,ref myScript.m_scollState,false );

    }
}
