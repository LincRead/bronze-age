using UnityEngine;
using System.Collections;

public class Stone : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        PlayerDataManager.instance.AddStoneToolsForPlayer(1, playerID);
    }
}
