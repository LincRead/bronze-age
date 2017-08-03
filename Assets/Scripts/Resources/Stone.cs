using UnityEngine;
using System.Collections;

public class Stone : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        PlayerDataManager.instance.AddStoneForPlayer(1, playerID);
    }
}
