using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Werewolf.StatusIndicators.Components;

public class KeyboardInput : MonoBehaviour {

    public int state = 0;

    public static int READY = 0;
    public static int LOBBY = 1;
    public static int IN_GAME = 2;
    public static int CHAT = 3;
    public static int MENU = 4;
    public static int CHARACTER = 5;
    public static int INVENTORY = 6;
    public static int TALENTS = 7;
    public static int SPELLS = 8;


    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("escape"))
        {
            Debug.Log("Escape is pressed state is : " + state);
            if (state == MENU)
            {
                Debug.Log("Hiding menu");
                getMenu().Hide();
                state = READY;
            }
            else if (state == READY)
            {
                state = MENU;
                getMenu().Show();
            }else if (state == CHAT)
			{
                getChat().escPressed();
            }
            if (state != MENU)
            {
                getCharacter().Hide();
                getInventory().Hide();
                getTalents().Hide();
                state = READY;
            }
        }
        if (Input.GetKeyUp("enter") || Input.GetKeyUp("return"))
        { 
			if(state == READY)
			{
                // Show chat
                state = CHAT;
                getChat().enterPressed();
            }else if (state == CHAT)
            {
                getChat().enterPressed();
                state = READY;
            }
        }

        // TODO : cant use toggle need to check each
        if (state != CHAT)
        {
            // Combat actions
            if (Input.GetKeyUp("a"))
            {
                bool autoAttacking = getGameLogic().getMyHero().getAutoAttacking();
                Debug.Log("Hero is now attacking : " + !autoAttacking);
                getGameLogic().getMyHero().setAutoAttacking(!autoAttacking);
            }

            if (Input.GetKeyUp("s"))
            {
                Debug.Log("Send stop");
                getGameLogic().stopHero(getGameLogic().getMyHero().id);
            }

            // Spell input


            // Character input
            if (Input.GetKeyDown("c"))
            {
                if (getCharacter().IsVisible())
                {
                    state = READY;
                }
                else
                {
                    state = CHARACTER;
                }
                getCharacter().Toggle();

            }

            // Talent input
            if (Input.GetKeyDown("t"))
            {
                Debug.Log("T is pressed and talent window is visible: " + getTalents().IsVisible());
                if (getTalents().IsVisible())
                {
                    state = READY;
                    Debug.Log("Hiding menu");
                    getTalents().Hide();
                }
                else
                {
                    state = TALENTS;
                    getTalents().Show();
                }
            }

            // Inventory
            if (Input.GetKeyDown("i"))
            {
                if (getInventory().IsVisible())
                {
                    state = READY;
                }
                else
                {
                    state = INVENTORY;
                }
                getInventory().toggleInventory();
            }

            // Spells
            if (Input.GetKeyDown("p"))
            {
                if (getSpells().IsVisible())
                {
                    state = READY;
                }
                else
                {
                    state = SPELLS;
                }
                getSpells().ToggleSpellBook();
            }

            // Next level
            if (Input.GetKeyUp("n"))
            {
                getCommunication().heroHasClickedPortal(getCommunication().getHeroId());
            }

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

        // For targeting system (cone/aoe/etc)
        //GetInputForSpells();
    }







    public SplatManager splats { get; set; }

    public void setUpSplats(Transform transform)
    {
        // Only works for warlocks so far
        splats = transform.GetComponentInChildren<SplatManager>();
    }

    void GetInputForSpells()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Confirming with left mouse, time to find target");
            splats.CancelSpellIndicator();
            splats.CancelRangeIndicator();
            splats.CancelStatusIndicator();
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Canceling with right mouse");
            splats.CancelSpellIndicator();
            splats.CancelRangeIndicator();
            splats.CancelStatusIndicator();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            splats.SelectSpellIndicator("Point");
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            splats.SelectSpellIndicator("Cone");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            splats.SelectSpellIndicator("Direction");
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            splats.SelectSpellIndicator("Line");
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            splats.SelectStatusIndicator("Status");
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            splats.SelectRangeIndicator("Range");
        }
    }














    TargetingByFKeys getTargetingByFKeys()
    {
        return ((TargetingByFKeys)GameObject.Find("GameLogicObject").GetComponent(typeof(TargetingByFKeys)));
    }

    InGameMenu getMenu()
    {
        return ((InGameMenu)GameObject.Find("GameLogicObject").GetComponent(typeof(InGameMenu)));
    }
    Talents getTalents()
	{
		return ((Talents)GameObject.Find("TalentsIcons").GetComponent(typeof(Talents)));
	}

    InventoryScript getInventory()
    {
        return ((InventoryScript)GameObject.Find("Window (Inventory)").GetComponent(typeof(InventoryScript)));
    }

    CharacterMenu getCharacter()
    {
        return ((CharacterMenu)GameObject.Find("Window (Character)").GetComponent(typeof(CharacterMenu)));
    }

    HideAndShowSpellBook getSpells()
    {
        return ((HideAndShowSpellBook)GameObject.Find("Window (Spell Book)").GetComponent(typeof(HideAndShowSpellBook)));
    }

    Chat getChat()
    {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

}
