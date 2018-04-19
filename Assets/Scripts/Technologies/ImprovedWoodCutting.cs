using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Technologies/Woodcutting")]
public class ImprovedWoodCutting : FinishedResearchAction
{
    public float extraWoodCuttingSpeed = 1.25f;

	public override void ActivateTechnology(int playerID)
    {
		PlayerDataManager.instance.GetPlayerData(playerID).woodCuttingSpeed += extraWoodCuttingSpeed;
    }
}
