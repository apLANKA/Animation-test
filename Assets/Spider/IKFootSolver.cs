using UnityEngine;

public class IKFootSolver : MonoBehaviour
{
    [SerializeField] private LayerMask terrainLayer = default;
    [SerializeField] private Transform body = default;
    [SerializeField] private IKFootSolver otherFoot = default;
    [SerializeField] private float speed = 1;
    [SerializeField] private float stepDistance = 4;
    [SerializeField] private float stepLength = 4;
    [SerializeField] private float stepHeight = 1;
    [SerializeField] private Vector3 footOffset = default;
    private float footSpacing;
    private Vector3 oldPosition;
    private Vector3 currentPosition;
    private Vector3 newPosition;
    private Vector3 oldNormal;
    private Vector3 currentNormal;
    private Vector3 newNormal;
    private float lerp;

    private void Start()
    {
        footSpacing = transform.localPosition.x;
        currentPosition = newPosition = oldPosition = transform.position;
        currentNormal = newNormal = oldNormal = transform.up;
        lerp = 1;
    }


    private void Update()
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
        var directionZ = body.InverseTransformPoint(newPosition).z < 0 ? 1 : -1;

        var ray = new Ray(body.position + (body.right * footSpacing) + (stepLength * directionZ * body.forward),
            Vector3.down);
        if (!Physics.Raycast(ray, out var info, 10, terrainLayer.value)) return;
        if (Vector3.Distance(newPosition, new Vector3(info.point.x, info.point.y, body.position.z)) <= stepDistance ||
            otherFoot.IsLegLifted() ||
            IsLegLifted()) return;
        lerp = 0;
        Debug.Log("body.InverseTransformPoint(info.point).z" + Mathf.Round(body.InverseTransformPoint(info.point).z));
        newPosition = info.point + footOffset;
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