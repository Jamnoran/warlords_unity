using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SpellbookLogic : MonoBehaviour, IDragHandler, IEndDragHandler
{
    private GameObject spellSlot1;
    private GameObject spellSlot2;
    private GameObject spellSlot3;
    private GameObject spellSlot4;
    private GameObject spellSlot5;
    private GameObject spellSlot6;
    private GameObject spellSlot7;
    private GameObject spellSlot8;
    private GameObject spellSlot9;
    private GameObject spellSlot10;
    private GameObject spellSlot11;
    //in this list we keep information about all the slots in the players action bar so we can calculate where to drop spells etc
    private List<GameObject> listOfSpellSlots = new List<GameObject>();
    //keep track of slots that allready have an ability in the players action bar so we can swap it with the one currently selected
    private List<string> listOfBusySpellSlots = new List<string>();    
    //store the original position of our spell so we can snap it back to the spellbook if placed in invalid places
    private Vector3 originalPosition;

    // Initialize spellbookslots and other crap
    public void Start()
    {
        spellSlot1 = GameObject.FindWithTag("spell1");
        spellSlot2 = GameObject.FindWithTag("spell2");
        spellSlot3 = GameObject.FindWithTag("spell3");
        spellSlot4 = GameObject.FindWithTag("spell4");
        spellSlot5 = GameObject.FindWithTag("spell5");
        spellSlot6 = GameObject.FindWithTag("spell6");
        spellSlot7 = GameObject.FindWithTag("spell7");
        spellSlot8 = GameObject.FindWithTag("spell8");
        spellSlot9 = GameObject.FindWithTag("spell9");
        spellSlot10 = GameObject.FindWithTag("spell10");
        spellSlot11 = GameObject.FindWithTag("spell11");
        //add all spellSlots to the list
        listOfSpellSlots.Add(spellSlot1);
        listOfSpellSlots.Add(spellSlot2);
        listOfSpellSlots.Add(spellSlot3);
        listOfSpellSlots.Add(spellSlot4);
        listOfSpellSlots.Add(spellSlot5);
        listOfSpellSlots.Add(spellSlot6);
        listOfSpellSlots.Add(spellSlot7);
        listOfSpellSlots.Add(spellSlot8);
        listOfSpellSlots.Add(spellSlot9);
        listOfSpellSlots.Add(spellSlot10);
        listOfSpellSlots.Add(spellSlot11);
        
        originalPosition = this.transform.position;
    }

    public void OnDrag(PointerEventData data)
    {
        this.gameObject.transform.position = Input.mousePosition;
    }

    public void OnDrop(PointerEventData data)
    {
     
    }

    public void OnEndDrag(PointerEventData data)
    {
     
            this.transform.position = snapToActiveSpell(this.transform.position.x, this.transform.position.y, listOfSpellSlots);
        
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
        for (int i = 0; i < spellSlots.Count; i++)
        {
            if ((spellX <= spellSlots[i].transform.position.x + 10) && (spellX >= spellSlots[i].transform.position.x - 10) && (spellY <= spellSlots[i].transform.position.y + 10) && (spellY >= spellSlots[i].transform.position.y - 10))
            {
                getSlotTracker().addToList(spellSlots[i].transform.name);
                return spellSlots[i].transform.position;
            }
        }
        return originalPosition;
    }


    public void OnMove(AxisEventData data)
    {
        Debug.Log("OnMove called.");
    }

    public void OnPointerClick(PointerEventData data)
    {
        Debug.Log("OnPointerClick called.");
    }

    public void OnPointerDown(PointerEventData data)
    {
         
    }

    public void OnPointerEnter(PointerEventData data)
    {
        
        Debug.Log("OnPointerEnter called on: ");
    }

    public void OnPointerExit(PointerEventData data)
    {
        Debug.Log("OnPointerExit called.");
    }

    public void OnPointerUp(PointerEventData data)
    {
        Debug.Log("OnPointerUp called.");
    }

    public void OnScroll(PointerEventData data)
    {
        Debug.Log("OnScroll called.");
    }

    public void OnSelect(BaseEventData data)
    {
        Debug.Log("OnSelect called.");
    }

    public void OnSubmit(BaseEventData data)
    {
        Debug.Log("OnSubmit called.");
    }

    public void OnUpdateSelected(BaseEventData data)
    {
        Debug.Log("OnUpdateSelected called.");
    }

    SpellSlotBusyOrNot getSlotTracker()
    {
        return ((SpellSlotBusyOrNot)GameObject.Find("SpellSlotBusyOrNot").GetComponent(typeof(SpellSlotBusyOrNot)));
    }
}
