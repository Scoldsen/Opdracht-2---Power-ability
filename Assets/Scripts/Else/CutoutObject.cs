using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutoutObject : MonoBehaviour
{
    [SerializeField]
    private Transform targetObject;

    [SerializeField]
    private LayerMask wallMask;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector2 cutOutPos = mainCamera.WorldToViewportPoint(targetObject.position);
        //Debug.Log($"cutoutpos = {cutOutPos}");
        //cutOutPos.y /= (Screen.width / Screen.height);

        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Renderer rend = hitObjects[i].transform.GetComponent<Renderer>();
            if (rend == null) continue;
            Material[] materials = rend.materials;

            for (int m = 0; m < materials.Length; ++m)
            {
                materials[m].SetVector("_CutoutPosition", cutOutPos);
                //materials[m].SetFloat("_CutoutSize", 0.1f);
                //materials[m].SetFloat("_FalloffSize", 0.05f);
            }
        }
    }
}
