using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalentsDisplay : MonoBehaviour {

    public GameObject talents;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("t") && !getChat().IsInputFieldFocused())
        {
            Debug.Log("Showing talents");
            if (!talents.active)
            {
                getTalentScript().refresh();
                talents.SetActive(true);
            }
            else
            {
                talents.SetActive(false);
            }
        }
    }

    Talents getTalentScript()
    {
        return ((Talents)talents.GetComponent(typeof(Talents)));
    }


    Chat getChat()
    {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }
}
