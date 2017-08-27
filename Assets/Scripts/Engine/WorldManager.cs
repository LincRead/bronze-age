using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour {

    // 1 for singleplayer, more for multiplayer
    public static int numPlayers = 4;

    public static bool firstUpdate = true;

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

    }

    public void Update()
    {
        firstUpdate = false;
    }
}
