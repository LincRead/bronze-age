using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {

    public static Grid instance;

    public LayerMask unwalkableMask;

    [Header("Size")]
    public int numTilesX;
    public int numTilesY;
    public float tileHeight;
    private float tileWidth;

    [HideInInspector]
    public int numNodesX;
    public int numNodesY;

    // The actual size in pixels
    private Vector2 gridWorldSize;

    [Header("Debugging")]
    public bool debugPathsFound = true;
    public Color partOfPathDebugColor = Color.blue;
    public float debugTileTypeAlpha = 0.4f;
    [HideInInspector]
    public List<List<Node>> pathsToDebug = new List<List<Node>>();

    private Tile[,] tiles;
    private Node[,] nodes;

    [Header("Tile set")]
    public Sprite[] tileSprites;

    [Header("Resources")]
    public GameObject treePrefab;
    public GameObject stonePrefab;
    public GameObject fruitBushPrefab;

    public int MaxSize
    {
        get
        {
            return numTilesX * numTilesY;
        }
    }

    public struct Point
    {
        public int x;
        public int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public struct FPoint
    {
        public float x;
        public float y;

        public FPoint(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    void Awake()
    {
        // Singleton
        instance = this;

        if (numTilesX < 16)
        {
            numTilesX = 16;
            Debug.LogError("gridSizeX must be set to minimum 16");
        }

        if (numTilesY < 10)
        {
            numTilesY = 10;
            Debug.LogError("gridSizeY must be set to minimum 10");
        }

        tileWidth = tileHeight * 2;
        numNodesX = numTilesX * 2;
        numNodesY = numTilesY * 2;
        gridWorldSize.x = numTilesX * tileWidth;
        gridWorldSize.y = numTilesY * tileWidth;

        // Create world
        CreateTiles();
        StartCoroutine(FertilizeTiles(0.1f));
    }

    void CreateTiles()
    {
        tiles = new Tile[numTilesX, numTilesY];
        nodes = new Node[numNodesX, numNodesY];

        for (int x = 0; x < numTilesX; x++)
        {
            for (int y = 0; y < numTilesY; y++)
            {
                // Calculate offset for each tile
                Vector3 worldPoint = GetPos2DToIsometric(new Vector2(
                    x * tileWidth + tileHeight,
                    y * tileWidth + tileHeight));

                tiles[x, y] = new Tile(true, worldPoint, x, y, this);

                if (Random.value < 0.01f)
                {
                    tiles[x, y].fertility = 30;
                    tiles[x, y].fertilityLocked = true;
                }

                AddNodes(tiles[x, y]);
            }
        }
    }

    // 4 nodes per tile:
    // NN
    // NN
    void AddNodes(Tile parent)
    {
        float nodeWidth = tileWidth / 2;
        float nodeHeight = tileHeight / 2;
        int index = -1;

        for (int x = parent.gridPosX * 2; x < (parent.gridPosX * 2) + 2; x++)
        {
            for (int y = parent.gridPosY * 2; y < (parent.gridPosY * 2) + 2; y++)
            {
                // Calculate offset for each node
                Vector3 worldPoint = GetPos2DToIsometric(new Vector2(
                    x * nodeWidth + nodeHeight,
                    y * nodeWidth + nodeHeight));

                nodes[x, y] = new Node(worldPoint, x, y, parent);
                parent.nodes[++index] = nodes[x, y];
            }
        }
    }

    IEnumerator FertilizeTiles(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        for (int x = 0; x < numTilesX; x++)
        {
            for (int y = 0; y < numTilesY; y++)
            {
                if (tiles[x, y].fertility == 30)
                    FertilizeSurroundingTiles(tiles[x, y]);
            }
        }

        for (int x = 0; x < numTilesX; x++)
        {
            for (int y = 0; y < numTilesY; y++)
            {
                tiles[x, y].CreateTile();
            }
        }
    }

    void FertilizeSurroundingTiles(Tile tile)
    {
        Tile startTile = tile;
        Heap<Tile> openSet = new Heap<Tile>(MaxSize);
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(startTile);

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile);

            List<Tile> tilesToCheck = GetNeighbourTiles(currentTile);
            foreach (Tile neighbour in tilesToCheck)
            {
                if (closedSet.Contains(neighbour))
                    continue;

                float newFertility = currentTile.fertility - (2 + (Random.value * 12));
                if (newFertility > neighbour.fertility)
                    neighbour.fertility = newFertility;

                if (neighbour.fertility <= 0)
                {
                    neighbour.fertility = 0;
                    closedSet.Add(neighbour);
                }

                else if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
            }
        }
    }

    void SpawnResource(Tile tile, GameObject resourcePrefab)
    {
        GameObject.Instantiate(resourcePrefab, tile.worldPosition, Quaternion.identity);
    }

    public void SpawnStone(Tile tile)
    {
        SpawnResource(tile, stonePrefab);

        List<Tile> tiles = GetAllTilesFromBox(tile.worldPosition, 2);

        for(int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].fertility > 1)
                tiles[i].fertility = 1;
        }
    }

    public void SpawnTree(Tile tile)
    {
        SpawnResource(tile, treePrefab);
    }

    public void SpawnFruitBush(Tile tile)
    {
        SpawnResource(tile, fruitBushPrefab);
    }

    Vector2 GetPosIsometricTo2D(Vector2 posIso)
    {
        Vector2 pos2D = Vector2.zero;
        pos2D.x = (2 * posIso.y + posIso.x) / 2;
        pos2D.y = (2 * posIso.y - posIso.x) / 2;
        return pos2D;
    }

    Vector2 GetPos2DToIsometric(Vector2 pos2D)
    {
        Vector2 posIso = Vector2.zero;
        posIso.x = pos2D.x - pos2D.y;
        posIso.y = (pos2D.x + pos2D.y) / 2;
        return posIso;
    }

    public Tile GetTileFromWorldPoint(Vector2 worldPoint)
    {
        Point coords = GetTileCoordinates(GetPosIsometricTo2D(worldPoint));

        if (coords.x > -1 && coords.y > -1 && coords.x < numTilesX && coords.y < numTilesY)
        {
            return tiles[coords.x, coords.y];
        }

        return null;
    }

    public Node GetNodeFromWorldPoint(Vector2 worldPoint)
    {
        Point coords = GetNodeCoordinates(GetPosIsometricTo2D(worldPoint));

        if (coords.x > -1 && coords.y > -1 && coords.x < numNodesX && coords.y < numNodesY)
        {
            return nodes[coords.x, coords.y];
        }

        return null;
    }

    Point GetTileCoordinates(Vector2 pos)
    {
        Point tileCoords = new Point();
        tileCoords.x = (int)Mathf.Floor(pos.x / tileWidth);
        tileCoords.y = (int)Mathf.Floor(pos.y / tileWidth);
        return (tileCoords);
    }

    Point GetNodeCoordinates(Vector2 pos)
    {
        Point tileCoords = new Point();
        tileCoords.x = (int)Mathf.Floor(pos.x / (tileWidth / 2));
        tileCoords.y = (int)Mathf.Floor(pos.y / (tileWidth / 2));
        return (tileCoords);
    }

    public Tile GetTileFromGridPos(int gridPosX, int gridPosY)
    {
        if (gridPosX < 0 || gridPosX > numTilesX - 1 || gridPosY < 0 || gridPosY > numTilesY - 1)
        {
            Debug.LogError("Trying to get a Tile outside grid boundaries");
            return null;
        }

        return tiles[gridPosX, gridPosY];
    }

    public Node GetNodeFromGridPos(int gridPosX, int gridPosY)
    {
        if (gridPosX < 0 || gridPosX > numNodesX - 1 || gridPosY < 0 || gridPosY > numNodesY - 1)
        {
            Debug.LogError("Trying to get a Node outside grid boundaries");
            return null;
        }

        return nodes[gridPosX, gridPosY];
    }

    public BaseController GetControllerFromWorldPoint(Vector2 worldPoint)
    {
        Point coords = GetTileCoordinates(GetPosIsometricTo2D(worldPoint));

        if (coords.x > -1 && coords.y > -1 && coords.x < numTilesX && coords.y < numTilesY)
        {
            return tiles[coords.x, coords.y].controllerOccupying;
        }

        return null;
    }

    public BaseController GetUnitFromWorldPoint(Vector2 worldPoint)
    {
        Point coords = GetNodeCoordinates(GetPosIsometricTo2D(worldPoint));

        if (coords.x > -1 && coords.y > -1 && coords.x < numNodesX && coords.y < numNodesY)
        {
            return nodes[coords.x, coords.y].unitControllerStandingHere;
        }

        return null;
    }

    public BaseController GetControllerFromGridPos(int gridPosX, int gridPosY)
    {
        if (gridPosX < 0 || gridPosX > numTilesX - 1 || gridPosY < 0 || gridPosY > numTilesY - 1)
        {
            Debug.LogError("Trying to get a Tile outside grid boundaries");
            return null;
        }

        return tiles[gridPosX, gridPosY].controllerOccupying;
    }

    public List<Tile> GetNeighbourTiles(Tile tile)
    {
        List<Tile> neighbours = new List<Tile>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Current node
                if (x == 0 && y == 0)
                    continue;

                int checkX = tile.gridPosX + x;
                int checkY = tile.gridPosY + y;

                if (checkX > -1 && checkX < numTilesX && checkY > -1 && checkY < numTilesY)
                    neighbours.Add(tiles[checkX, checkY]);
            }
        }

        return neighbours;
    }

    public List<Node> GetNeighbourNodes(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                // Current node
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridPosX + x;
                int checkY = node.gridPosY + y;

                if (checkX > -1 && checkX < numNodesX && checkY > -1 && checkY < numNodesY)
                    neighbours.Add(nodes[checkX, checkY]);
            }
        }

        return neighbours;
    }

    public Vector2 SnapToGrid(Vector2 posToSnap)
    {
        float w = tileWidth * 2;
        float h = tileHeight * 2;

        // Calculate ratios for simple grid snap
        float xx = Mathf.Round(posToSnap.y / h - posToSnap.x / w);
        float yy = Mathf.Round(posToSnap.y / h + posToSnap.x / w);

        // Calculate grid aligned position from current position
        Vector2 gridPosition = new Vector3(
        ((yy - xx) * 0.5f * w),
        ((yy + xx) * 0.5f * h));

        return gridPosition;
    }

    public void SetWalkableValueForTiles(Vector2 pos, int size, bool walkable)
    {
        Tile primaryTile = GetTileFromWorldPoint(pos);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile tile = GetTileFromGridPos(primaryTile.gridPosX + i, primaryTile.gridPosY + j);

                    if (walkable)
                        tile.SetWalkable();
                    else
                        tile.SetUnwalkable();
                }
            }
        }
    }

    public void SetWalkableValueForTiles(BaseController obj, bool walkable)
    {
        Tile primaryTile = GetTileFromWorldPoint(obj.GetPosition());

        for (int i = 0; i < obj.size; i++)
        {
            for (int j = 0; j < obj.size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile tile = GetTileFromGridPos(primaryTile.gridPosX + i, primaryTile.gridPosY + j);

                    if (walkable)
                        tile.SetWalkable();
                    else
                        tile.SetUnwalkable();
                }
            }
        }
    }

    public void SetTilesOccupiedByController(BaseController controller)
    {
        // Adding offset since building pivot point (buttom of sprite) is in the middle of two tiles.
        Tile tile = GetTileFromWorldPoint(controller.GetPosition() + new Vector2(0.04f, 0.04f));

        if (tile == null)
            return;

        for (int i = 0; i < controller.size; i++)
        {
            for (int j = 0; j < controller.size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile occupyTile = GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j);
                    occupyTile.SetUnwalkable();
                    occupyTile.controllerOccupying = controller;
                }
            }
        }
    }

    public void RemoveTilesOccupiedByController(BaseController controller)
    {
        Tile tile = GetTileFromWorldPoint(controller.GetPosition());

        if (tile == null)
            return;

        for (int i = 0; i < controller.size; i++)
        {
            for (int j = 0; j < controller.size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile tileToRemoveFrom = GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j);
                    tileToRemoveFrom.SetWalkable();
                    tileToRemoveFrom.controllerOccupying = null;
                }
            }
        }
    }

    public bool GetAllTilesFromBoxArEmpty(Vector2 pos, int size)
    {
        Tile tile = GetTileFromWorldPoint(pos);

        if (tile == null)
            return false;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i > -1 && j > -1 && tile.gridPosX + i < numTilesX && tile.gridPosY + j < numTilesY)
                {
                    if (!GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j).IsEmpty())
                        return false;
                }

                else
                    return false;
            }
        }

        return true;
    }

    public List<Tile> GetAllTilesFromBox(Vector2 pos, int size)
    {
        List<Tile> tiles = new List<Tile>();
        Tile tile = GetTileFromWorldPoint(pos);

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + i && j < numTilesY + j)
                {
                    tiles.Add(GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j));
                }
            }
        }

        return tiles;
    }

    public bool GetPositionIntersectsWithTilesFromBox(FPoint myPos, FPoint otherPos, int size)
    {
        if (myPos.x >= otherPos.x && myPos.x < otherPos.x + size
            && myPos.y >= otherPos.y && myPos.y < otherPos.y + size)
            return true;

        return false;
    }

    public List<Tile> GetAllTilesBasedOnVisibilityFromNode(int visiblity, Node node)
    {
        List<Tile> visibleTiles = new List<Tile>();
        Tile primaryTile = node.parentTile;

        for (int i = -visiblity; i < visiblity; i++)
        {
            for (int j = -visiblity; j < visiblity; j++)
            {
                if (primaryTile.gridPosX + i < 0
                    || primaryTile.gridPosY + j < 0
                    || primaryTile.gridPosX + i > numTilesX - 1
                    || primaryTile.gridPosY + j > numTilesY - 1
                    || GetDistanceBetweenTiles(primaryTile, tiles[primaryTile.gridPosX + i, primaryTile.gridPosY + j]) >= (visiblity * 10))
                {

                }

                else
                {
                    visibleTiles.Add(tiles[primaryTile.gridPosX + i, primaryTile.gridPosY + j]);
                }
            }
        }

        return visibleTiles;
    }


    /*public List<Tile> GetAllTilesBasedOnVisibilityFromNode(int visiblity, Node node)
    {
        Tile primaryTile = node.parentTile;
        List<Tile> visibleTiles = new List<Tile>();

        Heap<Tile> openSet = new Heap<Tile>(Grid.instance.MaxSize);
        HashSet<Tile> closedSet = new HashSet<Tile>();
        openSet.Add(primaryTile);

        while (openSet.Count > 0)
        {
            Tile currentTile = openSet.RemoveFirst();
            closedSet.Add(currentTile);

            List<Tile> neighboursToCheck = GetNeighbourTiles(currentTile);
            for(int i = 0; i < neighboursToCheck.Count; i++)
            {
                if (closedSet.Contains(neighboursToCheck[i]))
                    continue;

                if (GetDistanceBetweenTiles(primaryTile, neighboursToCheck[i]) <= (visiblity * 10))
                {
                    visibleTiles.Add(neighboursToCheck[i]);

                    if (!openSet.Contains(neighboursToCheck[i]))
                        openSet.Add(neighboursToCheck[i]);
                }
            }
        }

        return visibleTiles;
    }*/

    public int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
        int distY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    public int GetDistanceBetweenTiles(Tile tileA, Tile tileB)
    {
        int distX = Mathf.Abs(tileA.gridPosX - tileB.gridPosX);
        int distY = Mathf.Abs(tileA.gridPosY - tileB.gridPosY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }

    public float GetGridWorldSizeX()
    {
        return numTilesX * tileWidth * 2;
    }

    public float GetGridWorldSizeY()
    {
        return numTilesY * tileHeight * 2;
    }

    /*
     * Debug Grid size in Gizmo.
     * Debug which tiles are walkable and not.
     * Need to run game to see tiles in Editor Scene.
     * Grid gets created at Awake().
     */
        void OnDrawGizmos()
    {
        // Show grid size
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1.0f));

        // Grid gets created at Awake()
        if (tiles != null)
        {
            foreach (Tile t in tiles)
            {
                if (t.walkable)
                    Gizmos.color = new Color(0.1f, 0.7f, 0.1f, 0.5f);
                else
                    Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.5f);

                Gizmos.DrawCube(t.worldPosition, Vector3.one * (tileWidth / 2.5f));
            }
        }

        if (nodes != null)
        {
            foreach (Node n in nodes)
            {
                if(n!= null)
                {
                    if(n.unitControllerStandingHere)
                        Gizmos.color = new Color(1.0f, 0.0f, 1.0f, 0.5f);
                    else if (n.walkable)
                        Gizmos.color = new Color(0.0f, 1.0f, 0.0f, 0.6f);
                    else
                        Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);

                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (tileWidth / 5f));
                }
            }
        }

        if (pathsToDebug.Count > 0)
        {
            foreach (List<Node> path in pathsToDebug)
            {
                Gizmos.color = Color.blue;

                foreach (Node n in path)
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (tileWidth / 15));
            }
        }
    }
}
