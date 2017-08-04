using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public Vector2 worldPosition;
    public Grid.FPoint gridPosPoint;
    public int gridPosX;
    public int gridPosY;
    int heapIndex;

    // Reference to parent tile
    public Tile parentTile;

    [HideInInspector]
    public Node parent;

    [HideInInspector]
    public bool walkable = true;

    // Movement cost from the start point to this tile
    public int gCost = 0;

    // Estimated movement cost from this tile to target tile
    public int hCost = 0;

    // TODO REMOVE
    [HideInInspector]
    public UnitStateController unitControllerStandingHere;

    public Node(Vector2 _worldPosition, int _gridPosX, int _gridPosY, Tile tile)
    {
        // Make sure float is always rounded up
        _worldPosition = new Vector3(
            (float)System.Math.Round((double)_worldPosition.x, 2),
            (float)System.Math.Round((double)_worldPosition.y, 2));

        worldPosition = _worldPosition;

        gridPosX = _gridPosX;
        gridPosY = _gridPosY;

        gridPosPoint = new Grid.FPoint(gridPosX, gridPosY);

        this.parentTile = tile;
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

    public int CompareTo(Node tileToCompare)
    {
        int compare = fCost.CompareTo(tileToCompare.fCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(tileToCompare.hCost);
        }

        return -compare;
    }
}
