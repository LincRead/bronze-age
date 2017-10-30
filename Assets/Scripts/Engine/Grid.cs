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
    public GameObject metalPrefab;
    public GameObject fruitBushPrefab;
    public GameObject obsidianPrefab;
    public GameObject goldPrefab;

    [Header("Show Tile selected")]
    public GameObject selectedTilePrefab;

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

        selectedTilePrefab.GetComponent<SpriteRenderer>().enabled = false;

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
        FertilizeTiles();
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

                if (Random.value < 0.005f)
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

    void FertilizeTiles()
    {
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
                {
                    continue;
                }
                    

                float newFertility = currentTile.fertility - (1.5f + (Random.value * 10));

                if (newFertility > neighbour.fertility)
                {
                    neighbour.fertility = newFertility;
                }

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

    public void SpawnMetal(Tile tile)
    {
        SpawnResource(tile, metalPrefab);

        List<Tile> tiles = GetAllTilesFromBox(tile.worldPosition, 2);

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].fertility > 1)
                tiles[i].fertility = 1;
        }
    }

    public void SpawnGold(Tile tile)
    {
        SpawnResource(tile, goldPrefab);

        List<Tile> tiles = GetAllTilesFromBox(tile.worldPosition, 2);

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i].fertility > 0)
                tiles[i].fertility = 0;
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

    public void SpawnObsidian(Tile tile)
    {
        SpawnResource(tile, obsidianPrefab);
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

    public Tile GetNeighbourTileAbove(Tile tile)
    {
        int checkX = tile.gridPosX - 1;
        int checkY = tile.gridPosY - 1;

        if (checkX > -1 && checkX < numTilesX && checkY > -1 && checkY < numTilesY)
        {
            return tiles[checkX, checkY];
        }

        return null;
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

    public Node GetNeighbourNodeBelow(Node node)
    {
        int checkX = node.gridPosX - 1;
        int checkY = node.gridPosY - 1;

        if (checkX > -1 && checkX < numTilesX && checkY > -1 && checkY < numTilesY)
        {
            return nodes[checkX, checkY];
        }

        return null;
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
                    {
                        tile.SetWalkable();
                    }
                        
                    else
                    {
                        tile.SetUnwalkable();
                    }
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
                    {
                        // Only set to null if not a farm
                        if (!obj._basicStats.walkable)
                        {
                            tile.controllerOccupying = null;
                        }

                        tile.SetWalkable();
                    }
                        
                    else
                    {
                        tile.SetUnwalkable();
                        tile.controllerOccupying = obj;
                    }    
                }
            }
        }
    }

    public void SetTilesOccupiedByController(BaseController controller, bool walkable)
    {
        // Adding offset since building pivot point (buttom of sprite) is in the middle of two tiles.
        Tile tile = GetTileFromWorldPoint(controller.GetPosition() + new Vector2(0.04f, 0.04f));

        if (tile == null)
        {
            return;
        }

        for (int i = 0; i < controller.size; i++)
        {
            for (int j = 0; j < controller.size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile tileToOccupy = GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j);

                    // If a controller already stands here
                    if(tileToOccupy.controllerOccupying != null && controller != tileToOccupy.controllerOccupying)
                    {
                        // Remove the other
                        if(controller.controllerType == CONTROLLER_TYPE.BUILDING)
                        {
                            tileToOccupy.controllerOccupying.Destroy();

                            // Set this tile as occupied by building
                            tileToOccupy.controllerOccupying = controller;

                            if (!walkable)
                            {
                                tileToOccupy.SetUnwalkable();
                            }

                            // Reset for all other tiles the controller occupies, 
                            // because destroying a controller sets all tiles it occupied to not occupying any controllers
                            SetTilesOccupiedByController(controller, walkable);
                        }

                        // Remove me
                        else
                        {
                            // Keep reference so we can reset that this controller occipies all tiles it does.
                            BaseController currentOccupyingController = tileToOccupy.controllerOccupying;

                            // Destroy me if controller already is occupying this tile, and I'm not a building.
                            controller.Destroy();

                            // Tile should still be occupied by previous occupying controller
                            tileToOccupy.controllerOccupying = currentOccupyingController;

                            if(!walkable)
                            {
                                tileToOccupy.SetUnwalkable();
                            }

                            // Reset for all other tiles the controller occupies, 
                            // because destroying a controller sets all tiles it occupied to not occupying any controllerss
                            SetTilesOccupiedByController(currentOccupyingController, walkable);
                        }
                    }

                    else
                    {
                        // Set this tile as occupied by building
                        tileToOccupy.controllerOccupying = controller;

                        if (!walkable)
                        {
                            tileToOccupy.SetUnwalkable();
                        }
                    }
                }
            }
        }
    }

    public void SetTilesOccupied(Tile tile, int size)
    {
        if (tile == null)
        {
            return;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile tileToOccupy = GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j);
                    tileToOccupy.SetUnwalkable();
                }
            }
        }
    }

    public void RemoveControllersFrom(List<Tile> tiles, BaseController ignore)
    {
        for(int i = 0; i < tiles.Count; i++)
        {
            if(tiles[i].controllerOccupying != null && tiles[i].controllerOccupying != ignore)
            {
                tiles[i].controllerOccupying.Destroy();
                
                // Keep reference
                if(ignore != null)
                {
                    tiles[i].controllerOccupying = ignore;
                }
            }
        }
    }

    public List<Tile> GetTilesOccupiedByController(BaseController controller)
    {
        // Adding offset since building pivot point (buttom of sprite) is in the middle of two tiles.
        Tile tile = GetTileFromWorldPoint(controller.GetPosition() + new Vector2(0.04f, 0.04f));

        List<Tile> tilesToReturn = new List<Tile>();

        if (tile == null)
        {
            return tilesToReturn;
        }

        for (int i = 0; i < controller.size; i++)
        {
            for (int j = 0; j < controller.size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    tilesToReturn.Add(GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j));
                }
            }
        }

        return tilesToReturn;
    }

    public Node GetRandomNodeFromController(BaseController controller)
    {
        Tile tile = GetTileFromWorldPoint(controller.GetPosition());
        List<Node> nodesToSelectFrom = new List<Node>();

        for (int i = 0; i < controller.size; i++)
        {
            for (int j = 0; j < controller.size; j++)
            {
                if (i > -1 && j > -1 && i < numTilesX + 1 && j < numTilesY + 1)
                {
                    Tile tileToIncludeNodesFrom = GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j);

                    for(int n = 0; n < tileToIncludeNodesFrom.nodes.Length; n++)
                    {
                        if(tileToIncludeNodesFrom.nodes[n].unitControllerStandingHere == null)
                        {
                            nodesToSelectFrom.Add(tileToIncludeNodesFrom.nodes[n]);
                        }
                    }
                }
            }
        }

        int ran = (int)(Mathf.Floor(Random.value * nodesToSelectFrom.Count));

        return nodesToSelectFrom[ran];
    }

    public void RemoveTilesOccupiedByResource(BaseController controller)
    {
        // Adding offset since building pivot point (buttom of sprite) is in the middle of two tiles.
        Tile tile = GetTileFromWorldPoint(controller.GetPosition() + new Vector2(0.0f, -0.04f));

        if (tile == null)
        {
            return;
        }

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
        {
            return false;
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (i > -1 && j > -1 && tile.gridPosX + i < numTilesX && tile.gridPosY + j < numTilesY)
                {
                    Tile checkTile = GetTileFromGridPos(tile.gridPosX + i, tile.gridPosY + j);
                    if (!checkTile.IsEmpty() || !checkTile.explored)
                    {
                        return false;
                    }
                        
                }

                else
                    return false;
            }
        }

        return true;
    }

    public bool GetAllTilesFromBoxArEmpty(Tile tile, int size)
    {
        if (tile == null)
        {
            return false;
        }

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

    public List<Node> GetAllNodesBasedOnVisibilityFromNode(float visibilityValue, Node node, int size)
    {
        List<Node> visibleNodes = new List<Node>();
        int visibility = (int)(visibilityValue);

        for (int i = -visibility; i < visibility; i++)
        {
            for (int j = -visibility; j < visibility; j++)
            {
                if (!(node.gridPosX + i < 0
                    || node.gridPosY + j < 0
                    || node.gridPosX + i > numNodesX - 1
                    || node.gridPosY + j > numNodesY - 1))
                {
                    Debug.Log(GetDistanceBetweenNodes(node, nodes[node.gridPosX + i, node.gridPosY + j]));
                    if (GetDistanceBetweenNodes(node, nodes[node.gridPosX + i, node.gridPosY + j]) < (visibility * 10))
                    {
                        visibleNodes.Add(nodes[node.gridPosX + i, node.gridPosY + j]);
                    }
                }
            }
        }

        return visibleNodes;
    }

    public List<Tile> GetAllTilesBasedOnVisibilityFromNode(float visibilityValue, Node node, int size)
    {
        List<Tile> visibleTiles = new List<Tile>();
        Tile primaryTile = node.parentTile;
        visibleTiles.Add(primaryTile);
        int visibility = (int)(visibilityValue / 2);

        for (int i = -visibility; i < visibility; i++)
        {
            for (int j = -visibility; j < visibility; j++)
            {
                if (!(primaryTile.gridPosX + i < 0
                    || primaryTile.gridPosY + j < 0
                    || primaryTile.gridPosX + i > numTilesX - 1
                    || primaryTile.gridPosY + j > numTilesY - 1))
                {
                    // Already added primary tile
                    if(i == 0 && j == 0)
                    {
                        continue;
                    }

                    if(size % 2 == 0)
                    {
                        if (GetDistanceBetweenTileAndTileTopPos(tiles[primaryTile.gridPosX + i, primaryTile.gridPosY + j], primaryTile) <= (visibility * 10))
                        {
                            visibleTiles.Add(tiles[primaryTile.gridPosX + i, primaryTile.gridPosY + j]);
                        }
                    }

                    else
                    {
                        if (GetDistanceBetweenTiles(tiles[primaryTile.gridPosX + i, primaryTile.gridPosY + j], primaryTile) <= (visibility * 10))
                        {
                            visibleTiles.Add(tiles[primaryTile.gridPosX + i, primaryTile.gridPosY + j]);
                        }
                    }
                }
            }
        }

        return visibleTiles;
    }

    public float GetDistanceBetweenControllers(BaseController controllerA, BaseController controllerB)
    {
        if ((controllerA.size % 2 == 0 || controllerB.size % 2 == 0) && (controllerA.size % 2 != controllerB.size % 2))
        {
            return GetDistanceBetweenNodeTopPosAndNodeTopPos(controllerA.GetMiddleNode(), controllerB.GetMiddleNode()) 
                - (((controllerA.size - 1)) * 10f)
                - (((controllerB.size - 1)) * 10f);
        }

        else
        {
            return GetDistanceBetweenNodes(controllerA.GetMiddleNode(), controllerB.GetMiddleNode());
        }
    }

    public int GetDistanceBetweenNodes(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX);
        int distY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
            
        return 14 * distX + 10 * (distY - distX);
    }

    public float GetDistanceBetweenNodeTopPosAndNodeTopPos(Node nodeA, Node nodeB)
    {
        float distX = Mathf.Abs(nodeA.gridPosX - nodeB.gridPosX + 0.5f);
        float distY = Mathf.Abs(nodeA.gridPosY - nodeB.gridPosY + 0.5f);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }

        return 14 * distX + 10 * (distY - distX);
    }


    public int GetDistanceBetweenTiles(Tile tileA, Tile tileB)
    {
        int distX = Mathf.Abs(tileA.gridPosX - tileB.gridPosX);
        int distY = Mathf.Abs(tileA.gridPosY - tileB.gridPosY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
            
        return 14 * distX + 10 * (distY - distX);
    }

    public float GetDistanceBetweenTileAndTileTopPos(Tile tileA, Tile tileB)
    {
        float distX = Mathf.Abs(tileA.gridPosX - tileB.gridPosX + 0.5f);
        float distY = Mathf.Abs(tileA.gridPosY - tileB.gridPosY + 0.5f);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
            
        return 14 * distX + 10 * (distY - distX);
    }

    public Node FindClosestWalkableNode(Node node)
    {
        if (node == null)
        {
            return null;
        }
            
        if (node.walkable && node.unitControllerStandingHere == null)
        {
            return node;
        }

        Node startNode = node;
        Heap<Node> openSet = new Heap<Node>(MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            List<Node> nodesToCheck = GetNeighbourNodes(currentNode);

            foreach (Node neighbour in nodesToCheck)
            {
                if (closedSet.Contains(neighbour))
                {
                    continue;
                }

                if (neighbour.walkable && neighbour.unitControllerStandingHere == null)
                {
                    return neighbour;
                }

                if (!openSet.Contains(neighbour))
                {
                    openSet.Add(neighbour);
                }
            }
        }

        return null;
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
        Gizmos.color = Color.blue;

        // Show size of Grid
        float offsetY = (numTilesY * tileHeight);
        Gizmos.DrawLine(new Vector3(- numTilesX * tileHeight * 2, 0.0f + offsetY), new Vector3(0.0f, numTilesY * tileHeight + offsetY));
        Gizmos.DrawLine(new Vector3(0.0f, numTilesY * tileHeight + offsetY), new Vector3(numTilesX * tileHeight * 2, 0.0f + offsetY));
        Gizmos.DrawLine(new Vector3(numTilesX * tileHeight * 2, 0f + offsetY), new Vector3(0.0f, -numTilesY * tileHeight + offsetY));
        Gizmos.DrawLine(new Vector3(0.0f, -numTilesY * tileHeight + offsetY), new Vector3(-numTilesX * tileHeight * 2, 0.0f + offsetY));

        // Grid gets created at Awake()
        if (tiles != null)
        {
            foreach (Tile t in tiles)
            {
                if (t.walkable)
                {
                    Gizmos.color = new Color(0.1f, 0.7f, 0.1f, 0.5f);
                }
                else
                {
                    Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.5f);
                }

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
