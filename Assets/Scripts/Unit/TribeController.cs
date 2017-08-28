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
                Building camp = PlayerManager.instance.civilizationCenter;

                MoveTo(camp);
                
                movingTowarsCamp = true;

                // Select Camp...
                camp.Select();
                Deselect();

                // Make sure we can see health while Tribe unit moves towards Camp construction site,
                // even though we can't select Tribe unit in this state
                _healthBar.Activate();

                // Can't control Tribe anymore
                if (PlayerManager.myPlayerID == playerID)
                {
                    PlayerManager.instance.RemoveFriendlyUnitReference(this, playerID);

                    // Can't control Tribe anymore
                    PlayerManager.instance._controllerSelecting.RemoveFromSelectedUnits(this);
                }

                // Don't show Tribe actions anymore
                ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.CONSTRUCTION_PROGRESS, camp);

                // Make sure we don't keep on showing build cursor
                EventManager.TriggerEvent("SetDefaultCursor");
            }
        }
	}

    public void SetupCamp(Camp camp)
    {
        RemoveFromPathfinding();
        Destroy(gameObject);
        camp.FinishConstruction();
    }

    public override void Deselect()
    {
        // Return so that we don't deactivate health bar,
        // even though Tribe unit is no longer selectable, but might get deselected
        if(movingTowarsCamp)
        {
            selected = false;
            _selectedIndicatorRenderer.enabled = false;
            return;
        }

        base.Deselect();
    }
}
