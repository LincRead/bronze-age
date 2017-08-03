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
            _transform.position = WorldManager.instance.GetMousePosition() - new Vector2(0.0f, _spriteRenderer.bounds.size.y / 2);
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
        Vector3 positionToPlace = WorldManager.instance._grid.SnapToGrid(_transform.position);
        zIndex = _transform.position.y + size * (WorldManager.instance._grid.tileHeight / 2);
        _spriteRenderer.sprite = constructionSprites[0];
        _transform.position = new Vector3(positionToPlace.x, positionToPlace.y, zIndex);
        _spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        _spriteRenderer.sortingLayerName = "Object";
        WorldManager.instance.AddBuildingReference(this);
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
            UnitUIManager.instance.UpdateConstructionProgressElements(this, GetPercentageConstructed());
        }
	}

    void HandlePlacingBuilding()
    {
        bool canPlace = false;

        _transform.position = WorldManager.instance.GetGrid().SnapToGrid(Camera.main.ScreenToWorldPoint(Input.mousePosition) - new Vector3(0.0f, _spriteRenderer.bounds.size.y / 2));

        // Adding offset since building pivot point (buttom of sprite) is in the middle of two nodes.
        if (WorldManager.instance.GetGrid().GetAllTilesFromBoxArEmpty(_transform.position + new Vector3(0.04f, 0.04f), size))
        {
            canPlace = true;
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.5f);
        }

        else
        {
            _spriteRenderer.color = new Color(1.0f, 0.5f, 0.5f, 0.5f);
        }

        if (Input.GetMouseButtonUp(0) && !WorldManager.instance._cursorHoveringUI.IsCursorHoveringUI())
        {
            if (canPlace)
            {
                Place();
                WorldManager.instance.StopBuildPlacementState(this);
            }
        }

        else if (Input.GetMouseButtonUp(1))
        {
            Destroy(gameObject);
            WorldManager.instance.StopBuildPlacementState(null);
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

    public float GetPercentageConstructed()
    {
        return stepsConstructed / stepsToConstruct;
    }

    protected virtual void FinishConstruction()
    {
        constructed = true;
        _spriteRenderer.sprite = constructionSprites[2];

        if (selected)
            UnitUIManager.instance.ShowBuildingUI(this);

        AddPlayerStats();
    }

    protected virtual void AddPlayerStats()
    {

    }

    protected virtual void RemovePlayerStats()
    {

    }

    public virtual void Destroy()
    {
        WorldManager.instance.RemoveBuildingReference(this);
        RemovePlayerStats();
        Destroy(gameObject);
    }
}
