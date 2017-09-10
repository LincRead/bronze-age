using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class ControllerUIManager : MonoBehaviour {

    public Text ageText;

    [Header("Controller info")]
    public Text title;
    public Image icon;
    public Text tooltip;
    public GameObject controllerInfoBackground;

    [Header("Buttons")]
    public Button[] villagerButtons;
    public Button[] buildingButtons;
    public Button[] unitActionButtons;

    public Button selectedUnitButton;

    protected ProductionButton[] _productionButtonScripts;

    [HideInInspector]
    public enum CONTROLLER_UI_VIEW
    {
        NONE,
        VILLAGER,
        WARRIOR,
        TRIBE,
        SELECTED_UNITS,
        BUILDINGS,
        BUILDING_INFO,
        CONSTRUCTION_PROGRESS,
        PRODUCTION_PROGRESS,
        RESOURCE_INFO
    }

    [Header("Hitpoints bar")]
    public GameObject hitpointsPrefab;
    public Image hitpointsBackground;
    public Image hitpointsBar;
    public Text hitpointsText;
    HealthBarUI _healthBar;

    [Header("Production progress")]
    public GameObject productionProgressPrefab;
    public Image productionProgressBarImage;
    public Text productionProgressText;
    public Image productionControllererIcon;

    [Header("Units")]
    public GameObject selectedUnitsPrefab;

    [Header("Stats")]
    public GameObject statsInfoPrefab;

    [HideInInspector]
    public Image[] statIcons = new Image[4];

    [HideInInspector]
    public Text[] statTexts = new Text[4];

    [HideInInspector]
    public ProductionTooltip productionTooltip;

    CONTROLLER_UI_VIEW currentViewType = CONTROLLER_UI_VIEW.NONE;
    CONTROLLER_UI_VIEW lastViewType = CONTROLLER_UI_VIEW.NONE;

    ControllerUIView currentView;
    ControllerUIView lastView;

    [Header("Views")]
    private ControllerUIView nothingSelectedView;
    private VillagerView villagerView;
    private UnitView warriorView;
    private TribeView tribeView;
    private UnitsView selectedUnitsView;
    private BuildingsView buildingsView;
    private BuildingView buildingView;
    private ResourceView resourceView;
    private ConstructionView constructionView;

    [HideInInspector]
    public List<SelectedUnitButton> _selectedUnitButtons = new List<SelectedUnitButton>();

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
        // ...
    }

    void Start () {

        _healthBar = GetComponent<HealthBarUI>();

        foreach (Button btn in villagerButtons)
            btn.gameObject.SetActive(false);

        foreach (Button btn in buildingButtons)
            btn.gameObject.SetActive(false);

        foreach (Button btn in unitActionButtons)
            btn.gameObject.SetActive(false);

        nothingSelectedView = ScriptableObject.CreateInstance<ControllerUIView>();
        villagerView = ScriptableObject.CreateInstance<VillagerView>();
        warriorView = ScriptableObject.CreateInstance<UnitView>();
        tribeView = ScriptableObject.CreateInstance<TribeView>();
        SetupSelectedUnitsView();
        buildingsView = ScriptableObject.CreateInstance<BuildingsView>();
        buildingView = ScriptableObject.CreateInstance<BuildingView>();
        resourceView = ScriptableObject.CreateInstance<ResourceView>();
        constructionView = ScriptableObject.CreateInstance<ConstructionView>();

        productionTooltip = GetComponentInChildren<ProductionTooltip>();

        ChangeView(CONTROLLER_UI_VIEW.NONE, null);
        HideTooltip();
        HideHitpoints();
        HideStats();

        HideProductionTooltip();
    }

    void SetupSelectedUnitsView()
    {
        selectedUnitsView = ScriptableObject.CreateInstance<UnitsView>();

        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 9; i ++)
            {
                CreateUnitSelectedBotton(i, j);
            }
        }
    }

    public void CreateUnitSelectedBotton(int i, int j)
    {
        Button newButton = Button.Instantiate(selectedUnitButton, selectedUnitsPrefab.GetComponent<RectTransform>()) as Button;
        newButton.GetComponent<RectTransform>().localPosition = new Vector3(i * 21f, j * -32f, 0.0f);

        SelectedUnitButton buttonScript = newButton.GetComponent<SelectedUnitButton>();
        buttonScript.Clear();
        _selectedUnitButtons.Add(buttonScript);
    }

    public void SetupProductionButtonsView(ProductionButton[] buttons)
    {
        _productionButtonScripts = buttons;

        HideProductionButtons();
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
            // Only run exit code if we change view
            if(currentViewType != viewType)
            {
                currentView.OnExit();
            }
            
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

            case CONTROLLER_UI_VIEW.WARRIOR:
                currentView = warriorView;
                break;

            case CONTROLLER_UI_VIEW.TRIBE:
                currentView = tribeView;
                break;

            case CONTROLLER_UI_VIEW.SELECTED_UNITS:
                currentView = selectedUnitsView;
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

            case CONTROLLER_UI_VIEW.RESOURCE_INFO:
                currentView = resourceView;
                break;
        }

        currentView.OnEnter(this, controller);
        currentViewType = viewType;

        HideTooltip();

        if(currentViewType == CONTROLLER_UI_VIEW.NONE)
        {
            controllerInfoBackground.GetComponent<Image>().enabled = false;
        }

        else
        {
            controllerInfoBackground.GetComponent<Image>().enabled = true;
        }
    }

    public void GoBackToLastView()
    {
        ChangeView(lastViewType, lastView._controller);
    }

    private void Update()
    {
        currentView.Update();
    }

    public void UpdateTitle(string newTitle)
    {
        title.text = newTitle;
    }

    public void UpdateIcon(Sprite newSprite)
    {
        icon.sprite = newSprite;
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
        _healthBar.UpdateHitpointsAmount(hp, max);
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

    public void ShowProductionTooltip(ProductionButtonData data)
    {
        productionTooltip.gameObject.SetActive(true);
        productionTooltip.UpdateData(data);
    }

    public void HideProductionTooltip()
    {
        productionTooltip.gameObject.SetActive(false);
    }

    public void ShowTooltip(string tip)
    {

        tooltip.enabled = true;
        tooltip.text = tip;
    }

    public void HideTooltip()
    {
        tooltip.enabled = false;
    }

    public void HideSelectedUnitsButtons()
    {
        for(int i = 0; i < _selectedUnitButtons.Count; i++)
        {
            _selectedUnitButtons[i].Clear();
        }
    }

    public void ShowProductionButtons(ProductionButtonData[] data)
    {
        bool[] buttonWithIndexesActivated = new bool[_productionButtonScripts.Length];
        for (int i = 0; i < buttonWithIndexesActivated.Length; i++)
        {
            buttonWithIndexesActivated[i] = false;
        }

        for (int i = 0; i < data.Length; i++)
        {
            if (data != null
                && i < data.Length
                && data[i] != null
                && (data[i].type != PRODUCTION_TYPE.TECHNOLOGY || !Technologies.instance.GetTechnologyCompleted(data[i].title)))
            {
                // Todo make condition for required technology
                _productionButtonScripts[data[i].index].SetData(data[i]);
                _productionButtonScripts[data[i].index].Activate();
                buttonWithIndexesActivated[data[i].index] = true;
            }
        }

        // Deactivate buttons what didn't get activated
        for (int i = 0; i < buttonWithIndexesActivated.Length; i++)
        {
            if(!buttonWithIndexesActivated[i])
            {
                _productionButtonScripts[i].Deactivate();
            }
        }
    }

    public void UpdateProductionButtons()
    {
        for (int i = 0; i < _productionButtonScripts.Length; i++)
        {
            if(_productionButtonScripts[i].isActiveAndEnabled)
            {
                _productionButtonScripts[i].UpdateCanBeProduced();
            }
        }

        foreach (Button btn in buildingButtons)
        {
            ProductionButton _script = btn.gameObject.GetComponent<ProductionButton>();

            if (_script != null)
            {
                _script.UpdateCanBeProduced();
            }
        }
    }

    public void HideProductionButtons()
    {
        for (int i = 0; i < _productionButtonScripts.Length; i++)
        {
            _productionButtonScripts[i].Deactivate();
        }
    }
}
