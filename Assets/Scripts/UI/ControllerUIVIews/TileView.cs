using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TileView : ControllerUIView
{
    public override void OnEnter(ControllerUIManager ui, BaseController controller)
    {
        base.OnEnter(ui, controller);

        Tile tile = PlayerManager.instance.selectedTile;

        switch (tile.fertilityPoints)
        {
            case 3: ui.UpdateTitle("Wetlands"); break;
            case 2: ui.UpdateTitle("Grass"); break;
            case 1: ui.UpdateTitle("Plain"); break;
            case 0: ui.UpdateTitle("Dessert"); break;
        }

        ui.icon.enabled = true;
        ui.icon.sprite = tile._tileSpriteRenderer.sprite;
        ui.icon.rectTransform.sizeDelta = new Vector2(32, 16);

        ui.description.text = new StringBuilder("Fertility: " + tile.fertilityPoints + " " + tile.visibleForControllerCount).ToString();
        ui.description.enabled = true;


    }

    public override void OnExit()
    {
        ui.description.enabled = false;
    }
}
