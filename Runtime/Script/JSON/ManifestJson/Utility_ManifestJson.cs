using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using UnityEngine.Experimental.Playables;

[System.Serializable]
public class Utility_ManifestJson
{

    public List<DependencyJson> dependencies= new List<DependencyJson>();

    //{{
    // "dependencies": {
    //        "be.eloistree.randomtool": "https://gitlab.com/eloistree/2019_01_15_randomtool.git",
    //        "be.eloistree.unityprefsthemall": "https://gitlab.com/eloistree/2019_06_10_unityprefsthemall.git",
    //        "com.unity.ads": "2.0.8",
    //        "com.unity.analytics": "3.3.2",
    //        "com.unity.collab-proxy": "1.2.16",
    //        "com.unity.modules.wind": "1.0.0",
    //        "com.unity.modules.xr": "1.0.0"
    //  },
    //  "lock": {
    //    "be.eloistree.randomtool": {
    //      "hash": "39187c85824aa974aa6791fdfe34158989907b7e",
    //      "revision": "HEAD"
    //    },
    //    "be.eloistree.unityprefsthemall": {
    //      "hash": "bc3801d94db295e2c391dfc35b29487d683be7f2",
    //      "revision": "HEAD"
    //    }
    //}}







    public void Add(string nameId, int v1, int v2, int v3)
    {
        RemoveFromName(nameId);
        dependencies.Insert(0, new DependencyJson(nameId, v1, v2, v3));
    }
    public void Add(string nameId, string urlLink)
    {
        RemoveFromName(nameId);
        RemoveFromName(urlLink);
        dependencies.Insert(0, new DependencyJson(nameId, urlLink));

    }

    public void RemoveFromName(string nameId)
    {
        dependencies = dependencies.Where(t => t.nameId.Trim() != nameId.Trim()).ToList();

    }
    public void RemoveFromUrl(string url)
    {
        dependencies = dependencies.Where(t => t.value.Trim() != url.Trim()).ToList();

    }



    public string ToJson()
    {

        string jsonResult = "{ \"dependencies\": {\n";
        if (dependencies != null)
            for (int i = 0; i < dependencies.Count; i++)
            {
                jsonResult += string.Format("\"{0}\" : \"{1}\"{2}\n",
                    dependencies[i].nameId,
                    dependencies[i].value,
                    i >= dependencies.Count - 1 ? ' ' : ',');
            }


        jsonResult += "\n}}";
        return jsonResult;

    }

    public static Utility_ManifestJson CreateFromUnityEditor()
    {
        return CreateFromFile(UnityPackageUtility.GetManifestPath());
    }

    public static Utility_ManifestJson CreateFromFile(string filePath)
    {
        string file =  File.ReadAllText(filePath);
        return CreateFromJson(file);
    }

    private static string GetJsonProjectManifestAsText()
    {
        return File.ReadAllText(UnityPackageUtility.GetManifestPath());
    }
    private void SetJsonProjectManifestAsText(string json)
    {
        File.WriteAllText(UnityPackageUtility.GetManifestPath(), json);
    }

    public static Utility_ManifestJson CreateFromJson(string json)
    {
        Utility_ManifestJson man = new Utility_ManifestJson();
        try
        {
            man.dependencies = DependencyJson.GetDependeciesFromText(json);

        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning(e);
        }
        return man;
    }

    public void Add(List<DependencyJson> dependencies)
    {
        for (int i = 0; i < dependencies.Count; i++)
        {
            Add(dependencies[i]);

        }
    }

    public void RemoveLocker(params string[] nameId)
    {
        string json = GetJsonProjectManifestAsText();
        for (int i = 0; i < nameId.Length; i++)
        {
            RemoveLocker(ref json, nameId[i]);
        }
        SetJsonProjectManifestAsText(json);

    }
    public void RemoveLocker( string nameId)
    {
        string json = GetJsonProjectManifestAsText();
        RemoveLocker(ref json, nameId);
        SetJsonProjectManifestAsText(json);


    }

    

    public void RemoveLocker(ref string json, string nameId)
    {
        //VERY DIRTY AND UNSTABLE BUT THAT WORK FOR THE MOMENT.
        //NEED TO FIND BETTER SOLUTION


        int lastIndexOfNameId = json.LastIndexOf(nameId);
        if (lastIndexOfNameId < 0)
            return;
       
        string temp = json.Substring(lastIndexOfNameId);

        int lastIndexOf = temp.IndexOf("}");
        if (lastIndexOf < 0)
            return;
         temp = temp.Substring(0,lastIndexOf+1);

        int openIndex = temp.IndexOf("\"hash\"");
        if (openIndex < 0) 
            return;
        temp= temp.Substring(openIndex+6);
        int startIndex = temp.IndexOf('"');
        temp = temp.Substring(startIndex + 1);
        int endIndex= temp.IndexOf('"');
        temp = temp.Substring(0,endIndex);
        string hashId = temp;
        UnityEngine.Debug.Log(temp);
        json= json.Replace("\""+hashId+"\"", "\"\"");

        


    }

    public void Remove(List<DependencyJson> dependencies)
    {
        for (int i = 0; i < dependencies.Count; i++)
        {
            Remove(dependencies[i]);

        }
    }
    private void Add(DependencyJson dependencies)
    {
        Add(dependencies.nameId, dependencies.value);
    }
    private void Remove(DependencyJson dependencies)
    {
        RemoveFromName(dependencies.nameId);
        RemoveFromUrl(dependencies.value);
    }

    public void Remove(string removeValue)
    {
        RemoveFromName(removeValue);
        RemoveFromUrl(removeValue);
    }
}
