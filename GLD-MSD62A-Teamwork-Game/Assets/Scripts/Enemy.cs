using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    public int health = 10;

    private List<GameObject> _waypoints;
    private GameObject _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 destination;


    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();
        _player = GameObject.FindGameObjectWithTag("Player");

        foreach (GameObject wp in GameObject.FindGameObjectsWithTag("Waypoint"))
        {
            _waypoints.Add(wp);
        }

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            MoveToWaypoint();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
            {
                _agent.isStopped = true;
                _animator.SetBool("Walking", false);
                MoveToPlayer();
            }
        }

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
            {
                _agent.isStopped = true;
                _animator.SetBool("Walking", false);
                StartCoroutine(WaitTimer(2, MoveToWaypoint));
            }
        }

        GameManager.Instance.EnemyEliminated();
        
    }

    public void ReduceHealth()
    {
        health -= 1;
    }

    private void MoveToWaypoint()
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

        _animator.SetBool("Walking", true);
        _agent.isStopped = false;

        _agent.SetDestination(destination);
    }

    private void MoveToPlayer()
    {
        if(_player != null)
        {
            destination = _player.transform.position;
        }
        else{
            MoveToWaypoint();
        }

        _animator.SetBool("Walking", true);
        _agent.isStopped = false;

        _agent.SetDestination(destination);
    }

    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == _player.name)
        {
            print("PLayer is Hit");
            _player.GetComponent<Player>().ReduceHealth();
        }
    }
}
