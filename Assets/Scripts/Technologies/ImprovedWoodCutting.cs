using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Woodcutting")]
public class ImprovedWoodcutting : FinishedProductionAction
{
    public float extraWoodCuttingSpeed = 1.25f;

    public override void Action(Building building)
    {
        int id = building.playerID;

        PlayerDataManager.instance.GetPlayerData(id).woodCuttingSpeed += extraWoodCuttingSpeed;
    }
}
