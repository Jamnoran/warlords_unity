using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationHandler : MonoBehaviour {

    public GameObject topNotification;
    public GameObject bottomNotification;
    public GameObject bigMiddleNotification;

    private bool hordeMode = false;
    private List<string> notifications;
	// Use this for initialization
	void Start () {
        topNotification.transform.Find("Text").GetComponent<Text>().text = "";
        bottomNotification.transform.Find("Text").GetComponent<Text>().text = "";
    }
	
	// Update is called once per frame
	void Update () {
        if (hordeMode)
        {
            int minionsLeft = getHordeMode().getMinionsLeft();
            topNotification.transform.Find("Text").GetComponent<Text>().text = minionsLeft + " minions left";
        }
	}

    // Type 1 = top, 2 = bottom, 3 = middle
    public void showNotification(int type, string notification)
    {
        if (type == 1)
        {
            topNotification.transform.Find("Text").GetComponent<Text>().text = notification;
        }
        else if (type == 2) 
        {
            bottomNotification.transform.Find("Text").GetComponent<Text>().text = notification;
        }
        StartCoroutine(hideNotification(type));
    }
    IEnumerator hideNotification(int type)
    {
        yield return new WaitForSeconds(2);
        if (type == 1)
        {
            topNotification.transform.Find("Text").GetComponent<Text>().text = "";
        }
        else if (type == 2)
        {
            bottomNotification.transform.Find("Text").GetComponent<Text>().text = "";
        }
    }

    public void startHordeMode()
    {
        hordeMode = true;
    }



    HordeMode getHordeMode()
    {
        return ((HordeMode)GameObject.Find("GameLogicObject").GetComponent(typeof(HordeMode)));
    }
}
