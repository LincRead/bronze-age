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

        if(data.newCitizens > 0) AddRequired(iconsObject.icons[1], data.newCitizens);
        if (data.food > 0) AddRequired(iconsObject.icons[2], data.food);
        if (data.timber > 0) AddRequired(iconsObject.icons[3], data.timber);
        if (data.wealth > 0) AddRequired(iconsObject.icons[8], data.wealth);
        if (data.metal > 0) AddRequired(iconsObject.icons[7], data.metal);

        for (int i = numItemsRequired; i < resourceIcons.Length; i++)
        {
            resourceIcons[i].enabled = false;

            if(resourceAmounts[i] != null)
            {
                resourceAmounts[i].enabled = false;
            }
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
