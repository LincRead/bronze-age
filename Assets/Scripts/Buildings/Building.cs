using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : BaseController {

    protected int buildState = 0; // Three stages, 2 is fully constructed.

    public GameObject healthBar;
    HealthBar _healthBar;

    [Header("Construction")]

    [SerializeField]
    bool hasBeenPlaced = false;

    [SerializeField]
    public Sprite[] constructionSprites = new Sprite[3];

    [SerializeField]
    public Sprite[] damagedSprites = new Sprite[2];

    public float stepsToConstruct = 3f;
    private float stepsConstructed = 0f;

    [Header("Building stats")]
    public int maxHitPoints = 200;

    [HideInInspector]
    public int hitpointsLeft = 0;

    [HideInInspector]
    public bool constructed = false;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        if (!hasBeenPlaced)
        {
            _spriteRenderer.sprite = constructionSprites[2];
            _transform.position = PlayerManager.mousePosition - new Vector2(0.0f, _spriteRenderer.bounds.size.y / 2);
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
            _spriteRenderer.sortingLayerName = "Placing Building";

            _selectedIndicatorRenderer.enabled = true;
            _selectedIndicatorRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        } // I love my daughter Ivy. <3

        else
        {
            Place();
            FinishConstruction();
        }

        // And Kate <3

        // Setup health
        hitpointsLeft = maxHitPoints;
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.Init(size);
        _healthBar.SetAlignment(playerID == PlayerManager.myPlayerID);
        _healthBar.UpdateHitpointsAmount(hitpointsLeft, maxHitPoints);
        UpdateDamagedSprite();

        SetupTeamColor();
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

    public void Build(float buildAmount)
    {
        // Construct
        if(!constructed)
        {
            stepsConstructed += buildAmount * Time.deltaTime;

            if (stepsConstructed >= stepsToConstruct / 2)
            {
                _spriteRenderer.sprite = constructionSprites[1];

                if (stepsConstructed >= stepsToConstruct)
                {
                    FinishConstruction();
                }
            }
        }

        // Repair
        else if(hitpointsLeft < maxHitPoints)
        {
            stepsConstructed += buildAmount * Time.deltaTime;

            // Repair 2 HP per sec
            if(stepsConstructed > 0.5f)
            {
                stepsConstructed = 0.0f;
                hitpointsLeft += 1;
                _healthBar.UpdateHitpointsAmount(hitpointsLeft, maxHitPoints);
                UpdateDamagedSprite();
            }
        }
    }

    protected virtual void FinishConstruction()
    {
        constructed = true;
        _spriteRenderer.sprite = constructionSprites[2];

        AddPlayerStats();
        SetVisibility();

        if (selected)
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.BUILDING_INFO, this);
    }

    public void SetVisibility()
    {
        Node currentNode = GetPrimaryNode();
        List<Tile> visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(10, currentNode);

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
