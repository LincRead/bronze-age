﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateController : BaseController
{
    public UnitStats _unitStats;

    [Header("Debug")]

    [HideInInspector]
    public Animator _animator;

    [HideInInspector]
    public Pathfinding _pathfinder;

    [HideInInspector]
    public GameObject healthBar;
    protected HealthBar _healthBar;

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
    public UnitMoveToResourcePosition moveToResourcePositionState;

    [HideInInspector]
    public UnitMoveBackToResource moveBackToResource;

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

    //[HideInInspector]
    public UnitState currentState;

    //[HideInInspector]
    public bool isMoving = false;

    [HideInInspector]
    public BaseController targetController;

    [HideInInspector]
    public Vector2 targetPosition;

    [HideInInspector]
    public Node targetNode;

    [HideInInspector]
    public int maxHitpoints;

    [HideInInspector]
    public int hitpointsLeft = 0;

    [HideInInspector]
    public List<BaseController> ignoreControllers = new List<BaseController>();

    [HideInInspector]
    public float distanceToTarget = 10000;

    public GameObject shadow;

    [HideInInspector]
    public int resoureAmountCarrying = 0;

    [HideInInspector]
    public RESOURCE_TYPE resourceTypeCarrying = RESOURCE_TYPE.FOOD;

    [HideInInspector]
    bool gatheringResources;

    [HideInInspector]
    public Resource lastResouceGathered;

    [HideInInspector]
    public Vector3 lastResourceGatheredPosition;

    [HideInInspector]
    public bool harvestingResource = false;

    [HideInInspector]
    public string resourceTitleCarrying = "none";

    [HideInInspector]
    public BaseController rallyToController = null;

    private bool rallyToPositionAtInit = false;
    private Vector3 rallyToPosition;

    AudioSource _audioSource;

    [Header("Sounds effects")]
    public AudioClip soundChop;

    protected override void Start()
    {
        _basicStats = _unitStats;

        base.Start();

        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _pathfinder = GetComponent<Pathfinding>();

        // Hitpoints
        maxHitpoints = _unitStats.maxHitpoints;
        hitpointsLeft = maxHitpoints;

        SetupPathfinding();

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
            moveToResourcePositionState = ScriptableObject.CreateInstance<UnitMoveToResourcePosition>();
            moveBackToResource = ScriptableObject.CreateInstance<UnitMoveBackToResource>();

            // Extra HP if technology researched
            if(playerID > -1)
            {
                maxHitpoints += PlayerDataManager.instance.GetPlayerData(playerID).extraVillagerHP;
                hitpointsLeft += PlayerDataManager.instance.GetPlayerData(playerID).extraVillagerHP;
            }
        }

        // Special states
        if (_unitStats.isRanged)
        {
            rangedMoveToControllerState = ScriptableObject.CreateInstance<RangedUnitMoveToController>();
            rangedMoveToNearbyEnemyState = ScriptableObject.CreateInstance<RangedUnitMoveToNearbyEnemy>();
            rangedAttackState = ScriptableObject.CreateInstance<RangedUnitAttack>();
        }

        // Setup initial state
        currentState = idleState;
        currentState.OnEnter(this);

        if (rallyToPositionAtInit)
        {
            StartMovingToRallyPoint();
        }

        if (playerID == PlayerManager.myPlayerID)
        {
            PlayerManager.instance.AddFriendlyUnitReference(this, playerID);
        }

        if(playerID > -1)
        {
            PlayerDataManager.instance.AddPopulationForPlayer(1, playerID);
            PlayerDataManager.instance.AddFoodIntakeForPlayer(-_unitStats.foodConsuming, playerID);
        }

        SetupHealthBar();
        SetupTeamColor();

        UpdateVisibility();
        UpdateVisibilityOfAllControllerOccupiedTiles();
    }

    void StartMovingToRallyPoint()
    {
        // Gatherers
        if (_unitStats.isVillager)
        {
            if (rallyToController != null)
            {
                if(rallyToController.controllerType == CONTROLLER_TYPE.RESOURCE)
                {
                    Debug.Log("RALLY TO RESOURCE CONT");
                    MoveToResource(rallyToController);
                }

                else
                {
                    MoveTo(rallyToController);
                }
            }

            else if (!resourceTitleCarrying.Equals("none"))
            {
                MoveToResourcePos(rallyToPosition);
            }

            else
            {
                MoveTo(rallyToPosition);
            }
        }

        // Warriors
        else
        {
            if (rallyToController != null)
            {
                MoveTo(rallyToController);
            }

            else
            {
                MoveTo(rallyToPosition);
            } 
        }
    }

    public void RallyTo(Vector3 pos)
    {
        rallyToPositionAtInit = true;
        rallyToPosition = pos;
    }

    void SetupHealthBar()
    {
        // Setup health
        GameObject healthBar = GameObject.Instantiate(_unitStats.healthBar, _transform.position, Quaternion.identity);
        healthBar.transform.parent = gameObject.transform;
        _healthBar = healthBar.GetComponent<HealthBar>();
        _healthBar.Init(size);
        _healthBar.SetAlignment(playerID == PlayerManager.myPlayerID);
        _healthBar.UpdateHitpointsPercent(hitpointsLeft, maxHitpoints);
    }

    void SetupTeamColor()
    {
        if(playerID > -1)
        {
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.instance.playerData[playerID].teamColor);
        }
            
        else
        {
            _spriteRenderer.material.SetColor("_TeamColor", PlayerDataManager.neutralPlayerColor);
        }
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

    public virtual void MoveTo(BaseController targetController)
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
            if (targetController.controllerType != CONTROLLER_TYPE.RESOURCE
                && targetController.playerID != PlayerManager.myPlayerID)
            {
                TransitionToState(rangedMoveToControllerState);
                return;
            }
        }

        TransitionToState(moveToControllerState);
    }

    public virtual void MoveTo(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        // No longer targetting a Controller
        targetController = null;

        TransitionToState(moveToPositionState);
    }

    public void MoveToResource(BaseController targetController)
    {
        this.targetController = targetController;

        TransitionToState(moveBackToResource);
    }

    // Need this because:
    // Sometimes we go back to a resource that has been depleted while delivering resources,
    // so we go back to the position of the destroyed resource instead.
    public void MoveToResourcePos(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        // No longer targetting a Controller
        targetController = null;

        TransitionToState(moveToResourcePositionState);
    }

    public void MoveToInAttackMode(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

        // No longer targetting a Controller
        targetController = null;

        TransitionToState(attackMoveState);
    }

    public void TransitionToState(UnitState nextState)
    {
        distanceToTarget = 1000; //  Reset
        currentState.OnExit();
        currentState = nextState;
        currentState.OnEnter(this);
    }

    public virtual void ExecuteMovement(Vector2 velocity)
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
        if (targetController == null || targetController.dead)
        {
            return;
        }

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
            UpdateHealthBar();
        }
    }

    public void UpdateHealthBar()
    {
        _healthBar.UpdateHitpointsPercent(hitpointsLeft, maxHitpoints);
    }

    public override void Cancel()
    {
        TransitionToState(idleState);
    }

    protected void Kill()
    {
        hitpointsLeft = 0;
        _healthBar.Deactivate();

        if (PlayerManager.myPlayerID == playerID)
        {
            PlayerManager.instance.RemoveFriendlyUnitReference(this, playerID);
        }

        if(playerID > -1)
        {
            PlayerDataManager.instance.AddPopulationForPlayer(-1, playerID);
            PlayerDataManager.instance.AddFoodIntakeForPlayer(_unitStats.foodConsuming, playerID);
        }
            
        if (selected)
        {
            PlayerManager.instance._controllerSelecting.RemoveFromSelectedUnits(this);
        }

        RemoveFromPathfinding();
        TransitionToState(dieState);
    }

    protected void RemoveFromPathfinding()
    {
        _pathfinder.currentStandingOnNode.parentTile.unitsStandingHere.Remove(this);
        _pathfinder.currentStandingOnNode.unitControllerStandingHere = null;
    }

    public void UpdateVisibility()
    {
        if (playerID == PlayerManager.myPlayerID)
        {
            DecreaseVisibilityOfTiles();
        }

        Node currentNode = _pathfinder.currentStandingOnNode;

        visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(_basicStats.visionRange, currentNode, size);

        if (playerID == PlayerManager.myPlayerID)
        {
            IncreaseVisibilityOfTiles();
        }
    }

    public override void SetVisible(bool value)
    {
        if (value)
        {
            if(!_spriteRenderer.enabled)
            {
                _spriteRenderer.enabled = true;

                shadow.GetComponent<SpriteRenderer>().enabled = true;
            }
        }

        else
        {
            if (_spriteRenderer.enabled)
            {
                _spriteRenderer.enabled = false;

                shadow.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    IEnumerator DetectNearbyEnemies()
    {
        for (;;)
        {
            yield return new WaitForSeconds(.2f);
            LookForNearbyEnemyControllers();
        }
    }

    public void LookForNearbyEnemyControllers()
    {
        List<BaseController> enemyControllersDetected = new List<BaseController>();

        bool unitControllerDetected = false;
        for (int i = 0; i < visibleTiles.Count; i++)
        {
            // Check for building first
            if(!unitControllerDetected // Only add if no units close by, units are always prioritised targets
                && visibleTiles[i].controllerOccupying != null
                && visibleTiles[i].controllerOccupying.controllerType == CONTROLLER_TYPE.BUILDING
                && visibleTiles[i].controllerOccupying.playerID != this.playerID)
            {
                enemyControllersDetected.Add(visibleTiles[i].controllerOccupying);
            }

            // Check for units
            else
            {
                List<UnitStateController> units = visibleTiles[i].unitsStandingHere;
                for (int j = 0; j < units.Count; j++)
                {
                    if (units[j].playerID != this.playerID)
                    {
                        enemyControllersDetected.Add(units[j]);
                        unitControllerDetected = true;
                    }
                }
            }
        }

        if(enemyControllersDetected.Count > 0)
        {
            ChaseClosestEnemyController(enemyControllersDetected, unitControllerDetected);
        }
    }

    void ChaseClosestEnemyController(List<BaseController> enemyControllers, bool unitControllerDetected)
    {
        float closestDistance = 10000;
        BaseController closestEnemyController = null;
        for(int i = 0; i < enemyControllers.Count; i++)
        {
            float distance = Grid.instance.GetDistanceBetweenNodes(enemyControllers[i].GetPrimaryNode(), _pathfinder.currentStandingOnNode);

            // If a unit is part of the list, ignore all buildings
            if (distance < closestDistance && (enemyControllers[i].controllerType != CONTROLLER_TYPE.BUILDING || !unitControllerDetected))
            {
                closestDistance = distance;
                closestEnemyController = enemyControllers[i];
            }
        }

        if (closestDistance <= (_unitStats.attackTriggerRadius * 10))
        {
            targetController = closestEnemyController;

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

    public void SeekClosestResource(string resourceTitle)
    {
        float closestDistance = 10000;
        BaseController closestResource = null;

        visibleTiles = Grid.instance.GetAllTilesBasedOnVisibilityFromNode(_basicStats.visionRange, _pathfinder.currentStandingOnNode, size);

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

        // Didn't find any resource close by
        else
        {
            TransitionToState(idleState);
        }
    }

    public void seekClosestResourceDeliveryPoint()
    {
        List<Building> resourcePoints = PlayerDataManager.instance.GetPlayerData(playerID).friendlyResourceDeliveryPoints;
        float distance = 100000;
        Building gotoBuilding = null;

        for (int i = 0; i < resourcePoints.Count; i++)
        {
            float checkDistance = Grid.instance.GetDistanceBetweenControllers(this, resourcePoints[i]);

            if (checkDistance < distance)
            {
                distance = checkDistance;
                gotoBuilding = resourcePoints[i];
            }
        }

        if(gotoBuilding != null)
        {
            MoveTo(gotoBuilding);
        }

        else
        {
            TransitionToState(idleState);
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

    public void PlayIdleAnimation()
    {
        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
        {
            _animator.Play("idle", -1, 0.0f);
        }
    }

    public void PlaySound(string sound)
    {
        _audioSource.volume = 0.3f;

        switch(sound)
        {
            case "chop": _audioSource.PlayOneShot(soundChop); break;
        }
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

    public override Node GetMiddleNode()
    {
        return _pathfinder.currentStandingOnNode;
    }

    public override Tile GetPrimaryTile()
    {
        return _pathfinder.currentStandingOnNode.parentTile;
    }

    public override int[] GetUniqueStats()
    {
        int[] stats =  new int[3];

        stats[0] = _unitStats.damage;

        if (_unitStats.isRanged)
        {
            stats[1] = _unitStats.range;
        }

        else
        {
            stats[1] = -1;
        }

        if(resoureAmountCarrying > 0)
        {
            stats[2] = resoureAmountCarrying;

            switch (resourceTypeCarrying)
            {
                case RESOURCE_TYPE.FOOD: statSprites[2] = ControllerUIManager.instance.foodIcon; break;
                case RESOURCE_TYPE.WOOD: statSprites[2] = ControllerUIManager.instance.woodIcon; break;
                case RESOURCE_TYPE.WEALTH: statSprites[2] = ControllerUIManager.instance.wealthIcon; break;
                case RESOURCE_TYPE.METAL: statSprites[2] = ControllerUIManager.instance.metalIcon; break;
            }
        }

        else
        {
            stats[2] = -1;
        }

        return stats;
    }
}