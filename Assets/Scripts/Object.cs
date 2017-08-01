using UnityEngine;
using System.Collections;

public class Object : MonoBehaviour {

    // Todo: assign automatic
    [HideInInspector]
    public int playerID = 0;

    [HideInInspector]
    public Transform _transform;

    [HideInInspector]
    public SpriteRenderer _spriteRenderer;

    protected float zIndex = 0;

    public int tilesOccupiedWidth = 1;

    [HideInInspector]
    public bool completed = false;

    [HideInInspector]
    public bool selected = false;

    [Header("Title")]
    public string title;

    public enum CONTROLLER_TYPE
    {
        UNIT,
        STATIC_RESOURCE,
        BUILDING
    }

    public enum OBJECT_TYPE
    {
        FARM,
        WILD_GROWTH,
        TIMBER,
        STONE
    }

    [Header("Controller Type")]
    public CONTROLLER_TYPE controllerType;

    [Header("Object Type")]
    public OBJECT_TYPE type;

    [Header("Icon")]
    public Sprite iconSprite;

    [Header("Stats icons")]
    public Sprite[] statSprites = new Sprite[4];

    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
    }

	// Use this for initialization
	protected virtual void Start () {
        _spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	protected virtual void Update () {

        if (_spriteRenderer != null)
            zIndex = _transform.position.y;
    }

    public virtual Vector2 GetPosition()
    {
        return _transform.position + new Vector3(0.04f, 0.04f);
    }

    public virtual Node GetPrimaryNode()
    {
        return WorldManager.Manager.GetGrid().GetNodeFromWorldPoint(_transform.position + new Vector3(0.0f, 0.04f));
    }

    public Tile GetPrimaryTile()
    {
        return WorldManager.Manager.GetGrid().GetTileFromWorldPoint(_transform.position + new Vector3(0.04f, 0.04f));
    }

    public Rect GetCollisionBox()
    {
        Bounds b = _spriteRenderer.bounds;
        return new Rect(new Vector2(b.center.x, b.min.y + (tilesOccupiedWidth / 2)), new Vector2(b.size.x, b.size.x / 2));
    }

    public virtual bool IntersectsPoint(Grid.FPoint point)
    {
        Grid.FPoint myNodePoint = GetPrimaryNode().gridPosPoint;

        if (point.x >= myNodePoint.x && point.x < myNodePoint.x + (tilesOccupiedWidth * 2)
            && point.y >= myNodePoint.y && point.y < myNodePoint.y + (tilesOccupiedWidth * 2))
            return true;

        return false;
    }

    public virtual void Select() { selected = true; }
    public virtual void Deselect() { selected = false; }
    public virtual void Action(float value) { }

    public virtual int[] GetUniqueStats()
    {
        return new int[0];
    }
}
