using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStateController : BaseController
{
    [HideInInspector]
    public Animator _animator;

    [HideInInspector]
    public Pathfinding _pathfinder;

    [Header("Stats")]
    public UnitStats _unitStats;

    [Header("States")]
    public UnitIdle idleState;
    public UnitMoveToPosition moveToPositionState;
    public UnitMoveToController moveToControllerState;
    public UnitBuild buildState;
    public UnitGather gatherState;

    protected UnitState currentState;

    [HideInInspector]
    public bool waitingForNextNodeToGetAvailable = false;

    [HideInInspector]
    public BaseController targetController;

    [HideInInspector]
    public Vector2 targetPosition;

    [HideInInspector]
    public int hitpointsLeft = 0;

    protected override void Start()
    {
        base.Start();

        _animator = GetComponent<Animator>();
        _pathfinder = GetComponent<Pathfinding>();

        hitpointsLeft = _unitStats.maxHitpoints;

        SetupPathfinding();

        // Initial state
        currentState = idleState;
        currentState.OnEnter(this);
    }

    void SetupPathfinding()
    {
        WorldManager.Manager.AddFriendlyUnitReference(this, playerID);
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
        this.targetController = targetController;

        TransitionToState(moveToControllerState);
    }

    public void MoveTo(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;

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

    void FaceObject(Object obj)
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

    protected void Kill()
    {
        WorldManager.Manager.RemoveFriendlyUnitReference(this, playerID);
        Destroy(gameObject);
    }

    public bool IntersectsObject(Object other)
    {
        if (WorldManager.Manager._grid.GetPositionIntersectsWithTilesFromBox(
                _pathfinder.currentStandingOnNode.gridPosPoint,
                other.GetPrimaryNode().gridPosPoint,
                other.tilesOccupiedWidth))
        {
            return true;
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (_transform != null)
        {
            Gizmos.color = new Color(0.5f, 0.2f, 1.0f, 0.8f);
            Gizmos.DrawWireCube(_transform.position, new Vector3(0.32f, 0.32f, 0.0f));
        }
    }
}