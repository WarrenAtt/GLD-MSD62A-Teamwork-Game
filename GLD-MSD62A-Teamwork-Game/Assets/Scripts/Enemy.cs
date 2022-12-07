using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public int health = 10;

    private List<GameObject> _waypoints;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 destination;

    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(wp);
        }

        //destination = GameObject.FindGameObjectWithTag("Waypoint").transform.position;
        MoveToNextDestination();

    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }


        if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
        {
            _agent.isStopped = true;
            _animator.SetBool("Walking", false);
            StartCoroutine(WaitTimer(2, MoveToNextDestination));
        }
        
    }

    public void ReduceHealth()
    {
        health -= 1;
    }

    private void MoveToNextDestination()
    {
        List<GameObject> tempWaypoints = new List<GameObject>();

        foreach(GameObject waypoint in _waypoints)
        {
            if(waypoint.transform.position != destination)
            {
                tempWaypoints.Add(waypoint);
            }
        }

        if (_agent.remainingDistance <= 2f)
        {

            destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
        }

        print(destination.ToString());

        _animator.SetBool("Walking", true);
        _agent.isStopped = false;

        _agent.SetDestination(destination);


    }

    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
