using UnityEngine;
using System.Collections;

public class Tree : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        PlayerDataManager.instance.AddTimberForPlayer(1, playerID);
    }
}