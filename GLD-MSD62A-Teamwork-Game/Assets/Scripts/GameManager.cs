using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Unity Setup")]
    public static GameManager Instance = null;
    public GameObject NextLevelPortal;
    public float timePlayed;
    public AudioSource BackgroundMusic;

    private GameObject Player;
    private List<GameObject> Enemies;
    private GameObject canvas;
    private GameState gameState;

    private void Awake()
    {
        if (GameManager.Instance != null)
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
        BackgroundMusic.Play();

        canvas = GameObject.Find("Canvas");
        Player = GameObject.Find("Player");

        DontDestroyOnLoad(canvas);

        gameState = GameState.Safehouse;

    }

    private void Update()
    {
        NextLevelPortal = GameObject.Find("NextLevelPortal");
    }

    public void OnChangeGameState(GameState newGameState)
    {
        gameState = newGameState;
        if(canvas != null)
            canvas.GetComponentInChildren<InventoryManager>().RefreshInventoryGUI();
    }

    public GameState GetCurrentGameState()
    {
        return gameState;
    }

    public void OnButtonPressed(string key)
    {
        switch (key)
        {
            case "TAB":
                ShowToggleMenu();
                break;
            case "K":
                if(canvas.GetComponentInChildren<InventoryManager>().showMenu == true)
                    canvas.GetComponentInChildren<InventoryManager>().ChangeSelection(false);
                break;
            case "J":
                if (canvas.GetComponentInChildren<InventoryManager>().showMenu == true)
                {
                    canvas.GetComponentInChildren<InventoryManager>().ChangeSelection(true);
                }
                else
                {
                    if(Player != null)
                        Player.GetComponent<Player>().ToggleObjectives();
                }
                
                break;
            case "RETURN":
                if (canvas.GetComponentInChildren<InventoryManager>().showMenu == true)
                    canvas.GetComponentInChildren<InventoryManager>().ConfirmSelection();
                break;
            case "PERIOD":
                if (SceneManager.GetActiveScene().name == "Level1")
                    KillEnemiesCheat();

                if (SceneManager.GetActiveScene().name == "Level2")
                    KillPlayerCheat();
                if (SceneManager.GetActiveScene().name == "Level3")
                    UnlimitedAmmoCheat();
                break;

        }
    }

    public float GetTotalEnemies()
    {
        Enemies = new List<GameObject>();

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (!Enemies.Contains(enemy))
            {
                Enemies.Add(enemy);
            }
        }

        return Enemies.Count;
    }

    public bool GetBossStatus()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        if (boss != null && boss.activeSelf == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float GetTotalTimePlayed()
    {
        timePlayed += Time.deltaTime;

        return timePlayed;
    }

    public string GetCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //my game consists of two areas. AreaA (PlayArea) and AreaB (SafeZone)
    public enum GameState
    {
        Safehouse,
        Arena
    }

    private void ShowToggleMenu()
    {
        //so call method inside InventoryManager.cs to toggle the inventory window's animation
        canvas.GetComponentInChildren<InventoryManager>().ShowToggleMenu();
    }

    private void KillEnemiesCheat()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy != null)
                Destroy(enemy);
        }

        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Boss"))
        {
            if (enemy != null)
                Destroy(enemy);
        }
    }

    private void KillPlayerCheat()
    {
        if(Player != null)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Player.GetComponent<Player>().health = 0;
        }

    }

    private void UnlimitedAmmoCheat()
    {
        AutomaticGunScriptLPFP PlayerGun = Player.GetComponentInChildren<AutomaticGunScriptLPFP>();
        Player.GetComponentInChildren<AutomaticGunScriptLPFP>().currentAmmo = PlayerGun.ammo;
    }
}
