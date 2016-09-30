using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    #region private varialbes
    private Vector2 startPosition;
    #endregion

    #region public variables
    public Transform parentToReturnTo = null;
    public enum Slot { WEAPON, HEAD, CHEST, LEGS, FEET};
    public Slot typeOfItem = Slot.WEAPON;
    #endregion


    public void Start()
    {
        startPosition = this.transform.position;

    }

    /// <summary>
    /// Handle events when user beings to drag an UI item
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Started draging object");
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    /// <summary>
    /// Handles the current draging of an UI item
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Currently draging object");
        this.transform.position = eventData.position;
        
    }
    /// <summary>
    /// We end up here when we have finished draging an object
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //this.transform.position = startPosition;
        Debug.Log("Stoped draging object");
        this.transform.SetParent(parentToReturnTo);
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}
