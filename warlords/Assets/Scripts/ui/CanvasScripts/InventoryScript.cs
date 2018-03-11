using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Assets.scripts.util;

public class InventoryScript : MonoBehaviour {
    public GameObject inventoryUiPrefab;
    public GameObject generalContent;
    public int totalPoints = 0;
    public Text pointsLeft;
    public Text title;

    private List<Talent> talents = new List<Talent>();
    private List<Ability> abilities = new List<Ability>();

    // Send this up to server
    List<Item> updatedItems = new List<Item>();


    private static UIItemDatabase itemDatabase;


    // Use this for initialization
    void Start () {
        itemDatabase = Resources.Load("Databases/ItemDatabase") as UIItemDatabase;
        updateItems();
    }

    public void updateItems()
    {
        for (int i = 0; i < 42; i++)
        {
            UIItemSlot slot = GameObject.Find("Slot (" + (i + 1) + ")").GetComponent<UIItemSlot>();
            slot.ID = i;
            // This needs to be done to all slots, not this way cos it only maps to item counts
            slot.onAssign.AddListener(ItemWasAssigned);
            slot.Unassign();
        }

        List<Item> items = getGameLogic().getHeroItems();
        itemDatabase.items = new UIItemInfo[42];
        for (int i = 0; i < items.Count; i++)
        {
            Item updatedItem = items[i];
            if (!updatedItem.equipped) { 
                Debug.Log("Setting item : " + updatedItem.name + " on position : " + updatedItem.positionId);
                UIItemInfo item = GameUtil.convertItemToItemInfo(updatedItem);
                if (updatedItem.positionId != -1)
                {
                    item.ID = (updatedItem.positionId);
                }
                else
                {
                    item.ID = getFreeInventoryPosition(items);
                    updatedItem.positionId = item.ID;
                    updatedItems.Add(updatedItem);
                }
                itemDatabase.items[updatedItem.positionId] = item;
                // Try to assign equipslots
                UIItemSlot slot = GameObject.Find("Slot (" + updatedItem.positionId + ")").GetComponent<UIItemSlot>();
                slot.Assign(item);
            }
        }
    }

    private int getFreeInventoryPosition(List<Item> items)
    {
        for (int i = 0; i < 42; i++)
        {
            bool isFree = true;
            foreach(Item item in items)
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

    private void ItemWasAssigned(UIItemSlot slot)
    {
        if (slot.GetItemInfo() != null && (slot.ID != slot.GetItemInfo().ID))
        {
            itemDatabase.items[slot.GetItemInfo().ID] = null;
            Debug.Log("Item was assigned " + slot.GetItemInfo().Name + " arrayPos : " + slot.ID + " ItemInfoId: " + slot.GetItemInfo().ID);
            slot.GetItemInfo().ID = slot.ID;
            itemDatabase.items[slot.ID] = slot.GetItemInfo();

            Item item = getGameLogic().getHeroItemById(slot.GetItemInfo().ItemId);
            if (item != null)
            {
                Debug.Log("Items new positionId is : " + (slot.ID + 1));
                item.positionId = (slot.ID + 1);
                item.equipped = false;
                updatedItems.Add(item);
            }
            else
            {
                Debug.Log("Could not find item with itemId: " + slot.GetItemInfo().ItemId);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (getUIWindow().IsVisible)
        {
            //calculatePoints();
        }
    }

	public void toggleInventory(){
		if (getUIWindow ().IsVisible) {
			getUIWindow ().Hide ();
		} else {
			getUIWindow().Show();
            Debug.Log("Items in inventory: " + itemDatabase.items.Length);
            updateItems();
        }
	}

	public void showInventory(){
		getUIWindow ().Show ();
	}

	public void hideInventory(){
		getUIWindow ().Hide ();

        // Send this up to server
        getGameLogic().sendEquipment(updatedItems);
    }
 
    public void dismissWindow()
    {
        getUIWindow().Hide();

        // Send this up to server
        getGameLogic().sendEquipment(updatedItems);
    }

    UIWindow getUIWindow()
    {
        return ((UIWindow)transform.GetComponent(typeof(UIWindow)));
    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
