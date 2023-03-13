using Assets.scripts.util;
using Assets.scripts.vo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestInventory : MonoBehaviour {
    private static UIItemDatabase itemDatabase;

    // Send this up to server
    public List<Item> updatedItems = new List<Item>();
    
    // Use this for initialization
    void Start()
    {
        itemDatabase = Resources.Load("Databases/ItemDatabase") as UIItemDatabase;

        for (int i = 0; i < 42; i++)
        {
            itemDatabase.items[i] = null;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void itemAssigned(UIItemSlot item)
    {
        string newItemPos = item.name.Substring(6, item.name.Length - 7);
        if (int.Parse(newItemPos) != item.GetItemInfo().ID)
        {
            Debug.Log("Item assigned: " + item.GetItemInfo().Name + " Moved to position " + newItemPos + " Was pos: " + item.GetItemInfo().ID);
            // Change item position in gameLogicList

            for (int i = 0; i < getGameLogic().getHeroItems().Count; i++)
            {
                //Debug.Log("Comparing itemid on pos: " + item.GetItemInfo().ItemId + " With item in gameLogic : " + getGameLogic().getHeroItems()[i].itemId);
                if (getGameLogic().getHeroItems()[i].id == item.GetItemInfo().ItemId)
                {
                    getGameLogic().getHeroItems()[i].positionId = int.Parse(newItemPos);
                    Debug.Log("Moved item " + getGameLogic().getHeroItems()[i].name + " To position " + newItemPos);
                    getGameLogic().getHeroItems()[i].equipped = false;
                    updatedItems.Add(getGameLogic().getHeroItems()[i]);
                }
            }

        }
    }

    public void ChangedWindow(UIWindow window, UIWindow.VisualState state)
    {
        Debug.Log("Window visibility: " + state.ToString());
        if (state == UIWindow.VisualState.Hidden)
        {
            sendUpdatedItemsToServer();
        }
    }

    public void replaceDatabaseWithGameLogicItemSet()
    {
        List<Item> items = getGameLogic().getHeroItems();
        Debug.Log("Hero has this many items: " + items.Count);
        itemDatabase.items = new UIItemInfo[42];
        for (int i = 0; i < items.Count; i++)
        {
            Item updatedItem = items[i];
            //Debug.Log("Item in inventory: " + updatedItem.toString());
            if (!updatedItem.equipped)
            {
                UIItemInfo item = GameUtil.convertItemToItemInfo(updatedItem);
                if (updatedItem.positionId != -1 && updatedItem.positionId != 0)
                {
                    item.ID = (updatedItem.positionId);
                }
                else
                {
                    item.ID = getFreeInventoryPosition(items);
                    updatedItem.positionId = item.ID;
                    // Add to list to send up to server
                    updatedItems.Add(updatedItem);
                }
                Debug.Log("Setting item : " + updatedItem.name + " on position : " + item.ID);
                itemDatabase.items[updatedItem.positionId] = item;
            }
        }

    }

    private int getFreeInventoryPosition(List<Item> items)
    {
        for (int i = 0; i < 42; i++)
        {
            bool isFree = true;
            foreach (Item item in items)
            {
                if (item.positionId == i)
                {
                    isFree = false;
                }
            }
            if (isFree)
            {
                return i;
            }
        }
        return 0;
    }

    public void OpenInventory()
    {
        replaceDatabaseWithGameLogicItemSet();
        updateAllItemSlotsWithFreshItems();
        getUIWindow().Show();
    }

    private void updateAllItemSlotsWithFreshItems()
    {
        for (int i = 1; i < 42; i++)
        {
            Test_UIItemSlot_Assign slotItem = getUIWindow().gameObject.transform.Find("Content/Slots/Slot ("+i+")").GetComponent<Test_UIItemSlot_Assign>();
            slotItem.getFreshItemFromDatabase();
        }
    }

    public bool IsVisible()
    {
        return getUIWindow().IsVisible;
    }

    public void CloseInventory()
    {
        sendUpdatedItemsToServer();
        getUIWindow().Hide();
    }

    public void sendUpdatedItemsToServer()
    {
        // Send this up to server
        if (updatedItems.Count > 0)
        {
            Debug.Log("Sending these many items to sever that is updated: " + updatedItems.Count);
            getGameLogic().sendEquipment(updatedItems);
            updatedItems = new List<Item>();
        }
    }

    UIWindow getUIWindow()
    {
        return ((UIWindow)transform.GetComponent(typeof(UIWindow)));
    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    KeyboardInput getKeyboardInput()
    {
        return ((KeyboardInput)GameObject.Find("GameLogicObject").GetComponent(typeof(KeyboardInput)));
    }
}
