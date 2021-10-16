using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * The AIController is responsible of each agent
 * movement, animation, its transitions and phase swap.
 */

public class AIController : MonoBehaviour
{
    GameObject[] wp1; // waypoint first phase
    GameObject[] wp2; // waypoint second phase (to avatar)
    GameObject[] wp3; // waypoint third phase (right to death)
    GameObject agentObj;
    NavMeshAgent agent;
    Animator animator;
    int phase = 1; // range 1,4
    private float timeToLook;
    private float timeToTalk;
    private float timeToRaid;
    private float timeToReturn;
    private bool alive = true;



    // Start is called before the first frame update
    void Start()
    {   
        // Obstacle not needed
        // Destroy(GetComponent<NavMeshObstacle>());
        // this.GetComponent<NavMeshObstacle>().enabled = false;
        // Debug.Log("Obstacle active: " + this.GetComponent<NavMeshObstacle>().enabled);

        // Position controller
        wp1 = GameObject.FindGameObjectsWithTag("wp1");
        wp2 = GameObject.FindGameObjectsWithTag("wp2");
        wp3 = GameObject.FindGameObjectsWithTag("wp3");
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
        timeToLook = Random.Range(1, 30);
        this.GetComponent<HeadController>().lookObj = GameObject.Find("XR Rig/Player Offset/Camera Offset/Main Camera/Camera").transform;
        this.GetComponent<HeadController>().enabled = false;
        this.GetComponent<HeadController>().ikActive = true;
        Debug.Log("enabled: " + this.GetComponent<HeadController>().enabled);
        Debug.Log("loobkObj: " + this.GetComponent<HeadController>().lookObj);

        // Talking (no at the beginning)
        animator.SetBool("isTalking", false);
        timeToTalk = Random.Range(50, 100);

        // WP2 timer
        timeToRaid = Random.Range(40, 80);

        // WP3 timer
        timeToReturn = Random.Range(100, 150);
    }



    void Update()
    {
        if (alive == true)
        {
            // Random waypoint in wp1 picker
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
            
            // Phase2: wp1 to wp2
            if(timeToRaid > 0)
            {
                timeToRaid -= Time.deltaTime;
            }
            else if (phase != 3)
            {
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

            // Phase3: wp2 to wp3
            if(timeToReturn > 0)
            {
                timeToReturn -= Time.deltaTime;
            }
            else {
                phase = 3;
                agent.SetDestination(wp3[0].transform.position);
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
                agent.isStopped = false;
                if (agent.remainingDistance < 1) {
                    Destroy(agent);
                    alive = false; // the agent is dead here
                }
            }
        }
    }
}
