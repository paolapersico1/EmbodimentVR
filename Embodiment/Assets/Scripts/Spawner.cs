using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
    
    public List<GameObject> objectsToSpawn = new List<GameObject>();
    //public GameObject[] items;
    public float timeToStart;
    public float timeToSpawn;
    public int stopCount;

    private float currentTimeToSpawn;
    private int i = 0; // objectsToSpawn index
    private int count = 1; // how many to spawn

    void Start()
    {
        currentTimeToSpawn = timeToSpawn;
        //objectsToSpawn = Resources.LoadAll("Agents/Prefabs") as List<GameObject>();

       // items = Resources.LoadAll<GameObject>("Agents");
       // objectsToSpawn.AddRange(items);
       // while(!done)
       // {
       //     // We just keep loading until obj becomes null
       //     obj = Resources.Load("Agents/Prefabs/agent" + counter) as GameObject;
       //     if(obj == null)
       //         done = true; // Let's stop this now.
       //     else
       //         objectsToSpawn.Add(obj);
       //     ++counter;
       // }
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
            else if (stopCount > 0)
            {
                int j = 0;
                while (j < count)
                {
                    SpawnObject(i);
                    stopCount--;
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
        GameObject obj = Instantiate(objectsToSpawn[i], transform.position, Quaternion.identity) as GameObject;
        obj.SendMessage("SetTimeOffset", currentTimeToSpawn);

    }
}
