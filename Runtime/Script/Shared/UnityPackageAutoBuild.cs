using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class UnityPackageAutoBuild : MonoBehaviour
{
    [Header("Project Info info")]
    public PackageBuildInformationObject m_packageInfo;
    public ContactInformationObject m_contactInformation;
    public ProjectDirectoriesStructureObject m_directoriesStructure;
    public ListOfClassicPackages m_adviceRequiredLinks;
    public string m_projectPath;


    [Header("Linked")]
    public PackagePullPush m_pullPush;

    public void Reset()
    {
    

        m_projectPath = Application.dataPath;
        MakeSureThatPullPushScriptIsAssociatedToThisScript();
        m_packageInfo.m_data.m_projectId=EloiProjectIdFormat.GetProjectDateFormatFor( Application.productName);
        MakeSureThatTheAssemblyEditorTargetTheRuntimeOne();


    }

    public void OnValidate()
    {
        m_projectPath = Application.dataPath;
        if (m_packageInfo == null)
            return;
        if (m_packageInfo.m_data.m_assemblyRuntime == null)
            return;
        if (m_packageInfo.m_data.m_assemblyEditor == null)
            return;
        m_packageInfo.m_data.m_assemblyRuntime.m_packageName = m_packageInfo.m_data.GetProjectNameId(true);
        m_packageInfo.m_data.m_assemblyEditor.m_packageName = m_packageInfo.m_data.GetProjectNameId(true) + "editor";
        m_packageInfo.m_data.m_assemblyRuntime.m_packageNamespaceId = m_packageInfo.m_data.GetProjectNamespaceId(true);
        m_packageInfo.m_data.m_assemblyEditor.m_packageNamespaceId = m_packageInfo.m_data.GetProjectNamespaceId(true) + "editor";
        MakeSureThatPullPushScriptIsAssociatedToThisScript();
    }
    public void MakeSureThatTheAssemblyEditorTargetTheRuntimeOne() {


        AssemblyBuildInformation assEditor = m_packageInfo.m_data.m_assemblyEditor;
        AssemblyBuildInformation assRuntime = m_packageInfo.m_data.m_assemblyRuntime;

        List<string> editorRef = assEditor.m_reference.ToList();

        editorRef.Remove(assRuntime.m_packageNamespaceId);
        editorRef.Insert(0,assRuntime.m_packageNamespaceId);
        assEditor.m_reference = editorRef.ToArray();



    }


    public void MakeSureThatPullPushScriptIsAssociatedToThisScript()
    {
        if (m_pullPush == null)
            m_pullPush = GetComponent<PackagePullPush>();

        if (m_pullPush == null)
            m_pullPush = gameObject.AddComponent<PackagePullPush>();
    }

    public string GetFolderPath()
    {
        return m_projectPath + "/" + m_packageInfo.m_data.m_projectId;
    }

    private string CleanForNameSpace(string value)
    {
        return value.ToLower().Replace(" ", "").Replace(".", "");
    }
    
}

