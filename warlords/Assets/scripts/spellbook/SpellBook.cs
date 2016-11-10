using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour {
    GameObject spellbookPanel;            //this panel keeps the entire spellbook
    GameObject slotPanel;                 //this panel holds the slot to row up the spells
    public GameObject spellbookSlot;      //this is a single slot within the slotsPanel
    public GameObject spellItem;          //this is the actual image representing the spell
    int slotAmount = 24;                  //amount of slots we wish to produce
    public List<Item> items = new List<Item>();
    public List<GameObject> slots = new List<GameObject>();
    ItemDatabase database;

    void Start()
    {
        database = GetComponent<ItemDatabase>();    
        spellbookPanel = GameObject.Find("Spellbook panel");
        slotPanel = spellbookPanel.transform.FindChild("Slots panel").gameObject;
        generateSlots(slotAmount);
        addItem(1);
        addItem(0);
     
    }

  /**
  * Iterates and adds slots to the slotpanel gameobject
  * @param slotAmount The number of slots to be generated
  **/
    void generateSlots(int slotAmount)
    {
        for (int i = 0; i < slotAmount; i++)
        {
            items.Add(new Item());
            slots.Add(Instantiate(spellbookSlot));
            slots[i].transform.SetParent(slotPanel.transform);
        }
    }


    /**
     * Add a single spell to the spellbook
     * @param id The spell id to add to the spellbook
     **/
    public void addItem(int id)
    {
        Item itemToAdd = database.fetchItemByID(id);
        for (int i = 0; i < items.Count; i++)
        {
            //id-1 represents an empty item
            if (items[i].ID == -1)
            {
                items[i] = itemToAdd;
                GameObject spellItemObject = Instantiate(spellItem);
                spellItemObject.transform.SetParent(slots[i].transform);
                spellItemObject.GetComponent<Image>().sprite = itemToAdd.Sprite;
                spellItemObject.transform.position = slots[i].transform.position;
                spellItemObject.name = items[i].Title;
                break;
            }
        }

    }

 
}
