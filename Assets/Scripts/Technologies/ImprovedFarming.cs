using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Farming")]
public class ImprovedFarming : FinishedProductionAction
{
    public float extraFarmingSpeed = 1.25f;

    public override void Action(Building building)
    {
        int id = building.playerID;

        PlayerDataManager.instance.GetPlayerData(id).farmingSpeed += extraFarmingSpeed;
    }
}
