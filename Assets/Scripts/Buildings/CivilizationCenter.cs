using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

// TEMP victory condition
using UnityEngine.SceneManagement;

public class CivilizationCenter : Building
{
    [Header("Unique Stats")]
    public int housing = 10;

    public BuildingStats settlementStats;
    public BuildingStats empireStats;

    protected override void Start()
    {
        base.Start();

        if(playerID > -1)
        {
            if (PlayerDataManager.instance.GetPlayerData(playerID).age == 0)
            {
                title = new StringBuilder("Waiting for tribe...").ToString();
            }
        }

        PlayerManager.instance.civilizationCenter = this;
    }

    protected override void Place()
    {
        base.Place();

        if (playerID > -1)
        {
            PlayerDataManager.instance.GetPlayerData(playerID).placedCamp = true;

            if(playerID == PlayerManager.myPlayerID)
            {
                Select();
            }
        }
    }

    public override void FinishConstruction()
    {
        // Instantly constructed, so update hitpoints instantly too
        hitpointsLeft = maxHitPoints;

        title = _buildingStats.title;

        if (selected)
        {
            ControllerUIManager.instance.UpdateTitle(title);
        }

        base.FinishConstruction();
    }

    public void Upgrade()
    {
        if(PlayerDataManager.instance.playerData[playerID].age == 1)
        {
            _buildingStats = settlementStats;

            // Add housing.
            housing = 8;
            PlayerDataManager.instance.AddHousingForPlayer(housing, playerID);

            // Update housing stat in Stats View
            if (selected)
            {
                ControllerUIManager.instance.UpdateStat(0, housing);
                ControllerUIManager.instance.UpdateIcon(_buildingStats.iconSprite);
            }
        }

        SetNewBuildingStats();
    }

    protected override void AddPlayerStats()
    {
        if(playerID != -1)
        {
            PlayerDataManager.instance.AddHousingForPlayer(housing, playerID);
        }
    }

    protected override void RemovePlayerStats()
    {
        if (playerID != -1)
        {
            PlayerDataManager.instance.AddHousingForPlayer(-housing, playerID);
        }
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = housing;
        return stats;
    }

    public override void Destroy()
    {
        base.Destroy();

        if(playerID == -1)
        {
            SceneManager.LoadScene("victory");
        }
    }
}
