using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [Header("Unity Setup")]
    public static Player Instance = null;
    public List<GameObject> Doors;
    public Text MoneyText;
    private GameObject ObjectiveMenu;
    public float EnemiesSpawned;
    [Tooltip("List of items")]
    public List<ItemScriptableObject> itemsForPlayer;


    private GameObject Objective;

    [Header("Player")]
    [SerializeField]
    public float health = 100f;
    float initialWalkingSpeed;
    float initialRunningSpeed;
    private float maxHealth;
    private Image healthBar;

    private bool showObjectives = false;
    private Animator animator;

    private void Awake()
    {
        if (Player.Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ObjectiveMenu = GameObject.Find("ObjectivesMenu");

        maxHealth = health;
        healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        healthBar.fillAmount = CalculateHealth();

        MoneyText.text = "Score: $" + GameData.Money.ToString();

        animator = ObjectiveMenu.GetComponent<Animator>();

        EnemiesSpawned = GameManager.Instance.GetTotalEnemies();

        

        initialWalkingSpeed = gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().walkingSpeed;
        initialRunningSpeed = gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().runningSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        healthBar.fillAmount = CalculateHealth();

        if (health <= 0f)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Destroy(this.gameObject);
        }

        MoneyText.text = "Score: $" + GameData.Money.ToString();


        if (SceneManager.GetActiveScene().name == "Level1")
            UpdateObjectives();
        else if (SceneManager.GetActiveScene().name == "Level2")
            UpdatePersonalStats();
        else if(SceneManager.GetActiveScene().name == "Level3")
        {
            Objective = GameObject.Find("Objective");
            UpdateGameStats();
        }

    }

    public void ApplyHealthPotion()
    {
        foreach(ItemScriptableObject item in itemsForPlayer)
        {
            if (item.name.Contains("Health"))
            {
                health += item.increaseValue;
            }
        }
    }

    public IEnumerator ApplyShieldPotion()
    {
        float currentHealth = health;

        //Shield Method
        foreach (ItemScriptableObject item in itemsForPlayer)
        {
            if (item.name.Contains("Shield"))
            {
                health = item.increaseValue;
                yield return new WaitForSeconds(15);
                health = currentHealth;
            }
        }
    }

    public IEnumerator ApplySpeedPotion()
    {

        foreach (ItemScriptableObject item in itemsForPlayer)
        {
            if (item.name.Contains("Speed"))
            {
                gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().walkingSpeed = initialWalkingSpeed * item.increaseValue;
                gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().runningSpeed = initialRunningSpeed * item.increaseValue;

                yield return new WaitForSeconds(15);

                gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().walkingSpeed = initialWalkingSpeed;
                gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().runningSpeed = initialRunningSpeed;
            }
        }
    }

    public void ReduceHealth()
    {
        health -= 10f;
    }

    public void ToggleObjectives()
    {
        ObjectiveMenu = GameObject.Find("ObjectivesMenu");

        if(ObjectiveMenu != null)
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

        if(ObjectiveMenu != null)
            ObjectiveMenu.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                "Enemies Left: " + TotalEnemies + "/" + EnemiesSpawned +
                "<br> Boss Killed: " + bossText +
                "<br> Time Alive: " + TotalTimePlayed;
    }

    private void UpdatePersonalStats()
    {

        if (ObjectiveMenu != null)
            ObjectiveMenu.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Player Health: " + health;
                
    }

    private void UpdateGameStats()
    {
        float deltaTime = 0;
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        if (ObjectiveMenu != null)
            ObjectiveMenu.gameObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "FPS: " + Mathf.Ceil(fps).ToString(); ;
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

        if (other.gameObject.name == "Door")
        {
            other.GetComponent<Animator>().SetBool("isOpen", true);
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
        if (other.gameObject.name == "Door")
        {
            other.GetComponent<Animator>().SetBool("isOpen", false);
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

            if (GameManager.Instance.GetCurrentLevel() == "Level3")
            {
                SceneManager.LoadScene("Level1");
            }

            this.gameObject.transform.position = new Vector3(-11.0600004f, 0, -7.28999996f);
        }

        if(Objective != null && collision.gameObject == Objective)
        {
            Destroy(collision.gameObject);
        }
    }
}
