using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : IHeapItem<Tile>
{
    [HideInInspector]
    public Tile parent;

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
    public bool traversed = false;

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
            tileIndex = 3;
        else if (fertility > 16)
            tileIndex = 2;
        else if (fertility > 0)
            tileIndex = 1;

        _tile = GameObject.Instantiate(Resources.Load("Tile"), worldPosition, Quaternion.identity) as GameObject;

        if(explored || traversed)
            _tile.GetComponent<SpriteRenderer>().color = Color.white;
        else
            _tile.GetComponent<SpriteRenderer>().color = Color.black;

        _tile.GetComponent<SpriteRenderer>().sprite = Grid.instance.tileSprites[tileIndex];
        _tile.transform.SetParent(grid.transform);

        // Spawn tree?
        if (walkable)
        {
            float spawnValue = 0.0f;
            if (tileIndex == 0) spawnValue = 0.005f;
            if (tileIndex == 1) spawnValue = 0.01f;
            if (tileIndex == 2) spawnValue = 0.05f;
            if (tileIndex == 3) spawnValue = 0.3f;
            if (Random.value < spawnValue)
            {
                grid.SpawnTree(this);
            }
        }

        // Spawn Stone?
        if (walkable)
        {
            float spawnValue = 0.0f;
            if (tileIndex == 0) spawnValue = 0.05f;
            if (tileIndex == 1) spawnValue = 0.01f;
            if (Random.value < spawnValue)
            {
                if (grid.GetAllTilesFromBoxArEmpty(worldPosition, 2))
                {
                    grid.SpawnStone(this);
                }
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

    public void SetUnitsStandingOnTileAsVisible()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].unitControllerStandingHere != null)
                nodes[i].unitControllerStandingHere._spriteRenderer.enabled = true;
        }
    }

    public List<UnitStateController> GetUnitsStandingOnTile()
    {
        return unitsStandingHere;
    }

    // Set everything on tile to visible
    public void SetExplored()
    {
        explored = true;

        if (_tile)
            _tile.GetComponent<SpriteRenderer>().color = Color.white;

        if (controllerOccupying)
            controllerOccupying._spriteRenderer.enabled = true;

        SetUnitsStandingOnTileAsVisible();
    }

    public bool IsEmpty()
    {
        return walkable && unitsStandingHere.Count == 0;
    }
}
