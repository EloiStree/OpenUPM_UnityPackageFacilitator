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
    }
    [MenuItem("Window/Package Utility/2. Manifest", false, 10)]
    static void Init()
    {
        ManifestJsonEditor window = (ManifestJsonEditor)EditorWindow.GetWindow(typeof(ManifestJsonEditor));
        window.titleContent = new GUIContent("Manifest Configuration") ;
        window.Show();
    }
    void OnGUI()
    {
        GUILayout.Label("Manifest Configuration", EditorStyles.boldLabel);

        if (GUILayout.Button("Add"))
        {
            string[] lines = m_info.m_pastSeveralLinks.Split(',');
            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i];
                l = l.Trim();
                if (l.Length > 5)
                {
                    if (l[0] == '"' && l[l.Length - 1] == '"')
                    {
                        l = l.Replace("\"", "");
                        string[] tokens = l.Split(':');
                        if (tokens.Length == 2)
                        {
                            string nameId = tokens[0];
                            string url = tokens[1];
                            if (nameId.Trim().Length <= 0)
                            {
                                bool found;
                                DownloadInfoFromGitServer.LoadNamespaceFromProjectGitLink(url, out found, out nameId);
                                if (found)
                                {

                                    m_info.m_manifestInfo.Add(nameId, url);
                                }
                            }
                            else { 
                                m_info.m_manifestInfo.Add(nameId, url);
                            }
                        }
                    }
                    if (l.LastIndexOf(".git") == l.Length - 4)
                    {

                        Debug.Log("URL GIT:" + l);
                        bool found;
                        string nameId;
                        DownloadInfoFromGitServer.LoadNamespaceFromProjectGitLink(l, out found, out nameId);
                        if (found) {

                            m_info.m_manifestInfo.Add(nameId, l);
                        }

                    }
                }
            }
            m_info.m_pastSeveralLinks = "";
        }
        m_info.m_pastSeveralLinks=GUILayout.TextArea(m_info.m_pastSeveralLinks, GUILayout.MinHeight(200));
        UnityPackageEditorDrawer.DrawManifrest(ref m_info.m_manifestInfo, ref m_info.m_manifestaddNamespace, ref m_info.m_manifestaddgitlink);
        GUILayout.Label("Past several links split by ','", EditorStyles.boldLabel);
       
    }
}