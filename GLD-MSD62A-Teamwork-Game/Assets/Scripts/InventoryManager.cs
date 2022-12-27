using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Header("Unity Setup")]
    [Tooltip("Number of items in inventory")]
    public int numberOfItems = 5;

    [Tooltip("Items Selection Panel")]
    public GameObject itemsSelectionPanel;
    public GameObject shopSelectionPanel;

    [Tooltip("Selected Item Colour")]
    public Color selectedColour;

    [Tooltip("Not Selected Item Colour")]
    public Color notSelectedColour;

    public List<InventoryItem> itemsForPlayer; //the items visible to the player during the game
    public List<InventoryItem> itemsForShop;

    [Header("Inventory")]
    [Tooltip("List of items")]
    public List<ItemScriptableObject> itemsAvailable;

    public int currentSelectedIndex = 0; //by default start/select the first button in the inventory system

    private Animator animator;

    [Tooltip("Show Inventory GUI")]
    public bool showMenu = false;

    public int _buttonID = 0;

    private Transform button;

    private GameObject _player;


    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("Player");

        itemsForPlayer = new List<InventoryItem>();
        itemsForShop = new List<InventoryItem>();
        PopulateInventorySpawn();
        PopulateShopSpawn();
        RefreshInventoryGUI();
        
    }

    private void Update()
    {
        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            //itemsSelectionPanel.SetActive(true);
            //shopSelectionPanel.SetActive(false);
            //load the controller so that we can play the animations (inventoryIn/inventoryOut)
            animator = itemsSelectionPanel.GetComponent<Animator>();
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            //itemsSelectionPanel.SetActive(false);
            //shopSelectionPanel.SetActive(true);
            animator = shopSelectionPanel.GetComponent<Animator>();
        }
    }

    public void ShowToggleMenu()
    {
        if (showMenu == false)
        {
            showMenu = true;
            animator.SetBool("InventoryIn", true);
            animator.SetBool("InventoryOut", false);
        }
        else
        {
            showMenu = false;
            animator.SetBool("InventoryIn", false);
            animator.SetBool("InventoryOut", true);
        }
    }

    public void ConfirmSelection()
    {
        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            if (itemsForPlayer.Count != 0)
            {
                //get the item from the itemsForPlayer list using the currentSelectedIndex
                InventoryItem inventoryItem = itemsForPlayer[currentSelectedIndex];
                //print("Item Selected is:" + inventoryItem.item.name);

                //reduce the quantity by 1
                inventoryItem.quantity -= 1;

                if (inventoryItem.item.type == ItemScriptableObject.Type.Health)
                {
                    print("Add Health");
                    _player.GetComponent<Player>().ApplyHealthPotion();
                }

                if (inventoryItem.item.type == ItemScriptableObject.Type.Shield)
                {
                    print("Add Shield");
                    _player.GetComponent<Player>().ApplyShieldPotion();
                }

                if (inventoryItem.item.type == ItemScriptableObject.Type.Speed)
                {
                    print("Add Speed");
                    _player.GetComponent<Player>().ApplySpeedPotion();
                }

                //check if the quantity is 0, if it is we need to remove this item from the itemsForPlayer list
                if (inventoryItem.quantity == 0)
                {
                    itemsForPlayer.RemoveAt(currentSelectedIndex);
                    currentSelectedIndex = 0;
                }
            }

            RefreshInventoryGUI();
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            if (itemsForShop.Count != 0)
            {
                //get the item from the itemsForPlayer list using the currentSelectedIndex
                InventoryItem inventoryItem = itemsForShop[currentSelectedIndex];
                //print("Item Selected is:" + inventoryItem.item.name);

                //reduce the quantity by 1
                inventoryItem.quantity -= 1;

                if (inventoryItem.item.type == ItemScriptableObject.Type.Health)
                {
                    AddItemToInventory(itemsAvailable[0]);
                }

                if (inventoryItem.item.type == ItemScriptableObject.Type.Shield)
                {
                    AddItemToInventory(itemsAvailable[1]);
                }

                if (inventoryItem.item.type == ItemScriptableObject.Type.Speed)
                {
                    AddItemToInventory(itemsAvailable[2]);
                }

                //check if the quantity is 0, if it is we need to remove this item from the itemsForPlayer list
                if (inventoryItem.quantity == 0)
                {
                    itemsForShop.RemoveAt(currentSelectedIndex);
                    currentSelectedIndex = 0;
                }
            }

            RefreshInventoryGUI();
        }
    }

    public void ChangeSelection(bool moveLeft)
    {
        if (moveLeft == true)
            currentSelectedIndex -= 1;
        else
            currentSelectedIndex += 1;

        //check boundaries
        if (currentSelectedIndex < 0)
            currentSelectedIndex = 0;

        if (currentSelectedIndex == itemsForPlayer.Count || currentSelectedIndex == itemsForShop.Count)
            currentSelectedIndex = currentSelectedIndex - 1;

        RefreshInventoryGUI();
    }

    /// <summary>
    /// This method will generate the inventory items for the player to use
    /// during the game. The total number of inventory items cannot exceed
    /// the number set in the variable numberOfItems.
    /// </summary>
    public void PopulateInventorySpawn()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            //pick random object from list itemsAvailable
            ItemScriptableObject objItem = itemsAvailable[Random.Range(0, itemsAvailable.Count)];

            //check whether objItem exits in itemsForPlayer. So basically we need to count how
            //many times an item appears. i.e the number of objItems inside itemsForPlayer
            int countItems = itemsForPlayer.Where(x => x.item == objItem).ToList().Count;

            if (countItems == 0)
            {
                //add objItem with quantity of 1 because it is the first type inside itemsForPlayer
                itemsForPlayer.Add(new InventoryItem() { item = objItem, quantity = 1 });
            }
            else
            {
                //search for the element of the same type inside itemsForPlayer
                var item = itemsForPlayer.First(x => x.item == objItem);
                //increase the quantity by 1
                item.quantity += 1;
            }
        }
    }

    public void PopulateShopSpawn()
    {
        for (int i = 0; i < numberOfItems; i++)
        {
            //pick random object from list itemsAvailable
            ItemScriptableObject objItem = itemsAvailable[Random.Range(0, itemsAvailable.Count)];

            //check whether objItem exits in itemsForPlayer. So basically we need to count how
            //many times an item appears. i.e the number of objItems inside itemsForPlayer
            int countItems = itemsForShop.Where(x => x.item == objItem).ToList().Count;

            if (countItems == 0)
            {
                //add objItem with quantity of 1 because it is the first type inside itemsForPlayer
                itemsForShop.Add(new InventoryItem() { item = objItem, quantity = 1 });
            }
            else
            {
                //search for the element of the same type inside itemsForPlayer
                var item = itemsForShop.First(x => x.item == objItem);
                //increase the quantity by 1
                item.quantity += 1;
            }
        }
    }

    public void AddItemToInventory(ItemScriptableObject addedItem)
    {
        int countItems = itemsForPlayer.Where(x => x.item == addedItem).ToList().Count;

        if (itemsSelectionPanel.transform.Find("Button0").gameObject.active == false)
        {
            _buttonID = 0;
        }

        if (countItems == 0)
        {
            GameObject button = itemsSelectionPanel.transform.Find("Button" + _buttonID).gameObject;

            itemsForPlayer.Add(new InventoryItem() { item = addedItem, quantity = 1 });

            if(button != null)
            {
                button.SetActive(true);
            }

            print(addedItem);
        }
        else
        {
            //search for the element of the same type inside itemsForPlayer
            var item = itemsForPlayer.First(x => x.item == addedItem);
            //increase the quantity by 1
            item.quantity += 1;
        }
        
        RefreshInventoryGUI();
    }

    public void RefreshInventoryGUI()
    {
        int buttonId = 0;

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            foreach (InventoryItem i in itemsForPlayer)
            {
                //load the button
                //GameObject button = itemsSelectionPanel.transform.Find("Button" + buttonId).gameObject;
                button = itemsSelectionPanel.transform.Find("Button" + buttonId);

                if (button == null)
                {
                    itemsSelectionPanel.transform.Find("Button" + buttonId).gameObject.SetActive(true);
                    button = itemsSelectionPanel.transform.Find("Button" + buttonId);
                }

                //search for the child image and change the sprite of the item
                button.transform.Find("Image").GetComponent<Image>().sprite = i.item.icon;

                //change the name of the item
                button.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = i.item.name;

                //change the quantity of the item
                button.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = "x" + i.quantity;

                //show selected/not selected colour based on buttonId and currentSelectedIndex
                if (buttonId == currentSelectedIndex)
                {
                    button.GetComponent<Image>().color = selectedColour;
                }
                else
                {
                    button.GetComponent<Image>().color = notSelectedColour;
                }

                buttonId += 1;

            }

            //set active false redundant buttons
            for (int i = buttonId; i < 3; i++)
            {
                itemsSelectionPanel.transform.Find("Button" + i).gameObject.SetActive(false);
            }
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            foreach (InventoryItem i in itemsForShop)
            {
                //load the button
                //GameObject button = itemsSelectionPanel.transform.Find("Button" + buttonId).gameObject;
                button = shopSelectionPanel.transform.Find("Button" + buttonId);

                if (button == null)
                {
                    shopSelectionPanel.transform.Find("Button" + buttonId).gameObject.SetActive(true);
                    button = shopSelectionPanel.transform.Find("Button" + buttonId);
                }

                //search for the child image and change the sprite of the item
                button.transform.Find("Image").GetComponent<Image>().sprite = i.item.icon;

                //change the name of the item
                button.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = i.item.name;

                //change the quantity of the item
                button.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = "x" + i.quantity;

                //show selected/not selected colour based on buttonId and currentSelectedIndex
                if (buttonId == currentSelectedIndex)
                {
                    button.GetComponent<Image>().color = selectedColour;
                }
                else
                {
                    button.GetComponent<Image>().color = notSelectedColour;
                }

                buttonId += 1;
                _buttonID = buttonId;

            }

            //set active false redundant buttons
            for (int i = buttonId; i < 3; i++)
            {
                shopSelectionPanel.transform.Find("Button" + i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < itemsForPlayer.Count; i++)
        {
            print(itemsForPlayer[i].item);
        }

        print("Button Id: " + _buttonID);
    }

    public class InventoryItem
    {
        public ItemScriptableObject item { get; set; }
        public int quantity { get; set; }
    }
}
