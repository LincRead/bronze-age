using UnityEngine;
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
    public float stepsToConstruct = 3f;
    private float stepsConstructed = 0f;

    // Need this to caulcate efficieny of construction
    // Every number added to this float decreases efficient
    // Reason: avoid lots of Villagers constructing the same building being too efficient
    private float villagersWhoHasDoneActionThisUpdate = 0;

    // Decrease efficient for every Villager constructing Building at the same time
    private float decreaseActionEfficientPerVillager = 0.25f;

    // Don't decrease villagersWhoHasBuiltThisUpdate to less than this value
    private float minVillagerActionEfficiency = 0.25f;

    [HideInInspector]
    public Sprite[] constructionSprites = new Sprite[3];

    [HideInInspector]
    public Sprite[] damagedSprites = new Sprite[2];

    [HideInInspector]
    public int maxHitPoints = 200;

    [HideInInspector]
    public int hitpointsLeft = 0;

    [HideInInspector]
    public int visionRange;

    // Three stages, 2 is fully constructed.
    protected int buildState = 0; 

    [HideInInspector]
    public bool constructed = false;

    // Use this for initialization
    protected override void Start ()
    {
        _basicStats = _buildingStats;

        base.Start();

        // Set stats
        stepsToConstruct = _buildingStats.stepsToConstruct;
        constructionSprites = _buildingStats.constructionSprites;
        damagedSprites = _buildingStats.damagedSprites;
        maxHitPoints = _buildingStats.maxHitpoints;
        hitpointsLeft = maxHitPoints;
        visionRange = _buildingStats.visionRange;

        // Buildings in Scene at Scene start-up are placed
        if (WorldManager.firstUpdate)
        {
            Place();
            FinishConstruction();
        }

        else
        {
            SetupBuildingPlacement();
        }

        // I love my daughter Ivy. <3

        // And Kate <3

        SetupHealthBar();
        SetupTeamColor();
    }
    
    void SetupHealthBar()
    {
        GameObject healthBar = GameObject.Instantiate(_buildingStats.healthBar, _transform.position, Quaternion.identity);
        _healthBar = healthBar.GetComponent<HealthBar>();
        _healthBar.Init(size);
        _healthBar.SetAlignment(playerID == PlayerManager.myPlayerID);
        _healthBar.UpdateHitpointsAmount(hitpointsLeft, maxHitPoints);
        UpdateDamagedSprite();
    }

    void SetupTeamColor()
    {
        if (playerID > -1)
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.instance.playerData[playerID].teamColor);
        else
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.neutralPlayerColor);
    }

    public void Place()
    {
        hasBeenPlaced = true;

        // Position
        Vector3 positionToPlace = Grid.instance.SnapToGrid(_transform.position);
        zIndex = _transform.position.y + size * (Grid.instance.tileHeight / 2);
        _transform.position = new Vector3(positionToPlace.x, positionToPlace.y, zIndex);

        // Constructed sprite
        _spriteRenderer.sprite = constructionSprites[0];
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        _spriteRenderer.sortingLayerName = "Object";

        _selectedIndicatorRenderer.color = new Color(1f, 1f, 1f, 1f);
        _selectedIndicatorRenderer.enabled = false;

        Grid.instance.SetTilesOccupiedByController(this);
        CheckTileAndSetVisibility();
        PlayerManager.instance.PlacedBuilding(this);
    }

    // Update is called once per frame
    protected override void Update ()
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

    void SetupBuildingPlacement()
    {
        hasBeenPlaced = false;
        _spriteRenderer.sprite = constructionSprites[2];
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

        // Adding offset since building pivot point (buttom of sprite) is in the middle of two nodes
        // Show that location is suitable
        if (Grid.instance.GetAllTilesFromBoxArEmpty(_transform.position + new Vector3(0.04f, 0.04f), size))
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

        stepsConstructed += buildAmount * Time.deltaTime;

        if (stepsConstructed >= stepsToConstruct / 2)
        {
            _spriteRenderer.sprite = constructionSprites[1];

            if (stepsConstructed >= stepsToConstruct)
            {
                FinishConstruction();
            }
        }

        villagersWhoHasDoneActionThisUpdate++;
    }

    void Repair(float repairAmount)
    {
        repairAmount *= (1 - (decreaseActionEfficientPerVillager * villagersWhoHasDoneActionThisUpdate));

        if (repairAmount < minVillagerActionEfficiency)
        {
            repairAmount = minVillagerActionEfficiency;
        }

        stepsConstructed += repairAmount * Time.deltaTime;

        // Repair 2 HP per sec
        if (stepsConstructed > 0.5f)
        {
            stepsConstructed = 0.0f;
            hitpointsLeft += 1;
            _healthBar.UpdateHitpointsAmount(hitpointsLeft, maxHitPoints);
            UpdateDamagedSprite();
        }

        villagersWhoHasDoneActionThisUpdate++;
    }

    protected virtual void FinishConstruction()
    {
        constructed = true;
        _spriteRenderer.sprite = constructionSprites[2];

        AddPlayerStats();

        if(playerID == PlayerManager.myPlayerID)
        {
            SetVisibility();
        }

        if (selected)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, this);
        }
    }

    public void SetVisibility()
    {
        Node currentNode = GetMiddleNode();
        List<Tile> visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(visionRange, currentNode);

        if (!currentNode.parentTile.traversed)
        {
            for (int i = 0; i < visibleTiles.Count; i++)
            {
                visibleTiles[i].SetExplored();
            }

            currentNode.parentTile.traversed = true;
        }
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
            _healthBar.UpdateHitpointsAmount(hitpointsLeft, maxHitPoints);
            UpdateDamagedSprite();
        }
    }

    // Show how damaged the building is visually
    void UpdateDamagedSprite()
    {
        if (damagedSprites.Length == 2 && damagedSprites[0] != null)
        {
            if ((float)((float)hitpointsLeft / (float)maxHitPoints) < 0.33f)
            {
                _spriteRenderer.sprite = damagedSprites[1];
            }

            else if ((float)((float)hitpointsLeft / (float)maxHitPoints) < 0.66f)
            {
                _spriteRenderer.sprite = damagedSprites[0];
            }

            else
            {
                _spriteRenderer.sprite = constructionSprites[2];
            }
        }
    }

    // Todo change name
    protected void Kill()
    {
        _healthBar.Deactivate();

        RemovePlayerStats();

        Grid.instance.RemoveTilesOccupiedByController(this);

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

    public float GetPercentageConstructed()
    {
        return stepsConstructed / stepsToConstruct;
    }

    public virtual void Destroy()
    {
        if (selected)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
        }

        Destroy(gameObject);
    }
}
