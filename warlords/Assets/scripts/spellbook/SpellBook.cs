using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.scripts.vo;
using UnityEngine.EventSystems;

public class SpellBook : MonoBehaviour {
    
    private bool spellsAreFetched = false;
    private bool isSpellBookOpen = false;
    private Vector3 spellBookOriginalPosition;
    private Vector3 spellBookHidePosition = new Vector3(1000000, 1000000, 0);

    ItemDatabase itemDatabase;
    GameObject spellbookPanel;
    GameObject slotPanel;
    public GameObject spellbookSlot;
    public GameObject spellbookItem;

    public List<Ability> abilities = new List<Ability>();
    public List<GameObject> slots = new List<GameObject>();

    public int slotAmount = 12;

    void Start()
    {
        
        //Grab our panel that holds the spellbook
        spellbookPanel = GameObject.Find("Spellbook Panel");
        spellBookOriginalPosition = spellbookPanel.transform.position;
        //Grab the slot panel that holds the slots for all abilities
        slotPanel = spellbookPanel.transform.Find("Slot Panel").gameObject;

      
        //Loop trough the ammount of slots we want and fill the list with spellbook slots
        for (int i = 0; i < slotAmount; i++)
        {
            //add a null ability to occupy the slots, we can later check if an ability is null and then replace it with a real ability
            abilities.Add(new Ability());
            slots.Add(Instantiate(spellbookSlot));
            slots[i].transform.SetParent(slotPanel.transform);

        }

        spellbookPanel.transform.position = spellBookHidePosition;
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            if (isSpellBookOpen)
            {
                spellbookPanel.transform.position = spellBookHidePosition;
                isSpellBookOpen = false;
            }
            else if(!isSpellBookOpen && !spellsAreFetched)
            {
                
                spellbookPanel.transform.position = spellBookOriginalPosition;
             
                abilities = getGameLogic().getAbilities();
                isSpellBookOpen = true;
                spellsAreFetched = true;
                AddItem();
               
            }
            else if(!isSpellBookOpen && spellsAreFetched)
            {
                spellbookPanel.transform.position = spellBookOriginalPosition;
                isSpellBookOpen = true;

            }
            
        }

        else if (Input.GetKeyDown("p") && abilities != null)
        {
            spellbookPanel.SetActive(true);
        }
    }

    public void AddItem()
    {
     
        for (int i = 0; i < abilities.Count; i++)
        {
            
            GameObject spellObject = Instantiate(spellbookItem);
            spellObject.transform.SetParent(slots[i].transform);
            spellObject.transform.position = slots[i].transform.position;
            Debug.Log(abilities[i].image);
            
            Sprite abilitySprite = Resources.Load<Sprite>("Spells/"+ abilities[i].image);
            spellObject.GetComponent<Image>().sprite = abilitySprite;

            spellObject.transform.name = abilities[i].name;


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
