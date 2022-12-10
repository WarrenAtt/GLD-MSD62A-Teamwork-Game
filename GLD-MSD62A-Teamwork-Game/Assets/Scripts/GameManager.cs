using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    private GameObject canvas;
    private GameObject[] enemies;
    private GameObject playerInventory;
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
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        playerInventory = GameObject.Find("ItemsSelectionPanel");

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnChangeGameState(GameState newGameState)
    {
        print("Changing game state to:" + newGameState.ToString());
        gameState = newGameState;
    }

    public void OnButtonPressed(string key)
    {
        switch (key)
        {
            case "TAB":
                ShowToggleInventory();
                break;
            case "K":
                canvas.GetComponentInChildren<InventoryManager>().ChangeSelection(false);
                break;
            case "J":
                canvas.GetComponentInChildren<InventoryManager>().ChangeSelection(true);
                break;
            case "RETURN":
                canvas.GetComponentInChildren<InventoryManager>().ConfirmSelection();
                break;
        }
    }

    public void EnemyEliminated()
    {
        foreach (GameObject enemy in enemies)
        {
            if(enemy != null)
            {
                if (enemy.GetComponent<Enemy>().health == 0)
                {
                    //playerInventory.GetComponentInChildren<InventoryManager>().AddItem();
                    Destroy(enemy);
                }
            }
        }
    }

    private void ShowToggleInventory()
    {
        //so call method inside InventoryManager.cs to toggle the inventory window's animation
        canvas.GetComponentInChildren<InventoryManager>().ShowToggleInventory();
    }


    //my game consists of two areas. AreaA (PlayArea) and AreaB (SafeZone)
    public enum GameState
    {
        AreaA,
        AreaB
    }
}
