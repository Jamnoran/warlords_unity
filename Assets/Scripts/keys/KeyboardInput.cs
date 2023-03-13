using System;
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

    public GameObject prefabEffect;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("escape"))
        {
            handleEscapePressed();
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
        if (state != LOBBY)
        {
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
                if (Input.GetKeyDown("b"))
                {
                    Instantiate(prefabEffect, new Vector3(0, 0, 0), prefabEffect.transform.rotation);
                }

                // Character input
                if (Input.GetKeyDown("c"))
                {
                    toggleCharacter();
                }

                // Talent input
                if (Input.GetKeyDown("t"))
                {
                    toggleTalents();
                }

                // Inventory
                if (Input.GetKeyDown("i"))
                {
                    toggleInventory();
                }

                // Spells
                if (Input.GetKeyDown("p"))
                {
                    toggleSpells();
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
            setCameraSettings((state == READY));
        }
    }

    private void handleEscapePressed()
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
        }
        else if (state == CHAT)
        {
            getChat().escPressed();
        }
        if (state != MENU)
        {
            getCharacter().Hide();
            getInventory().CloseInventory();
            getTalents().Hide();
            state = READY;
        }
    }

    private void setCameraSettings(bool acceptsCameraMovement)
    {
        getCameraHandler().acceptsRotationOfCamera = acceptsCameraMovement;
        getCameraHandler().acceptsZoomOfCamera = acceptsCameraMovement;
    }

    public void toggleInventory()
    {
        if (getInventory().IsVisible())
        {
            state = READY;
            getInventory().CloseInventory();
        }
        else
        {
            state = INVENTORY;
            getInventory().OpenInventory();
        }
    }

    public void toggleMenu()
    {
        state = MENU;
        getMenu().Show();
    }

    public void toggleCharacter()
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

    public void toggleSpells()
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

    public void toggleTalents()
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

    public void clearState(int checkState)
    {
        if (state == checkState)
        {
            state = READY;
        }
    }

    public void setState(int newState)
    {
        state = newState;
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

    TestInventory getInventory()
    {
        return ((TestInventory)GameObject.Find("Window (Inventory)").GetComponent(typeof(TestInventory)));
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
    CameraHandler getCameraHandler()
    {
        return ((CameraHandler)GameObject.Find("GameLogicObject").GetComponent(typeof(CameraHandler)));
    }

}
