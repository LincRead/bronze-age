using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    Transform _transform;
    Vector2 velocity = Vector2.zero;
    Node targetNode;
    BaseController parentController;
    BaseController targetController;
    int damage = 0;
    float moveSpeed = 2f;
    bool rotated = false;

    public void SetTarget(BaseController parentController, BaseController targetController, Node targetNode, int damage)
    {
        this.parentController = parentController;
        this.targetController = targetController;
        this.targetNode = targetNode;
        this.damage = damage;

        _transform = GetComponent<Transform>();
        _transform.position = new Vector3(_transform.position.x, _transform.position.y, _transform.position.y);
    }
	
	void Update ()
    {
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
        if (Vector2.Distance(_transform.position, targetNode.worldPosition) < 0.1f)
        {
            return true;
        }

        return false;
    }

    void HitControllerOnTargetNode()
    {
        if (targetController != null && !targetController.dead)
        {
            if (targetNode.unitControllerStandingHere == targetController)
            {
                targetController.Hit(damage, parentController);
            }

            else if (targetNode.parentTile.controllerOccupying == targetController)
            {
                targetController.Hit(damage, parentController);
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
