using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationHandler : MonoBehaviour {

    public GameObject hordeNotification;
    public GameObject generalNotification;

    private bool hordeMode = false;
    private List<string> notifications;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (hordeMode)
        {
            hordeNotification.SetActive(true);
            int minionsLeft = getHordeMode().getMinionsLeft();
            hordeNotification.transform.Find("Text 1").GetComponent<Text>().text = minionsLeft + " minions left";
        }
        else
        {
            hordeNotification.SetActive(false);
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
