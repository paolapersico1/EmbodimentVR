using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 6f;

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
 
        Vector3 right = transform.right * h * Time.deltaTime * speed;
        Vector3 forward = transform.forward * v * Time.deltaTime * speed;
 
        transform.Translate(right + forward);
        
    }
}
