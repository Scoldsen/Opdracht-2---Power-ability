using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : MonoBehaviour
{
    Vector3 hitPos;
    public LayerMask hitMask;

    List<Vector3> hits = new List<Vector3>();
    public float vecLength = 2;
    public int nRays = 10;

    Vector3 opDir = Vector3.zero;

    private void FixedUpdate()
    {
        hits.Clear();
        opDir = Vector3.zero;
        float anglePerRay = 360 / nRays;

        for (int i = 0; i < nRays; i++)
        {
            float currentAngle = i * anglePerRay;
            var startVector = transform.TransformDirection(transform.up);
            var rightVector = transform.right;
            rightVector.y = 0;
            var rotationVector = Quaternion.AngleAxis(currentAngle, transform.right) * startVector;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, rotationVector, out hit, vecLength, hitMask))
            {
                hits.Add(hit.point);
                hitPos = hit.point;
                opDir += rotationVector;
            }
        }

        opDir = opDir.normalized;

        if (opDir != Vector3.zero)
        {
            //transform.up = opDir.normalized * -1;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        
        foreach (var i in hits)
        {
            Gizmos.DrawLine(transform.position, i);
        }

        Gizmos.color = Color.red;

        float anglePerRay = 360 / nRays;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + opDir);
        /*
        for (int i = 0; i < nRays; i++)
        {
            float currentAngle = i * anglePerRay;
            var startVector = transform.TransformDirection(transform.up);
            var rotationVector = Quaternion.AngleAxis(currentAngle, transform.right) * startVector * vecLength;
            Gizmos.DrawLine(transform.position, transform.position + rotationVector);
        }
        */
    }
}
