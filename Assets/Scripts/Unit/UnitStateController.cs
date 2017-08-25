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

    public UnitStats _unitStats;

    [HideInInspector]
    public UnitIdle idleState;

    [HideInInspector]
    public UnitMoveToPosition moveToPositionState;

    [HideInInspector]
    public UnitMoveToController moveToControllerState;

    [HideInInspector]
    public RangedUnitMoveToController rangedMoveToControllerState;

    [HideInInspector]
    public UnitMoveToNearbyEnemy moveToNearbyEnemyState;

    [HideInInspector]
    public RangedUnitMoveToNearbyEnemy rangedMoveToNearbyEnemyState;

    [HideInInspector]
    public UnitAttackMode attackMoveState;

    [HideInInspector]
    public UnitAttack attackState;

    [HideInInspector]
    public RangedUnitAttack rangedAttackState;

    [HideInInspector]
    public UnitBuild buildState;

    [HideInInspector]
    public UnitGather gatherState;

    [HideInInspector]
    public UnitDie dieState;

    [HideInInspector]
    public UnitState currentState;

    [HideInInspector]
    public bool isMoving = false;

    [HideInInspector]
    public BaseController targetController;

    [HideInInspector]
    public Vector2 targetPosition;

    [HideInInspector]
    public int hitpointsLeft = 0;

    [HideInInspector]
    public List<Tile> visibleTiles = new List<Tile>();

    [HideInInspector]
    public List<BaseController> ignoreControllers = new List<BaseController>();

    [HideInInspector]
    public float distanceToTarget = 10000;

    protected override void Start()
    {
        _basicStats = _unitStats;

        base.Start();

        _animator = GetComponent<Animator>();
        _pathfinder = GetComponent<Pathfinding>();

        // Setup health
        hitpointsLeft = _unitStats.maxHitpoints;
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.Init(size);
        _healthBar.SetAlignment(playerID == PlayerManager.myPlayerID);
        _healthBar.UpdateHitpointsAmount(hitpointsLeft, _unitStats.maxHitpoints);

        SetupPathfinding();
        CheckTileAndSetVisibility();
        UpdateVisibility();

        // Init states
        idleState = ScriptableObject.CreateInstance<UnitIdle>();
        moveToPositionState = ScriptableObject.CreateInstance<UnitMoveToPosition>();
        moveToControllerState = ScriptableObject.CreateInstance<UnitMoveToController>();
        moveToNearbyEnemyState = ScriptableObject.CreateInstance<UnitMoveToNearbyEnemy>();
        attackMoveState = ScriptableObject.CreateInstance<UnitAttackMode>();

        attackState = ScriptableObject.CreateInstance<UnitAttack>();
        dieState = ScriptableObject.CreateInstance<UnitDie>();

        // Villager states
        if (_unitStats.isVillager)
        {
            buildState = ScriptableObject.CreateInstance<UnitBuild>();
            gatherState = ScriptableObject.CreateInstance<UnitGather>();
        }

        // Special states
        if (_unitStats.isRanged)
        {
            rangedMoveToControllerState = ScriptableObject.CreateInstance<RangedUnitMoveToController>();
            rangedMoveToNearbyEnemyState = ScriptableObject.CreateInstance<RangedUnitMoveToNearbyEnemy>();
            rangedAttackState = ScriptableObject.CreateInstance<RangedUnitAttack>();
        }

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
        // Don't do anything if target is already set
        // Don't target self
        if (this.targetController == targetController || this == targetController)
        {
            return;
        }

        this.targetController = targetController;

        // Special cases for ranged units
        if (_unitStats.isRanged)
        {
            if (targetController.controllerType != CONTROLLER_TYPE.STATIC_RESOURCE
                && targetController.playerID != PlayerManager.myPlayerID)
            {
                TransitionToState(rangedMoveToControllerState);
                return;
            }
        }

        TransitionToState(moveToControllerState);
    }

    public void MoveTo(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        // No longer targetting a Controller
        targetController = null;

        TransitionToState(moveToPositionState);
    }

    public void MoveToInAttackMode(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        // No longer targetting a Controller
        targetController = null;

        TransitionToState(attackMoveState);
    }

    public void SeekClosestResource(string resourceTitle)
    {
        float closestDistance = 10000;
        BaseController closestResource = null;

        visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(_unitStats.visionRange, _pathfinder.currentStandingOnNode);

        for (int i = 0; i < visibleTiles.Count; i++)
        {
            if (visibleTiles[i].controllerOccupying != null 
                && visibleTiles[i].controllerOccupying.title == resourceTitle
                && !ignoreControllers.Contains(visibleTiles[i].controllerOccupying))
            {
                float dist = Grid.instance.GetDistanceBetweenTiles(_pathfinder.currentStandingOnNode.parentTile, visibleTiles[i]);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    closestResource = visibleTiles[i].controllerOccupying;
                }
            }
        }

        if (closestResource != null)
        {
            MoveTo(closestResource);
        }

        else
        {
            TransitionToState(idleState);
        }
    }

    public void TransitionToState(UnitState nextState)
    {
        distanceToTarget = 1000; //  Reset
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

    public void AttackTarget()
    {
        if (targetController == null || targetController.dead)
        {
            return;
        }
         
        targetController.Hit(_unitStats.damage);
    }

    public void FireProjectile()
    {
        GameObject newProjectile = GameObject.Instantiate(
            _unitStats.projectile, 
            transform.position + new Vector3(0.0f, (_spriteRenderer.bounds.size.y / 2)), 
            Quaternion.identity);
        Projectile p = newProjectile.GetComponent<Projectile>();
        p.SetTarget(targetController, targetController.GetPrimaryNode(), _unitStats.damage);
    }

    public void FireProjectileTowards(Node node)
    {

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
        {
            PlayerManager.instance.RemoveFriendlyUnitReference(this, playerID);
        }

        if(playerID > 0)
        {
            PlayerDataManager.instance.AddPopulationForPlayer(-1, playerID);
        }
            
        if (selected)
        {
            PlayerManager.instance._controllerSelecting.RemoveFromSelectedUnits(this);
        }
            
        // Remove from path finding
        _pathfinder.currentStandingOnNode.parentTile.unitsStandingHere.Remove(this);
        _pathfinder.currentStandingOnNode.unitControllerStandingHere = null;

        TransitionToState(dieState);
    }

    public void UpdateVisibility()
    {
        Node currentNode = _pathfinder.currentStandingOnNode;
        visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(_unitStats.visionRange, currentNode);

        if (playerID == PlayerManager.myPlayerID)
        {
            ExploreFogOfWar();
        }
    }

    void ExploreFogOfWar()
    {
        for (int i = 0; i < visibleTiles.Count; i++)
        {
            if (!visibleTiles[i].explored)
            {
                visibleTiles[i].SetExplored();
            }
        }
    }

    IEnumerator DetectNearbyEnemies()
    {
        for (;;)
        {
            yield return new WaitForSeconds(.2f);
            LookForNearbyEnemies();
        }
    }

    public void LookForNearbyEnemies()
    {
        List<UnitStateController> enemyUnitsDetected = new List<UnitStateController>();

        for (int i = 0; i < visibleTiles.Count; i++)
        {
            List<UnitStateController> units = visibleTiles[i].unitsStandingHere;
            for(int j = 0; j < units.Count; j++)
            {
                if(units[j].playerID != this.playerID)
                {
                    enemyUnitsDetected.Add(units[j]);
                }
            }
        }

        if(enemyUnitsDetected.Count > 0)
        {
            ChaseClosestEnemy(enemyUnitsDetected);
        }
    }

    void ChaseClosestEnemy(List<UnitStateController> enemyUnits)
    {
        float closestDistance = 10000;
        UnitStateController closestEnemy = null;
        for(int i = 0; i < enemyUnits.Count; i++)
        {
            float distance = Grid.instance.GetDistanceBetweenNodes(enemyUnits[i].GetPrimaryNode(), _pathfinder.currentStandingOnNode);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemyUnits[i];
            }
        }

        if (closestDistance <= (_unitStats.attackTriggerRadius * 10))
        {
            targetController = closestEnemy;

            // Chase closest enemy
            if (_unitStats.isRanged)
            {
                TransitionToState(rangedMoveToNearbyEnemyState);
            }

            else
            {
                TransitionToState(moveToNearbyEnemyState);
            }
        }
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
        int[] stats;
        if (_unitStats.isRanged)
        {
            stats = new int[2];
            stats[0] = _unitStats.damage;
            stats[1] = _unitStats.range;
        }

        else
        {
            stats = new int[1];
            stats[0] = _unitStats.damage;
        }

        return stats;
    }
}