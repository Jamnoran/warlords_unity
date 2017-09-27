using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellMove : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler, IBeginDragHandler
{
    public int spell;
    private bool flag = true;
    private List<Ability> abilities;
    private GameObject TheCanvas;
    public GameObject OriginalParent;
    public GameObject BackDropImage;
    private GameObject actionBarSlot1;
    private GameObject actionBarSlot2;
    private GameObject actionBarSlot3;
    private GameObject actionBarSlot4;
    private GameObject actionBarSlot5;
    private GameObject actionBarSlot6;
    private GameObject actionBarSlot7;
    private GameObject spellBookSlot1;
    private GameObject spellBookSlot2;
    private GameObject spellBookSlot3;
    private GameObject spellBookSlot4;
    private GameObject spellBookSlot5;
    private GameObject spellBookSlot6;
    private GameObject spellBookSlot7;
    private GameObject spellBookSlot8;
    private GameObject spellBookSlot9;
    private GameObject spellBookSlot10;
    private GameObject spellBookSlot11;
    private GameObject spellBookSlot12;
    private List<GameObject> actionBarSlots = new List<GameObject>();
    private bool fromSpellBook;

    private GameObject currentParent;

    void Start () {
       TheCanvas = GameObject.FindWithTag("Canvas");
        actionBarSlot1 = GameObject.FindWithTag("spell1");
        actionBarSlot2 = GameObject.FindWithTag("spell2");
        actionBarSlot3 = GameObject.FindWithTag("spell3");
        actionBarSlot4 = GameObject.FindWithTag("spell4");
        actionBarSlot5 = GameObject.FindWithTag("spell5");
        actionBarSlot6 = GameObject.FindWithTag("spell6");
        actionBarSlot7 = GameObject.FindWithTag("spell7");
        actionBarSlots.Add(actionBarSlot1);
        actionBarSlots.Add(actionBarSlot2);
        actionBarSlots.Add(actionBarSlot3);
        actionBarSlots.Add(actionBarSlot4);
        actionBarSlots.Add(actionBarSlot5);
        actionBarSlots.Add(actionBarSlot6);
        actionBarSlots.Add(actionBarSlot7);

        spellBookSlot1 = GameObject.FindWithTag("slot1");
        spellBookSlot2 = GameObject.FindWithTag("slot2");
        spellBookSlot3 = GameObject.FindWithTag("slot3");
        spellBookSlot4 = GameObject.FindWithTag("slot4");
        spellBookSlot5 = GameObject.FindWithTag("slot5");
        spellBookSlot6 = GameObject.FindWithTag("slot6");
        spellBookSlot7 = GameObject.FindWithTag("slot7");
        spellBookSlot8 = GameObject.FindWithTag("slot8");
        spellBookSlot9 = GameObject.FindWithTag("slot9");
        spellBookSlot10 = GameObject.FindWithTag("slot10");
        spellBookSlot11 = GameObject.FindWithTag("slot11");
        spellBookSlot12 = GameObject.FindWithTag("slot12");

       

    }
	
	void Update () {
        if (abilities == null)
        {
            abilities = getGameLogic().getAbilities();
        }
        if (abilities != null && flag)
        {
            flag = false;
            for (int i = 0; i < 11; i++)
            {
                if (this.transform.parent.parent.name == "Spell" + i.ToString())
                {
                    if ((abilities.Count >= i-1) && abilities[i] != null)
                    {
                        Sprite abilitySprite = Resources.Load<Sprite>("sprites/items/" + abilities[i].image);
                        this.GetComponent<Image>().sprite = abilitySprite;
                    }
                    
                }

             
            }

            if (abilities[spell].position == 1)
            {
                setAbilityFromDb(actionBarSlot1.transform);
            }
            if (abilities[spell].position == 2)
            {
                setAbilityFromDb(actionBarSlot2.transform);
            }
            if (abilities[spell].position == 3)
            {
                setAbilityFromDb(actionBarSlot3.transform);
            }
            if (abilities[spell].position == 4)
            {
                setAbilityFromDb(actionBarSlot4.transform);
            }
            if (abilities[spell].position == 5)
            {
                setAbilityFromDb(actionBarSlot5.transform);
            }
            if (abilities[spell].position == 6)
            {
                setAbilityFromDb(actionBarSlot6.transform);
            }
            if (abilities[spell].position == 7)
            {
                setAbilityFromDb(actionBarSlot7.transform);
            }
        }


        if (this.gameObject.transform.parent.gameObject == OriginalParent && !fromSpellBook)
        {
            savePositionToDb();
            fromSpellBook = true;
 
        }

        if (currentParent != this.transform.parent.gameObject)
        {
            for (int i = 1; i < 7; i++)
            {
                if (this.transform.parent.name == "Spell" + i.ToString())
                {
                    savePositionToDb(i-1);
                    currentParent = this.transform.parent.gameObject;
                    
                }
            }

      
        }




    }

  

    public void OnDrag(PointerEventData data)
    {
        this.transform.SetParent(TheCanvas.transform, false);
        this.transform.position = Input.mousePosition;
      
    }

    public void OnDrop(PointerEventData data)
    {

    }


    public void OnBeginDrag(PointerEventData data)
    {

    }

    public void OnEndDrag(PointerEventData data)
    {
        this.transform.position = SnapToActionBar(this.transform.position.x, this.transform.position.y);
        //this.transform.SetParent(OriginalParent.transform);
        //this.transform.position = BackDropImage.transform.position;

    }

    public Vector3 SnapToActionBar(float xPos, float yPos)
    {
        var margin = 20;
        for (int i = 0; i < actionBarSlots.Count; i++)
        {

            if ((xPos <= actionBarSlots[i].transform.position.x + margin) && (xPos >= actionBarSlots[i].transform.position.x - margin) && (yPos <= actionBarSlots[i].transform.position.y + margin) && (yPos >= actionBarSlots[i].transform.position.y - margin))
            {

                if (actionBarSlots[i].transform.childCount > 0 && fromSpellBook)
                {
                    //todo: swap spells between actionbar slots
                    //todo: save spells position from database

                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell0Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot1);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell1Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot2);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell2Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot3);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell3Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot4);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell4Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot5);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell5Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot6);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell6Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot7);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell7Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot8);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell8Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot9);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell9Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot10);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell10Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot11);
                    }
                    if (actionBarSlots[i].transform.GetChild(0).transform.name == "spell11Image")
                    {
                        savePositionToDb(i);
                        return calcPos(actionBarSlots[i], actionBarSlots[i].transform.GetChild(0), spellBookSlot12);
                    }

                }

                else if (actionBarSlots[i].transform.childCount > 0 && !fromSpellBook)
                {
                    Debug.Log("Moving " + this.transform.name + " to " + actionBarSlots[i].transform.name);

                    actionBarSlots[i].transform.GetChild(0).transform.position = currentParent.transform.position;
                    actionBarSlots[i].transform.GetChild(0).transform.SetParent(currentParent.transform);
                    this.transform.SetParent(actionBarSlots[i].transform);
                    this.transform.localScale = this.transform.parent.localScale;
                    fromSpellBook = false;
                    savePositionToDb(i);
                    return actionBarSlots[i].transform.position;
                }

                else
                {
                    this.transform.SetParent(actionBarSlots[i].transform);
                    this.transform.localScale = this.transform.parent.localScale;
                    currentParent = this.transform.parent.gameObject;
                    Debug.Log("Added to empty slot, current parent is: " + currentParent.transform.name);
                    fromSpellBook = false;
                    savePositionToDb(i);
                    return actionBarSlots[i].transform.position;
                }
               

                //if we come from spellbook and replace a spell in actionbar, replay the current spell to its original position

            }

        }
        this.transform.SetParent(OriginalParent.transform);
        this.transform.localScale = this.transform.parent.localScale;
        savePositionToDb();
        return BackDropImage.transform.position;

    }

    private Vector3 calcPos(GameObject setParentToThis, Transform child, GameObject spellBookSlot )
    {
        this.transform.SetParent(setParentToThis.transform);
        this.transform.localScale = this.transform.parent.localScale;
        currentParent = this.transform.parent.gameObject;
        child.SetParent(spellBookSlot.transform, false);
 
        for (int i = 0; i < 12; i++)
        {
            if (spellBookSlot.transform.Find("original" + i.ToString()) != null)
            {
                child.transform.position = spellBookSlot.transform.Find("original" + i.ToString()).transform.position;
            }
        }
        
        fromSpellBook = false;
        return setParentToThis.transform.position;
    }

    private void setAbilityFromDb(Transform actionBarSlot)
    {
        this.transform.SetParent(actionBarSlot.transform);
        this.transform.position = actionBarSlot.transform.position;
        this.transform.localScale = this.transform.parent.localScale;
        currentParent = this.transform.parent.gameObject;
        fromSpellBook = false;
    }


    //use overloaded method to save spell to spellbook
    private void savePositionToDb()
    {
        if (getCommunication() != null && abilities != null && abilities[spell] != null)
        {
			Hero myHero = getGameLogic().getMyHero();
			getCommunication().updateAbilityPosition(myHero.id, abilities[spell].id, 0);
        }
    }

    private void savePositionToDb(int position)
    {
        if (getCommunication() != null && abilities != null && abilities[spell] != null)
		{
			Hero myHero = getGameLogic().getMyHero();
			getCommunication().updateAbilityPosition(myHero.id, abilities[spell].id, position + 1);
        }
    }




    GameLogic getGameLogic()
    {
        if (GameObject.Find("GameLogicObject") != null)
        {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        }
        else
        {
            return null;
        }
    }

    ServerCommunication getCommunication()
    {
        if (GameObject.Find("Communication") != null)
        {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        }
        else
        {
            return null;
        }
    }
}
