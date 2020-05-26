
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Utility_PackageJson
{
    //"name": "be.eloistree.unitypackagefacilitator",                              
    public string name;
    public string GetNamespaceID() { return name; }
    //"displayName": "Unity Package Creator",                        
    public string displayName;
    public string GetDisplayName() { return displayName; }
    //"version": "0.0.1",                         
    public string version;
    public string GetPackageVersion() { return version; }
    //"unity": "2018.1",                             
    public string unity;
    public string GetUnityVersion() { return unity; }
    //"description": "Tools to create the structure of new unity package under 2 minutes.",                         
    public string description;
    public string GetDescriptionVersion() { return description; }
    //"keywords": ["Script","Tool","Productivity","Git","Unity Package"],                       
    public string[] keywords;
    public string[] GetKeywords() { return keywords; }
    //"category": "Script",                   
    public string category;
    public string GetCategory()
    {
        return category;
    }
    //"dependencies":{}   

    private List<DependencyJson> dependencies;

    public string ToJson() { return JsonUtility.ToJson(this, true); }
    public static Utility_PackageJson CreateFromFile(string filePath)
    {
        string file = File.ReadAllText(filePath);
        return CreateFromJson(file);
    }

    public static Utility_PackageJson CreateFromJson(string json)
    {
        Utility_PackageJson value = null;
        try
        {
            value = JsonUtility.FromJson<Utility_PackageJson>(json);
        }
        catch (Exception)
        {
            //  UnityEngine.Debug.LogWarning(e);
        }
        value.dependencies = DependencyJson.GetDependeciesFromText(json);


        return value;
    }

}

