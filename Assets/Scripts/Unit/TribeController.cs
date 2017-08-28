using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeController : UnitStateController {

    bool movingTowarsCamp = false;

	protected override void Update ()
    {
        base.Update();

        if(!movingTowarsCamp)
        {
            if (PlayerDataManager.instance.GetPlayerData(playerID).placedCamp)
            {
                MoveTo(PlayerManager.instance.civilizationCenter);

                movingTowarsCamp = true;

                // Can't control Tribe anymore
                if (PlayerManager.myPlayerID == playerID)
                {
                    PlayerManager.instance.RemoveFriendlyUnitReference(this, playerID);
                }

                // Can't control Tribe anymore
                if (selected)
                {
                    PlayerManager.instance._controllerSelecting.RemoveFromSelectedUnits(this);
                }
            }
        }
	}

    public void SetupCamp(Camp camp)
    {
        camp.FinishConstruction();
        RemoveFromPathfinding();
        Destroy(gameObject);
    }
}
