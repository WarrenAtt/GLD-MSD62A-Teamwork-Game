using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Properties")]
    [SerializeField]
    public float Health;
    public Slider HealthBar;
    
    private float maxHealth;
    private List<GameObject> _waypoints;
    private GameObject _player;
    private NavMeshAgent _agent;
    private Animator _animator;
    private Vector3 destination;
    private GameObject _objective;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _waypoints = new List<GameObject>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _objective = GameObject.Find("Objective");
    }

    // Start is called before the first frame update
    void Start()
    {
        

        if(this.gameObject.name == "Boss")
        {
            Health = 300f;
        }
        else
        {
            Health = 100f;
        }

        maxHealth = Health;

        HealthBar.value = CalculateHealth();

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
            MoveToPlayer();

            if (_agent.remainingDistance <= 2f)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
        }

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            StartCoroutine(WaitTimer(2, MoveToWaypoint));

            if (_agent.remainingDistance <= 2f)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
        }

        if(SceneManager.GetActiveScene().name == "Level3")
        {
            if (_objective != null)
                MoveObjective();

            StartCoroutine(WaitTimer(2, MoveToWaypoint));

            if (_agent.remainingDistance <= 2f)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
            }
        }

        HealthBar.value = Health / maxHealth;

        if (Health <= 0f)
        {
            Destroy(gameObject);

            InventoryManager InventoryManager = GameObject.Find("CanvasScript").GetComponent<InventoryManager>();

            ItemScriptableObject objItem = InventoryManager.itemsAvailable[UnityEngine.Random.Range(0, InventoryManager.itemsAvailable.Count)];

            InventoryManager.AddItemToInventory(objItem);
        }
        

    }

    private float CalculateHealth()
    {
        return Health / maxHealth;
    }

    public void ReduceHealth()
    {
        Health -= 30f;
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

        if (_agent.isOnNavMesh)
        {

            if (_agent.remainingDistance <= 2f)
            {
                if(_objective == null)
                    destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
                else
                    destination = _objective.transform.position;
            }

            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(destination);
        }
    }

    private void MoveToPlayer()
    {
        if (_player != null && GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            destination = _player.transform.position;
        }
        else
        {
            StartCoroutine(WaitTimer(2, MoveToWaypoint));
        }

        if (_agent.isOnNavMesh)
        {
            _animator.SetBool("isWalking", true);
            _agent.isStopped = false;

            _agent.SetDestination(destination);
        }
    }

    private void MoveObjective()
    {
        List<GameObject> tempWaypoints = new List<GameObject>();

        foreach (GameObject waypoint in _waypoints)
        {
            if (waypoint.transform.position != destination)
            {
                tempWaypoints.Add(waypoint);
            }
        }

        NavMeshAgent objective = _objective.GetComponent<NavMeshAgent>();

        objective.transform.rotation = Quaternion.Euler(-90, 0, 0);

        if (objective.isOnNavMesh)
        {

            if (objective.remainingDistance <= 2f)
            {
                if (_objective != null)
                {
                    objective.isStopped = false;
                    objective.SetDestination(tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position);
                }   
            }
        } 
    }

    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
