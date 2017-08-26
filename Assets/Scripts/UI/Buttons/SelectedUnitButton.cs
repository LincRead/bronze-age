using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SelectedUnitButton : UnitUIButton
{
    public Image controllerIcon;
    public Image healthBar;

    UnitStateController controller;
    RectTransform healthBarRectTransform;

    public void Clear()
    {
        gameObject.SetActive(false);
        this.controller = null;
    }

    public void UpdateButton(UnitStateController controller)
    {
        gameObject.SetActive(true);
        this.controller = controller;
        tooltip = title + controller.title; // Stringbuilder
        controllerIcon.sprite = controller._unitStats.iconSprite;

        healthBarRectTransform = healthBar.GetComponent<RectTransform>();
        Debug.Log(controller.hitpointsLeft / controller.maxHitpoints);
        healthBarRectTransform.localScale = new Vector3((float)((float)controller.hitpointsLeft / (float)controller.maxHitpoints), 1.0f, 1.0f);
    }

    public void UpdateHealth()
    {
        if(controller == null || controller.dead)
        {
            Clear();
        }

        else
        {
            healthBarRectTransform.localScale = new Vector3((float)((float)controller.hitpointsLeft / (float)controller.maxHitpoints), 1.0f, 1.0f);
        }
    }

    protected override void OnClick()
    {
        ControllerSelecting controllerSelection = PlayerManager.instance._controllerSelecting;
        controllerSelection.ResetSelection();
        PlayerManager.instance.selectableController = controller;
        controllerSelection.SetUnitAsSelected();

        if (controller._unitStats.isVillager)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.VILLAGER, controller);
        }

        else
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.WARRIOR, controller);
        }
    }
}
