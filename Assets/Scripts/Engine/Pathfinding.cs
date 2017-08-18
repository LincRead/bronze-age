using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    [HideInInspector]
    public List<Node> path = new List<Node>();

    protected Grid grid = null;

    [HideInInspector]
    public Node currentStandingOnNode;

    protected Node startNode;
    protected Node destinationNode;
    protected UnitStateController _parentController;

    [HideInInspector]
    public List<UnitStateController> unitsToAvoid = new List<UnitStateController>();

    [HideInInspector]
    public List<Tile> limitSearchToTiles = new List<Tile>();

    [HideInInspector]
    public bool enteredNewNode = false;

    void Start()
    {

    }

    public void AddUnit(UnitStateController unit)
    {
        SetGridReference();
        _parentController = unit;
        DetectCurrentPathfindingNode(unit._transform.position);
        currentStandingOnNode.unitControllerStandingHere = unit;
    }

    public void SetGridReference()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("Grid");

        if (gridObj != null)
            grid = gridObj.GetComponent<Grid>();
    }

    public void DetectCurrentPathfindingNode(Vector3 pos)
    {
        Node node = grid.GetNodeFromWorldPoint(pos);

        // Outside grid
        if (node == null)
        {
            Debug.LogError(name + " is standing outside of the Grid");
            return;
        }

        currentStandingOnNode = node;
        currentStandingOnNode.unitControllerStandingHere = _parentController;
        currentStandingOnNode.parentTile.unitsStandingHere.Add(_parentController);
    }

    public void SetCurrentPathfindingNode(Node node)
    {
        if(currentStandingOnNode.parentTile != node.parentTile)
        {
            currentStandingOnNode.parentTile.unitsStandingHere.Remove(_parentController);
            node.parentTile.unitsStandingHere.Add(_parentController);
        }

        currentStandingOnNode.unitControllerStandingHere = null;
        currentStandingOnNode = node;
        currentStandingOnNode.unitControllerStandingHere = _parentController;
    }

    void UpdateUnitStandingOnTile()
    {

    }

    public Node GetNodeFromPoint(Vector3 pos)
    {
        return grid.GetNodeFromWorldPoint(pos);
    }

    public Node GetNodeFromGridPos(int x, int y)
    {
        return grid.GetNodeFromGridPos(x, y);
    }

    public void FindPath()
    {
        if (destinationNode == null)
            return;

        FindPath(destinationNode.worldPosition);
    }

    public void FindPath(Node endNode)
    {
        path.Clear();

        if (!currentStandingOnNode.walkable)
        {
            currentStandingOnNode.walkable = true;
            FindPath(endNode.worldPosition);
            currentStandingOnNode.walkable = false;
        }

        else
            FindPath(endNode.worldPosition);
    }

    public void FindPath(Vector2 endPos)
    {
        destinationNode = grid.GetNodeFromWorldPoint(endPos);
        if (destinationNode == null)
            return;

        startNode = currentStandingOnNode;
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == destinationNode)
            {
                path = RetracePath(startNode, destinationNode);
                return;
            }

            List<Node> nodesToCheck = grid.GetNeighbourNodes(currentNode);
            foreach (Node neighbour in nodesToCheck)
            {
                if (!neighbour.walkable
                    || AvoidUnit(neighbour)
                    || (limitSearchToTiles.Count > 0 && !limitSearchToTiles.Contains(neighbour.parentTile))
                    || closedSet.Contains(neighbour))
                    continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistanceBetweenNodes(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistanceBetweenNodes(neighbour, destinationNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    bool AvoidUnit(Node node)
    {
        if(node != destinationNode 
            && node.unitControllerStandingHere != null 
            && !node.unitControllerStandingHere.isMoving)
            return true;

        for(int i = 0; i < unitsToAvoid.Count; i++)
        {
            if (node.unitControllerStandingHere == unitsToAvoid[i])
                return true;
        }

        return false;
    }

    List<Node> RetracePath(Node startNode, Node endNode)
    {
        grid.pathsToDebug.Remove(path);
        path.Clear();

        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        grid.pathsToDebug.Add(path);

        return path;
    }

    int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
        int distY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}