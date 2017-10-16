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
        public bool inQueue;
        public bool completed;
    }

    public void Init()
    {
        technologyDictionary = new Dictionary<string, Technology>();

        AddTechnology("Mesolithic Age");
        AddTechnology("Polished Stone Axe");
        AddTechnology("Weaving");
        AddTechnology("Domesticated Animals");
    }

    public void AddTechnology(string title)
    {
        Technology newTechnology = new Technology();
        newTechnology.title = title;
        newTechnology.completed = false;
        newTechnology.inQueue = false;
        technologyDictionary.Add(title, newTechnology);
    }

    public bool CanProduceTechnology(string key)
    {
        Technology technology = null;

        if (technologyDictionary.TryGetValue(key, out technology))
        {
            return !technology.completed && !technology.inQueue;
        }

        else
        {
            Debug.LogError("Trying to get completed value of technology not added to Dictionary: " + key);
        }

        return true;
    }

    public bool GetTechnologyCompleted(string key)
    {
        Technology technology = null;

        if(technologyDictionary.TryGetValue(key, out technology))
        {
            return technology.completed;
        }

        else
        {
            Debug.LogError("Trying to get completed value of technology not added to Dictionary: " + key);
        }

        return true;
    }

    public void SetTechnologyInQueue(string key)
    {
        Technology technology = null;

        if (technologyDictionary.TryGetValue(key, out technology))
        {
            technology.inQueue = true;
        }

        else
        {
            Debug.LogError("Trying to set inQueue value for technology not added to Dictionary: " + key);
        }
    }

    public void RemoveTechnologyFromQueue(string key)
    {
        Technology technology = null;

        if (technologyDictionary.TryGetValue(key, out technology))
        {
            technology.inQueue = false;
        }

        else
        {
            Debug.LogError("Trying to set inQueue value for technology not added to Dictionary: " + key);
        }
    }

    public void CompleteTechnology(string key)
    {
        Technology technology = null;

        if(technologyDictionary.TryGetValue(key, out technology))
        {
            technology.completed = true;
        }

        else
        {
            Debug.LogError("Trying to set completed value for technology not added to Dictionary: " + key);
        }
    }
}
