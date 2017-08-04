using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class ControllerUIManager : MonoBehaviour {

    [Header("Main Info")]
    public Text title;
    public Image icon;

    public Text tooltipText;

    public Button[] villagerButtons;
    public Button[] buildingButtons;

    [HideInInspector]
    public enum CONTROLLER_UI_TYPE
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

    [HideInInspector]
    public Image[] statIcons = new Image[4];

    [HideInInspector]
    public Text[] statTexts = new Text[4];

    CONTROLLER_UI_TYPE currentActiveView = CONTROLLER_UI_TYPE.NONE;
    CONTROLLER_UI_TYPE lastActiveView = CONTROLLER_UI_TYPE.NONE;

    string none = "";

    private static ControllerUIManager controllerUIManager;

    public static ControllerUIManager instance
    {
        get
        {
            if (!controllerUIManager)
            {
                controllerUIManager = FindObjectOfType(typeof(ControllerUIManager)) as ControllerUIManager;

                if (!controllerUIManager)
                {
                    Debug.LogError("There needs to be one active ControllerUIManager script on a GameObject in your scene.");
                }
                else
                {
                    controllerUIManager.Init();
                }
            }

            return controllerUIManager;
        }
    }

    void Init()
    {

    }

	void Start () {
        DeactivateVillagerView();
        DeactivateBuildingsView();
        DeactivateStatsView();
        DeactivateConstructionProgressView();
        DeactivateHitpointsView();
        ChangeUI(CONTROLLER_UI_TYPE.NONE);
        HideTooltip();
    }

    public void GoBack()
    {
        if(lastActiveView != CONTROLLER_UI_TYPE.NONE)
        {
            ChangeUI(lastActiveView);
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
        ChangeUI(CONTROLLER_UI_TYPE.NONE);
    }

    public void ShowVillagerUI(UnitStateController unit)
    {
        title.text = unit.title;
        icon.sprite = unit.iconSprite;
        ChangeUI(CONTROLLER_UI_TYPE.VILLAGER);
        ActivateHitpointsUI(unit.hitpointsLeft, unit._unitStats.maxHitpoints);
        DeactivateStatsView(); // Todo show unit stats like attack, defence etc.
    }

    public void ShowBuildingUI(Building building)
    {
        title.text = building.title;
        icon.sprite = building.iconSprite;
        ChangeUI(CONTROLLER_UI_TYPE.BUILDING_INFO);
        ActivateHitpointsUI(building.hitpoints, building.maxHitPoints);
        ActivateStatsUI(building.statSprites, building.GetUniqueStats());
    }

    public void ShowConstructionProgressView()
    {
        ChangeUI(CONTROLLER_UI_TYPE.CONSTRUCTION_PROGRESS);
    }

    public void ShowResourceView(Resource resource)
    {
        title.text = resource.title;
        icon.sprite = resource.iconSprite;
        ChangeUI(CONTROLLER_UI_TYPE.RECOURSE_INFO);
        ActivateStatsUI(resource.statSprites, resource.GetUniqueStats());
    }

    void ActivateVillagerView()
    {
        for(int i = 0; i < villagerButtons.Length; i++)
            villagerButtons[i].gameObject.SetActive(true);
    }

    void DeactivateVillagerView()
    {
        for (int i = 0; i < villagerButtons.Length; i++)
            villagerButtons[i].gameObject.SetActive(false);
    }

    public void ActivateBuildingsView()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
            buildingButtons[i].gameObject.SetActive(true);
    }

    public void DeactivateBuildingsView()
    {
        for (int i = 0; i < buildingButtons.Length; i++)
            buildingButtons[i].gameObject.SetActive(false);
    }

    public void ActivateStatsView(Sprite[] icons, int[] stats)
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

    public void DeactivateStatsView()
    {
        statsInfoPrefab.gameObject.SetActive(false);
    }

    public void ActivateConstructionProgressView()
    {
        constructionProgressPrefab.gameObject.SetActive(true);
    }

    public void DeactivateConstructionProgressView()
    {
        constructionProgressPrefab.gameObject.SetActive(false);
    }

    void ActivateHitpointsView(int hp, int max)
    {
        hitpointsPrefab.gameObject.SetActive(true);
        UpdateHitpoints(hp, max);
    }

    public void DeactivateHitpointsView()
    {
        hitpointsPrefab.gameObject.SetActive(false);
    }

    public void ChangeUI(CONTROLLER_UI_TYPE viewType)
    {
        DisableLastControllerView(lastActiveView);

        lastActiveView = currentActiveView;
        currentActiveView = viewType;

        if (currentActiveView != CONTROLLER_UI_TYPE.NONE)
            icon.enabled = true;

        ActivateCurrentView(currentActiveView);

        HideTooltip();
    }

    void ActivateCurrentView(CONTROLLER_UI_TYPE viewType)
    {
        switch (viewType)
        {
            case CONTROLLER_UI_TYPE.NONE:
                title.text = none;
                icon.enabled = false;
                DeactivateHitpointsView();
                DeactivateStatsView();
                break;
            case CONTROLLER_UI_TYPE.BUILDINGS:
                ActivateBuildingsView();
                break;
            case CONTROLLER_UI_TYPE.VILLAGER:
                ActivateVillagerView();
                break;
            case CONTROLLER_UI_TYPE.CONSTRUCTION_PROGRESS:
                ActivateConstructionProgressView();
                DeactivateHitpointsView();
                DeactivateStatsView();
                break;
            case CONTROLLER_UI_TYPE.BUILDING_INFO:
                break;
            case CONTROLLER_UI_TYPE.RECOURSE_INFO:
                DeactivateHitpointsView();
                break;
        }
    }

    void DisableLastControllerView(CONTROLLER_UI_TYPE viewType)
    {
        switch (viewType)
        {
            case CONTROLLER_UI_TYPE.NONE:
                break;
            case CONTROLLER_UI_TYPE.BUILDINGS:
                DeactivateBuildingsView();
                break;
            case CONTROLLER_UI_TYPE.VILLAGER:
                DeactivateVillagerView();
                break;
            case CONTROLLER_UI_TYPE.CONSTRUCTION_PROGRESS:
                DeactivateConstructionProgressView();
                break;
            case CONTROLLER_UI_TYPE.BUILDING_INFO:
                break;
            case CONTROLLER_UI_TYPE.RECOURSE_INFO:
                break;
        }
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
