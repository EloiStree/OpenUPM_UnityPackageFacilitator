using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnityPackageEditorDrawer
{
   

    public  static void DrawManifrest(ref UnityPackageManifest manifestInfo, ref string remove, ref string addname, ref string addvalue, bool withButtonToPushAndLoad=true, bool withDirectoryButton=true)
    {
        int buttonlenght = 25;
        int nameLength = 200;
        if (withButtonToPushAndLoad) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load"))
            {
                manifestInfo = UnityPackageUtility.GetManifest();
            }
            if (GUILayout.Button("Push"))
            {
                UnityPackageUtility.SetManifest(manifestInfo);
                AssetDatabase.Refresh();
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        GUILayout.Label("Add & Remove");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("+", GUILayout.Width(buttonlenght)))
        {
            manifestInfo.Add(addname, addvalue);
        }
        addname = GUILayout.TextField(addname, GUILayout.Width(nameLength));
        addvalue = GUILayout.TextField(addvalue);

      

        GUILayout.EndHorizontal();

      
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("-", GUILayout.Width(buttonlenght)))
        {
            manifestInfo.Remove(remove);
        }
        remove = GUILayout.TextField(remove);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (withDirectoryButton)
        {
            if (GUILayout.Button("Go to package manifest", GUILayout.Width(150)) )
            {
                UnityPackageUtility.OpenManifestFile();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("Current package");
        List<DependencyJson> m_dependencies = manifestInfo.dependencies;
        for (int i = 0; i < m_dependencies.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("-", GUILayout.Width(buttonlenght)))
            {
                manifestInfo.Remove(m_dependencies[i].nameId);
            }
            GUILayout.TextField(m_dependencies[i].nameId, GUILayout.Width(nameLength));
            if (m_dependencies[i].value.IndexOf("http")>-1  &&  GUILayout.Button("->", GUILayout.Width(buttonlenght)))
            {
                Application.OpenURL(m_dependencies[i].value);
            }
            GUILayout.TextField(m_dependencies[i].value);



            GUILayout.EndHorizontal();

        }
    }
}
