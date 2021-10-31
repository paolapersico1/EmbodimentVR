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
    public bool collisionMode;

    public float avatarHeadHeight;
    public float avatarArmLength;

    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Transform leftHandCollider;
    public Transform rightHandCollider;
    public bool isReplica;

    private Transform vrHead;
    private Transform vrLeftHand;
    private Transform vrRightHand;

    public bool bodyRotation = true;
    public int rotThreshold;
    public float turnSmoothness;
    public float crouchingThreshold;
    public float crouchSmoothness;
    public LayerMask floorLayer;

    //DEBUG
    public float handsTorsoRotation;
    public bool isLookingDown;

    public Transform leftHandTarget;
    public Transform rightHandTarget;

    private Animator animator;

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
        float playerArmLength = playerOffset.GetComponent<Calibrator>().GetPlayerArmLength();

        avatarHeadHeight = head.rigTarget.position.y - transform.position.y;
        Debug.Log("Height difference: " + (avatarHeadHeight - playerHeadHeight));
        playerOffset.transform.position = new Vector3(0, avatarHeadHeight - playerHeadHeight, 0);

        //how longer are the avatar's arms
        avatarArmLength = Vector3.Distance(rightHand.rigTarget.position, head.rigTarget.position);
        float armOffset = avatarArmLength - playerArmLength;
        leftHand.trackingPositionOffset = new Vector3(0, 0, armOffset);
        rightHand.trackingPositionOffset = new Vector3(0, 0, armOffset);
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (isReplica)
        {
            UpdateHand(true, vrRightHand.TransformPoint(rightHand.trackingPositionOffset),
               vrRightHand.rotation * Quaternion.Euler(rightHand.trackingRotationOffset));
            UpdateHand(false, vrLeftHand.TransformPoint(leftHand.trackingPositionOffset),
             vrLeftHand.rotation * Quaternion.Euler(leftHand.trackingRotationOffset));
        }
        else
        {
            if (collisionMode && HasCollided(rightHandCollider))
            {
                //Debug.Log("right");
                UpdateHand(true, rightHandTarget.position, rightHandTarget.rotation);
            }
            else
            {
                UpdateHand(true, vrRightHand.TransformPoint(rightHand.trackingPositionOffset),
                    vrRightHand.rotation * Quaternion.Euler(rightHand.trackingRotationOffset));
            }

            if (collisionMode && HasCollided(leftHandCollider))
            {
                //Debug.Log("left");
                UpdateHand(false, leftHandTarget.position, leftHandTarget.rotation);
            }
            else
            {
                UpdateHand(false, vrLeftHand.TransformPoint(leftHand.trackingPositionOffset),
                    vrLeftHand.rotation * Quaternion.Euler(leftHand.trackingRotationOffset));
            }

            rightHandTarget.position = animator.GetIKPosition(AvatarIKGoal.RightHand);
            rightHandTarget.rotation = animator.GetIKRotation(AvatarIKGoal.RightHand);
            leftHandTarget.position = animator.GetIKPosition(AvatarIKGoal.LeftHand);
            leftHandTarget.rotation = animator.GetIKRotation(AvatarIKGoal.LeftHand);
        }
    }

    // Update is called once per frame
    void OnAnimatorMove()
    {
        isLookingDown = IsLookingDown(head.rigTarget);

        transform.position = new Vector3(head.rigTarget.position.x,
                                        isLookingDown? transform.position.y :
                                            Mathf.Lerp(transform.position.y, head.rigTarget.position.y - avatarHeadHeight, Time.deltaTime * crouchSmoothness),
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

    private bool IsLookingDown(Transform head)
    {
        RaycastHit hit;
        Ray ray = new Ray(head.position, head.forward);

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, floorLayer.value))
        {
            float distance = Vector3.Distance(head.position, hit.point);
            if (distance < (avatarHeadHeight * crouchingThreshold))
                return true;
        }

        return false;
    }

    private bool HasCollided(Transform bodyPart) => bodyPart.gameObject.GetComponent<CollisionDetection>().HasCollided();
}
