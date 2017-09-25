using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : IHeapItem<Tile>
{
    [HideInInspector]
    public Tile parent;

    [HideInInspector]
    public SpriteRenderer _tileSpriteRenderer;

    // Reference
    Grid grid;

    public Vector2 worldPosition;
    public Grid.FPoint gridPosPoint;
    public int gridPosX;
    public int gridPosY;
    int heapIndex;

    // Nodes reference
    [HideInInspector]
    public Node[] nodes = new Node[4];

    // Movement cost from the start point to this node
    public int gCost = 0;

    // Estimated movement cost from this node to target node
    public int hCost = 0;

    public GameObject tilePrefab;

    GameObject _tile;

    [HideInInspector]
    public bool walkable = true;

    [HideInInspector]
    public List<UnitStateController> unitsStandingHere = new List<UnitStateController>();

    [HideInInspector]
    public BaseController controllerOccupying = null;

    [HideInInspector]
    public float fertility = 0;

    [HideInInspector]
    public bool fertilityLocked = false;

    [HideInInspector]
    public bool explored = false;

    [HideInInspector]
    public int visibleForControllerCount = 0;

    public Tile(bool _walkable, Vector2 _worldPosition, int _gridPosX, int _gridPosY, Grid grid)
    {
        walkable = _walkable;

        worldPosition = _worldPosition;

        gridPosX = _gridPosX;
        gridPosY = _gridPosY;

        gridPosPoint = new Grid.FPoint(gridPosX, gridPosY);

        this.grid = grid;
    }

    public void CreateTile()
    {
        int tileIndex = 0;

        if (fertility > 25)
        {
            tileIndex = 3;
        }
            
        else if (fertility > 16)
        {
            tileIndex = 2;
        }
            
        else if (fertility > 0)
        {
            tileIndex = 1;
        }

        _tile = GameObject.Instantiate(Resources.Load("Tile"), worldPosition, Quaternion.identity) as GameObject;
        _tileSpriteRenderer = _tile.GetComponent<SpriteRenderer>();
        _tileSpriteRenderer.sprite = Grid.instance.tileSprites[tileIndex];
        _tile.transform.SetParent(grid.transform);
        _tileSpriteRenderer.color = Color.black;

        // Spawn Metal?
        if (Grid.instance.GetAllTilesFromBoxArEmpty(this, 2))
        {
            float spawnValue = 0.0f;
            if (tileIndex == 0) spawnValue = 0.05f;
            if (tileIndex == 1) spawnValue = 0.01f;
            if (Random.value < spawnValue)
            {
                grid.SpawnMetal(this);
                Grid.instance.SetTilesOccupied(this, 2);
            }
        }

        // Spawn obsidian?
        if (Grid.instance.GetAllTilesFromBoxArEmpty(this, 2))
        {
            float spawnValue = 0.0f;
            if (tileIndex == 0) spawnValue = 0.05f;
            if (tileIndex == 1) spawnValue = 0.01f;
            if (Random.value < spawnValue)
            {
                grid.SpawnObsidian(this);
                Grid.instance.SetTilesOccupied(this, 2);
            }
        }

        // Spawn tree?
        if (walkable)
        {
            float spawnValue = 0.0f;

            if (tileIndex == 0) spawnValue = 0.02f;
            if (tileIndex == 1) spawnValue = 0.05f;
            if (tileIndex == 2) spawnValue = 0.25f;
            if (tileIndex == 3) spawnValue = 0.5f;

            if (Random.value < spawnValue)
            {
                grid.SpawnTree(this);
                Grid.instance.SetTilesOccupied(this, 1);
            }
        }

        // Spawn fruit bush?
        if (walkable)
        {
            float spawnValue = 0.0f;
            if (tileIndex != 0) spawnValue = 0.01f;
            if (Random.value < spawnValue)
            {
                grid.SpawnFruitBush(this);
                Grid.instance.SetTilesOccupied(this, 1);
            }  
        }
    }

    public void SetWalkable()
    {
        walkable = true;
        nodes[0].walkable = true;
        nodes[1].walkable = true;
        nodes[2].walkable = true;
        nodes[3].walkable = true;
    }

    public void SetUnwalkable()
    {
        walkable = false;
        nodes[0].walkable = false;
        nodes[1].walkable = false;
        nodes[2].walkable = false;
        nodes[3].walkable = false;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Tile nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }

    public List<UnitStateController> GetUnitsStandingOnTile()
    {
        return unitsStandingHere;
    }

    public void ChangeVisibilityCount(int value)
    {
        visibleForControllerCount += value;

        if (value > 0)
        {
            explored = true;
        }
    }

    public void UpdateVisibilityOfTileAndControllers()
    {
        // Tell controller standing here to update visibility of all tiles it occupies
        if (controllerOccupying != null)
        {
            controllerOccupying.UpdateVisibilityOfAllControllerOccupiedTiles();
        }

        // Just update thus tile based on visibility count
        else
        {
            SetVisible(visibleForControllerCount > 0);
        }

        // Update visibility of units standing on this tile based on tile's visibility
        for(int i = 0; i < unitsStandingHere.Count; i++)
        {
            unitsStandingHere[i].SetVisible(visibleForControllerCount > 0);
        }
    }

    public void SetVisible(bool visible)
    {
        if (visible)
        {
            explored = true;
            _tileSpriteRenderer.color = Color.white;
        }

        else if (explored)
        {
            _tileSpriteRenderer.color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }

        else
        {
            _tileSpriteRenderer.color = Color.black;
        }
    }


    public bool IsEmpty()
    {
        return walkable && unitsStandingHere.Count == 0;
    }
}
