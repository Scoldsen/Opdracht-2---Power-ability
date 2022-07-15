using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RisingLiquid : MonoBehaviour
{
    public bool rising = false;
    public float moveSpeedY = 1;
    public float maxHeight = 10;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    public void InitForLevel(float _maxHeight)
    {
        rising = false;
        maxHeight = _maxHeight;
        transform.position = startPosition;
    }

    public void StartRising()
    {
        rising = true;
    }
    /*
    private void FixedUpdate()
    {
        if (rising && transform.position.y < maxHeight) transform.position = new Vector3(transform.position.x, transform.position.y + moveSpeedY * Time.deltaTime, transform.position.z);
    }*/
}
