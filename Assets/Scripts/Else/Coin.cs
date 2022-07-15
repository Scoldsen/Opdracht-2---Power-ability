using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private SphereCollider sphereCollider;
    public GameObject coinObject;
    public GameObject particleObject;
    public LayerMask layerMask;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (layerMask.Contains(other.gameObject.layer))
        {
            sphereCollider.enabled = false;
            coinObject.SetActive(false);
            particleObject.SetActive(true);
        }
    }
}
