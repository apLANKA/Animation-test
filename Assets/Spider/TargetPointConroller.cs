using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPointConroller : MonoBehaviour
{
    public LayerMask groundLayer;
    public float distanceThreshold = 0.1f;
    public float height;
    public Vector3 hitPoint;
    public float legDistance;
    public Transform legObject;

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down * height, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //Debug.Log(hit.distance);
            //transform.position =
                //new Vector3(transform.position.x, hit.point.y + distanceThreshold, transform.position.z);
                if (hit.collider.tag == "ground")
                {
                    hitPoint = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                    
                }
                legDistance = Vector3.Distance(hitPoint, legObject.position); 
                //Debug.Log(hitPoint);
        }
        
    }
}
