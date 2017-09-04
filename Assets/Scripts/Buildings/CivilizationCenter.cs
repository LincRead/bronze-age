using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilizationCenter : Building
{
    [Header("Unique Stats")]
    public int housing = 10;

    public BuildingStats baseCampStats;

    protected override void Start()
    {
        base.Start();

        PlayerManager.instance.civilizationCenter = this;
    }

    public void Upgrade()
    {
        if(PlayerDataManager.instance.playerData[playerID].age == 1)
        {
            _buildingStats = baseCampStats;

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
}
