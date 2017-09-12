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

            newPlayerData.foodStock = playerStartingResources.food;
            newPlayerData.timber = playerStartingResources.timber;
            newPlayerData.metal = playerStartingResources.metal;
            newPlayerData.population = playerStartingResources.population;
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
        EventManager.TriggerEvent("UpdateHousingStockUI");
    }

    public void AddHousingForPlayer(int value, int player)
    {
        playerData[player].housing += value;
        EventManager.TriggerEvent("UpdateHousingStockUI");
    }

    public void AddFoodStockForPlayer(int value, int player)
    {
        playerData[player].foodStock += value;
        EventManager.TriggerEvent("UpdateFoodStockUI");
    }

    public void AddFoodProductionForPlayer(int value, int player)
    {
        playerData[player].foodIntake += value;
        EventManager.TriggerEvent("UpdateFoodIntakeUI");
    }

    public void AddTimberForPlayer(int value, int player)
    {
        playerData[player].timber += value;
        EventManager.TriggerEvent("UpdateTimberStockUI");
    }

    public void AddMetalForPlayer(int value, int player)
    {
        playerData[player].metal += value;
        EventManager.TriggerEvent("UpdateMetalStockUI");
    }

    public PlayerData GetPlayerData(int player)
    {
        return playerData[player];
    }
}
