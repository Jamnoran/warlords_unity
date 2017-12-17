using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameMenu : MonoBehaviour {

    public GameObject menu;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("escape") && !getChat().IsInputFieldFocused())
        {
            Debug.Log("Showing menu");
            if (getUIWindow().IsVisible)
            {
                getUIWindow().Hide();
            }
            else
            {
                getUIWindow().Show();
            }
        }
    }

    public void restartLevel()
    {
        Debug.Log("Sending restart level");
        getCommunication().restartLevel();
    }

    public void exitGame()
    {
        getCommunication().endGame();
        getCommunication().closeCommunication();

        Debug.Log("Exit to lobby screen");
        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single);
        getLobbyCommunication().getHeroes();
    }

    public void showSettings()
    {

    }


    UIWindow getUIWindow()
    {
        if (menu != null)
        {
            return ((UIWindow)menu.GetComponent(typeof(UIWindow)));
        }
        else
        {
            return null;
        }
    }



    private long getMillis()
    {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    ServerCommunication getCommunication()
    {
        if (GameObject.Find("Communication") != null)
        {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        }
        else
        {
            return null;
        }
    }

    LobbyCommunication getLobbyCommunication()
    {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
    }

    Chat getChat()
    {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }
}
