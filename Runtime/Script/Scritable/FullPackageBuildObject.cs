using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[CreateAssetMenu(fileName = "FullPackageBuild", menuName = "Facilitator/Create/Full Package Build", order = 1)]
public class FullPackageBuildObject : ScriptableObject
{

    //public ProjectGitLinkObject m_gitLink;
    public PackageBuildInformationObject m_package;
    public ProjectDirectoriesStructureObject m_structure;
    public ListOfClassicPackagesObject m_links;
    public ContactInformationObject m_contact;



   
}
