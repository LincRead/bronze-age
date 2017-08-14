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
    protected BaseController parentController;

    [HideInInspector]
    public List<UnitStateController> unitsToAvoid = new List<UnitStateController>();

    [HideInInspector]
    public bool enteredNewNode = false;

    void Start()
    {
        parentController = transform.GetComponent<BaseController>();
    }

    public void AddUnit(UnitStateController unit)
    {
        SetGridReference();
        DetectCurrentPathfindingNode(unit._transform.position);
        currentStandingOnNode.unitControllerStandingHere = unit;
    }

    public void SetGridReference()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("Grid");

        if (gridObj != null)
            grid = gridObj.GetComponent<Grid>();
    }

    public Node DetectCurrentPathfindingNode(Vector3 pos)
    {
        Node node = grid.GetNodeFromWorldPoint(pos);

        // Outside grid
        if (node == null)
        {
            Debug.LogError(name + " is standing outside of the Grid");
            return null;
        }

        // Standing on another node than currently stored
        if (currentStandingOnNode != node)
        {
            if (currentStandingOnNode != null
                && currentStandingOnNode.unitControllerStandingHere == parentController)
                currentStandingOnNode.unitControllerStandingHere = null;

            // Store the node this controller is currently standing on
            currentStandingOnNode = node;

            enteredNewNode = true;
        }

        else
            enteredNewNode = false;

        if (currentStandingOnNode.unitControllerStandingHere == null)
            currentStandingOnNode.unitControllerStandingHere = (UnitStateController)parentController;

        return node;
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
        {
            path.Clear();
            return;
        }

        FindPath(destinationNode.worldPosition);
    }

    public void FindPath(Node endNode)
    {
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
                // Todo avoid friendly units who are in action mode
                if (!neighbour.walkable
                    || AvoidUnit(neighbour)
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