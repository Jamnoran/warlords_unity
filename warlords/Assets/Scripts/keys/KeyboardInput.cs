using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour {

    public int state = 0;

    public static int READY = 0;
    public static int LOBBY = 1;
    public static int IN_GAME = 2;
    public static int CHAT = 3;
    public static int MENU = 4;


    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("escape"))
        {
			if(state == CHAT)
			{
				// Close chat
			}
        }
        if (Input.GetKeyUp("enter"))
        { 
			if(state == READY)
			{
				// Show chat

			}
        }
        // Spell input

        // Chat input

        // Menu input

        // Character input

        // Talent input

        if (Input.GetKeyDown("t"))
        {
			getTalents ().toggleTalents();
        }



        if (Input.GetKeyUp("n"))
        {
            getCommunication().heroHasClickedPortal(getCommunication().getHeroId());
        }

        // Targeting system
        if (Input.GetKeyDown(KeyCode.F1))
        {
            getTargetingByFKeys().setTarget(0);
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            getTargetingByFKeys().setTarget(1);
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            getTargetingByFKeys().setTarget(2);
        }
        else if (Input.GetKeyDown(KeyCode.F4))
        {
            getTargetingByFKeys().setTarget(3);
        }
    }




    TargetingByFKeys getTargetingByFKeys()
    {
        return ((TargetingByFKeys)GameObject.Find("GameLogicObject").GetComponent(typeof(TargetingByFKeys)));
    }

	Talents getTalents()
	{
		return ((Talents)GameObject.Find("TalentsIcons").GetComponent(typeof(Talents)));
	}


    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }
}
