using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Text;
using UnityEngine.EventSystems;

public class SelectedUnitButton : UnitUIButton
{
    public Image controllerIcon;
    public Image healthBar;

    UnitStateController _controller;
    RectTransform healthBarRectTransform;

    public void UpdateButton(UnitStateController controller)
    {
        gameObject.SetActive(true);
        _controller = controller;
        controllerIcon.sprite = controller._unitStats.iconSprite;

        healthBarRectTransform = healthBar.GetComponent<RectTransform>();
        healthBarRectTransform.localScale = new Vector3((float)((float)controller.hitpointsLeft / (float)controller.maxHitpoints), 1.0f, 1.0f);
    }

    public void UpdateHealth()
    {
        if(_controller == null || _controller.dead)
        {
            Clear();
        }

        else
        {
            healthBarRectTransform.localScale = new Vector3((float)((float)_controller.hitpointsLeft / (float)_controller.maxHitpoints), 1.0f, 1.0f);
        }
    }

    protected override void OnClick()
    {
        ControllerSelecting controllerSelection = PlayerManager.instance._controllerSelecting;
        controllerSelection.ResetSelection();
        PlayerManager.instance.selectableController = _controller;
        controllerSelection.SetUnitAsSelected();

        if (_controller._unitStats.isVillager)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.VILLAGER, _controller);
        }

        else
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.WARRIOR, _controller);
        }
    }

    // Had to override and create the string here, ...
    // ...or it wouldn't work first time the button got activated
    public override void OnPointerEnter(PointerEventData eventData)
    {
        ControllerUIManager.instance.ShowTooltip(new StringBuilder(title + _controller.title).ToString());
    }

    public void Clear()
    {
        gameObject.SetActive(false);
        _controller = null;
    }
}
