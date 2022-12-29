using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Unity Setup")]
    public List<GameObject> Doors;
    public Text MoneyText;
    public GameObject ObjectiveMenu;
    public float EnemiesSpawned;

    [Header("Health")]
    [SerializeField]
    public float health = 100f;
    private float maxHealth;
    private Image healthBar;

    private bool showObjectives = false;
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        maxHealth = health;
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        healthBar.fillAmount = CalculateHealth();

        MoneyText.text = "Score: $" + GameData.Money.ToString();

        animator = ObjectiveMenu.GetComponent<Animator>();

        EnemiesSpawned = GameManager.Instance.GetTotalEnemies();
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

        UpdateObjectives();
    }

    public void ApplyHealthPotion()
    {
        health += 25f;
    }

    public void ApplyShieldPotion()
    {
        //Shield Method
    }

    public void ApplySpeedPotion()
    {

    }

    public void ReduceHealth()
    {
        health -= 10f;
    }

    public void ToggleObjectives()
    {
        if (showObjectives == false)
        {
            showObjectives = true;
            animator.SetBool("isOpen", true);
        }
        else
        {
            showObjectives = false;
            animator.SetBool("isOpen", false);
        }
    }

    private void UpdateObjectives()
    {
        float TotalEnemies = GameManager.Instance.GetTotalEnemies();
        float TotalTimePlayed = GameManager.Instance.GetTotalTimePlayed();
        bool BossStatus = GameManager.Instance.GetBossStatus();
        string bossText;

        if (BossStatus == false)
        {
            bossText = "Complete";
        }
        else
        {
            bossText = "Not Complete";
        }

        ObjectiveMenu.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "Enemies Left: " + TotalEnemies + "/" + EnemiesSpawned +
            "<br> Boss Killed: " + bossText +
            "<br> Time Alive: " + TotalTimePlayed;
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

        foreach(GameObject Door in Doors)
        {
            if(other.gameObject == Door)
            {
                Door.GetComponent<Animator>().SetBool("isOpen", true);
            }
        }

        if (other.gameObject.name == "SafehouseFloor")
        {
            GameManager.Instance.OnChangeGameState(GameManager.GameState.Safehouse);
        }

        if (other.gameObject.name == "ArenaFloor")
        {
            GameManager.Instance.OnChangeGameState(GameManager.GameState.Arena);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (GameObject Door in Doors)
        {
            if (other.gameObject == Door)
            {
                Door.GetComponent<Animator>().SetBool("isOpen", false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == GameManager.Instance.NextLevelPortal)
        {
            if (GameManager.Instance.GetCurrentLevel() == "Level1")
            {
                SceneManager.LoadScene("Level2");
            }

            if (GameManager.Instance.GetCurrentLevel() == "Level2")
            {
                SceneManager.LoadScene("Level3");
            }
        }
    }
}
