using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;

public class ControllerUIManager : MonoBehaviour {

    [Header("Controller info")]
    public Text title;
    public Image icon;

    public Text tooltipText;

    [Header("Buttons")]
    public Button[] villagerButtons;
    public Button[] buildingButtons;

    [HideInInspector]
    public enum CONTROLLER_UI_VIEW
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

    CONTROLLER_UI_VIEW currentViewType = CONTROLLER_UI_VIEW.NONE;
    CONTROLLER_UI_VIEW lastViewType = CONTROLLER_UI_VIEW.NONE;

    ControllerUIView currentView;
    ControllerUIView lastView;

    [Header("Views")]
    public ControllerUIView nothingSelectedView;
    public VillagerView villagerView;
    public BuildingsView buildingsView;
    public BuildingView buildingView;
    public ResourceView resourceView;
    public ConstructionView constructionView;

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
                    Debug.LogError("There needs to be one active ControllerUIManager script on a GameObject in the scene.");
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

        foreach (Button btn in villagerButtons)
            btn.gameObject.SetActive(false);

        foreach (Button btn in buildingButtons)
            btn.gameObject.SetActive(false);

        ChangeView(CONTROLLER_UI_VIEW.NONE, null);
        HideTooltip();
        HideHitpoints();
        HideStats();
    }

    public void ShowDefaultUI()
    {
        ChangeView(CONTROLLER_UI_VIEW.NONE, null);
    }

    public void ChangeView(CONTROLLER_UI_VIEW viewType, BaseController controller)
    {
        lastViewType = currentViewType;

        if (currentView != null)
        {
            currentView.OnExit();
            lastView = currentView;
        }

        switch (viewType)
        {
            case CONTROLLER_UI_VIEW.NONE:
                currentView = nothingSelectedView;
                break;

            case CONTROLLER_UI_VIEW.VILLAGER:
                currentView = villagerView;
                break;

            case CONTROLLER_UI_VIEW.BUILDINGS:
                controller = lastView._controller;
                currentView = buildingsView;
                break;

            case CONTROLLER_UI_VIEW.BUILDING_INFO:
                currentView = buildingView;
                break;

            case CONTROLLER_UI_VIEW.CONSTRUCTION_PROGRESS:
                currentView = constructionView;
                break;

            case CONTROLLER_UI_VIEW.RECOURSE_INFO:
                currentView = resourceView;
                break;
        }

        currentView.OnEnter(this, controller);
        currentViewType = viewType;

        HideTooltip();
    }

    public void GoBackToLastView()
    {
        ChangeView(lastViewType, lastView._controller);
    }

    private void Update()
    {
        currentView.Update();
    }

    public void ShowHitpoints(int hp, int max)
    {
        hitpointsPrefab.gameObject.SetActive(true);
        UpdateHitpoints(hp, max);
    }

    public void HideHitpoints()
    {
        hitpointsPrefab.gameObject.SetActive(false);
    }

    public void UpdateHitpoints(int hp, int max)
    {
        hitpointsText.text = new StringBuilder(hp.ToString() + "/" + max.ToString()).ToString();
    }

    public void ShowStats(Sprite[] icons, int[] stats)
    {
        statsInfoPrefab.gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            if (i < stats.Length)
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

    public void UpdateStats(Sprite[] icons, int[] stats)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < stats.Length)
            {
                statIcons[i].sprite = icons[i];
                statTexts[i].text = stats[i].ToString();
            }
        }
    }

    public void UpdateStat(int index, int value)
    {
        statTexts[index].text = value.ToString();
    }

    public void HideStats()
    {
        statsInfoPrefab.gameObject.SetActive(false);
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
