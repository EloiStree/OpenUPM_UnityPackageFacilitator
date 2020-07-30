using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ManifestJsonEditor : EditorWindow
{
    public Info m_info = new Info();
    public class Info
    {
        public Utility_ManifestJson m_manifestInfo = new Utility_ManifestJson();
        public string m_manifestaddNamespace="";
        public string m_manifestaddgitlink="";
        public string m_pastSeveralLinks="";
        public Vector2 m_scollValue;
        public Vector2 m_windowScrollValue;
        internal string givenUrlLink;
    }
    [MenuItem("ꬲ🧰/Package Utility/2. Manifest", false, 10)]
    static void Init()
    {
        ManifestJsonEditor window = (ManifestJsonEditor)EditorWindow.GetWindow(typeof(ManifestJsonEditor));
        window.titleContent = new GUIContent("Manifest Configuration") ;
        window.Show();
    }
    void OnGUI()
    {
         GUILayout.Label("Manifest Configuration", EditorStyles.boldLabel);
        UnityPackageEditorDrawer.DrawManifrest(ref m_info.m_manifestInfo, ref m_info.m_manifestaddNamespace, ref m_info.m_manifestaddgitlink, ref m_info.m_scollValue);
        m_info.m_windowScrollValue = EditorGUILayout.BeginScrollView(m_info.m_windowScrollValue, false, false);
        GUILayout.Label("Copy past gits", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Copy/past your git links split by a line return '\\n'", MessageType.Info);
        if (GUILayout.Button("Load in the manifest"))
        {
            string[] lines = m_info.m_pastSeveralLinks.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];
                l = l.Trim();
                string nameId;
                bool found;
                DownloadInfoFromGitServer.LoadNamespaceFromProjectGitLink(l, out found, out nameId);
                if (found)
                {
                    if (l.ToLower().IndexOf(".git") < 0)
                        l += ".git";
                    m_info.m_manifestInfo.Add(nameId, l);
                }
            }
            m_info.m_pastSeveralLinks = "";
        }
        GUILayout.BeginHorizontal();
        //https://raw.githubusercontent.com/EloiStree/UnityToolbox/master/CopyPast/GitLinks/TestGroup.md
        
        List<string> urlsFound;
        if (GUILayout.Button("Extract gits")) {

            DownloadInfoFromGitServer.LoadGitClassicGitLinksInUrl(m_info.givenUrlLink, out urlsFound);
            for (int i = 0; i < urlsFound.Count; i++)
            {
                m_info.m_pastSeveralLinks += urlsFound[i].Trim()+'\n';
            }
        }
        m_info.givenUrlLink = GUILayout.TextField(m_info.givenUrlLink);
        GUILayout.EndHorizontal();
        //be.eloistree.teleportvirtualrealityuser
        m_info.m_pastSeveralLinks = GUILayout.TextArea(m_info.m_pastSeveralLinks, GUILayout.MinHeight(200));
         EditorGUILayout.EndScrollView();
    }
}