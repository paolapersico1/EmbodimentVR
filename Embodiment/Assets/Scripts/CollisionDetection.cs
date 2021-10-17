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

    /*void OnTriggerEnter(Collider other)
    {
        hasCollided = true;
    }

    void OnTriggerExit(Collider other)
    {
        hasCollided = false;
    }*/

    public bool HasCollided()
    {
        return hasCollided;
    }

    private void OnTriggerEnter()
    {
        hasCollided = true;
        Debug.Log("collision enter");
    }

    private void OnTriggerExit()
    {
        hasCollided = false;
        Debug.Log("collision exit");
    }

    public void AddForce(Vector3 force)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(force, ForceMode.Impulse);
    }
}
