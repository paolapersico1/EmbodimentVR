using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAnimatorController : MonoBehaviour
{
    public float speedThreshold = 0.1f;
    [Range(0, 1)] 
    public float smoothing = 1;
    private Animator animator;
    private Vector3 previousPos;
    private VRRig vrRig;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        vrRig = GetComponent<VRRig>();
        //previousPos = vrRig.head.vrTarget.position;
        previousPos = vrRig.head.rigTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        //compute the speed
        Vector3 pos = (vrRig.head.vrTarget != null) ? vrRig.head.vrTarget.position : vrRig.head.rigTarget.position;
        Vector3 headsetSpeed = (pos - previousPos) / Time.deltaTime;

        headsetSpeed.y = 0;
        //Local speed
        Vector3 headsetLocalSpeed = transform.InverseTransformDirection(headsetSpeed);
        previousPos = pos;

        //Set Animator Values
        float previousDirectionX = animator.GetFloat("DirectionX");
        float previousDirectionY = animator.GetFloat("DirectionY");

        animator.SetBool("isMoving", headsetLocalSpeed.magnitude > speedThreshold);
        animator.SetFloat("DirectionX", Mathf.Lerp(previousDirectionX, Mathf.Clamp(headsetLocalSpeed.x, -1, 1), smoothing));
        animator.SetFloat("DirectionY", Mathf.Lerp(previousDirectionY, Mathf.Clamp(headsetLocalSpeed.z, -1, 1), smoothing));
    }
}
