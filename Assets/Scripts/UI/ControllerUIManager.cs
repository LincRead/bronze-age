﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class ControllerUIManager : MonoBehaviour {

	[HideInInspector]
	public Canvas _canvas;

    [Header("Controller info")]
    public Text title;
    public Text description;
    public Image icon;
    public Text actionTooltip;
	public Text baseControllerTooltip;
    public GameObject infoBackground;

    [Header("Buttons")]
    public Button[] villagerButtons;
    public Button[] buildingButtons;
    public Button[] unitActionButtons;

    public Button selectedUnitButton;
    public Button rallyPointButton;

    protected ProductionButton[] _productionButtonScripts;

    [HideInInspector]
    public enum CONTROLLER_UI_VIEW
    {
        NONE,
        TILE,
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

    [HideInInspector]
    public ProductionProgressCanvas productionProgressCanvas;

    [HideInInspector]
    public ProductionQueueCanvas productionQueueCanvas;

	// Want to fetch this from Tech Tree
	public ProductionButtonsCanvas productionButtonsCanvas;

    [HideInInspector]
    public HealthBarUI healthBar;

    [Header("Units")]
    public GameObject selectedUnitsPrefab;

    [Header("Stats")]
    public GameObject statsInfoPrefab;

    public Image[] statIcons = new Image[6];
    public Text[] statTexts = new Text[6];

    [HideInInspector]
    public ProductionTooltip productionTooltip;

    [HideInInspector]
    public ResourceTooltip resourceTooltip;

    public CONTROLLER_UI_VIEW currentViewType = CONTROLLER_UI_VIEW.NONE;
    CONTROLLER_UI_VIEW lastViewType = CONTROLLER_UI_VIEW.NONE;

    ControllerUIView currentView;
    ControllerUIView lastView;

    [Header("Views")]
    public ConstructionView constructionView;

    private ControllerUIView nothingSelectedView;
    private TileView tileView;
    private VillagerView villagerView;
    private UnitView warriorView;
    private TribeView tribeView;
    private UnitsView selectedUnitsView;
    private BuildingsView buildingsView;
    private BuildingView buildingView;
    private ResourceView resourceView;

    [HideInInspector]
    public List<SelectedUnitButton> _selectedUnitButtons = new List<SelectedUnitButton>();

    [Header("Resource icons")]
    public Sprite foodIcon;
    public Sprite woodIcon;
    public Sprite metalIcon;
    public Sprite wealthIcon;

    [Header("Stat icons")]
    public Sprite attackIcon;
    public Sprite attackSiegeIcon;
    public Sprite movementSpeedIcon;
    public Sprite pierceArmorIcon;
    public Sprite meleeArmorIcon;
    public Sprite rangeIcon;

    private static ControllerUIManager controllerUIManager;

    [Header("Cursor information")]
    public MouseCursorInfo cursorInformation;

    [Header("Team colour shader")]
    public Material teamColorMaterial;

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
        teamColorMaterial.SetColor("_MyTeamColor", PlayerDataManager.instance.playerData[PlayerManager.myPlayerID].teamColor);
    }

    void Start ()
    {
		_canvas = GetComponent<Canvas> ();

        foreach (Button btn in villagerButtons)
            btn.gameObject.SetActive(false);

        foreach (Button btn in buildingButtons)
            btn.gameObject.SetActive(false);

        foreach (Button btn in unitActionButtons)
            btn.gameObject.SetActive(false);

        // Only show in special cases, eg. Tile
        description.enabled = false;

        // Disabled as default
        rallyPointButton.gameObject.SetActive(false);

        nothingSelectedView = ScriptableObject.CreateInstance<ControllerUIView>();
        tileView = ScriptableObject.CreateInstance<TileView>();
        villagerView = ScriptableObject.CreateInstance<VillagerView>();
        warriorView = ScriptableObject.CreateInstance<UnitView>();
        tribeView = ScriptableObject.CreateInstance<TribeView>();
        SetupSelectedUnitsView();
        buildingsView = ScriptableObject.CreateInstance<BuildingsView>();
        buildingView = ScriptableObject.CreateInstance<BuildingView>();
        resourceView = ScriptableObject.CreateInstance<ResourceView>();

        // Init production tooltip
        productionTooltip = GetComponentInChildren<ProductionTooltip>();
        HideProductionTooltip();

        // Init resource tooltip
        resourceTooltip = GetComponentInChildren<ResourceTooltip>();
        HideResourceTooltip();

        productionProgressCanvas = GetComponentInChildren<ProductionProgressCanvas>();
        productionProgressCanvas.gameObject.SetActive(false);

        productionQueueCanvas = GetComponentInChildren<ProductionQueueCanvas>();
        productionQueueCanvas.gameObject.SetActive(false);

        healthBar = GetComponentInChildren<HealthBarUI>();

        ChangeView(CONTROLLER_UI_VIEW.NONE, null);
        HideActionTooltip();
		HideBaseControllerTooltip();
        healthBar.HideHitpoints();
        HideStats();

        cursorInformation.gameObject.SetActive(false);
    }

    void SetupSelectedUnitsView()
    {
        selectedUnitsView = ScriptableObject.CreateInstance<UnitsView>();

        for (int j = 0; j < 2; j++)
        {
            for (int i = 0; i < 11; i ++)
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

    // Don't run OnExit unless view actually changes.
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

            case CONTROLLER_UI_VIEW.TILE:
                currentView = tileView;
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

        HideActionTooltip();

        // Disabled as default
        rallyPointButton.gameObject.SetActive(false);

        currentView.OnEnter(this, controller);
        currentViewType = viewType;

        if(currentViewType == CONTROLLER_UI_VIEW.NONE)
        {
            infoBackground.GetComponent<Image>().enabled = false;
        }

        else
        {
            infoBackground.GetComponent<Image>().enabled = true;
        }
    }

    public void ResetView(BaseController controller)
    {
        if(currentView != null)
        {
            currentView.OnEnter(this, controller);
        }
    }

    public void ChangeAndResetView(CONTROLLER_UI_VIEW viewType, BaseController controller)
    {
        // Make sure OnExit is also run when restarting.
        currentView.OnExit();

        currentViewType = viewType;

        ChangeView(viewType, controller);
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

	public void ShowStats(Sprite[] icons, int[] stats, string[] descriptions)
    {
        statsInfoPrefab.gameObject.SetActive(true);

        for (int i = 0; i < 6; i++)
        {
            if (i < stats.Length && stats[i] != -1)
            {
                statIcons[i].enabled = true;
                statIcons[i].sprite = icons[i];
                
				statIcons[i].GetComponent<UnitUIButton>().tooltip = descriptions[i].ToString();

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
        for (int i = 0; i < 6; i++)
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

    public void ShowProductionTooltip(ProductionButtonData data, string description)
    {
        productionTooltip.gameObject.SetActive(true);
        productionTooltip.UpdateData(data, description);
    }

    public void HideProductionTooltip()
    {
        productionTooltip.gameObject.SetActive(false);
    }

    public void ShowResourceTooltip(string tip)
    {
        resourceTooltip.gameObject.SetActive(true);
        resourceTooltip.UpdateData(tip);
    }

    public void HideResourceTooltip()
    {
        resourceTooltip.gameObject.SetActive(false);
    }

    public void ShowActionTooltip(string tip)
    {
        actionTooltip.enabled = true;
        actionTooltip.text = tip;
        actionTooltip.color = Color.white;
    }

    public void HideActionTooltip()
    {
        actionTooltip.enabled = false;
    }

	public void ShowBaseControllerTooltip(string tip)
	{
		baseControllerTooltip.enabled = true;
		baseControllerTooltip.text = tip;
		baseControllerTooltip.color = Color.white;
	}

	public void HideBaseControllerTooltip()
	{
		baseControllerTooltip.enabled = false;
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
                && data[i] != null)
            {
                // Todo make condition for required technology
                _productionButtonScripts[data[i].position].index = i;
                _productionButtonScripts[data[i].position].SetData(data[i]);
                _productionButtonScripts[data[i].position].Activate();
                buttonWithIndexesActivated[data[i].position] = true;
            }
        }

        // Deactivate buttons that didn't get activated
        for (int i = 0; i < buttonWithIndexesActivated.Length; i++)
        {
            if(!buttonWithIndexesActivated[i])
            {
                _productionButtonScripts[i].Deactivate();
            }
        }
    }

    public void DeactivateProductionButton(int index)
    {
        _productionButtonScripts[index].Deactivate();
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
