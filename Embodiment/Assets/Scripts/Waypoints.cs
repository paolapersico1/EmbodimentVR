using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    public GameObject[] waypoints;
    private Rigidbody rb;
    int current = 0;
    float rotSpeed;
    public float speed;
    float WPradius = 1;

    // Update is called once per frame
    void Update()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(1, 1, 1);


        if (Vector3.Distance(waypoints[current].transform.position,
            transform.position) < WPradius) {
            current++;
            if (current >= waypoints.Length) {
                current = 0;
            }
        }

        Vector3 direction = ( waypoints[current].transform.position - transform.position).normalized; 
        rb.MovePosition(transform.position + direction * Time.deltaTime * speed);
        
        // rotate char
        Vector3 relativePos = waypoints[current].transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(relativePos);
        
    }
}
