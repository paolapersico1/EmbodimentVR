using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    static WebCamTexture backCam;

    void Start()
    {
	WebCamDevice[] devices = WebCamTexture.devices;
	foreach (var device in devices){
		Debug.Log(device.name);
	}
	
        if (backCam == null)
            backCam = new WebCamTexture(devices[0].name);

        GetComponent<Renderer>().material.mainTexture = backCam;

        if (!backCam.isPlaying)
            backCam.Play();

    }

    void Update()
    {

    }
}