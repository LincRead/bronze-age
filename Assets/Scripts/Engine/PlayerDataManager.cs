using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDataManager : MonoBehaviour
{
    private static PlayerDataManager playerDataManager;

    [HideInInspector]
    public List<PlayerData> playerData;

    public PlayerStartResources playerStartingResources;

    [HideInInspector]
    public static Color neutralPlayerColor = new Color(0.4f, 0.2f, 0.1f);

    public static float foodPerSurplusLevel = 50;

	public static int[] knowledgeGeneratedByCivCenter = new int[] { 1, 2, 3, 4, 5 };

    [HideInInspector]
    public float timeToStartBeforeGameOver = 90;

    [HideInInspector]
    public float[] timeSinceStartedStarving;

    public StarvingPopup starvingPopupBox;

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

        InitStarvingValues(numPlayers);

        playerData = new List<PlayerData>(numPlayers);

        for (int i = 0; i < numPlayers; i++)
        {
            PlayerData newPlayerData = new PlayerData();
            playerData.Add(newPlayerData);

            newPlayerData.foodInStock = playerStartingResources.food;
            newPlayerData.timber = playerStartingResources.timber;
            newPlayerData.wealth = playerStartingResources.wealth;
            newPlayerData.metal = playerStartingResources.metal;
			newPlayerData.population = playerStartingResources.population;

            /* 
             * Make sure this is done straight away so UI shows correct values
             * and bonuses are correct
             */
            CalculateFoodSurplusLevelFor(i);
        }
    }

    void InitStarvingValues(int numPlayers)
    {
        timeSinceStartedStarving = new float[numPlayers];

        for (int i = 0; i < numPlayers; i++)
        {
            timeSinceStartedStarving[i] = 0.0f;
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

	public float CalculateKnowledgeGenerationFor(int player)
	{
		float knowledgeToGenerate = 0;

		switch (playerData[player].foodSurplusLevel) 
		{
		case 0:
			knowledgeToGenerate += playerData [player].numPriests * 0.2f;
			break;
		case 1:
			knowledgeToGenerate += playerData [player].numPriests * 0.4f;
			break;
		case 2:
			knowledgeToGenerate += playerData [player].numPriests * 0.6f;
			break;
		case 3:
			knowledgeToGenerate += playerData [player].numPriests * 0.8f;
			break;
		case 4:
			knowledgeToGenerate += playerData [player].numPriests * 1f;
			break;
		}

		knowledgeToGenerate += knowledgeGeneratedByCivCenter[playerData[player].foodSurplusLevel];

		knowledgeToGenerate *= playerData[player].knowledgeGenerationFactor;

		playerData[player].knowledgeGeneration = knowledgeToGenerate;

		return playerData[player].knowledgeGeneration;
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

	public void AddKnowledgeForPlayer(int value, int player)
	{
		playerData[player].knowledgeGeneration += value;

		// Knowledge generation in the Stock UI is updated every update loop
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

	void Update()
	{
		for (int i = 0; i < playerData.Count; i++)
		{
			CalculateKnowledgeGenerationFor(i);
            UpdateStarvingPeopleValue(i);
        }
    }

    void UpdateStarvingPeopleValue(int playerID)
    {
        if (playerData[playerID].foodSurplusLevel <= 0)
        {
            timeSinceStartedStarving[playerID] += Time.deltaTime;

            if (timeSinceStartedStarving[playerID] >= timeToStartBeforeGameOver)
            {
                SceneManager.LoadScene("gameover");
            }

            if(playerID == PlayerManager.myPlayerID)
            {
                starvingPopupBox.Show();
            }
        }
        
        else
        {
            timeSinceStartedStarving[playerID] = 0.0f;

            if (playerID == PlayerManager.myPlayerID)
            {
                starvingPopupBox.Hide();
            }
        }
    }
}
