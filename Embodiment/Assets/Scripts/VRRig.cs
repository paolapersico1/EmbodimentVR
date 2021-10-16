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
    //public bool collisionMode;

    public float avatarHeadHeight;
    public float avatarArmLength;

    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Transform leftHandCollider;
    public Transform rightHandCollider;

    private Transform vrHead;
    private Transform vrLeftHand;
    private Transform vrRightHand;

    public bool bodyRotation = true;
    public int rotThreshold;
    public float turnSmoothness;
    public float crouchingThreshold;

    //DEBUG
    public float handsTorsoRotation;
    public bool isCrouched;

    private Animator animator;
    public Vector3 leftHandPrevPosition;
    private Quaternion leftHandPrevRotation;
    private Vector3 rightHandPrevPosition;
    private Quaternion rightHandPrevRotation;

    private float playerArmLength;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        XRRig rig = FindObjectOfType<XRRig>();
        vrHead = head.vrTarget;
        vrLeftHand = leftHand.vrTarget;
        vrRightHand = rightHand.vrTarget;

        GameObject playerOffset = GameObject.Find("XR Rig/Player Offset");

        float playerHeadHeight = playerOffset.GetComponent<Calibrator>().GetPlayerHeadHeight();
        playerArmLength = playerOffset.GetComponent<Calibrator>().GetPlayerArmLength();

        avatarHeadHeight = head.rigTarget.position.y - transform.position.y;
        Debug.Log("Height difference: " + (avatarHeadHeight - playerHeadHeight));
        playerOffset.transform.position = new Vector3(0, avatarHeadHeight - playerHeadHeight, 0);

        //how longer are the avatar's arms
        avatarArmLength = Vector3.Distance(rightHand.rigTarget.position, head.rigTarget.position);
        float armOffset = avatarArmLength - playerArmLength;
        leftHand.trackingPositionOffset = new Vector3(0, 0, armOffset);
        rightHand.trackingPositionOffset = new Vector3(0, 0, armOffset);
        leftHand.vrTarget.localPosition = leftHand.trackingPositionOffset;
        rightHand.vrTarget.localPosition = rightHand.trackingPositionOffset;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (HasCollided(rightHandCollider))
        {
            Debug.Log("right");
            UpdateHand(true, rightHandPrevPosition, rightHandPrevRotation);
            rightHandPrevPosition = animator.GetIKPosition(AvatarIKGoal.RightHand);
            rightHandPrevRotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
        }
        else
        {
            UpdateHand(true, vrRightHand.TransformPoint(rightHand.trackingPositionOffset),
              vrRightHand.rotation * Quaternion.Euler(rightHand.trackingRotationOffset));
            rightHandPrevPosition = animator.GetIKPosition(AvatarIKGoal.RightHand) + transform.forward * 0.1f;
            rightHandPrevRotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
        }

        if (HasCollided(leftHandCollider))
        {
            Debug.Log("left");
            UpdateHand(false, leftHandPrevPosition, leftHandPrevRotation);
            leftHandPrevPosition = animator.GetIKPosition(AvatarIKGoal.LeftHand);
            leftHandPrevRotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);
        }
        else
        { 
            UpdateHand(false, vrLeftHand.TransformPoint(leftHand.trackingPositionOffset),
                   vrLeftHand.rotation * Quaternion.Euler(leftHand.trackingRotationOffset));
            leftHandPrevPosition = animator.GetIKPosition(AvatarIKGoal.LeftHand) + transform.forward * 0.1f;
            leftHandPrevRotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);
        }

        /*rightHandPrevPosition = animator.GetIKPosition(AvatarIKGoal.RightHand) + transform.forward * 0.01f;
        rightHandPrevRotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
        leftHandPrevPosition = animator.GetIKPosition(AvatarIKGoal.LeftHand) + transform.forward * 0.01f;
        leftHandPrevRotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);*/

    }

    // Update is called once per frame
    void OnAnimatorMove()
    {
        isCrouched = IsCrouched(head.rigTarget.position);
        transform.position = new Vector3(head.rigTarget.position.x,
                                        isCrouched ? head.rigTarget.position.y - avatarHeadHeight :
                                                                              transform.position.y,
                                        head.rigTarget.position.z);

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

        head.Map(vrHead);
    }

    private void UpdateHand(bool isRightHand, Vector3 handPos,  Quaternion handRot)
    {
        if ((isRightHand) ? vrRightHand : vrLeftHand)
        {
            float reach = animator.GetFloat((isRightHand) ? "RightHand" : "LeftHand");
            AvatarIKGoal ikGoal = (isRightHand) ? AvatarIKGoal.RightHand : AvatarIKGoal.LeftHand;

            animator.SetIKPositionWeight(ikGoal, reach);
            animator.SetIKPosition(ikGoal, handPos);
            animator.SetIKRotationWeight(ikGoal, reach);
            animator.SetIKRotation(ikGoal, handRot);
        }
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
