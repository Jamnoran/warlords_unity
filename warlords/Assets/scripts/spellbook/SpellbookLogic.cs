using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Assets.scripts.vo;
public class SpellbookLogic : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler, IBeginDragHandler
{
    private GameObject spellBook;
    public GameObject spell;
    private GameObject spellSlot1;
    private GameObject spellSlot2;
    private GameObject spellSlot3;
    private GameObject spellSlot4;
    private GameObject spellSlot5;
    private GameObject spellSlot6;
    private GameObject spellSlot7;
    //in this list we keep information about all the slots in the players action bar so we can calculate where to drop spells etc
    private List<GameObject> listOfSpellSlots = new List<GameObject>();
    //keep track of slots that allready have an ability in the players action bar so we can swap it with the one currently selected
    private List<string> listOfBusySpellSlots = new List<string>();
    //store the original position of our spell so we can snap it back to the spellbook if placed in invalid places
    private GameObject originalParent;
    private Vector3 originalPosition;
    private Vector3 currentPosition;
    private Transform currentParent;
    private Transform tempParent;
    private int thisPosition;


    // Initialize spellbookslots and other crap
    public void Start()
    {

        spellBook = GameObject.FindWithTag("slotpanel");
        spell = GameObject.Find(this.transform.name);
        spellSlot1 = GameObject.FindWithTag("spell1");
        spellSlot2 = GameObject.FindWithTag("spell2");
        spellSlot3 = GameObject.FindWithTag("spell3");
        spellSlot4 = GameObject.FindWithTag("spell4");
        spellSlot5 = GameObject.FindWithTag("spell5");
        spellSlot6 = GameObject.FindWithTag("spell6");
        spellSlot7 = GameObject.FindWithTag("spell7");

        //add all spellSlots to the list
        listOfSpellSlots.Add(spellSlot1);
        listOfSpellSlots.Add(spellSlot2);
        listOfSpellSlots.Add(spellSlot3);
        listOfSpellSlots.Add(spellSlot4);
        listOfSpellSlots.Add(spellSlot5);
        listOfSpellSlots.Add(spellSlot6);
        listOfSpellSlots.Add(spellSlot7);

        //Initilize size of spell icon
        ChangeScaleOnIcon(0.8f, 0.8f, 0.8f);


        thisPosition = getGameLogic().getAbility((int)getGameLogic().getAbilityIdByAbilityName(spell.transform.name)).position;

        if (thisPosition == 4)
        {
            spell.transform.SetParent(spellSlot4.transform);
            spell.transform.position = spellSlot4.transform.position;
        }

        
        originalPosition = this.transform.position;
        originalParent = this.transform.parent.gameObject;
        currentParent = spell.transform.parent;
        currentPosition = spell.transform.position;

        
    }

    public void OnDrag(PointerEventData data)
    {
      
        this.gameObject.transform.position = Input.mousePosition;
        spell.transform.SetParent(spellBook.transform);
    }

    public void OnDrop(PointerEventData data)
    {
     
    }


    public void OnBeginDrag(PointerEventData data)
    {
        tempParent = this.transform.parent;
        var name = tempParent.name;
    }

    public void OnEndDrag(PointerEventData data)
    {
        
        this.transform.position = snapToActiveSpell(this.transform.position.x, this.transform.position.y, listOfSpellSlots);
        currentPosition = spell.transform.position;
        currentParent = spell.transform.parent;
        Debug.Log("Current parent is: " + currentParent.name);
        
    }


    /// <summary>
    /// Calculate and return the vector3 position where we wish to put the spell, if within range put it in action bar, else return it to the spellbook
    /// </summary>
    /// <param name="spellX"></param>
    /// <param name="spellY"></param>
    /// <param name="spellSlots"></param>
    /// <returns>Vector3 position to drop spell.</returns>
    public Vector3 snapToActiveSpell(float spellX, float spellY, List<GameObject> spellSlots)
    {

        //loop trough all slots in actionbar to check if we can put spell there
        for (int i = 0; i < spellSlots.Count; i++)
        {

            //snap if within proximity of actionslot
            if ((spellX <= spellSlots[i].transform.position.x + 10) && (spellX >= spellSlots[i].transform.position.x - 10) && (spellY <= spellSlots[i].transform.position.y + 10) && (spellY >= spellSlots[i].transform.position.y - 10))
            {
                getCommunication().updateAbilityPosition((int)getGameLogic().getAbilityIdByAbilityName(spell.transform.name), i);
                    
                  
                return spellSlots[i].transform.position;
            }
        }

        return originalPosition;
    
    }


  private void ChangeScaleOnIcon(float xValue, float yValue, float zValue)
    {
        spell.transform.localScale = new Vector3(xValue, yValue, zValue);
    }





    SpellSlotBusyOrNot getSlotTracker()
    {
        return ((SpellSlotBusyOrNot)GameObject.Find("SpellSlotBusyOrNot").GetComponent(typeof(SpellSlotBusyOrNot)));
    }
    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
