using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager playerDataManager;

    [HideInInspector]
    public List<PlayerData> playerData;

    public PlayerStartResources playersStartingResources;

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
    }

    void InitStartingResourcesForAllPlayers()
    {
        int numPlayers = WorldManager.instance.numPlayers;

        playerData = new List<PlayerData>(numPlayers);

        for (int i = 0; i <numPlayers; i++)
        {
            PlayerData newPlayerData = new PlayerData();
            newPlayerData.food = playersStartingResources.food;
            newPlayerData.timber = playersStartingResources.timber;
            newPlayerData.stone = playersStartingResources.stone;
            newPlayerData.copper = playersStartingResources.copper;
            newPlayerData.tin = playersStartingResources.tin;
            newPlayerData.bronze = playersStartingResources.bronze;
            playerData.Add(new PlayerData());
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
        playerData[player].food += value;
        EventManager.TriggerEvent("UpdateFoodStockUI");
    }

    public void AddFoodProductionForPlayer(int value, int player)
    {
        playerData[player].foodProduction += value;
        EventManager.TriggerEvent("UpdateFoodProductionUI");
    }

    public void AddTimberForPlayer(int value, int player)
    {
        playerData[player].timber += value;
        EventManager.TriggerEvent("UpdateTimberStockUI");
    }

    public void AddStoneForPlayer(int value, int player)
    {
        playerData[player].stone += value;
        EventManager.TriggerEvent("UpdateStoneStockUI");
    }
}
