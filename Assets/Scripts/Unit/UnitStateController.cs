using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateController : BaseController
{
    [HideInInspector]
    public Animator _animator;

    [HideInInspector]
    public Pathfinding _pathfinder;

    public GameObject healthBar;
    HealthBar _healthBar;

    [Header("Stats")]
    public UnitStats _unitStats;

    [HideInInspector]
    public UnitIdle idleState;

    [HideInInspector]
    public UnitMoveToPosition moveToPositionState;

    [HideInInspector]
    public UnitMoveToController moveToControllerState;

    [HideInInspector]
    public UnitBuild buildState;

    [HideInInspector]
    public UnitGather gatherState;

    [HideInInspector]
    public UnitAttack attackState;

    [HideInInspector]
    public UnitDie dieState;

    protected UnitState currentState;

    [HideInInspector]
    public bool waitingForNextNodeToGetAvailable = false;

    //[HideInInspector]
    public bool isMoving = false;

    [HideInInspector]
    public BaseController targetController;

    [HideInInspector]
    public Vector2 targetPosition;

    [HideInInspector]
    public int hitpointsLeft = 0;

    protected override void Start()
    {
        base.Start();

        title = _unitStats.title;

        _animator = GetComponent<Animator>();
        _pathfinder = GetComponent<Pathfinding>();

        hitpointsLeft = _unitStats.maxHitpoints;
        _healthBar = GetComponent<HealthBar>();
        _healthBar.Init();
        _healthBar.SetAlignment(playerID == PlayerManager.myPlayerID);
        _healthBar.UpdateHitpointsAmount(hitpointsLeft, _unitStats.maxHitpoints);

        SetupPathfinding();
        CheckTileAndSetVisibility();
        UpdateVisibility();

        // Init states
        idleState = ScriptableObject.CreateInstance<UnitIdle>();
        moveToPositionState = ScriptableObject.CreateInstance<UnitMoveToPosition>();
        moveToControllerState = ScriptableObject.CreateInstance<UnitMoveToController>();
        buildState = ScriptableObject.CreateInstance<UnitBuild>();
        gatherState = ScriptableObject.CreateInstance<UnitGather>();
        attackState = ScriptableObject.CreateInstance<UnitAttack>();
        dieState = ScriptableObject.CreateInstance<UnitDie>();

        // Initial state
        currentState = idleState;
        currentState.OnEnter(this);

        if(playerID == PlayerManager.myPlayerID)
                PlayerManager.instance.AddFriendlyUnitReference(this, playerID);

        if(playerID > -1)
            PlayerDataManager.instance.AddPopulationForPlayer(1, playerID);

        SetupTeamColor();
    }

    void SetupTeamColor()
    {
        if(playerID > -1)
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.instance.playerData[playerID].teamColor);
        else
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.neutralPlayerColor);
    }

    void SetupPathfinding()
    {
        _pathfinder = GetComponent<Pathfinding>();
        _pathfinder.AddUnit(this);
    }

    protected override void Update()
    {
        currentState.UpdateState();

        base.Update();
    }

    public void MoveTo(BaseController targetController)
    {
        if (this.targetController == targetController)
            return;

        this.targetController = targetController;

        TransitionToState(moveToControllerState);
    }

    public void MoveTo(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        // No longer targetting a Controller
        targetController = null;

        TransitionToState(moveToPositionState);
    }

    public void TransitionToState(UnitState nextState)
    {
        currentState.OnExit();
        currentState = nextState;
        currentState.OnEnter(this);
    }

    public void ExecuteMovement(Vector2 velocity)
    {
        float moveX = velocity.x * _unitStats.moveSpeed * Time.deltaTime;
        float moveY = velocity.y * _unitStats.moveSpeed * Time.deltaTime;
        _transform.position = new Vector3(_transform.position.x + moveX, _transform.position.y + moveY, zIndex);
    }

    public void FaceMoveDirection(Vector2 velocity)
    {
        if (velocity.x < 0)
        {
            _transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        else if (velocity.x > 0)
        {
            _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void FaceController(BaseController obj)
    {
        if (_transform.position.x > obj.GetPosition().x)
        {
            _transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        }

        else
        {
            _transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        }
    }

    public void Attack()
    {
        if (targetController == null || targetController.dead)
            return;
         
        targetController.Hit(_unitStats.damage);
    }

    public override void Hit(int damageValue)
    {
        hitpointsLeft -= damageValue;

        if(hitpointsLeft <= 0)
        {
            Kill();
        }

        else
        {
            _healthBar.UpdateHitpointsAmount(hitpointsLeft, _unitStats.maxHitpoints);
        }
    }

    protected void Kill()
    {
        hitpointsLeft = 0;
        _healthBar.Deactivate();

        if (PlayerManager.myPlayerID == playerID)
            PlayerManager.instance.RemoveFriendlyUnitReference(this, playerID);

        if(playerID > 0)
            PlayerDataManager.instance.AddPopulationForPlayer(-1, playerID);

        TransitionToState(dieState);
    }

    public void UpdateVisibility()
    {
        if (playerID != PlayerManager.myPlayerID)
            return;

        Node currentNode = GetPrimaryNode();
        List<Tile> visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(_unitStats.visionRange, currentNode);

        if(!currentNode.parentTile.traversed)
        {
            for (int i = 0; i < visibleTiles.Count; i++)
            {
                visibleTiles[i].SetExplored();
            }

            currentNode.parentTile.traversed = true;
        }
    }

    IEnumerator DetectNearbyEnemies()
    {
        for (;;)
        {
            LookForNearbyEnemies();
            yield return new WaitForSeconds(.1f);
        }
    }

    // Todo include animals
    void LookForNearbyEnemies()
    {
        List<Tile> visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(_unitStats.visionRange, GetPrimaryNode());
        List<UnitStateController> enemyUnitsDetected = new List<UnitStateController>();
        for (int i = 0; i < visibleTiles.Count; i++)
        {
            List<UnitStateController> units = visibleTiles[i].GetUnitsStandingOnTile();
            for(int j = 0; j < units.Count; j++)
            {
                if(units[j].playerID != playerID)
                    enemyUnitsDetected.Add(units[j]);
            }
        }

        if(enemyUnitsDetected.Count > 0 && playerID == PlayerManager.myPlayerID)
            ChaseClosestEnemy(enemyUnitsDetected);
    }

    void ChaseClosestEnemy(List<UnitStateController> enemyUnits)
    {
        UnitStateController closestEnemy = enemyUnits[0];
        float closestDistance = Grid.instance.GetDistanceBetweenNodes(closestEnemy.GetPrimaryNode(), this.GetPrimaryNode());
        for(int i = 1; i < enemyUnits.Count; i++)
        {
            float distance = Grid.instance.GetDistanceBetweenNodes(closestEnemy.GetPrimaryNode(), this.GetPrimaryNode());

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyUnits[i];
            }
        }

        Debug.Log(closestDistance);

        if(closestDistance <= (_unitStats.attackRange * 20))
            MoveTo(closestEnemy);
    }

    public bool IntersectsObject(BaseController other)
    {
        if (Grid.instance.GetPositionIntersectsWithTilesFromBox(
                _pathfinder.currentStandingOnNode.gridPosPoint,
                other.GetPrimaryNode().gridPosPoint,
                other.size))
        {
            return true;
        }

        return false;
    }

    public override bool IntersectsPoint(Grid.FPoint point)
    {
        return _pathfinder.currentStandingOnNode.gridPosPoint.x == point.x 
            && _pathfinder.currentStandingOnNode.gridPosPoint.y == point.y;
    }

    public override void Select()
    {
        base.Select();

        _healthBar.Activate();
    }

    public override void Deselect()
    {
        base.Deselect();

        _healthBar.Deactivate();
    }

    void OnDrawGizmos()
    {
        if (_transform != null)
        {
            Gizmos.color = new Color(0.5f, 0.2f, 1.0f, 0.8f);
            Gizmos.DrawWireCube(_transform.position, new Vector3(0.32f, 0.32f, 0.0f));
        }
    }

    public override Node GetPrimaryNode()
    {
        return _pathfinder.currentStandingOnNode;
    }

    public override Tile GetPrimaryTile()
    {
        return _pathfinder.currentStandingOnNode.parentTile;
    }

    public override int[] GetUniqueStats()
    {
        int[] stats = new int[1];
        stats[0] = _unitStats.damage;
        return stats;
    }
}