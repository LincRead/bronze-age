using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager playerDataManager;

    [HideInInspector]
    public List<PlayerData> playerData;

    public PlayerStartResources playerStartingResources;

    [HideInInspector]
    public static Color neutralPlayerColor = new Color(0.4f, 0.2f, 0.1f);

    public static float foodPerSurplusLevel = 50;

    public static PlayerDataManager instance
    {
        get
        {
            if (!playerDataManager)
            {
                playerDataManager = FindObjectOfType(typeof(PlayerDataManager)) as PlayerDataManager;

                if (!playerDataManager)
                {
                    Debug.LogError("There needs to be one active PlayerDataManager script on a GameObject in your scene.");
                }

                else
                {
                    playerDataManager.Init();
                }
            }

            return playerDataManager;
        }
    }

    void Init()
    {
        InitStartingResourcesForAllPlayers();
        SetTeamColors();
    }

    void InitStartingResourcesForAllPlayers()
    {
        int numPlayers = WorldManager.instance.numPlayers;

        playerData = new List<PlayerData>(numPlayers);

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerData newPlayerData = new PlayerData();
            playerData.Add(newPlayerData);

            newPlayerData.foodInStock = playerStartingResources.food;
            newPlayerData.timber = playerStartingResources.timber;
            newPlayerData.wealth = playerStartingResources.wealth;
            newPlayerData.metal = playerStartingResources.metal;

            /* 
             * Make sure this is done straight away so UI shows correct values
             * and bonuses are correct
             */
            CalculateFoodSurplusLevelFor(i);
        }
    }

    void SetTeamColors()
    {
        for (int i = 0; i < WorldManager.instance.numPlayers; i++)
        {
            switch (i)
            {
                case 0: playerData[i].teamColor = Color.blue; break;
                case 1: playerData[i].teamColor = Color.red; break;
                case 2: playerData[i].teamColor = Color.green; break;
                case 3: playerData[i].teamColor = Color.yellow; break;
            }
        }
    }

    public void AddPopulationForPlayer(int value, int player)
    {
        playerData[player].population += value;

        if(player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateHousingStockUI");
        }
    }

    public void AddHousingForPlayer(int value, int player)
    {
        playerData[player].housing += value;

        if (player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateHousingStockUI");
        }
    }

    public void AddFoodStockForPlayer(int value, int player)
    {
        playerData[player].foodInStock += value;

        CalculateFoodSurplusLevelFor(player);

        if (player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateFoodStockUI");
        }
    }

    public void CalculateFoodSurplusLevelFor(int player)
    {
        int foodSurplusLevel = (int)Mathf.Floor(((PlayerDataManager.foodPerSurplusLevel + playerData[player].foodInStock) / (PlayerDataManager.foodPerSurplusLevel * 5) * 5));

        if (foodSurplusLevel > 4)
        {
            foodSurplusLevel = 4;
        }
        
        playerData[player].foodSurplusLevel = foodSurplusLevel;
    }

    public int GetFoodSurplusLevelFor(int player)
    {
        return playerData[player].foodSurplusLevel;
    }

    public void AddFoodIntakeForPlayer(int value, int player)
    {
        playerData[player].foodIntake += value;

        if (player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateFoodIntakeUI");
        }
    }

    public void AddTimberForPlayer(int value, int player)
    {
        playerData[player].timber += value;

        if (player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateTimberStockUI");
        }
    }

    public void AddWealthForPlayer(int value, int player)
    {
        playerData[player].wealth += value;

        if (player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateWealthStockUI");
        }
    }

    public void AddMetalForPlayer(int value, int player)
    {
        playerData[player].metal += value;

        if (player == PlayerManager.myPlayerID)
        {
            EventManager.TriggerEvent("UpdateMetalStockUI");
        }
    }

    public PlayerData GetPlayerData(int player)
    {
        return playerData[player];
    }

    public void AddResourceForPlayer(int value, int player, RESOURCE_TYPE type)
    {
        switch(type)
        {
            case RESOURCE_TYPE.FOOD: AddFoodStockForPlayer(value, player); break;
            case RESOURCE_TYPE.WOOD: AddTimberForPlayer(value, player); break;
            case RESOURCE_TYPE.METAL: AddMetalForPlayer(value, player); break;
            case RESOURCE_TYPE.WEALTH: AddWealthForPlayer(value, player); break;
        }
    }
}
