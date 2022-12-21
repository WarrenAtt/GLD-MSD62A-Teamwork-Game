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

    public GameObject Door;
    public Text MoneyText;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        healthBar.fillAmount = CalculateHealth();

        MoneyText.text = "Score: $" + GameData.Money.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = CalculateHealth();

        if (health <= 0f)
        {
            Destroy(this.gameObject);
        }

        MoneyText.text = "Score: $" + GameData.Money.ToString();
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

        if(other.gameObject.name == Door.gameObject.name)
        {
            Door.GetComponent<Animator>().SetBool("isOpen", true);
        }

        if (other.gameObject.name == "SafehouseFloor")
        {
            GameManager.Instance.OnChangeGameState(GameManager.GameState.AreaA);
        }

        if (other.gameObject.name == "ArenaFloor")
        {
            GameManager.Instance.OnChangeGameState(GameManager.GameState.AreaB);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == Door.gameObject.name)
        {
            Door.GetComponent<Animator>().SetBool("isOpen", false);
        }
    }
}
