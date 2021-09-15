using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    GameObject[] goalLocations;
    NavMeshAgent agent;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {   
        // Position controller
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        agent = this.GetComponent<NavMeshAgent>();
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);

        // Animator controller
        animator = this.GetComponent<Animator>();
        animator.SetFloat("walkOffset", Random.Range(0, 1)); // walk unsynch
        animator.SetFloat("talkOffset", Random.Range(0, 1)); // talk unsych
        float ws = Random.Range(0.5f, 1.25f);
        agent.speed *= ws;
        animator.SetFloat("walkMult", ws);
        animator.SetFloat("talkMult", Random.Range(0.75f, 1.25f));

    }

    void Update()
    {
        if(agent.remainingDistance < 1) {
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        }
    }

}
