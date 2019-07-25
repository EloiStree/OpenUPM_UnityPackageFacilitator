using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentProjectManifest : MonoBehaviour
{
    public UnityPackageManifest m_manifestInfo;
    public string m_toRemove;
    public string m_toAddName = "com.organisatin.application";
    public string m_toAddValue ="http://yourgitserver/path.git";

    private void Reset()
    {
        m_manifestInfo=  UnityPackageUtility.GetManifest();    
    }

}
