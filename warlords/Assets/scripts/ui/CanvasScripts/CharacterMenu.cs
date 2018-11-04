using Assets.scripts.util;
using Assets.scripts.vo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (IsVisible())
        {
            updateShownStats();
        }
    }

    private void updateShownStats()
    {
        Hero hero = getGameLogic().getMyHero();

        GameObject.Find("Character Title Text").GetComponent<Text>().text = hero.class_type;
        GameObject.Find("Stat (HP)/Value Text").GetComponent<Text>().text = "" + hero.maxHp;
        GameObject.Find("Stat (Level)/Value Text").GetComponent<Text>().text = "" + hero.level;
        GameObject.Find("Stat (Armor)/Value Text").GetComponent<Text>().text = "" + hero.armor;
        GameObject.Find("Stat (Energy)/Value Text").GetComponent<Text>().text = hero.resource + "/" + hero.maxResource;
    }

    public void Toggle()
    {
        if (getUIWindow().IsVisible)
        {
            Debug.Log("Hiding character menu");
            getUIWindow().Hide();
        }
        else
        {
            Debug.Log("Showing character menu");
            getUIWindow().Show();
            updateInfo(true);
        }
    }

    public bool IsVisible()
    {
        return getUIWindow().IsVisible;
    }

    public void Show()
    {
        getUIWindow().Show();
        updateInfo(true);
    }

    public void Hide()
    {
        getUIWindow().Hide();
    }

    public void OnAssign(UIEquipSlot slot)
    {
        Debug.Log("Item equipped: " + slot.GetItemInfo().Name);
        Item item = getGameLogic().getHeroItemById(slot.GetItemInfo().ItemId);
        if (item != null)
        {
            Debug.Log("Items equipped is : " + item.name);
            item.equipped = true;
            item.setPositionId(-1);
        }
        else
        {
            Debug.Log("Could not find item with itemId: " + slot.GetItemInfo().ItemId);
        }
        List<Item> updatedItems = new List<Item>();
        updatedItems.Add(item);
        // Send this up to server
        getGameLogic().sendEquipment(updatedItems);
    }


    public void updateInfo(bool setSlots)
    {
        Hero hero = getGameLogic().getMyHero();
        
        List<Item> items = getGameLogic().getHeroItems();
        foreach(Item item in items)
        {
            if (item.equipped) {
                UIItemInfo itemInfo = GameUtil.convertItemToItemInfo(item);
                string name = "";
                if (item.position.Equals("MAIN_HAND"))
                {
                    name = "Main Hand";
                }
                else if (item.position.Equals("OFF_HAND"))
                {
                    name = "Off Hand";
                }
                else if (item.position.Equals("HEAD"))
                {
                    name = "Head";
                }
                else if (item.position.Equals("SHOULDERS"))
                {
                    name = "Shoulders";
                }
                else if (item.position.Equals("CHEST"))
                {
                    name = "Chest";
                }
                else if (item.position.Equals("LEGS"))
                {
                    name = "Pants";
                }
                else if (item.position.Equals("BOOTS"))
                {
                    name = "Boots";
                }

                UIEquipSlot slot = GameObject.Find("Slot (" + name + ")").GetComponent<UIEquipSlot>();
                slot.Assign(itemInfo);
                Debug.Log("Item equipped: " + item.toString());
            }
        }
    }

    public void AssignedItem(UIEquipSlot itemSlot)
    {
        Debug.Log("--- Character Assigned item item " + itemSlot.name + " " + itemSlot.GetItemInfo().Name);
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

    Chat getChat()
    {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }

}
