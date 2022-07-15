using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    private Vector3 lastPosition;
    private Vector3 startPosition;
    private Rigidbody rb;
    public float resetHeight = -10;
    public float maxStillTime = 2;

    private float stillTimer = 0;

    public ParticleSystem partSystem;
    public int nParticles = 100;
    private float maxSpeed = 0;

    public bool PointToCenter = false;
    public Vector3 centerPos;

    public Mesh[] rockMeshes;
    public Material[] mats;

    private Vector3 startScale = Vector3.one;
    public bool shouldRespawn = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        var meshFilter = GetComponent<MeshFilter>();
        var rockMesh = rockMeshes[Random.Range(0, rockMeshes.Length)];
        meshFilter.sharedMesh = rockMesh;

        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.materials = mats;
        /*
        var meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = rockMesh;*/
    }

    private void Start()
    {
        startPosition = transform.position;
    }

    private void JumpToStartPosition()
    {
        transform.localScale = startScale;

        rb.velocity = Vector3.zero;
        transform.position = startPosition;

        if (PointToCenter)
        {
            Vector3 forceDir = (centerPos - startPosition);
            forceDir.y = 0;
            forceDir = forceDir.normalized;
            rb.AddForce(forceDir * 5, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        var currentSpeed = Mathf.Abs(rb.velocity.magnitude);
        if (currentSpeed > maxSpeed) maxSpeed = currentSpeed;

        if (Mathf.Abs(Vector3.Distance(transform.position, lastPosition)) < .1f) stillTimer += Time.deltaTime;
        else stillTimer = 0;

        lastPosition = transform.position;
        if (transform.position.y <= resetHeight || stillTimer > maxStillTime)
        {
            if (!shouldRespawn)
            {
                Destroy(gameObject);
            }

            JumpToStartPosition();
            stillTimer = 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        transform.localScale *= 0.9f;

        var chunk = collision.gameObject.GetComponent<Chunk>();
        NewMovingSphere sphere = collision.gameObject.GetComponent<NewMovingSphere>();

        Vector3 impactForce = collision.impulse;
        Vector3 impactPosition = collision.GetContact(0).point;
        partSystem.transform.position = impactPosition;

        if (sphere != null)
        {
            //sphere.DisableClimbing(3);
            var force = rb.velocity;
            sphere.gameObject.GetComponent<Rigidbody>().AddForce(force * 1.5f, ForceMode.Impulse);
            sphere.GetHitAt(impactPosition);
        }
        
        
        for (int i = 0; i < nParticles; i++)
        {
            float speedMultiplier = Random.Range(0.2f, 1f);
            //partSystem.startSpeed = speedMultiplier * maxSpeed;
            var main = partSystem.main;
            main.startSpeed = speedMultiplier * maxSpeed;
            //partSystem.mainm
            partSystem.Emit(1);
        }

        maxSpeed = 0;
    }
}
