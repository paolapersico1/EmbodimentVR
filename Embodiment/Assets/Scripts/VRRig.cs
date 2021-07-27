using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map(Transform vrTransform)
    {
        vrTarget = vrTransform;
        //TransformPoint -> Transforms trackingPositionOffset from local space to world space
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        //Euler -> Returns a rotation that rotates z degrees around the z axis, etc.
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }

}
public class VRRig : MonoBehaviour
{
    public float turnSmoothness;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    private Transform vrHead;   //camera
    private Transform vrLeftHand;   //left controller
    private Transform vrRightHand;  //right controller

    public Transform headConstraint;    
    public Vector3 headBodyOffset; 

    // Start is called before the first frame update
    void Start()
    {

        XRRig rig = FindObjectOfType<XRRig>();
        vrHead = rig.transform.Find("Camera Offset/Main Camera");
        vrLeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
        vrRightHand = rig.transform.Find("Camera Offset/RightHand Controller");

        //difference in position between the avatar head and the avatar body
        headBodyOffset = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = headConstraint.position + headBodyOffset;


        //if the angle between the head rotation on the y axis and the body rotation on the y axis is greater than 90 degrees
        //eulerAngles represents the rotation in world space
        if (Quaternion.Angle(Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0),
                Quaternion.Euler(0, headConstraint.rotation.eulerAngles.y, 0)) > 90)
        {
            //rotate also the body by projecting the head z axis on the y axis 
            //linear interpolation with the previous position is used to smooth the movement
            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized,
                                        Time.deltaTime * turnSmoothness);
        }

        //map the real to the virtual
        head.Map(vrHead);
        leftHand.Map(vrLeftHand);
        rightHand.Map(vrRightHand);
    }
}
