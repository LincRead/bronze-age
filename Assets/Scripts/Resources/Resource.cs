﻿using UnityEngine;
using System.Collections;

public class Resource : BaseController {

    public enum HARVEST_TYPE
    {
        GATHER,
        CHOP,
        MINE,
        FARM
    }

    [Header("Type")]
    public HARVEST_TYPE resourceType;

    [Header("Stages")]
    [SerializeField]
    public Sprite[] harvestStagesSprites = new Sprite[0];

    [Header("Resource stats")]
    public int amount = 10;
    public float harvestDifficulty = 1;

    protected int amountLeft = 0;
    protected float harvestProgress = 0.0f;

    [HideInInspector]
    public bool depleted = false;

    protected override void Start()
    {
        base.Start();

        // Center resource based on number of tiles resource occupies in each directions.
        transform.position += new Vector3(0.0f, 0.08f * (size - 1));

        // Set correct zIndex
        zIndex = _transform.position.y;
        _transform.position = new Vector3(_transform.position.x, _transform.position.y, zIndex);

        Grid.instance.SetTilesOccupiedByController(this);

        _spriteRenderer.sortingLayerName = "Object";

        amountLeft = amount;
    }

    protected override void Update()
    {
        base.Update();

        if (depleted)
            Destroy();
    }

    public void Harvest(float harvestRate, int playerID)
    {
        harvestProgress += harvestRate * Time.deltaTime;

        if (harvestProgress >= harvestDifficulty)
        {
            harvestProgress = 0.0f;
            amountLeft--;
            UpdateResourceAmountForPlayer(playerID);
            UpdateStat();

            if (amountLeft <= 0)
            {
                depleted = true;
            }

            // TODO fade out ??
            else if (amountLeft < amount / 2.5f && harvestStagesSprites.Length == 3)
                _spriteRenderer.sprite = harvestStagesSprites[1];
            else if (amountLeft < amount / 1.4f && harvestStagesSprites.Length == 3)
                _spriteRenderer.sprite = harvestStagesSprites[0];
        }
    }

    protected virtual void UpdateResourceAmountForPlayer(int playerID)
    {

    }

    public override void Select()
    {
        base.Select();

        UpdateStat();
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

    // Todo: don't just update stat unless actually showing stats for this resource
    protected void UpdateStat()
    {
        ControllerUIManager.instance.UpdateStat(0, amountLeft);
    }

    public override bool IntersectsPoint(Grid.FPoint point)
    {
        Grid.FPoint myNodePoint = GetPrimaryNode().gridPosPoint;

        // Take centering position of all resources into account
        if (point.x >= myNodePoint.x - size && point.x < myNodePoint.x + size
            && point.y >= myNodePoint.y - size && point.y < myNodePoint.y + size)
            return true;

        return false;
    }

    public override Vector2 GetPosition()
    {
        // Take centering position into account
        return transform.position - new Vector3(0.0f, 0.08f * (size - 1));
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = amountLeft;
        return stats;
    }

    public virtual void Destroy()
    {
        Grid.instance.RemoveTilesOccupiedByController(this);
        Destroy(gameObject);
    }
}
