using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    public float health = 100f;
    private float maxHealth;
    private Image healthBar;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Image>();

        healthBar.fillAmount = CalculateHealth();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = CalculateHealth();

        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }
    }

    public void ReduceHealth()
    {
        health -= 10f;
    }

    private float CalculateHealth()
    {
        return health / maxHealth;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "EnemyHitbox")
        {
            ReduceHealth();
        }
    }
}
