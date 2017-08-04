using UnityEngine;
using System.Collections;

public class Building : BaseController {

    protected int buildState = 0; // Three stages, 2 is fully constructed.

    [Header("Construction")]

    [SerializeField]
    bool hasBeenPlaced = false;

    [SerializeField]
    public Sprite[] constructionSprites = new Sprite[3];

    public float stepsToConstruct = 3f;
    private float stepsConstructed = 0f;

    [Header("Building stats")]
    public int maxHitPoints = 200;

    [HideInInspector]
    public int hitpoints = 0;

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
        } // I love my daughter Ivy. <3

        else
        {
            Place();
            FinishConstruction();
        }

        // And Kate <3

        hitpoints = maxHitPoints;
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

        Grid.instance.SetTilesOccupiedByController(this);
        PlayerManager.instance.PlacedBuilding(this);
    }

    // Update is called once per frame
    protected override void Update ()
    {
        if (!hasBeenPlaced)
        {
            HandlePlacingBuilding();
        }
            
        else if(selected && !constructed)
        {
            UnitUIManager.instance.UpdateConstructionProgressElements(this, CalculatePercentageConstructed());
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
        }

        // Show that location is not suitable
        else
        {
            _spriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
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
        stepsConstructed += buildAmount * Time.deltaTime;

        if(stepsConstructed >= stepsToConstruct / 2)
        {
            _spriteRenderer.sprite = constructionSprites[1];

            if (stepsConstructed >= stepsToConstruct)
            {
                FinishConstruction();
            }
        }
    }

    public float CalculatePercentageConstructed()
    {
        return stepsConstructed / stepsToConstruct;
    }

    protected virtual void FinishConstruction()
    {
        constructed = true;
        _spriteRenderer.sprite = constructionSprites[2];

        AddPlayerStats();

        if (selected)
            UnitUIManager.instance.ShowBuildingUI(this);
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

    public virtual void Destroy()
    {
        Grid.instance.RemoveTilesOccupiedByController(this);
        RemovePlayerStats();
        Destroy(gameObject);
    }
}
