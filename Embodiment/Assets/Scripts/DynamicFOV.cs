using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFOV : MonoBehaviour 
{
    
    public float maxCamFOV = 100f;
    public float minCamFOV = 60f;
    public float fovSpeed = 0.2f;
    public float timeToZoomOut = 60f;

    public Transform target;
    public Camera myCam;

    // float initialFOV;

    void Start() {
        myCam = this.GetComponent<Camera>();
    }

    void Update() {
        
        if(timeToZoomOut > 0)
        {
            timeToZoomOut -= Time.deltaTime;
        }
        else {
            if(myCam.fieldOfView >= maxCamFOV) {
                myCam.fieldOfView = maxCamFOV;
                return;
            }
            else {
                myCam.fieldOfView += fovSpeed;
            }
        }
    }
}

