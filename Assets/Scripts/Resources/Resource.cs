using UnityEngine;
using System.Collections;

public enum HARVEST_TYPE
{
    GATHER_BERRIES,
    GATHER_MEAT,
    FISHING,
    CHOP,
    MINE,
    FARM
}

public enum RESOURCE_TYPE
{
    FOOD,
    WOOD,
    WEALTH,
    METAL,
    CROPS
}

public class Resource : BaseController {

    public ResourceStats _resourceStats;

    [HideInInspector]
    public HARVEST_TYPE harvestType;

    [HideInInspector]
    public RESOURCE_TYPE resourceType;

    [HideInInspector]
    public Sprite[] harvestStagesSprites = new Sprite[0];

    [HideInInspector]
    public int amount = 10;

    [HideInInspector]
    public float harvestDifficulty = 1;

    protected int amountLeft = 0;

    [HideInInspector]
    public bool depleted = false;

    protected override void Start()
    {
        _basicStats = _resourceStats;

        base.Start();

        harvestType = _resourceStats.harvestType;
        resourceType = _resourceStats.resourceType;
        harvestStagesSprites = _resourceStats.harvestStagesSprites;
        amount = _resourceStats.amount;
        harvestDifficulty = _resourceStats.harvestDifficulty;

        // Center resource based on number of tiles resource occupies in each directions.
        transform.position += new Vector3(0.0f, 0.08f * (size - 1));

        Grid.instance.SetTilesOccupiedByController(this, false);

        playerID = -1;
        _spriteRenderer.sortingLayerName = "Object";
        _spriteRenderer.enabled = false;

        amountLeft = amount;

        visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(visionRange, GetMiddleNode(), size);
        UpdateVisibilityOfAllControllerOccupiedTiles();
    }

    protected override void Update()
    {
        base.Update();

        if (depleted)
        {
            Destroy();
        }
    }

    public void Harvest()
    {
        amountLeft--;

        if(selected)
        {
            UpdateStat();
        }
 
        if (amountLeft <= 0)
        {
            depleted = true;
        }

        else if (amountLeft < amount / 2.5f && harvestStagesSprites.Length == 2)
        {
            _spriteRenderer.sprite = harvestStagesSprites[1];
        }
                
        else if (amountLeft < amount / 1.4f && harvestStagesSprites.Length == 2)
        {
            _spriteRenderer.sprite = harvestStagesSprites[0];
        }
    }

    protected void UpdateResourceAmountForPlayer(int playerID)
    {
        // Todo remove
    }

    public override void Select()
    {
        base.Select();

        UpdateStat();
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
        {
            return true;
        }

        return false;
    }

    public override Vector2 GetPosition()
    {
        // Take centering position into account
        return transform.position - new Vector3(0.0f, 0.08f * (size - 1));
    }

    public override Node GetMiddleNode()
    {
        // Take centering position into account
        return Grid.instance.GetNodeFromWorldPoint(transform.position - new Vector3(0.0f, 0.08f * (size - 1)));
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = amountLeft;
        return stats;
    }

    public override void Destroy()
    {
        if(selected)
        {
            ControllerUIManager.instance.ChangeView(ControllerUIManager.CONTROLLER_UI_VIEW.NONE, null);
        }

        base.Destroy();
    }
}
