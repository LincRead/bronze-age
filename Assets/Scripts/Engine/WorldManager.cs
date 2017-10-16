using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    // 1 for singleplayer, more for multiplayer
    public int numPlayers = 4;

    public static bool firstUpdate = true;

    public static string[] civAgeNames = new string[6];

    private static WorldManager worldManager;

    public static WorldManager instance
    {
        get
        {
            if (!worldManager)
            {
                worldManager = FindObjectOfType(typeof(WorldManager)) as WorldManager;

                if (!worldManager)
                {
                    Debug.LogError("There needs to be one active WorldManager script on a GameObject in your scene.");
                }

                else
                {
                    worldManager.Init();
                }
            }

            return worldManager;
        }
    }

    void Init()
    {
        firstUpdate = true;
    }

    public void Awake()
    {
        civAgeNames[0] = "Paleolithic Age";
        civAgeNames[1] = "Mesolithic Age";
        civAgeNames[2] = "Neolithic Age";
        civAgeNames[3] = "Calcholitic Age";
        civAgeNames[4] = "Early Bronze Age";
        civAgeNames[5] = "Bronze Age";
    }

    public void Update()
    {
        firstUpdate = false;
    }

    public bool CanDeliverResourceTo(Building building, RESOURCE_TYPE resourceType)
    {
        if (resourceType == RESOURCE_TYPE.FOOD)
        {
            if (building._buildingStats.deliveryPointFood)
            {
                return true;
            }
        }

        else
        {
            if (building._buildingStats.deliveryPointMaterials)
            {
                return true;
            }
        }

        return false;
    }
}
