using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    GameObject[] goalLocations;
    GameObject agentObj;
    NavMeshAgent agent;
    Animator animator;
    int phase = 1; // range 1,4
    private float timeToLook;
//    public float timetophase2;
//    public float timetophase3;
//    public float timetophase4;
    


    // Start is called before the first frame update
    void Start()
    {   
        // Position controller
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>();
        agentObj = this.GetComponent<GameObject>();
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);

        // Animator controller
        animator = this.GetComponent<Animator>();
        animator.SetFloat("walkOffset", Random.Range(0, 1)); // walk unsynch
        animator.SetFloat("talkOffset", Random.Range(0, 1)); // talk unsych
        float ws = Random.Range(0.5f, 1.25f);
        agent.speed *= ws;
        animator.SetFloat("walkMult", ws);
        animator.SetFloat("talkMult", Random.Range(0.75f, 1.25f));

        // Head Controller
        timeToLook = Random.Range(0, 60);
        this.GetComponent<HeadController>().lookObj = GameObject.Find("CameraTest").transform;
        this.GetComponent<HeadController>().enabled = false;
        this.GetComponent<HeadController>().ikActive = true;
        Debug.Log("enabled: " + this.GetComponent<HeadController>().enabled);
        Debug.Log("loobkObj: " + this.GetComponent<HeadController>().lookObj);
    }

    void Update()
    {
        if(agent.remainingDistance < 1)
        {
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        }

        // Start looking
        if(timeToLook > 0)
        {
            timeToLook -= Time.deltaTime;
        }
        else {
            this.GetComponent<HeadController>().enabled = true;
        }

//        // wait for phase 2
//        if(timeToPhase2 > 0)
//        {
//            timeToPhase2 -= Time.deltaTime;
//        }
//        else if(phase = 1){
//            secondPhase();
//            phase = 2;
//        }
//
//        // wait for phase 3
//        if(timeToPhase3 > 0)
//        {
//            timeToPhase3 -= Time.deltaTime;
//        }
//        else if(phase == 2){
//            thirdPhase();
//            phase = 3;
//        }
//
//        // wait for phase 4
//        if(timeToPhase4 > 0)
//        {
//            timeToPhase4 -= Time.deltaTime;
//        }
//        else if(phase = 3) {
//            fourthPhase();
//            phase = 4;
//        }

    }

//    void firstPhase() {
//        
//    }
//
//    void secondPhase() {
//        
//    }
//
//    void thirdPhase() {
//
//    }
//
//    void fourthPhase() {
//
//    }
}
