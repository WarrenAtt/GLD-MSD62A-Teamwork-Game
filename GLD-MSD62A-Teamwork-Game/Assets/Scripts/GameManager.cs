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
    private GameObject Player;
    private List<GameObject> Enemies;
    private GameObject canvas;
    private GameState gameState;

    // Start is called before the first frame update
    void Start()
    {
        //create singleton
        if (GameManager.Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        canvas = GameObject.Find("Canvas");
        Player = GameObject.Find("Player");

    }

    public void OnChangeGameState(GameState newGameState)
    {
        gameState = newGameState;
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
                    Player.GetComponent<Player>().ToggleObjectives();
                }
                
                break;
            case "RETURN":
                if (canvas.GetComponentInChildren<InventoryManager>().showMenu == true)
                    canvas.GetComponentInChildren<InventoryManager>().ConfirmSelection();
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

    private void ShowToggleMenu()
    {
        //so call method inside InventoryManager.cs to toggle the inventory window's animation
        canvas.GetComponentInChildren<InventoryManager>().ShowToggleMenu();
    }

    public string GetCurrentLevel()
    {
        return SceneManager.GetActiveScene().name;
    }

    //my game consists of two areas. AreaA (PlayArea) and AreaB (SafeZone)
    public enum GameState
    {
        Safehouse,
        Arena
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
