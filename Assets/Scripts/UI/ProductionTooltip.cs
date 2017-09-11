using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionTooltip : MonoBehaviour {

    public Text title;
    public Text description;

    public Image[] resourceIcons;
    public Text[] resourceAmounts;

    public ResourceIcons iconsObject;

    int numItemsRequired = 0;

    public void UpdateData(ProductionButtonData data)
    {
        title.text = data.title;
        description.text = data.description;
        SetRequiredVisuals(data);
    }

    void SetRequiredVisuals(ProductionButtonData data)
    {
        // Reset
        numItemsRequired = 0;

        AddRequired(iconsObject.icons[1], data.population);
        AddRequired(iconsObject.icons[2], data.food);
        AddRequired(iconsObject.icons[3], data.timber);
        AddRequired(iconsObject.icons[4], data.stoneTools);
        AddRequired(iconsObject.icons[4], data.copper);

        for(int i = numItemsRequired; i < resourceIcons.Length; i++)
        {
            resourceIcons[i].enabled = false;
            resourceAmounts[i].enabled = false;
        }
    }

    void AddRequired(Sprite icon, int amount)
    {
        if(amount <= 0 || numItemsRequired >= resourceIcons.Length)
        {
            return;
        }

        resourceIcons[numItemsRequired].enabled = true;
        resourceIcons[numItemsRequired].sprite = icon;

        resourceAmounts[numItemsRequired].enabled = true;
        resourceAmounts[numItemsRequired].text = amount.ToString();

        numItemsRequired++;
    }
}
