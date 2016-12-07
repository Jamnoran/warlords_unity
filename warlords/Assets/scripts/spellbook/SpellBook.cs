using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.scripts.vo;

public class SpellBook : MonoBehaviour {

    private bool isFetched = false;

    ItemDatabase itemDatabase;
    GameObject spellbookPanel;
    GameObject slotPanel;
    public GameObject spellbookSlot;
    public GameObject spellbookItem;

    public List<Ability> abilities = new List<Ability>();
    public List<GameObject> slots = new List<GameObject>();

    public int slotAmount = 20;

    void Start()
    {
        //Grab our panel that holds the spellbook
        spellbookPanel = GameObject.Find("Spellbook Panel");
        //Grab the slot panel that holds the slots for all abilities
        slotPanel = spellbookPanel.transform.FindChild("Slot Panel").gameObject;


        //Loop trough the ammount of slots we want and fill the list with spellbook slots
        for (int i = 0; i < slotAmount; i++)
        {
            //add a null ability to occupy the slots, we can later check if an ability is null and then replace it with a real ability
            abilities.Add(new Ability());
            slots.Add(Instantiate(spellbookSlot));
            slots[i].transform.SetParent(slotPanel.transform);

        }
    }

    void Update()
    {
        if (Input.GetKeyDown("p") && !isFetched )
        {
            abilities = getGameLogic().getAbilities();
            isFetched = true;
           

        }

        else if (Input.GetKeyDown("p"))
        {
           
            for (int i = 0; i < slotAmount; i++)
            {

            }
            AddItem(0);
        }
    }

    public void AddItem(int id)
    {
     
        for (int i = 0; i < abilities.Count; i++)
        {
   
            GameObject spellObject = Instantiate(spellbookItem);
            spellObject.transform.SetParent(slots[i].transform);
            spellObject.transform.position = slots[i].transform.position;
        
        }
    }

 /**
 * Get an item from the database from ID
 * @param id The items ID you are looking for
 * @return Item The item matching the param ID if successful, null otherwise
 **/
    public Ability fetchItemByID(int id)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (id == abilities[i].id)
            {
                return abilities[i];
            }
        }
        return null;
    }



    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
