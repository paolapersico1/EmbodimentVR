using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map(GameObject vrObj)
    {
        vrTarget = vrObj.transform;
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }

}
public class VRRig : MonoBehaviour
{
    public float turnSmoothness;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    private GameObject vrHead;
    private GameObject vrLeftHand;
    private GameObject vrRightHand;

    public Vector3 headBodyOffset;

    // Start is called before the first frame update
    void Start()
    {
        vrHead = GameObject.Find("Main Camera");
        vrLeftHand = GameObject.Find("LeftHand Controller");
        vrRightHand = GameObject.Find("RightHand Controller");
        headBodyOffset = transform.position - vrHead.transform.position;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = vrHead.transform.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(vrHead.transform.up, Vector3.up).normalized, 
                                        Time.deltaTime * turnSmoothness);

        head.Map(vrHead);
        leftHand.Map(vrLeftHand);
        rightHand.Map(vrRightHand);

    }
}
