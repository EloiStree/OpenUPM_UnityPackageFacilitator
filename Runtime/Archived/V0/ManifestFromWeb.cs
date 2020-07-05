using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ManifestFromWeb : MonoBehaviour
{
    public string m_name;
    public string m_manifestUrl;
    [TextArea(0,20)]
    public string m_downloaded;

    [HideInInspector]
    public Utility_ManifestJson m_manifest;
    [HideInInspector]
    public string m_previousValue;
    [HideInInspector]
    public string m_toRemove;
    [HideInInspector]
    public string m_toAddName;
    [HideInInspector]
    public string m_toAddValue;
    public Vector2 m_scollState;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(m_name) && !string.IsNullOrEmpty(m_manifestUrl))
        {
            int index = m_manifestUrl.LastIndexOf('/');
            if (index < 0)
                m_name = m_manifestUrl;
            else
                m_name = m_manifestUrl.Substring(index);
        }
        if (string.IsNullOrEmpty(m_name))
            m_name = "Not set yet";
        this.gameObject.name = m_name;

        if (m_previousValue != m_manifestUrl) {
            m_previousValue = m_manifestUrl;
            if (string.IsNullOrEmpty(m_manifestUrl)){
                m_downloaded = "";
                m_manifest = new Utility_ManifestJson();
            }
            else {
                DownloadWebManifest();
            }
        }
    }

    public  void DownloadWebManifest()
    {
        m_manifest =
        UnityPackageUtility.DownloadPackageManifest(m_manifestUrl, out m_downloaded);
      
    }
    
}
