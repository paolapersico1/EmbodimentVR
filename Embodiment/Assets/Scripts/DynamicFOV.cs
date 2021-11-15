using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicFOV : MonoBehaviour 
{
    
    public float maxCamFOV = 600f;
    public float minCamFOV = 45f;
    public float fovSpeed = 0.05f;
    public float fovAcceleration = 0.99f;
    public float timeToZoomOut = 40f;

    private Transform target;
    private Camera myCam;

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
                if (fovSpeed > 0.001)
                {
                    fovSpeed *= fovAcceleration;
                }
            }
        }
    }
}

