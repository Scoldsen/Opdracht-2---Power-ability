using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerScript : MonoBehaviour
{
    public float sphereCastRadius = 1;
    public float maxDistance = 1;
    public LayerMask layerMask;
    public int nLines = 8;

    public Vector3 upDirection;
    public Camera cam;

    private Rigidbody rb;
    private Vector3 forwardDirection;

    private void CalculateDirection()
    {
        upDirection = Vector3.zero;

        float radsPerRay = 2 * Mathf.PI / nLines;
        float degsPerRay = 360 / nLines;

        Vector3 startPoint = transform.position;

        for (int i = 0; i < nLines; i++)
        {
            float currentAngle = i * radsPerRay;
            float xPos = Mathf.Cos(currentAngle);
            float yPos = Mathf.Sin(currentAngle);

            Vector3 rayEndPosition = new Vector3(xPos, yPos, 0);

            for (int j = 0; j < nLines; j++)
            {
                var newEndPos = Quaternion.AngleAxis(j * degsPerRay, Vector3.up) * rayEndPosition;
                RaycastHit hit;
                
                if (Physics.Raycast(transform.position, newEndPos, out hit, maxDistance, layerMask))
                {
                    upDirection += newEndPos;
                }
            }
        }

        upDirection.Normalize();
        rb.AddForce(upDirection * 9.81f, ForceMode.Force);
    }

    private void HandleMovement()
    {
        forwardDirection = cam.transform.forward;
        float zMovement = 0;

        if (Input.GetKey(KeyCode.W))
        {
            zMovement += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            zMovement -= 1;
        }

        rb.AddForce(forwardDirection * zMovement * 20, ForceMode.Force);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        CalculateDirection();
        HandleMovement();
    }

    private void OnDrawGizmos()
    {   
        float radsPerRay = 2 * Mathf.PI / nLines;
        float degsPerRay = 360 / nLines;

        Vector3 startPoint = transform.position;
        int nHalf = Mathf.FloorToInt(nLines / 2.0f);

        for (int i = 0; i < nLines; i++)
        {
            float currentAngle = i * radsPerRay;
            float xPos = Mathf.Cos(currentAngle);
            float yPos = Mathf.Sin(currentAngle);

            Vector3 rayEndPosition = new Vector3(xPos, yPos, 0);
            
            for (int j = 0; j < nHalf; j++)
            {
                if ((j == 0 || j == nHalf) && i != 0) continue;
                var newEndPos = Quaternion.AngleAxis(j * degsPerRay, Vector3.right) * rayEndPosition;
                Debug.DrawLine(startPoint, startPoint + newEndPos, Color.red);
            }
        }

        Debug.DrawLine(startPoint, startPoint + upDirection, Color.blue);
    }
}
