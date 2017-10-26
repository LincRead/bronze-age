using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public enum CONTROLLER_TYPE
{
    UNIT,
    ANIMAL,
    RESOURCE,
    BUILDING
};

public class BaseController : MonoBehaviour {

    public int playerID = 0;

    [HideInInspector]
    public float zIndex = 0;

    private bool firstUpdate = true;

    [HideInInspector]
    public bool selected = false;

    [HideInInspector]
    public bool dead = false;

    [HideInInspector]
    public DefaultStats _basicStats;

    [HideInInspector]
    public Transform _transform;

    [HideInInspector]
    public SpriteRenderer _spriteRenderer;

    [HideInInspector]
    public Collider2D _collider;

    [HideInInspector]
    public CONTROLLER_TYPE controllerType;

    [HideInInspector]
    public string title = "Default title";

    [HideInInspector]
    public int size = 1;

    [HideInInspector]
    public int visionRange = 0;

    [HideInInspector]
    public Sprite iconSprite;

    [HideInInspector]
    public Sprite[] statSprites = new Sprite[4];

    protected GameObject _selectionIndicator;
    protected SpriteRenderer _selectedIndicatorRenderer;

    protected List<Tile> visibleTiles = new List<Tile>();

    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    protected virtual void Start ()
    {
        title = _basicStats.title;

        size = _basicStats.size;
        iconSprite = _basicStats.iconSprite;
        statSprites = _basicStats.statSprites;
        controllerType = _basicStats.controllerType;
        visionRange = _basicStats.visionRange;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;

        _collider = GetComponent<Collider2D>();

        SetupSelectIndicator();
    }

    protected virtual void SetupSelectIndicator()
    {
        if(_basicStats != null)
        {
            _selectionIndicator = GameObject.Instantiate(_basicStats.selectionCircle, transform.position, Quaternion.identity);
            _selectionIndicator.transform.parent = gameObject.transform;
            _selectedIndicatorRenderer = _selectionIndicator.GetComponent<SpriteRenderer>();
            _selectedIndicatorRenderer.enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (_spriteRenderer != null)
        {
            UpdateZIndex();
        }

        if(firstUpdate)
        {
            firstUpdate = false;
            FirstUpdate();
        }
    }

    protected virtual void FirstUpdate()
    {

    }

    void UpdateZIndex()
    {
        zIndex = _transform.position.y;
        _transform.position = new Vector3(_transform.position.x, _transform.position.y, zIndex);
    }

    public virtual void Select()
    {
        selected = true;
        _selectedIndicatorRenderer.enabled = true;
    }

    public virtual void Deselect()
    {
        selected = false;
        _selectedIndicatorRenderer.enabled = false;
    }

    public virtual void Hit(int damageValue, BaseController hitByController)
    {

    }

    // Make sure all tiles occupied gets the same visibility value
    public void UpdateVisibilityOfAllControllerOccupiedTiles()
    {
        bool visible = false;

        // Small controller
        if (size == 1)
        {
            Tile tile = GetPrimaryTile();

            if (tile.visibleForControllerCount > 0)
            {
                visible = true;
                tile.SetVisible(true);
            }

            else
            {
                tile.SetVisible(false);
            }
        }

        // Large controller
        else
        {
            List<Tile> tiles = Grid.instance.GetTilesOccupiedByController(this);

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].visibleForControllerCount > 0)
                {
                    visible = true;
                }
            }

            for (int i = 0; i < tiles.Count; i++)
            {
                if (visible)
                {
                    tiles[i].SetVisible(true);
                }

                else
                {
                    tiles[i].SetVisible(false);
                }
            }
        }

        int visibilityValue = 0;

        if (visible)
        {
            // Fully visible
            visibilityValue = 2;
        }

        // Explored, but not currently visible
        else if(GetPrimaryTile().explored)
        {
            visibilityValue = 1;
        }

        SetVisible(visibilityValue);
    }

    public void IncreaseVisibilityOfTiles()
    {
        if (playerID != PlayerManager.myPlayerID)
        {
            return;
        }

        for (int i = 0; i < visibleTiles.Count; i++)
        {
            visibleTiles[i].ChangeVisibilityCount(1);
            visibleTiles[i].UpdateVisibilityOfTileAndControllers();
        }
    }

    public void DecreaseVisibilityOfTiles()
    {
        if(playerID != PlayerManager.myPlayerID)
        {
            return;
        }

        for (int i = 0; i < visibleTiles.Count; i++)
        {
            visibleTiles[i].ChangeVisibilityCount(-1);
            visibleTiles[i].UpdateVisibilityOfTileAndControllers();
        }
    }

    // If we are not an allied controller and current tile is unexplored,
    // make sure we are not visible
    public virtual void SetVisible(int value)
    {
        if (value == 0)
        {
            _spriteRenderer.enabled = false;

        }

        else
        {
            _spriteRenderer.enabled = true;

            if (value == 1)
            {
                _spriteRenderer.color = new Color(.5f, .5f, .5f, 1.0f);
            }

            else
            {
                _spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
        }
    }

    public virtual void SetVisible(bool value)
    {
        if(value)
        {
            _spriteRenderer.enabled = true;
        }

        else
        {
            _spriteRenderer.enabled = false;
        }
    }

    // Cancel whatever action is being done
    public virtual void Cancel()
    {

    }

    public virtual Vector2 GetPosition()
    {
        return _transform.position + new Vector3(0.04f, 0.04f);
    }

    public virtual Node GetPrimaryNode()
    {
        return Grid.instance.GetNodeFromWorldPoint(_transform.position + new Vector3(0.0f, 0.04f));
    }

    public virtual Node GetMiddleNode()
    {
        return Grid.instance.GetNodeFromWorldPoint(_transform.position + new Vector3(0.0f, size * Grid.instance.tileHeight));
    }

    public virtual Tile GetPrimaryTile()
    {
        return Grid.instance.GetTileFromWorldPoint(_transform.position + new Vector3(0.04f, 0.04f));
    }

    public Rect GetCollisionBox()
    {
        Bounds b = _spriteRenderer.bounds;
        return new Rect(new Vector2(b.center.x, b.min.y + (size / 2)), new Vector2(b.size.x, b.size.x / 2));
    }

    public virtual bool IntersectsPoint(Grid.FPoint point)
    {
        Grid.FPoint myNodePoint = GetPrimaryNode().gridPosPoint;

        if (point.x >= myNodePoint.x && point.x < myNodePoint.x + (size * 2)
            && point.y >= myNodePoint.y && point.y < myNodePoint.y + (size * 2))
        {
            return true;
        }

        return false;
    }

    public bool IntersectsRectangle(Rect otherBox)
    {
        Rect rect = new Rect(
            _collider.bounds.center - new Vector3(_collider.bounds.size.x / 2, _collider.bounds.size.y / 2, 0.0f),
            _collider.bounds.size);

        if (otherBox.Overlaps(rect, true))
        {
            return true;
        }

        return false;
    }

    public virtual int[] GetUniqueStats()
    {
        return new int[0];
    }

    public bool IsVisible()
    {
        return _spriteRenderer.enabled;
    }

    public virtual void Destroy()
    {
        DecreaseVisibilityOfTiles();
        Grid.instance.RemoveTilesOccupiedByController(this);
        Destroy(gameObject);
    }
}
