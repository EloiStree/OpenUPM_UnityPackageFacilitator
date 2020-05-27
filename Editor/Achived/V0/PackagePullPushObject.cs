using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "PullPushLink", menuName = "Facilitator/Create/Pull Push Link", order = 1)]
public class PackagePullPushObject : ScriptableObject
{

    public PackagePullPush m_data;
}
[System.Serializable]
public class PackagePullPush {

    public string m_relativeFolderPath;
    public string m_packageNamespaceId;
    public string m_gitUrl;
}
// PULL PUSH WINDOW ???