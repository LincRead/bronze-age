using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technologies : MonoBehaviour
{
    static Dictionary<string, Technology> technologyDictionary;

    private static Technologies technologies;

    public static Technologies instance
    {
        get
        {
            if (!technologies)
            {
                technologies = FindObjectOfType(typeof(Technologies)) as Technologies;

                if (!technologies)
                {
                    Debug.LogError("There needs to be one active Technologies script on a GameObject in your scene.");
                }

                else
                {
                    technologies.Init();
                }
            }

            return technologies;
        }
    }

    public class Technology
    {
        public string title;
        public bool completed;
    }

    public void Init()
    {
        technologyDictionary = new Dictionary<string, Technology>();

        AddTechnology("Mesolithic Age");
        AddTechnology("Stone Axe");
        AddTechnology("Weaving");
        AddTechnology("Archery");
    }

    public void AddTechnology(string title)
    {
        Technology newTechnology = new Technology();
        newTechnology.title = title;
        newTechnology.completed = false;
        technologyDictionary.Add(title, newTechnology);
    }

    public bool GetTechnologyCompleted(string key)
    {
        Technology technology = null;

        if(technologyDictionary.TryGetValue(key, out technology))
        {
            return technology.completed;
        }

        return true;
    }

    public void CompleteTechnology(string key)
    {
        Technology technology = null;

        if(technologyDictionary.TryGetValue(key, out technology))
        {
            technology.completed = true;
        }
    }
}
