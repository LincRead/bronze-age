using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Farming")]
public class ImprovedFarming : FinishedResearchAction
{
    public float extraFarmingSpeed = 1.25f;

	public override void ActivateTechnology(int playerID)
    {
		PlayerDataManager.instance.GetPlayerData(playerID).farmingSpeed += extraFarmingSpeed;
    }
}
