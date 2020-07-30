using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LicenseEditor : EditorWindow
{
    private bool m_pathFound;
    private UnityPathSelectionInfo m_selector;
    private string m_licenseText;
    private string m_licenseLink;
    private bool m_hide;

    [MenuItem("ꬲ🧰/Package Utility/Window/License", false, 20)]
    static void Init()
    {
        LicenseEditor window = (LicenseEditor) EditorWindow.GetWindow(typeof(LicenseEditor));
        window.Show();
    }

    void OnGUI()
    {
        UnityPathSelectionInfo.Get(out m_pathFound, out m_selector);
        DrawEditorDefaultInterface(new LicenseFileStream(m_selector.GetAbsolutePath(false)), ref m_licenseLink, ref m_licenseText,ref m_hide);
    }

    private static void DrawEditorLicenseCreationLinks()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("creative Commons")) { Application.OpenURL("https://creativecommons.org/choose/"); }
        if (GUILayout.Button("Open Srouce")) { Application.OpenURL("https://opensource.org/licenses"); }
        
        GUILayout.EndHorizontal();
    }

    public static void DrawEditorDefaultInterface(LicenseFileStream license,  ref string licenseLink, ref string licenseText, ref bool hide)
    {
        hide = EditorGUILayout.Foldout(hide, hide ? "→ License" : "↓ License", EditorStyles.boldLabel);
        if (!hide)
        {
            DrawEditorLicenseCreationLinks();
            GUILayout.Label("Have a link:");
            licenseLink = EditorGUILayout.TextField(licenseLink);
            GUILayout.Label("Have text:");
            licenseText = EditorGUILayout.TextArea(licenseText, GUILayout.MinHeight(60));
            GUILayout.BeginHorizontal();
            if (license.Exist() && GUILayout.Button("Open"))
            {
                license.Open();
            }
            if (GUILayout.Button("Create / Override"))
            {
                license.Set("# Tool License  \n> " + licenseLink + "  \n  \n" + licenseText);
            }
            GUILayout.EndHorizontal();
        }

    }
}