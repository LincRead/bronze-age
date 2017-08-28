using UnityEngine;
using System.Collections;

public enum CONTROLLER_TYPE
{
    UNIT,
    ANIMAL,
    STATIC_RESOURCE,
    BUILDING
};

public class BaseController : MonoBehaviour {

    public int playerID = 0;

    protected float zIndex = 0;

    [HideInInspector]
    public bool selected = false;

    [HideInInspector]
    public bool dead = false;

    protected DefaultStats _basicStats;

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
    public Sprite iconSprite;

    [HideInInspector]
    public Sprite[] statSprites = new Sprite[4];

    protected GameObject _selectionIndicator;
    protected SpriteRenderer _selectedIndicatorRenderer;

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

        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            zIndex = _transform.position.y;
        }
    }

    void OnMouseEnter()
    {
        PlayerManager.instance.mouseHoveringController = this;
    }

    void OnMouseExit()
    {
        if (PlayerManager.instance.mouseHoveringController == this)
            PlayerManager.instance.mouseHoveringController = null;
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

    public virtual void Hit(int damageValue)
    {

    }

    // If we are not an allied controller and current tile is unexplored,
    // make sure we are not visible
    protected void CheckTileAndSetVisibility()
    {
        if (playerID != PlayerManager.myPlayerID && !GetPrimaryTile().explored)
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

    public Node GetMiddleNode()
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
}
