using System.Collections;
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
    public UnitMoveBackToResource moveBackToResourceState;

    [HideInInspector]
    public UnitMoveToEmptyNode moveToEmptyNodeState;

    [HideInInspector]
    public UnitMoveToFarm moveToFarm;

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
    public UnitFarm farmState;

    [HideInInspector]
    public UnitDie dieState;

    //[HideInInspector]
    public UnitState currentState;

    [HideInInspector]
    public UnitState lastState;

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
    private float actualMoveSpeed = .2f;

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
    public Farm farm = null;

    [HideInInspector]
    public BaseController rallyToController = null;

    private bool rallyToPositionAtInit = false;
    private Vector3 rallyToPosition;

    protected AudioSource _audioSource;

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
        moveToEmptyNodeState = ScriptableObject.CreateInstance<UnitMoveToEmptyNode>();
        attackMoveState = ScriptableObject.CreateInstance<UnitAttackMode>();

        attackState = ScriptableObject.CreateInstance<UnitAttack>();
        dieState = ScriptableObject.CreateInstance<UnitDie>();

        // Villager states
        if (_unitStats.isVillager)
        {
            buildState = ScriptableObject.CreateInstance<UnitBuild>();
            gatherState = ScriptableObject.CreateInstance<UnitGather>();
            farmState = ScriptableObject.CreateInstance<UnitFarm>();
            moveToResourcePositionState = ScriptableObject.CreateInstance<UnitMoveToResourcePosition>();
            moveBackToResourceState = ScriptableObject.CreateInstance<UnitMoveBackToResource>();
            moveToFarm = ScriptableObject.CreateInstance<UnitMoveToFarm>();

            // Extra HP if technology researched
            if (playerID > -1)
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

        // Set actual move speed
        actualMoveSpeed = .2f + (_unitStats.moveSpeed * 0.02f);

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
        // Move away from unwalkable tiles
        if (_pathfinder.currentStandingOnNode.parentTile.controllerOccupying != null
            && !_pathfinder.currentStandingOnNode.parentTile.controllerOccupying._basicStats.walkable
            && currentState != moveToEmptyNodeState)
        {
            lastState = currentState;
            TransitionToState(moveToEmptyNodeState);
        }

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

        TransitionToState(moveBackToResourceState);
    }

    public void MoveToFarm(BaseController targetController)
    {
        this.targetController = targetController;

        TransitionToState(moveToFarm);
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
        float moveX = velocity.x * actualMoveSpeed * Time.deltaTime;
        float moveY = velocity.y * actualMoveSpeed * Time.deltaTime;

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
         
        targetController.Hit(this);
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
        p.SetParentAndTargetControllers(this, targetController);
    }

    public void FireProjectileTowards(Node node)
    {

    }

	public override void Hit(UnitStateController hitByController)
    {
		int damage = hitByController._unitStats.damage;

		if (hitByController._unitStats.isRanged) 
		{
			damage -= _unitStats.pierceArmor;
		} 

		else 
		{
			damage -= _unitStats.meleeArmor;
		}

		// Always deal at least 1 damage
		if (damage < 1) 
		{
			damage = 1;
		}

		hitpointsLeft -= damage;

        if(hitpointsLeft <= 0)
        {
            Kill();
        }

        else
        {
            UpdateHealthBar();

            if(currentState == idleState)
            {
                MoveTo(hitByController);
            }
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

        if(farm != null)
        {
            farm.hasFarmer = false;
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
			if (selected) 
			{
				PlayerManager.instance._controllerSelecting.DeselectEnemyUnit();
			}

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

            if (WorldManager.instance.CanDeliverResourceTo(resourcePoints[i], resourceTypeCarrying) && checkDistance < distance)
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
        switch (sound)
        {
            case "chop": _audioSource.PlayOneShot(soundChop); _audioSource.volume = .1f; break;
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
        int[] stats = new int[6];

	    stats[0] = _unitStats.damage;
        statSprites[0] = ControllerUIManager.instance.attackIcon;
        statsDescriptions[0] = "Attack damage";

        //stats[2] = _unitStats.damageSiege;
        //statSprites[2] = ControllerUIManager.instance.attackSiegeIcon;

        stats[1] = _unitStats.moveSpeed;
        statSprites[1] = ControllerUIManager.instance.movementSpeedIcon;
        statsDescriptions[1] = "Movement speed";

        stats[2] = _unitStats.damageSiege;
        statSprites[2] = ControllerUIManager.instance.attackSiegeIcon;
        statsDescriptions[2] = "Attack damage to buildings";

        stats[3] = _unitStats.pierceArmor;
        statSprites[3] = ControllerUIManager.instance.pierceArmorIcon;
        statsDescriptions[3] = "Armor agains ranged attacks";

        stats[5] = _unitStats.meleeArmor;
        statSprites[5] = ControllerUIManager.instance.meleeArmorIcon;
        statsDescriptions[5] = "Armour against melee attacks";


        // Special (bot right)
        if (_unitStats.isRanged)
        {
            stats[4] = _unitStats.range;
            statSprites[4] = ControllerUIManager.instance.rangeIcon;
            statsDescriptions[4] = "Attack range";
        }

        else if(_unitStats.isVillager && resoureAmountCarrying > 0)
        {
            stats[4] = resoureAmountCarrying;
            statsDescriptions[4] = "Carrying resources";

            switch (resourceTypeCarrying)
            {
                case RESOURCE_TYPE.FOOD: statSprites[4] = ControllerUIManager.instance.foodIcon; break;
                case RESOURCE_TYPE.WOOD: statSprites[4] = ControllerUIManager.instance.woodIcon; break;
                case RESOURCE_TYPE.WEALTH: statSprites[4] = ControllerUIManager.instance.wealthIcon; break;
                case RESOURCE_TYPE.METAL: statSprites[4] = ControllerUIManager.instance.metalIcon; break;
            }
        }

        else
        {
            stats[4] = -1;
        }

        return stats;
    }
}