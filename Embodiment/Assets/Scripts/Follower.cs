using UnityEngine;
using PathCreation;

public class Follower : MonoBehaviour
{
    public PathCreator pathCreator;
    private Rigidbody rb;
    public float speed = 5;
    float distanceTravelled;

    void Update() {
        rb = GetComponent<Rigidbody>();

        distanceTravelled += speed * Time.deltaTime;
        //Vector3 direction = (pathCreator.path.GetPointAtDistance(distanceTravelled) - transform.position).normalized; 
        //rb.MovePosition(transform.position + direction * Time.deltaTime * speed);
        rb.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled);
    }
}
