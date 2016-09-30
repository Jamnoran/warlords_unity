using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{

    #region public variables
    public Draggable.Slot typeOfItem = Draggable.Slot.WEAPON;
    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("OnPointerEnter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("OnPointerExit");
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop to: " + gameObject.name);
        Draggable itemToDrag = eventData.pointerDrag.GetComponent<Draggable>();
        if (itemToDrag != null)
        {
            if(typeOfItem == itemToDrag.typeOfItem)
            { 
            itemToDrag.parentToReturnTo = this.transform;
            }
        }
    }
}
