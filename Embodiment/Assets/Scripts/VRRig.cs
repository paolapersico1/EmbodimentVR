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
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }

}
public class VRRig : MonoBehaviour
{
    public bool cameraRigMode;

    public float avatarHeadHeight;
    public float avatarArmLength;
    public Vector3 avatarEyeHeadOffset;
    public float torsoOffset;
    
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    private Transform vrHead;
    private Transform vrLeftHand;
    private Transform vrRightHand;

    public bool bodyRotation = true;
    public int rotThreshold;
    public float turnSmoothness;
    public float handsTorsoRotation; //DEBUG
    public float crouchingThreshold;

    private Animator animator;
    private Vector3 leftHandGoalPosition;
    private Quaternion leftHandGoalRotation;
    private Vector3 rightHandGoalPosition;
    private Quaternion rightHandGoalRotation;

    private float playerArmLength;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        XRRig rig = FindObjectOfType<XRRig>();
        vrHead = rig.transform.Find("Camera Offset/Main Camera");
        vrLeftHand = rig.transform.Find("Camera Offset/LeftHand Controller/LeftHandCollider");
        vrRightHand = rig.transform.Find("Camera Offset/RightHand Controller/RightHandCollider");
    }

    public void Calibrate()
    {
        GameObject camOffset = GameObject.Find("XR Rig/Camera Offset");

        float playerHeadHeight = camOffset.GetComponent<Calibrator>().GetPlayerHeadHeight();
        playerArmLength = camOffset.GetComponent<Calibrator>().GetPlayerArmLength();

        avatarHeadHeight = head.rigTarget.position.y - transform.position.y;
        camOffset.transform.position = new Vector3(0, avatarHeadHeight - playerHeadHeight, 0);

        //how longer are the avatar's arms
        avatarArmLength = Vector3.Distance(rightHand.rigTarget.position, head.rigTarget.position);
        float armOffset = avatarArmLength - playerArmLength;
        leftHand.trackingPositionOffset = new Vector3(0, 0, armOffset);
        rightHand.trackingPositionOffset = new Vector3(0, 0, armOffset);
        leftHand.vrTarget.localPosition = leftHand.trackingPositionOffset;
        rightHand.vrTarget.localPosition = rightHand.trackingPositionOffset;

        if (!cameraRigMode)
        {
            Transform cam = camOffset.transform.Find("Main Camera/Camera");
            cam.localPosition = avatarEyeHeadOffset;
        }
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (vrRightHand)
        {
            float reach = animator.GetFloat("RightHand");
            if (!HasCollided(vrRightHand))
            {
                rightHandGoalPosition = vrRightHand.TransformPoint(rightHand.trackingPositionOffset);
                rightHandGoalRotation = vrRightHand.rotation * Quaternion.Euler(rightHand.trackingRotationOffset);
            }
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, reach);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandGoalPosition);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, reach);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandGoalRotation);
        }
        if (vrLeftHand)
        {
            float reach = animator.GetFloat("LeftHand");
            if (!HasCollided(vrLeftHand))
            {
                leftHandGoalPosition = vrLeftHand.TransformPoint(leftHand.trackingPositionOffset);
                leftHandGoalRotation = vrLeftHand.rotation * Quaternion.Euler(leftHand.trackingRotationOffset);
            }
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, reach);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandGoalPosition);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, reach);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandGoalRotation);
        }
    }

    // Update is called once per frame
    void OnAnimatorMove()
    {
        Vector3 handsPosition = Vector3.Lerp(rightHand.rigTarget.position, leftHand.rigTarget.position, 0.5f);

        handsTorsoRotation = Vector3.Angle(
                                Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized,
                                Vector3.ProjectOnPlane(handsPosition - transform.position, Vector3.up).normalized
                                );

        if (bodyRotation && handsTorsoRotation > rotThreshold)
        {
            //transform.forward = Vector3.ProjectOnPlane(head.rigTarget.transform.up, Vector3.up).normalized;
            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(head.rigTarget.forward, Vector3.up).normalized,
                                        Time.deltaTime * turnSmoothness);
        }

        transform.position = new Vector3(head.rigTarget.position.x,
                                        IsCrouched(head.rigTarget.position) ? head.rigTarget.position.y - avatarHeadHeight: 
                                                                              transform.position.y,
                                        head.rigTarget.position.z) + transform.forward * torsoOffset;

        head.Map(vrHead);
    }

    private bool IsCrouched(Vector3 headPosition)
    {
        RaycastHit hit;
        Ray ray = new Ray(headPosition, Vector3.down);

        if(Physics.Raycast(ray, out hit))
        {
            float distance = Vector3.Distance(headPosition, hit.transform.position);
            if (distance < (avatarHeadHeight * crouchingThreshold))
                return false;
        }

        return true;
    }

    private bool HasCollided(Transform bodyPart) => bodyPart.gameObject.GetComponent<CollisionDetection>().HasCollided();
}
