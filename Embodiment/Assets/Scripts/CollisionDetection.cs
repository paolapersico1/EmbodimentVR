using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public bool hasCollided;

    private void Start()
    {
        hasCollided = false;
    }

    public bool HasCollided()
    {
        return hasCollided;
    }

    private void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
    }

    private void OnTriggerStay(Collider other)
    {
        hasCollided = true;
    }

    private void OnTriggerExit(Collider other)
    {
        hasCollided = false;
    }
}
