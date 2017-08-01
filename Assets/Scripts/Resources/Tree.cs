using UnityEngine;
using System.Collections;
using System.Text;

public class Tree : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        WorldManager.Manager.GetPlayerDataForPlayer(playerID).timber++;
        UnitUIManager.Manager.timberText.text = WorldManager.Manager.GetPlayerDataForPlayer(playerID).timber.ToString();
    }
}
