using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InventoryScript : MonoBehaviour {
    public GameObject inventoryUiPrefab;
    public GameObject generalContent;
    public int totalPoints = 0;
    public Text pointsLeft;
    public Text title;

    private List<Talent> talents = new List<Talent>();
    private List<Ability> abilities = new List<Ability>();
    private int calculationOfPoints = 0;


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

            // Need to add this as well
            //slot.onUnassign.AddListener();
            slot.Unassign();
        }

        List<Item> items = getGameLogic().getHeroItems();
        itemDatabase.items = new UIItemInfo[42];
        for (int i = 0; i < items.Count; i++)
        {
            Item updatedItem = items[i];
            Debug.Log("Setting item : " + updatedItem.name + " on position : " + updatedItem.positionId);
            UIItemInfo item = new UIItemInfo();
            item.Name = updatedItem.name;
            item.ID = (updatedItem.positionId);
            item.ItemId = updatedItem.id;
            item.Quality = UIItemQuality.Common;
            item.Description = "Cool item";
            item.AttackSpeed = 1.0f;
            item.Type = "Sword";
            item.ItemType = (int)UIEquipmentType.Weapon_MainHand;
            item.Subtype = "One handed";
            item.Icon = Resources.Load<Sprite>("sprites/items/" + updatedItem.image);
            item.EquipType = UIEquipmentType.Weapon_MainHand;
            itemDatabase.items[updatedItem.positionId] = item;
            // Try to assign equipslots
            UIItemSlot slot = GameObject.Find("Slot (" + updatedItem.positionId + ")").GetComponent<UIItemSlot>();
            slot.Assign(item);
        }
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
                Debug.Log("Items new positionId is : " + slot.ID + 1);
                item.positionId = slot.ID + 1;
            }
            else
            {
                Debug.Log("Could not find item with itemId: " + slot.GetItemInfo().ItemId);
            }
        }
       
        // Send this up to server
        
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
	}

    

    public void refresh() {
        Debug.Log("Refreshing the talents");
        abilities = getGameLogic().getAbilities();
        talents = getGameLogic().getMyHero().talents;
        totalPoints = getGameLogic().getMyHero().getTotalTalentPoints();
        
        int i = 0;
        foreach (Ability ability in abilities)
        {
            // Set all icons in menu bar
            //Debug.Log("Trying to find talent icon on position : " + ability.position + " With ability id: " + ability.id + " i : " + i + " ability name: " + ability.name);
            GameObject spellicon = GameObject.Find("TalentMenuIcon (" + i  + ")");
            UISpellSlot script = ((UISpellSlot)spellicon.GetComponent(typeof(UISpellSlot)));
            UISpellInfo spInfo = new UISpellInfo();
            spInfo.ID = ability.id;
            if (i == 0)
            {
                spInfo.Icon = Resources.Load<Sprite>("sprites/items/general");
            }
            else
            {
                spInfo.Icon = Resources.Load<Sprite>("sprites/items/" + ability.image);
            }
            script.Assign(spInfo);
            i++;
        }
    }
    

    public void saveTalents() {
        Debug.Log("Saving talents");
        List<Talent> talentsToSend = new List<Talent>();
        foreach (var talent in talents) {
            if (talent.getGameObject() != null)
            {
                foreach (var buttonHolder in talent.getGameObject().GetComponentsInChildren<Image>()) {
                    if (buttonHolder.name.Equals("Talent Slot")) {
                        UITalentSlot script = ((UITalentSlot)buttonHolder.GetComponent(typeof(UITalentSlot)));
                        Debug.Log("We found UiTalentSlot : " + script.getCurrentPoints() + " For talent " + talent.talentId);
                        talent.setPointAdded(script.getCurrentPoints());
                        if (talent.getPointAdded() > 0)
                        {
                            talentsToSend.Add(talent);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Talent had no gameObject, why? " + talent.description);
            }
        }

		Hero myHero = getGameLogic().getMyHero();
		getCommunication().updateTalents(myHero.id, talentsToSend);

        dismissWindow();
    }

    public void dismissWindow()
    {
        getUIWindow().Hide();
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
