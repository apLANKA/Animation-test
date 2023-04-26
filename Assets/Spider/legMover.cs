using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class legMover : MonoBehaviour
{
    public Transform rayCastPoint;
    public Transform target;
    public Vector3 restingPosition;
    public LayerMask mask;
    public Vector3 newPosition;
    public Transform steppingPoint;
    public bool legGrounded;
    public GameObject player;
    public float offset;
    public float moveDistance;
    public static int currentMoveValue = 1;
    public int moveValue;
    public float speed;
    public legMover otherLeg;
    public bool hasMoved;
    public bool moving;
    public bool movingDown;
    void Start()
    {
        restingPosition = target.position;
        steppingPoint.position = new Vector3(restingPosition.x + offset, restingPosition.y, restingPosition.z);
    }

    
    void Update()
    {
        newPosition = CalculatePoint(steppingPoint.position);

        if (Vector3.Distance(restingPosition, newPosition) > moveDistance || moving && legGrounded)
        {
            Step(newPosition);
        }
        UpdateIK();
    }

    public Vector3 CalculatePoint(Vector3 position)
    {
        Vector3 dir = position - rayCastPoint.position;
        RaycastHit hit;
        if (Physics.SphereCast(rayCastPoint.position, 1f, dir, out hit, 5f, mask))
        {
            position = hit.point;
            legGrounded = true;
        }
        else
        {
            position = restingPosition;
            legGrounded = false;
        }

        return position;
    }

    public void Step(Vector3 position)
    {
        if (currentMoveValue == moveValue)
        {
            legGrounded = false;
            hasMoved = false;
            moving = true;

            target.position = Vector3.MoveTowards(target.position, position + Vector3.up, speed * Time.deltaTime);
            restingPosition = Vector3.MoveTowards(target.position, position + Vector3.up, speed * Time.deltaTime);

            if (target.position == position + Vector3.up)
            {
                movingDown = true;
            }

            if (movingDown == true)
            {
                target.position = Vector3.MoveTowards(target.position, position, speed * Time.deltaTime);
                restingPosition = Vector3.MoveTowards(target.position, position, speed * Time.deltaTime);
            }

            if (target.position == position)
            {
                legGrounded = true;
                hasMoved = true;
                moving = false;
                movingDown = false;

                if (currentMoveValue == moveValue && otherLeg.hasMoved == target)
                {
                    currentMoveValue = currentMoveValue * -1 + 3;
                }
            }
        }
    }

    public void UpdateIK()
    {
        target.position = restingPosition;
    }
}
