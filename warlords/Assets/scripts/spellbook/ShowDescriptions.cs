using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ShowDescriptions : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler
{
    private string description;
    private Text desc;
    private Vector3 descriptionHidePosition = new Vector3(1000000, 1000000, 0);
    public GameObject descriptionPanel;
    private GameObject TopObject;
    public void Start()
    {
        TopObject = GameObject.Find("Canvas");
        description = getGameLogic().getAbilityDescriptionByAbilityName(this.gameObject.transform.name);
        descriptionPanel = GameObject.Find("description panel");
        desc = GameObject.Find("desc").GetComponent<Text>();
        descriptionPanel.transform.SetParent(TopObject.transform);
        descriptionPanel.transform.position = descriptionHidePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {

        descriptionPanel.transform.position = descriptionHidePosition;
        desc.text = "";
        Debug.Log("draging item");

    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (transform.parent.name == "Slot(Clone)")
        {
            descriptionPanel.transform.position = new Vector3(this.transform.position.x + 100, this.transform.position.y - 50, 0);
            desc.text = description;
            Debug.Log("description: " + description);
        }
        else
        {
            descriptionPanel.transform.position = descriptionHidePosition;
            desc.text = "";
        }
   
    }
    public void OnPointerExit(PointerEventData data)
    {
        descriptionPanel.transform.position = descriptionHidePosition;
        desc.text = "";
        Debug.Log("Mouse has left the building");
    }



    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
