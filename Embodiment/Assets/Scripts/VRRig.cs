using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public abstract class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public abstract void Map(GameObject vrObj);
}

[System.Serializable]
public class VRMapHead : VRMap
{
    public override void Map(GameObject vrHead)
    {
        vrTarget = vrHead.transform;
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);

    }
}

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}

[System.Serializable]
public class VRMapHand : VRMap
{
    public Transform[] vrNodes;
    [NamedArrayAttribute(new string[] { "Wrist",
    "Thumb root", "Thumb joint 1", "Thumb joint 2", "Thumb top",
    "Index root", "Index joint 1", "Index joint 2", "Index top",
    "Middle root", "Middle joint 1", "Middle joint 2", "Middle top",
    "Ring root", "Ring joint 1", "Ring joint 2", "Ring top",
    "Pinky root", "Pinky joint 1", "Pinky joint 2", "Pinky top" })]
    public Transform[] rigNodes;
    
    public void SetVrNodes(Transform[] vrNodes)
    {
        this.vrNodes = vrNodes;
    }

    public override void Map(GameObject vrHand)
    {
        //map the hand
        vrTarget = vrHand.transform;
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);

        for(int i = 1; i < vrNodes.Length; i++)
        {
            rigNodes[i].position = vrNodes[i].position;
            rigNodes[i].rotation = vrNodes[i].rotation;
        }
    }
}

public class VRRig : MonoBehaviour
{
    public float turnSmoothness;
    public VRMapHead head;
    public VRMapHand leftHand;
    public VRMapHand rightHand;

    private GameObject vrHead;
    private GameObject vrLeftHand;
    private GameObject vrRightHand;

    public Transform headConstraint;
    public Vector3 headBodyOffset;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        XRRig rig = FindObjectOfType<XRRig>();
        vrHead = GameObject.Find("Main Camera");
        vrLeftHand = GameObject.Find("LeftHand Controller");
        vrRightHand = GameObject.Find("RightHand Controller");
        leftHand.SetVrNodes(vrLeftHand.GetComponent<ViveHandTracking.ModelRenderer>().Nodes);
        rightHand.SetVrNodes(vrRightHand.GetComponent<ViveHandTracking.ModelRenderer>().Nodes);

        headBodyOffset = transform.position - headConstraint.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            transform.position = headConstraint.position + headBodyOffset;
            //transform.forward = Vector3.ProjectOnPlane(head.rigTarget.transform.up, Vector3.up).normalized;
            transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstraint.forward, Vector3.up).normalized,
                                            Time.deltaTime * turnSmoothness);

            head.Map(vrHead);
            leftHand.Map(vrLeftHand);
            rightHand.Map(vrRightHand);
        }
    }
}