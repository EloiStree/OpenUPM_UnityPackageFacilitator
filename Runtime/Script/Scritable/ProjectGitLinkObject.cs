using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "ProjectGitLink", menuName = "Facilitator/Create/Project Git Link", order = 1)]
public class ProjectGitLinkObject : ScriptableObject
{
    public ProjectGitLink m_data;
}
[System.Serializable]
public class ProjectGitLink
{
    public string m_urlAccount;
    public string m_projectGitUrl;

}