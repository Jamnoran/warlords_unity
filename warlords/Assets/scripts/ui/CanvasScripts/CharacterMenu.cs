using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("c") && !getChat().IsInputFieldFocused())
        {
            //Debug.Log("Showing character");
            if (getUIWindow().IsVisible)
            {
                getUIWindow().Hide();
            }
            else
            {
                getUIWindow().Show();
            }
        }
		if (getUIWindow ().IsVisible) {
			updateInfo();
		}
    }

    void updateInfo()
    {
        Hero hero = getGameLogic().getMyHero();

        GameObject.Find("Character Title Text").GetComponent<Text>().text = hero.class_type;
        GameObject.Find("Stat (HP)/Value Text").GetComponent<Text>().text = "" + hero.maxHp;
        GameObject.Find("Stat (Level)/Value Text").GetComponent<Text>().text = "" + hero.level;
		GameObject.Find("Stat (Armor)/Value Text").GetComponent<Text>().text = "" + hero.armor;
		GameObject.Find("Stat (Energy)/Value Text").GetComponent<Text>().text = hero.resource + "/" + hero.maxResource;
        
    }





    UIWindow getUIWindow()
    {
        return ((UIWindow)transform.GetComponent(typeof(UIWindow)));
    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    Chat getChat()
    {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }

}
