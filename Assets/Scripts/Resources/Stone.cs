using UnityEngine;
using System.Collections;

public class Stone : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        WorldManager.Manager.GetPlayerDataForPlayer(playerID).stone++;
        UnitUIManager.Manager.stoneText.text = WorldManager.Manager.GetPlayerDataForPlayer(playerID).stone.ToString();
    }
}
