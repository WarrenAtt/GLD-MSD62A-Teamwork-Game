using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [Tooltip("Number of items in inventory")]
    public int numberOfItems = 5;

    [Tooltip("Items Selection Panel")]
    public GameObject itemsSelectionPanel;

    [Tooltip("List of items")]
    public List<ItemScriptableObject> itemsAvailable;

    [Tooltip("Selected Item Colour")]
    public Color selectedColour;

    [Tooltip("Not Selected Item Colour")]
    public Color notSelectedColour;

    public List<InventoryItem> itemsForPlayer; //the items visible to the player during the game

    public int currentSelectedIndex = 0; //by default start/select the first button in the inventory system

    private Animator animator;

    [Tooltip("Show Inventory GUI")]
    public bool showInventory = false;

    private int _buttonID = 0;


    // Start is called before the first frame update
    void Start()
    {
        //load the controller so that we can play the animations (inventoryIn/inventoryOut)
        animator = itemsSelectionPanel.GetComponent<Animator>();

        itemsForPlayer = new List<InventoryItem>();
        PopulateInventorySpawn();
        RefreshInventoryGUI();
    }

    public void ShowToggleInventory()
    {
        if (showInventory == false)
        {
            showInventory = true;
            animator.SetBool("InventoryIn", true);
            animator.SetBool("InventoryOut", false);
        }
        else
        {
            showInventory = false;
            animator.SetBool("InventoryIn", false);
            animator.SetBool("InventoryOut", true);
        }
    }

    public void ConfirmSelection()
    {
        if (itemsForPlayer.Count != 0)
        {
            //get the item from the itemsForPlayer list using the currentSelectedIndex
            InventoryItem inventoryItem = itemsForPlayer[currentSelectedIndex];
            print("Item Selected is:" + inventoryItem.item.name);

            //reduce the quantity by 1
            inventoryItem.quantity -= 1;

            //check if the quantity is 0, if it is we need to remove this item from the itemsForPlayer list
            if (inventoryItem.quantity == 0)
            {
                itemsForPlayer.RemoveAt(currentSelectedIndex);
                currentSelectedIndex = 0;
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

        if (currentSelectedIndex == itemsForPlayer.Count)
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

    public void AddItemToInventory(ItemScriptableObject addedItem)
    {
        int countItems = itemsForPlayer.Where(x => x.item == addedItem).ToList().Count;

        if(countItems == 0)
        {
            itemsForPlayer.Add(new InventoryItem() { item = addedItem, quantity = 1 });

            itemsSelectionPanel.transform.Find("Button" + _buttonID).gameObject.SetActive(true);

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

    private void RefreshInventoryGUI()
    {
        int buttonId = 0;

        foreach (InventoryItem i in itemsForPlayer)
        {
            //load the button
            //GameObject button = itemsSelectionPanel.transform.Find("Button" + buttonId).gameObject;
            Transform button = itemsSelectionPanel.transform.Find("Button" + buttonId);

            if(button == null)
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
            _buttonID = buttonId;

        }

        //set active false redundant buttons
        for (int i = buttonId; i < 3; i++)
        {
            itemsSelectionPanel.transform.Find("Button" + i).gameObject.SetActive(false);
            
        }

    }

    public class InventoryItem
    {
        public ItemScriptableObject item { get; set; }
        public int quantity { get; set; }
    }
}
