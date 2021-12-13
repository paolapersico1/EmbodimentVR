using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ApplicationManager : MonoBehaviour
{
    private float timeToCheck = 30; // must be higher than timeToSpawn

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Is Device Active " + XRSettings.isDeviceActive);
        Debug.Log("Device Name " + XRSettings.loadedDeviceName);

        if (!XRSettings.isDeviceActive)
        {
            Debug.Log("No Headset plugged");
        }
        else if(XRSettings.isDeviceActive && (XRSettings.loadedDeviceName == "Mock HMD" ||
            XRSettings.loadedDeviceName == "MockHMDDisplay"))
        {
            Debug.Log("Using Mock HMD");
        }
        else
        {
            Debug.Log("Headset plugged: " + XRSettings.loadedDeviceName);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        else if (Input.GetKeyDown(KeyCode.Space))
            //restart scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    
        // restart when finished
        if(timeToCheck > 0)
        {
            timeToCheck -= Time.deltaTime;
        }
        else {
            GameObject[] allObjects = GameObject.FindGameObjectsWithTag("agent");
            if (allObjects.Length == 0){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
            }
        }
    }
}
