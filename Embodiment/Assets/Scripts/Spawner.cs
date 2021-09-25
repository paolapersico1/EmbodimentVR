using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    
    public List<GameObject> objectsToSpawn = new List<GameObject>();
    public float timeToStart;
    public float timeToSpawn;
    private float currentTimeToSpawn;
    private int i = 0; // objectsToSpawn index
    private int count = 1; // how many to spawn


    void Star()
    {
        currentTimeToSpawn = timeToSpawn;
    }

    void Update()
    {
        // wait to start spawning
        if(timeToStart > 0)
        {
            timeToStart -= Time.deltaTime;
        }
        else 
        {
            // decrease currentTimeToSpawn until 0
            if(currentTimeToSpawn > 0)
            {
                currentTimeToSpawn -= Time.deltaTime;
            }
            // when below zero start spawning until count
            else
            {
                int j = 0;
                while (j < count)
                {
                    SpawnObject(i);
                    if (i == (objectsToSpawn.Count - 1))
                    {
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }

                    j++;
                }
                
                // reset 
                currentTimeToSpawn = timeToSpawn;

                if (count != objectsToSpawn.Count)
                {
                    count++;
                }
                //Debug.Log(count);
            }
        }
    }

    void SpawnObject(int i)
    {
            Instantiate(objectsToSpawn[i], transform.position, Quaternion.identity);
    }

}