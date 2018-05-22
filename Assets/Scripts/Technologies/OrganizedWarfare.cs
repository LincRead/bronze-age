using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Ogranized Warfare")]
public class OrganizedWarfare : FinishedResearchAction
{
   public float upgradeToSpeed = 1.1f;

    public override void ActivateTechnology(int playerID)
    {
        PlayerDataManager.instance.GetPlayerData(playerID).militaryUnitTrainingSpeed = upgradeToSpeed;
    }
}
