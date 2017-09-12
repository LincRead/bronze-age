using UnityEngine;
using System.Collections;

public class Metal : Resource {

    protected override void UpdateResourceAmountForPlayer(int playerID)
    {
        PlayerDataManager.instance.AddMetalForPlayer(1, playerID);
    }
}
