using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TribeController : UnitStateController {

    [HideInInspector]
    public bool movingTowarsCamp = false;

	protected override void Update ()
    {
        base.Update();

        if(!movingTowarsCamp)
        {
            if (PlayerDataManager.instance.GetPlayerData(playerID).placedCamp)
            {
                MoveTo(PlayerManager.instance.civilizationCenter);

                movingTowarsCamp = true;

                Deselect();

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

                // Don't show Tribe actions anymore
                ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);

                // Make sure we don't keep on showing build cursor
                EventManager.TriggerEvent("SetDefaultCursor");
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
