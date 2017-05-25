using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
    }



    void OnGUI()
    {
        //if (((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).isInGame)
        //{
        //    // Show in game UI
        //    if (GUI.Button(new Rect(600, 0, 100, 100), "Leave Game"))
        //{
        //((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).isInGame = false;
        //((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).endGame();
        // }
        //}
        //else
        //{
        // Show in lobby UI
        //if (GUI.Button(new Rect(100, 0, 100, 100), "Join Game"))
        //{
        //((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).isInGame = true;
        //((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).joinServer();
        //}
        //if (GUI.Button(new Rect(100, 100, 100, 100), "Create user"))
        //{
        //}
        //if (GUI.Button(new Rect(100, 200, 100, 100), "Create hero"))
        //{
        //}
        //}
    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
