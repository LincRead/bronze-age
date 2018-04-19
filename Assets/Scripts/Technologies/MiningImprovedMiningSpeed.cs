using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Mining")]
public class ImprovedMiningSpeed : FinishedResearchAction
{
    public float extraMiningSpeed = 1.25f;

    public override void ActivateTechnology(int playerID)
    {
        PlayerDataManager.instance.GetPlayerData(playerID).miningSpeed += extraMiningSpeed;
    }
}
