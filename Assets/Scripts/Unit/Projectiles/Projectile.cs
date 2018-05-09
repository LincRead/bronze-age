using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Transform _transform;
    Vector2 velocity = Vector2.zero;
	Node currentNode;
	Node targetNode;
    UnitStateController parentController;
    BaseController targetController;
    int damageSiege = 1;
    float moveSpeed = 2f;
    bool rotated = false;

    public void SetParentAndTargetControllers(UnitStateController parentController, BaseController targetController)
    {
        this.parentController = parentController;
        this.targetController = targetController;
        this.targetNode = targetController.GetPrimaryNode();
        this.damageSiege = parentController._unitStats.damageSiege;

        _transform = GetComponent<Transform>();
        _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y);
    }
	
	void Update ()
    {
		currentNode = Grid.instance.GetNodeFromWorldPoint (_transform.position);

        if(IntersectsTarget())
        {
            HitControllerOnTargetNode();
        }

        else
        {
            MoveTowardsTargetNode(velocity);
        }
    }

    bool IntersectsTarget()
    {
		if(currentNode == targetNode || currentNode == targetController.GetMiddleNode())
        //if (Vector2.Distance(_transform.position, targetNode.worldPosition) < 0.1f)
        {
            return true;
        }

        return false;
    }

    void HitControllerOnTargetNode()
    {
        if (targetController != null && !targetController.dead)
        {
			// Unit
			if (currentNode.unitControllerStandingHere == targetController)
            {
                targetController.Hit(parentController);
            }

			// Building
			else if (currentNode.parentTile.controllerOccupying == targetController)
            {
                targetController.Hit(damageSiege);
            }
        }

        Destroy(gameObject);   
    }

    void MoveTowardsTargetNode(Vector2 velocity)
    {
        float dirx = targetNode.worldPosition.x - _transform.position.x;
        float diry = targetNode.worldPosition.y - _transform.position.y;

        velocity = new Vector2(dirx, diry);
        velocity.Normalize();

        float moveX = velocity.x * moveSpeed * Time.deltaTime;
        float moveY = velocity.y * moveSpeed * Time.deltaTime;

        _transform.position = new Vector3(_transform.position.x + moveX, _transform.position.y + moveY, _transform.position.y + moveY);

        if(!rotated)
        {
            rotated = true;

            float angle = Mathf.Atan2(moveY, moveX) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
}
