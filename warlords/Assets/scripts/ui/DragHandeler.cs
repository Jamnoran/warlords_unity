using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragHandeler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform SetThisAsParent;
    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    public Transform snapBack;
    public Transform originalParent;
    public Transform startParent;
    public Transform slot1;
    public Transform slot2;
    public Transform slot3;
    public Transform slot4;
    public Transform slot5;
    public Transform slot6;
    public Transform slot7;
    public Transform slot8;
    public Transform slot9;
    public Transform slot10;
    public Transform slot11;


    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData)
    {
      
            transform.position = eventData.position;
        
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData)
    {
        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent)
        {
            transform.position = startPosition;
        }
        if(transform.parent == snapBack)
        {
            transform.position = originalParent.position;
            transform.parent = originalParent;
            Debug.Log("was in slot1");
        }
     
    }

    #endregion



}