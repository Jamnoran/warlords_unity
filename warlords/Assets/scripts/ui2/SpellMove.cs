using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpellMove : MonoBehaviour, IDragHandler, IEndDragHandler, IDropHandler, IBeginDragHandler
{
    private bool flag = true;
    private List<Ability> abilities;
    private GameObject TheCanvas;
    public GameObject OriginalParent;
    public GameObject BackDropImage;

	void Start () {
       TheCanvas = GameObject.FindWithTag("Canvas");
       
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
                    Sprite abilitySprite = Resources.Load<Sprite>("sprites/items/" + abilities[i].image);
                    this.GetComponent<Image>().sprite = abilitySprite;
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
        this.transform.SetParent(OriginalParent.transform);
        this.transform.position = BackDropImage.transform.position;

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
}
