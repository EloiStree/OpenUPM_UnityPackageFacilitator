using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(CurrentProjectManifest))]
public class CurrentProjectManifestEditor : Editor
{
    public override void OnInspectorGUI()
    {


        CurrentProjectManifest myScript = (CurrentProjectManifest)target;
        UnityPackageEditorDrawer.DrawManifrest(ref myScript.m_manifestInfo, ref myScript.m_toRemove, ref myScript.m_toAddName, ref myScript.m_toAddValue);
       

    }
}
