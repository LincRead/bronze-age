using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Writing")]
public class Writing : FinishedResearchAction
{
    public float upgradeToFactor = 1.25f;

    public override void ActivateTechnology(int playerID)
    {
        PlayerDataManager.instance.GetPlayerData(playerID).knowledgeGenerationFactor = upgradeToFactor;
    }
}