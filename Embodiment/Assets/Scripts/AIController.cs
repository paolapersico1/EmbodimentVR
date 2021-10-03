using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    GameObject[] wp1;
    GameObject[] wp2;
    GameObject agentObj;
    NavMeshAgent agent;
    Animator animator;
    int phase = 1; // range 1,4
    private float timeToLook;
    private float timeToTalk;
    private float timeToWaypoint2;


    // Start is called before the first frame update
    void Start()
    {   
        // Obstacle not needed
        Destroy(GetComponent<NavMeshObstacle>());
        this.GetComponent<NavMeshObstacle>().enabled = false;
        // Position controller
        wp1 = GameObject.FindGameObjectsWithTag("wp1");
        wp2 = GameObject.FindGameObjectsWithTag("wp2");
        agent = this.GetComponent<NavMeshAgent>();
        agentObj = this.GetComponent<GameObject>();
        agent.SetDestination(wp1[Random.Range(0, wp1.Length)].transform.position);

        // Animator controller
        animator = this.GetComponent<Animator>();
        animator.SetFloat("walkOffset", Random.Range(0, 1)); // walk unsynch
        animator.SetFloat("talkOffset", Random.Range(0, 1)); // talk unsych
        animator.SetTrigger("isWalking");
        float ws = Random.Range(0.8f, 1.35f);
        agent.speed *= ws;
        animator.SetFloat("walkMult", ws);
        animator.SetFloat("talkMult", Random.Range(0.75f, 1.25f));

        // Head Controller
        timeToLook = Random.Range(0, 30);
        this.GetComponent<HeadController>().lookObj = GameObject.Find("CameraTest").transform;
        this.GetComponent<HeadController>().enabled = false;
        this.GetComponent<HeadController>().ikActive = true;
        Debug.Log("enabled: " + this.GetComponent<HeadController>().enabled);
        Debug.Log("loobkObj: " + this.GetComponent<HeadController>().lookObj);

        // Talking (no at the beginning)
        animator.SetBool("isTalking", false);
        timeToWaypoint2 = Random.Range(50, 100);

        // WP2 timer
        timeToWaypoint2 = Random.Range(40, 80);
    }

    void Update()
    {
        if(agent.remainingDistance < 1 && phase == 1)
        {
            agent.SetDestination(wp1[Random.Range(0, wp1.Length)].transform.position);
        }

        // Start looking
        if(timeToLook > 0)
        {
            timeToLook -= Time.deltaTime;
        }
        else {
            this.GetComponent<HeadController>().enabled = true;
        }
        
        // Change waypoint to avatar
        if(timeToWaypoint2 > 0)
        {
            timeToWaypoint2 -= Time.deltaTime;
        }
        else {
            phase = 2;
            agent.SetDestination(wp2[0].transform.position);
            if(agent.remainingDistance < 1) {
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                agent.isStopped = true;
            }
        }

        // Talk timer
        if(timeToTalk > 0)
        {
            timeToTalk -= Time.deltaTime;
        }
        else {
            animator.SetBool("isTalking", true);
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
