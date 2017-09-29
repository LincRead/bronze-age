using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionQueueButton : UnitUIButton
{
    public Text number;

    int index;

    ProductionQueueCanvas parentCanvas;

    public void SetReferenceToParent(ProductionQueueCanvas parentCanvas)
    {
        this.parentCanvas = parentCanvas;
        index = int.Parse(number.text) - 2;
    }

    protected override void OnClick()
    {
        parentCanvas.CancelProduction(index);
    }

    public void ActivateIcon(Sprite newIconSprite)
    {
        _button.interactable = true;

        _icon.enabled = true;
        _icon.sprite = newIconSprite;

        number.enabled = false;

        if (hovered)
        {
            ShowTooltip();
        }
    }

    public void DeactivateIcon()
    {
        _button.interactable = false;

        _icon.enabled = false;
        number.enabled = true;

        if (hovered)
        {
            HideTooltip();
        }
    }
}
