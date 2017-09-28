using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionTooltip : MonoBehaviour {

    public Text title;
    public Text description;
    public Text requiresTitle;

    public Image[] resourceIcons;
    public Text[] resourceAmounts;

    public ResourceIcons iconsObject;

    int numItemsRequired = 0;

    public void UpdateData(ProductionButtonData data, string desc)
    {
        title.text = data.title;
        description.text = desc;
        SetRequiredVisuals(data);
    }

    void SetRequiredVisuals(ProductionButtonData data)
    {
        // Reset
        numItemsRequired = 0;
        requiresTitle.enabled = false;

        AddRequired(iconsObject.icons[1], data.newCitizens);
        AddRequired(iconsObject.icons[2], data.food);
        AddRequired(iconsObject.icons[3], data.timber);
        AddRequired(iconsObject.icons[4], data.wealth);
        AddRequired(iconsObject.icons[5], data.metal);


        for (int i = numItemsRequired; i < resourceIcons.Length; i++)
        {
            resourceIcons[i].enabled = false;
            resourceAmounts[i].enabled = false;
        }
    }

    void AddRequired(Sprite icon, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        requiresTitle.enabled = true;

        if (numItemsRequired >= resourceIcons.Length)
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
