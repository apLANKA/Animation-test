using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] Transform body = default;
    [SerializeField] IKFootSolver otherFoot = default;
    [SerializeField] float speed = 1;
    [SerializeField] float stepDistance = 4;
    [SerializeField] float stepLength = 4;
    [SerializeField] float stepHeight = 1;
    [SerializeField] Vector3 footOffset = default;
    float footSpacing;
    Vector3 oldPosition;
    Vector3 currentPosition;
    Vector3 newPosition;
    Vector3 oldNormal;
    Vector3 currentNormal;
    Vector3 newNormal;
    float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }


    void Update()
    {
        transform.position = currentPosition;
        transform.up = currentNormal;

        SetNewFootPosition();

        if (IsLegLifted())
        {
            PerformLegLift();
        }
        else
        {
            oldPosition = newPosition;
            oldNormal = newNormal;
        }
    }

    private void SetNewFootPosition()
    {
        var ray = new Ray(body.position + (body.right * footSpacing), Vector3.down);
        if (!Physics.Raycast(ray, out var info, 10, terrainLayer.value)) return;
        if (Vector3.Distance(newPosition, info.point) <= stepDistance || otherFoot.IsLegLifted() ||
            IsLegLifted()) return;
        lerp = 0;
        var directionZ = body.InverseTransformPoint(info.point).z > body.InverseTransformPoint(newPosition).z
            ? 1
            : -1;
        newPosition = info.point + (stepLength * directionZ * body.forward) + footOffset;
        newNormal = info.normal;
    }

    private void PerformLegLift()
    {
        Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
        tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

        currentPosition = tempPosition;
        currentNormal = Vector3.Lerp(oldNormal, newNormal, lerp);
        lerp += Time.deltaTime * speed;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.5f);
    }


    public bool IsLegLifted()
    {
        return lerp < 1;
    }
}