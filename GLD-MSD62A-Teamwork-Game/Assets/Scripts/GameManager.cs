using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Unity Setup")]
    public static GameManager Instance = null;
    public GameObject NextLevelPortal;
    private GameObject canvas;
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
        playerInventory = GameObject.Find("ItemsSelectionPanel");

    }

    // Update is called once per frame
    void Update()
    {
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
