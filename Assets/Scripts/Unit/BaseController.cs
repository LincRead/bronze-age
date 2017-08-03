﻿using UnityEngine;
using System.Collections;

public class BaseController : MonoBehaviour {

    public string title = "Title";

    [HideInInspector]
    public int playerID = 0;

    public enum CONTROLLER_TYPE
    {
        UNIT,
        ANIMAL,
        STATIC_RESOURCE,
        BUILDING
    };

    public CONTROLLER_TYPE controllerType;

    [HideInInspector]
    public Transform _transform;

    [HideInInspector]
    public SpriteRenderer _spriteRenderer;

    [HideInInspector]
    public Collider2D _collider;

    protected float zIndex = 0;

    public int size = 1;

    [HideInInspector]
    public bool selected = false;

    [Header("UI icon")]
    public Sprite iconSprite;

    [Header("Stats icons")]
    public Sprite[] statSprites = new Sprite[4];

    [Header("Selection indicator")]
    public GameObject selectionIndicator;
    SpriteRenderer _selectedIndicatorRenderer;

    protected virtual void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    protected virtual void Start ()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();

        SetupSelectIndicator();
    }

    void SetupSelectIndicator()
    {
        if(selectionIndicator != null)
        {
            _selectedIndicatorRenderer = selectionIndicator.GetComponent<SpriteRenderer>();
            _selectedIndicatorRenderer.enabled = false;
        }
    }

    protected virtual void Update()
    {
        if (_spriteRenderer != null)
            zIndex = _transform.position.y;
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

    public virtual Vector2 GetPosition()
    {
        return _transform.position + new Vector3(0.04f, 0.04f);
    }

    public virtual Node GetPrimaryNode()
    {
        return WorldManager.instance.GetGrid().GetNodeFromWorldPoint(_transform.position + new Vector3(0.0f, 0.04f));
    }

    public Tile GetPrimaryTile()
    {
        return WorldManager.instance.GetGrid().GetTileFromWorldPoint(_transform.position + new Vector3(0.04f, 0.04f));
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
            return true;

        return false;
    }

    public bool IntersectsRectangle(Rect otherBox)
    {
        Rect rect = new Rect(
            _collider.bounds.center - new Vector3(_collider.bounds.size.x / 2, _collider.bounds.size.y / 2, 0.0f),
            _collider.bounds.size);

        if (rect.Overlaps(otherBox, true))
        {
            return true;
        }

        return false;
    }

    public virtual bool IsMoving()
    {
        return false;
    }

    public virtual int[] GetUniqueStats()
    {
        return new int[0];
    }
}