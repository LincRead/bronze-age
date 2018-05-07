using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Wheel")]
public class Wheel : FinishedResearchAction
{
    public override void ActivateTechnology(int playerID)
    {
        PlayerDataManager.instance.GetPlayerData(playerID).villagerCarryLimit += 5;
    }
}
