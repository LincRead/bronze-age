using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class UnitUIManager : MonoBehaviour {

    // Singleton
    public static UnitUIManager Manager;

    [Header("Main Info")]
    public Text title;
    public Image icon;

    public Text tooltipText;

    public Button[] villagerButtons;
    public Button[] buildingButtons;

    [HideInInspector]
    public enum UNIT_UI_TYPE
    {
        NONE,
        VILLAGER,
        BUILDINGS,
        BUILDING_INFO,
        CONSTRUCTION_PROGRESS,
        RECOURSE_INFO
    }

    [Header("Hitpoints bar")]
    public GameObject hitpointsPrefab;
    public Image hitpointsBackground;
    public Image hitpointsBar;
    public Text hitpointsText;

    [Header("Constrcution progress")]
    public GameObject constructionProgressPrefab;
    public Image constructionProgressBarImage;
    public Text constructionProgressText;

    [Header("Stats")]
    public GameObject statsInfoPrefab;
    public Image[] statIcons = new Image[4];
    public Text[] statTexts = new Text[4];

    [Header("Resources")]
    public Text housingText;
    public Text foodSurplusText;
    public Text timberText;
    public Text stoneText;

    UNIT_UI_TYPE currentActivatedUI = UNIT_UI_TYPE.NONE;
    UNIT_UI_TYPE lastActivateUI = UNIT_UI_TYPE.NONE;

    string none = "";

    void Awake()
    {
        // Singleton
        Manager = this;
    }

	// Use this for initialization
	void Start () {
        DeactivateVillagerUI();
        DeactivateBuildingsUI();
        DeactivateStatsUI();
        DeactivateConstructionProgressUI();
        DeactivateHitpointsUI();
        ChangeUI(UNIT_UI_TYPE.NONE);
        HideTooltip();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GoBack()
    {
        if(lastActivateUI != UNIT_UI_TYPE.NONE)
        {
            ChangeUI(lastActivateUI);
        }
    }

    public void UpdateConstructionProgressElements(Building building, float percent)
    {
        title.text = building.title;
        icon.sprite = building.iconSprite;
        constructionProgressText.text = ((int)(percent * 100)).ToString() + "%";
        constructionProgressBarImage.fillAmount = percent;
    }

    public void ShowDefaultUI()
    {
        ChangeUI(UNIT_UI_TYPE.NONE);
    }

    public void ShowVillagerUI(UnitStateController unit)
    {
        title.text = unit.title;
        icon.sprite = unit.iconSprite;
        ChangeUI(UNIT_UI_TYPE.VILLAGER);
        ActivateHitpointsUI();
        UpdateHitpoints(unit.hitpointsLeft, unit._unitStats.maxHitpoints);
        DeactivateStatsUI(); // todo: show unit stats like attack, defence etc.
    }

    public void ShowConstructionProgress()
    {
        ChangeUI(UNIT_UI_TYPE.CONSTRUCTION_PROGRESS);
    }

    public void ShowBuildingUI(Building building)
    {
        title.text = building.title;
        icon.sprite = building.iconSprite;
        ChangeUI(UNIT_UI_TYPE.BUILDING_INFO);
        ActivateHitpointsUI();
        UpdateHitpoints(building.hitpoints, building.maxHitPoints);
        ActivateStatsUI(building.statSprites, building.GetUniqueStats());
    }

    public void ShowResourceUI(Resource resource)
    {
        title.text = resource.title;
        icon.sprite = resource.iconSprite;
        ChangeUI(UNIT_UI_TYPE.RECOURSE_INFO);
        ActivateStatsUI(resource.statSprites, resource.GetUniqueStats());
    }

    void ActivateVillagerUI()
    {
        for(int i = 0; i < villagerButtons.Length; i++)
            villagerButtons[i].gameObject.SetActive(true);
    }

    void DeactivateVillagerUI()
    {
        for (int i = 0; i < villagerButtons.Length; i++)
            villagerButtons[i].gameObject.SetActive(false);
    }

    public void ActivateBuildingsUI()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
            buildingButtons[i].gameObject.SetActive(true);
    }

    public void DeactivateBuildingsUI()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
            buildingButtons[i].gameObject.SetActive(false);
    }

    public void ActivateStatsUI(Sprite[] icons, int[] stats)
    {
        statsInfoPrefab.gameObject.SetActive(true);

        for(int i = 0; i < 4; i++)
        {
            if(i < stats.Length)
            {
                statIcons[i].enabled = true;
                statIcons[i].sprite = icons[i];

                statTexts[i].enabled = true;
                statTexts[i].text = stats[i].ToString();
            }

            else
            {
                statIcons[i].enabled = false;
                statTexts[i].enabled = false;
            }
        }
    }

    public void DeactivateStatsUI()
    {
        statsInfoPrefab.gameObject.SetActive(false);
    }

    public void ActivateConstructionProgressUI()
    {
        constructionProgressPrefab.gameObject.SetActive(true);
    }

    public void DeactivateConstructionProgressUI()
    {
        constructionProgressPrefab.gameObject.SetActive(false);
    }

    void ActivateHitpointsUI()
    {
        hitpointsPrefab.gameObject.SetActive(true);
    }

    public void DeactivateHitpointsUI()
    {
        hitpointsPrefab.gameObject.SetActive(false);
    }

    public void ChangeUI(UNIT_UI_TYPE uiType)
    {
        switch (currentActivatedUI)
        {
            case UNIT_UI_TYPE.NONE:
                break;
            case UNIT_UI_TYPE.BUILDINGS:
                DeactivateBuildingsUI();
                break;
            case UNIT_UI_TYPE.VILLAGER:
                DeactivateVillagerUI();
                break;
            case UNIT_UI_TYPE.CONSTRUCTION_PROGRESS:
                DeactivateConstructionProgressUI();
                break;
            case UNIT_UI_TYPE.BUILDING_INFO:
                break;
            case UNIT_UI_TYPE.RECOURSE_INFO:
                break;
        }

        lastActivateUI = currentActivatedUI;
        currentActivatedUI = uiType;

        if (currentActivatedUI != UNIT_UI_TYPE.NONE)
            icon.enabled = true;

        switch (currentActivatedUI)
        {
            case UNIT_UI_TYPE.NONE:
                title.text = none;
                icon.enabled = false;
                DeactivateHitpointsUI();
                DeactivateStatsUI();
                break;
            case UNIT_UI_TYPE.BUILDINGS:
                ActivateBuildingsUI();
                break;
            case UNIT_UI_TYPE.VILLAGER:
                ActivateVillagerUI();
                break;
            case UNIT_UI_TYPE.CONSTRUCTION_PROGRESS:
                ActivateConstructionProgressUI();
                DeactivateHitpointsUI();
                DeactivateStatsUI();
                break;
            case UNIT_UI_TYPE.BUILDING_INFO:
                break;
            case UNIT_UI_TYPE.RECOURSE_INFO:
                DeactivateHitpointsUI();
                break;
        }

        HideTooltip();
    }

    public void UpdateHitpoints(int hp, int max)
    {
        hitpointsText.text = new StringBuilder(hp.ToString() + "/" + max.ToString()).ToString();
    }

    public void UpdateStat(int index, int value)
    {
        statTexts[index].text = value.ToString();
    }

    public void ShowTooltip(string tip)
    {
        tooltipText.enabled = true;
        tooltipText.text = tip;
    }

    public void HideTooltip()
    {
        tooltipText.enabled = false;
    }
}
