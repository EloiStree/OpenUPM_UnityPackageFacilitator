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

    void OnGUI()
    {

    }
}