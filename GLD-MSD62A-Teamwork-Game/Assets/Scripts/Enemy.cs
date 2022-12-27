using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField]
    public float Health;
    public Slider HealthBar;

    private float maxHealth;
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
            if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
                MoveToPlayer();
            }
        }

        if (SceneManager.GetActiveScene().name == "Level2")
        {
            if (_agent.remainingDistance <= 0.5f && _agent.isStopped == false)
            {
                _agent.isStopped = true;
                _animator.SetBool("isWalking", false);
                StartCoroutine(WaitTimer(2, MoveToWaypoint));
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

        if (_agent.remainingDistance <= 2f)
        {

            destination = tempWaypoints[UnityEngine.Random.Range(0, tempWaypoints.Count)].transform.position;
        }

        _animator.SetBool("isWalking", true);
        _agent.isStopped = false;

        _agent.SetDestination(destination);
    }

    private void MoveToPlayer()
    {
        if(_player != null && GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            destination = _player.transform.position;
        }
        else{
            MoveToWaypoint();
        }

        _animator.SetBool("isWalking", true);
        _agent.isStopped = false;

        _agent.SetDestination(destination);
    }

    private IEnumerator WaitTimer(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
