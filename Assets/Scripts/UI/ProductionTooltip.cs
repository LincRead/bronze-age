using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProductionTooltip : MonoBehaviour {

    public Text title;
    public Text description;
    public Text requiresTitle;

    public Image[] resourceIcons;
    public Text[] resourceAmountTexts;

    private int[] resourceAmounts = new int[5];
    private int[] requiredTypes = new int[5];

    public ResourceIcons iconsObject;

    int numItemsRequired = 0;

    private Color defaultTextColor;
    private Color insufficientResourcesTextColor;

    private void Start()
    {
        for (int i = 0; i < requiredTypes.Length; i++)
        {
            requiredTypes[i] = -1;
        }

        defaultTextColor = new Color32(0x8F, 0x7E, 0x60, 0xFF);
        insufficientResourcesTextColor = new Color32(0xE8, 0x18, 0x18, 0xFF);
    }

    private void Update()
    {
        UpdateRequiredTexts();
    }

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

        if (data.food > 0) AddRequired(iconsObject.icons[2], data.food, 0);
        if (data.timber > 0) AddRequired(iconsObject.icons[3], data.timber, 1);
        if (data.wealth > 0) AddRequired(iconsObject.icons[8], data.wealth, 2);
        if (data.metal > 0) AddRequired(iconsObject.icons[7], data.metal, 3);
        if (data.stepsRequired > 0) AddRequired(iconsObject.icons[9], data.stepsRequired, 4);

        for (int i = numItemsRequired; i < resourceIcons.Length; i++)
        {
            resourceIcons[i].enabled = false;

            if(resourceAmountTexts[i] != null)
            {
                resourceAmountTexts[i].enabled = false;
            }
        }
    }

    void AddRequired(Sprite icon, int amount, int requiredType)
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

        Debug.Log(requiredType);

        requiredTypes[numItemsRequired] = requiredType;
        resourceAmounts[numItemsRequired] = amount;

        resourceIcons[numItemsRequired].enabled = true;
        resourceIcons[numItemsRequired].sprite = icon;

        resourceAmountTexts[numItemsRequired].enabled = true;
        resourceAmountTexts[numItemsRequired].text = amount.ToString();

        numItemsRequired++;
    }

    void UpdateRequiredTexts()
    {
        PlayerData myPlayerData = PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID);

        for (int i = 0; i < numItemsRequired; i++)
        {
            bool haveRequired = true;

            switch(requiredTypes[i])
            {
                case 0: if (resourceAmounts[i] > myPlayerData.foodInStock) haveRequired = false; break;
                case 1: if (resourceAmounts[i] > myPlayerData.timber) haveRequired = false; break;
                case 2: if (resourceAmounts[i] > myPlayerData.wealth) haveRequired = false; break;
                case 3: if (resourceAmounts[i] > myPlayerData.metal) haveRequired = false; break;
            }

            if(haveRequired)
            {
                resourceAmountTexts[i].color = defaultTextColor;
            }

            else
            {
                resourceAmountTexts[i].color = insufficientResourcesTextColor;
            }
        }
    }
}
