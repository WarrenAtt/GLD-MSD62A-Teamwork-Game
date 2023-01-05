using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public int currentSelectedInventoryItem = 0; //by default start/select the first button in the inventory system
    public int currentSelectedShopItem = 0;

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
            itemsSelectionPanel.SetActive(true);
            shopSelectionPanel.SetActive(false);
            //load the controller so that we can play the animations (inventoryIn/inventoryOut)
            animator = itemsSelectionPanel.GetComponent<Animator>();
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            itemsSelectionPanel.SetActive(false);
            shopSelectionPanel.SetActive(true);
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
                InventoryItem inventoryItem = itemsForPlayer[currentSelectedInventoryItem];
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
                    StartCoroutine(_player.GetComponent<Player>().ApplyShieldPotion());
                }

                if (inventoryItem.item.type == ItemScriptableObject.Type.Speed)
                {
                    print("Add Speed");
                    StartCoroutine(_player.GetComponent<Player>().ApplySpeedPotion());
                }

                //check if the quantity is 0, if it is we need to remove this item from the itemsForPlayer list
                if (inventoryItem.quantity == 0)
                {
                    itemsForPlayer.RemoveAt(currentSelectedInventoryItem);
                    currentSelectedInventoryItem = 0;
                }
            }

            RefreshInventoryGUI();
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            if (itemsForShop.Count != 0)
            {
                //get the item from the itemsForPlayer list using the currentSelectedIndex
                InventoryItem inventoryItem = itemsForShop[currentSelectedShopItem];

                //print("Item Selected is:" + inventoryItem.item.name);

                if(GameData.Money >= inventoryItem.item.cost)
                {
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

                    GameData.Money -= inventoryItem.item.cost;
                }

                //check if the quantity is 0, if it is we need to remove this item from the itemsForPlayer list
                if (inventoryItem.quantity == 0)
                {
                    itemsForShop.RemoveAt(currentSelectedShopItem);
                    currentSelectedShopItem = 0;
                }
            }

            RefreshInventoryGUI();
        }
    }

    public void ChangeSelection(bool moveLeft)
    {
        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            if (moveLeft == true)
                currentSelectedInventoryItem -= 1;
            else
                currentSelectedInventoryItem += 1;

            //check boundaries
            if (currentSelectedInventoryItem < 0)
                currentSelectedInventoryItem = 0;

            if (currentSelectedInventoryItem == itemsForPlayer.Count)
                currentSelectedInventoryItem = currentSelectedInventoryItem - 1;
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            if (moveLeft == true)
                currentSelectedShopItem -= 1;
            else
                currentSelectedShopItem += 1;

            //check boundaries
            if (currentSelectedShopItem < 0)
                currentSelectedShopItem = 0;

            if (currentSelectedShopItem == itemsForShop.Count)
                currentSelectedShopItem = currentSelectedShopItem - 1;
        }


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
        int countItems = itemsForPlayer.Where(x => x.item
        == addedItem).ToList().Count;

        if (itemsSelectionPanel.transform.Find("Button0").gameObject.activeSelf == false)
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
        int inventoryButtonId = 0;
        int shopButtonId = 0;

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Arena)
        {
            foreach (InventoryItem i in itemsForPlayer)
            {
                //load the button
                //GameObject button = itemsSelectionPanel.transform.Find("Button" + buttonId).gameObject;
                button = itemsSelectionPanel.transform.Find("Button" + inventoryButtonId);

                if (button == null)
                {
                    itemsSelectionPanel.transform.Find("Button" + inventoryButtonId).gameObject.SetActive(true);
                    button = itemsSelectionPanel.transform.Find("Button" + inventoryButtonId);
                }

                //search for the child image and change the sprite of the item
                button.transform.Find("Image").GetComponent<Image>().sprite = i.item.icon;

                //change the name of the item
                button.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = i.item.name;

                //change the quantity of the item
                button.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = "x" + i.quantity;

                //show selected/not selected colour based on buttonId and currentSelectedIndex
                if (inventoryButtonId == currentSelectedInventoryItem)
                {
                    button.GetComponent<Image>().color = selectedColour;
                }
                else
                {
                    button.GetComponent<Image>().color = notSelectedColour;
                }

                inventoryButtonId += 1;

            }

            //this section will check for redudent buttons and deactivate them if necisccary.
            //whilst implementing this feature we found an issue with the button id that is why we hard coded this section
            if(inventoryButtonId < 3)
            {
                itemsSelectionPanel.transform.Find("Button2").gameObject.SetActive(false);
            }
            else
            {
                itemsSelectionPanel.transform.Find("Button2").gameObject.SetActive(true);
            }

            if (inventoryButtonId < 2)
            {
                itemsSelectionPanel.transform.Find("Button1").gameObject.SetActive(false);
            }
            else
            {
                itemsSelectionPanel.transform.Find("Button1").gameObject.SetActive(true);
            }

            if (inventoryButtonId < 1)
            {
                itemsSelectionPanel.transform.Find("Button0").gameObject.SetActive(false);
            }
            else
            {
                itemsSelectionPanel.transform.Find("Button0").gameObject.SetActive(true);
            }

            //this below code was the one we tried using but did not work in this section.

            //set active false redundant buttons
            //for (int i = inventoryButtonId; i < 3; i++)
            //{
            //    itemsSelectionPanel.transform.Find("Button" + i).gameObject.SetActive(false);
            //}
        }

        if (GameManager.Instance.GetCurrentGameState() == GameManager.GameState.Safehouse)
        {
            foreach (InventoryItem i in itemsForShop)
            {
                //load the button
                //GameObject button = itemsSelectionPanel.transform.Find("Button" + buttonId).gameObject;
                button = shopSelectionPanel.transform.Find("Button" + shopButtonId);

                if (button == null)
                {
                    shopSelectionPanel.transform.Find("Button" + shopButtonId).gameObject.SetActive(true);
                    button = shopSelectionPanel.transform.Find("Button" + shopButtonId);
                }

                //search for the child image and change the sprite of the item
                button.transform.Find("Image").GetComponent<Image>().sprite = i.item.icon;

                //change the name of the item
                button.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = i.item.name;

                //change the quantity of the item
                button.transform.Find("Quantity").GetComponent<TextMeshProUGUI>().text = "x" + i.quantity;

                button.transform.Find("Cost").GetComponent<TextMeshProUGUI>().text = "$" + i.item.cost;

                //show selected/not selected colour based on buttonId and currentSelectedIndex
                if (shopButtonId == currentSelectedShopItem)
                {
                    button.GetComponent<Image>().color = selectedColour;
                }
                else
                {
                    button.GetComponent<Image>().color = notSelectedColour;
                }

                shopButtonId += 1;
                //_buttonID = inventoryButtonId - 1;

            }

            //set active false redundant buttons
            for (int i = shopButtonId; i < 3; i++)
            {
                shopSelectionPanel.transform.Find("Button" + i).gameObject.SetActive(false);
            }
        }
    }

    public class InventoryItem
    {
        public ItemScriptableObject item { get; set; }
        public int quantity { get; set; }
    }
}
