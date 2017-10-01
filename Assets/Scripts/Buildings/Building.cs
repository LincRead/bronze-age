﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : BaseController {

    // Set to true at:
    // 1. Scene start-up or
    // 2. When Player has successfully placed the building to be constructed
    private bool hasBeenPlaced = false;

    public BuildingStats _buildingStats;

    [HideInInspector]
    public GameObject healthBar;
    HealthBar _healthBar;

    [HideInInspector]
    public float stepsToProduce = 3f;
    private float stepsProduced = 0f;

    [HideInInspector]
    public bool inProductionProcess = false;

    [HideInInspector]
    public bool startedProduction = false;

    protected FinishedProductionAction _finishedProductionAction;

    [HideInInspector]
    public ProductionButtonData[] productionButtonsData;

    // Store current production
    [HideInInspector]

    public
    List<int> productionList = new List<int>();
    int productionIndex = -1;

    // Need this to caulcate efficieny of construction
    // Every number added to this float decreases efficient
    // Reason: avoid lots of Villagers constructing the same building being too efficient
    private float villagersWhoHasDoneActionThisUpdate = 0;

    // Decrease efficient for every Villager constructing Building at the same time
    private float decreaseActionEfficientPerVillager = 0.25f;

    // Don't decrease villagersWhoHasBuiltThisUpdate to less than this value
    private float minVillagerActionEfficiency = 0.25f;

    [HideInInspector]
    public Sprite[] constructionSprites = new Sprite[2];

    [HideInInspector]
    public Sprite damagedSprite;

    [HideInInspector]
    public int maxHitPoints = 200;

    [HideInInspector]
    public int hitpointsLeft = 0;

    // Three stages, 2 is fully constructed
    protected int buildState = 0; 

    [HideInInspector]
    public bool constructed = false;

    // Villager can deliver resources to this building
    [HideInInspector]
    public bool resourceDeliveryPoint = false;

    protected override void Start ()
    {
        _basicStats = _buildingStats;

        base.Start();

        stepsToProduce = _buildingStats.stepsToConstruct;
        constructionSprites = _buildingStats.constructionSprites;
        damagedSprite = _buildingStats.damagedSprite;
        maxHitPoints = _buildingStats.maxHitpoints;
        hitpointsLeft = maxHitPoints;

        // Only set once
        productionButtonsData = _buildingStats.productionButtons;

        // Can deliver resources to this building
        resourceDeliveryPoint = _buildingStats.resourceDeliveryPoint;

        _selectionIndicator.GetComponent<Transform>().localPosition = new Vector3(0.0f, size * 0.08f, 0.0f);

        // I love my daughter Ivy. <3

        // And Kate <3

        SetupHealthBar();
        SetupTeamColor();

        // Buildings in Scene at Scene start-up are placed
        if (WorldManager.firstUpdate)
        {
            Place();
            FinishConstruction();

            // Make sure we show full hitpoints since building is already constructed
            _healthBar.UpdateHitpointsPercent(hitpointsLeft, maxHitPoints);
        }

        else
        {
            SetupBuildingPlacement();
        }
    }

    public void SetNewBuildingStats()
    {
        // Set stats
        title = _buildingStats.title;
        stepsToProduce = _buildingStats.stepsToConstruct;
        constructionSprites = _buildingStats.constructionSprites;
        damagedSprite = _buildingStats.damagedSprite;
        maxHitPoints = _buildingStats.maxHitpoints;
        hitpointsLeft = maxHitPoints;
        visionRange = _buildingStats.visionRange;
        iconSprite = _buildingStats.iconSprite;

        // Update constructed
        _spriteRenderer.sprite = constructionSprites[1];

        // Update damaged art
        UpdateDamagedSprite();
    }

    public void SetRequiredResources(ProductionButtonData data)
    {
        _buildingStats.food = data.food;
        _buildingStats.timber = data.timber;
        _buildingStats.metal = data.metal;
        _buildingStats.newCitizens = data.newCitizens;
    }

    void SetupHealthBar()
    {
        GameObject healthBar = GameObject.Instantiate(_buildingStats.healthBar, _transform.position, Quaternion.identity);
        healthBar.transform.parent = transform;
        _healthBar = healthBar.GetComponent<HealthBar>();
        _healthBar.Init(size);
        _healthBar.SetAlignment(playerID == PlayerManager.myPlayerID);
        _healthBar.UpdateHitpointsPercent(0, (int)stepsToProduce);

        UpdateDamagedSprite();
    }

    void SetupTeamColor()
    {
        if (playerID > -1)
        {
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.instance.playerData[playerID].teamColor);
        }

        else
        {
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.neutralPlayerColor);
        }
    }

    protected virtual void Place()
    {
        hasBeenPlaced = true;

        if(playerID == PlayerManager.myPlayerID)
        {
            SetVisible(true);
        }

        // Position
        Vector3 positionToPlace = Grid.instance.SnapToGrid(_transform.position);
        zIndex = GetMiddleNode().worldPosition.y;
        _transform.position = new Vector3(positionToPlace.x, positionToPlace.y, zIndex);

        // Constructed sprite
        _spriteRenderer.sprite = constructionSprites[0];
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        _spriteRenderer.sortingLayerName = "Object";
        _spriteRenderer.sortingOrder = 2;

        _selectedIndicatorRenderer.color = new Color(1f, 1f, 1f, 1f);
        _selectedIndicatorRenderer.enabled = false;

        Grid.instance.SetTilesOccupiedByController(this);
        PlayerManager.instance.PlacedBuilding(this);

        // Placed during gameplays
        if(!WorldManager.firstUpdate)
        {
            UseResourcesForConstruction(-1);
        }
    }

    protected override void Update()
    {
        if(!constructed)
        {
            if (!hasBeenPlaced)
            {
                HandlePlacingBuilding();
            }

            else
            {
                // Reset
                villagersWhoHasDoneActionThisUpdate = 0;
            }
        }

        else if(inProductionProcess)
        {
            if(startedProduction)
            {
                stepsProduced += 1 * Time.deltaTime;

                if (stepsProduced >= stepsToProduce)
                {
                    FinishedProduction();
                }
            }

            else
            {
                if(HaveRequiredResourcesToProduce())
                {
                    UseResourcesForProduction(-1);
                    startedProduction = true;
                }
            }
        }

        // TEMP! For testing purpose.
        if (selected && Input.GetKeyUp(KeyCode.Delete))
        {
            Kill();
        }
    }

    void SetupBuildingPlacement()
    {
        hasBeenPlaced = false;
        _spriteRenderer.enabled = true;
        _spriteRenderer.sprite = constructionSprites[1];
        _transform.position = PlayerManager.mousePosition - new Vector2(0.0f, _spriteRenderer.bounds.size.y / 2);
        _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        _spriteRenderer.sortingLayerName = "Placing Building";

        _selectedIndicatorRenderer.enabled = true;
        _selectedIndicatorRenderer.color = new Color(1f, 1f, 1f, 0.5f);
    }

    void HandlePlacingBuilding()
    {
        _transform.position = Grid.instance.SnapToGrid(PlayerManager.mousePosition - new Vector2(0.0f, _spriteRenderer.bounds.size.y / 2));

        bool canPlace = false;

        if (!HaveRequiredResourcesToPlaceBuilding())
        {
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            _selectedIndicatorRenderer.color = new Color(1f, 0.5f, 0.0f, 1.0f);
        }

        // Adding offset since building pivot point (buttom of sprite) is in the middle of two nodes
        // Show that location is suitable
        else if (Grid.instance.GetAllTilesFromBoxArEmpty(_transform.position + new Vector3(0.04f, 0.04f), size))
        {
            canPlace = true;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            _selectedIndicatorRenderer.color = new  Color(1f, 1f, 1f, 0.5f);
        }

        // Show that location is not suitable
        else
        {
            _spriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
            _selectedIndicatorRenderer.color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
        }

        // Clicked left mouse button to place building on a suitable location
        if (canPlace
            && Input.GetMouseButtonUp(0) 
            && !CursorHoveringUI.value)
        {
            Place();
        }

        // Cancel placing by clicking right mouse button
        else if (Input.GetMouseButtonUp(1))
        {
            CancelPlacing();
            PlayerManager.instance.CancelPlaceBuildingState();
        }
    }

    public void Build(float amount)
    {
        // Construct
        if(!constructed)
        {
            Construct(amount);
        }

        // Repair
        else if(hitpointsLeft < maxHitPoints)
        {
            Repair(amount);
        }
    }

    void Construct(float buildAmount)
    {
        buildAmount *= (1 - (decreaseActionEfficientPerVillager * villagersWhoHasDoneActionThisUpdate));

        if(buildAmount < minVillagerActionEfficiency)
        {
            buildAmount = minVillagerActionEfficiency;
        }

        stepsProduced += buildAmount * Time.deltaTime;

        hitpointsLeft = (int)(maxHitPoints * GetPercentageProduced());
        _healthBar.UpdateHitpointsPercent((int)stepsProduced, (int)stepsToProduce);
        villagersWhoHasDoneActionThisUpdate++;

        if (stepsProduced >= stepsToProduce)
        {
            _spriteRenderer.sprite = constructionSprites[1];
            FinishConstruction();
        }

        else
        {
            _spriteRenderer.sprite = constructionSprites[0];
        }
    }

    void Repair(float repairAmount)
    {
        repairAmount *= (1 - (decreaseActionEfficientPerVillager * villagersWhoHasDoneActionThisUpdate));

        if (repairAmount < minVillagerActionEfficiency)
        {
            repairAmount = minVillagerActionEfficiency;
        }

        stepsProduced += repairAmount * Time.deltaTime;

        // Repair 2 HP per sec
        if (stepsProduced > 0.5f)
        {
            stepsProduced = 0.0f;
            hitpointsLeft += 1;
            _healthBar.UpdateHitpointsPercent(hitpointsLeft, maxHitPoints);
            UpdateDamagedSprite();
        }

        villagersWhoHasDoneActionThisUpdate++;
    }

    public virtual void FinishConstruction()
    {
        constructed = true;

        _spriteRenderer.sprite = constructionSprites[1];

        hitpointsLeft = maxHitPoints;
        _healthBar.UpdateHitpointsPercent(hitpointsLeft, maxHitPoints);

        AddPlayerStats();

        if(resourceDeliveryPoint && playerID == PlayerManager.myPlayerID)
        {
            PlayerDataManager.instance.GetPlayerData(playerID).friendlyResourceDeliveryPoints.Add(this);
        }

        if (selected)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, this);
        }

        visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(visionRange, GetMiddleNode(), size);

        if (playerID == PlayerManager.myPlayerID)
        {
            IncreaseVisibilityOfTiles();
            UpdateVisibilityOfAllControllerOccupiedTiles();
        }
    }

    public void Produce(int buttonIndex)
    {
        if (!inProductionProcess)
        {
            stepsProduced = 0f;
            productionIndex = buttonIndex;
            stepsToProduce = productionButtonsData[buttonIndex].stepsRequired;
            inProductionProcess = true;

            // Change icon to next production item's
            ControllerUIManager.instance.productionProgressCanvas.icon.sprite = productionButtonsData[buttonIndex].icon;

            if (selected)
            {
                // Update UI
                ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, this);
            }
        }

        else if (productionList.Count < ProductionQueueCanvas.max)
        {
            productionList.Add(buttonIndex);
            UpdateProductionQueue();
        }

        // If technology, then remove technology production button and tooltip
        // Need to store that technology is set in queue, so we can add button again if user cancels production 
        if (productionButtonsData[buttonIndex].type == PRODUCTION_TYPE.TECHNOLOGY)
        {
            ControllerUIManager.instance.DeactivateProductionButton(buttonIndex);
            ControllerUIManager.instance.productionTooltip.gameObject.SetActive(false);
            Technologies.instance.SetTechnologyInQueue(productionButtonsData[buttonIndex].title);
        }
    }

    bool HaveRequiredResourcesToPlaceBuilding()
    {
        PlayerData playerData = PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID);

        return playerData.foodStock >= _buildingStats.food
            && playerData.timber >= _buildingStats.timber
            && playerData.metal >= _buildingStats.metal
            && playerData.population >= _buildingStats.newCitizens;
    }

    public bool HaveRequiredResourcesToProduce()
    {
        return HaveRequiredResourcesToProduce(productionIndex);
    }

    public bool HaveRequiredResourcesToProduce(int productionIndex)
    {
        ProductionButtonData data = productionButtonsData[productionIndex];
        PlayerData playerData = PlayerDataManager.instance.GetPlayerData(PlayerManager.myPlayerID);

        return playerData.foodStock >= data.food
            && playerData.timber >= data.timber
            && playerData.metal >= data.metal
            && playerData.newCitizens >= data.newCitizens;
    }

    void UseResourcesForConstruction(int factor)
    {
        int playerID = PlayerManager.myPlayerID;

        if (_buildingStats.food > 0) PlayerDataManager.instance.AddFoodStockForPlayer(_buildingStats.food * factor, playerID);
        if (_buildingStats.timber > 0) PlayerDataManager.instance.AddTimberForPlayer(_buildingStats.timber * factor, playerID);
        if (_buildingStats.wealth > 0) PlayerDataManager.instance.AddMetalForPlayer(_buildingStats.wealth * factor, playerID);
        if (_buildingStats.metal > 0) PlayerDataManager.instance.AddMetalForPlayer(_buildingStats.metal * factor, playerID);
        if (_buildingStats.newCitizens > 0) PlayerDataManager.instance.AddNewCitizensForPlayer(_buildingStats.newCitizens * factor, playerID);
    }

    void UseResourcesForProduction(int factor)
    {
        int playerID = PlayerManager.myPlayerID;

        ProductionButtonData data = productionButtonsData[productionIndex];

        if (data.food > 0) PlayerDataManager.instance.AddFoodStockForPlayer(data.food * factor, playerID);
        if (data.timber > 0) PlayerDataManager.instance.AddTimberForPlayer(data.timber * factor, playerID);
        if (data.wealth > 0) PlayerDataManager.instance.AddWealthForPlayer(data.wealth * factor, playerID);
        if (data.metal > 0) PlayerDataManager.instance.AddMetalForPlayer(data.metal * factor, playerID);
        if (data.newCitizens > 0) PlayerDataManager.instance.AddNewCitizensForPlayer(data.newCitizens * factor, playerID);
    }

    public void FinishedProduction()
    {
        stepsProduced = 0.0f;
        stepsToProduce = 0.0f;
        inProductionProcess = false;
        startedProduction = false;

        // Only attempt to execute if script has been attached
        if (productionButtonsData[productionIndex].executeScript != null)
        {
            productionButtonsData[productionIndex].executeScript.Action(this);
        }

        if(productionButtonsData[productionIndex].type == PRODUCTION_TYPE.TECHNOLOGY)
        {
            Technologies.instance.CompleteTechnology(productionButtonsData[productionIndex].title);
            ControllerUIManager.instance.UpdateProductionButtons();
        }

        else if(productionButtonsData[productionIndex].type == PRODUCTION_TYPE.UNIT)
        {
            Node spawnToNode = Grid.instance.FindClosestWalkableNode(Grid.instance.GetNodeFromWorldPoint(transform.position + new Vector3(0.0f, (Grid.instance.tileHeight / 4))));
            GameObject newUnit = Instantiate(productionButtonsData[productionIndex].productionPrefab, spawnToNode.worldPosition, Quaternion.identity) as GameObject;
            newUnit.GetComponent<BaseController>().playerID = playerID;
        }

        if (productionList.Count > 0)
        {
            Produce(productionList[0]);
            productionList.RemoveAt(0);
        }

        if (selected)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, this);
        }
    }

    void CancelProduction()
    {
        stepsProduced = 0.0f;
        stepsToProduce = 0.1f;
        inProductionProcess = false;

        // Give back resources
        if (startedProduction)
        {
            UseResourcesForProduction(1);
            startedProduction = false;
        }
        
        // Once off production no longer in queue
        if (productionButtonsData[productionIndex].type == PRODUCTION_TYPE.TECHNOLOGY)
        {
            Technologies.instance.RemoveTechnologyFromQueue(productionButtonsData[productionIndex].title);
        }

        if (productionList.Count > 0)
        {
            Produce(productionList[0]);
            productionList.RemoveAt(0);
            UpdateProductionQueue();
        }

        else
        {
            if (selected)
            {
                ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, this);
            }
        }
    }

    public void RemoveProductionAtQueue(int index)
    {
        productionList.RemoveAt(index);
        UpdateProductionQueue();
    }

    public void UpdateProductionQueue()
    {
        ControllerUIManager.instance.productionQueueCanvas.UpdateData(this);
    }

    public override void Hit(int damageValue)
    {
        hitpointsLeft -= damageValue;

        if (hitpointsLeft <= 0)
        {
            Kill();
        }

        else
        {
            _healthBar.UpdateHitpointsPercent(hitpointsLeft, maxHitPoints);
            UpdateDamagedSprite();
        }
    }

    // Show how damaged the building is visually
    void UpdateDamagedSprite()
    {
        if (damagedSprite != null)
        {
            if ((float)((float)hitpointsLeft / (float)maxHitPoints) < 0.5f)
            {
                _spriteRenderer.sprite = damagedSprite;
            }

            else
            {
                _spriteRenderer.sprite = constructionSprites[1];
            }
        }
    }

    public override void Cancel()
    {
        if(!constructed)
        {
            // Give back resources
            UseResourcesForConstruction(1);

            Kill();
        }

        else if(inProductionProcess)
        {
            CancelProduction();
        }
    }

    // Todo change name
    protected void Kill()
    {
        _healthBar.Deactivate();

        RemovePlayerStats();

        if (_buildingStats.resourceDeliveryPoint && playerID == PlayerManager.myPlayerID)
        {
            PlayerDataManager.instance.GetPlayerData(playerID).friendlyResourceDeliveryPoints.Remove(this);
        }

        // Todo add transition that plays animation before calling Destroy
        Destroy();
    }

    public override void Select()
    {
        base.Select();

        _healthBar.Activate();
    }

    public override void Deselect()
    {
        base.Deselect();

        _healthBar.Deactivate();
    }

    // Unique per building
    protected virtual void AddPlayerStats()
    {

    }

    // Unique per building
    protected virtual void RemovePlayerStats()
    {

    }

    public void CancelPlacing()
    {
        Destroy(gameObject);
    }

    public float GetPercentageProduced()
    {   
        float percent = stepsProduced / stepsToProduce;

        if(percent < 0)
        {
            percent = 0;
        }

        return percent;
    }

    public override void Destroy()
    {
        if (selected)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
        }

        base.Destroy();
    }
}