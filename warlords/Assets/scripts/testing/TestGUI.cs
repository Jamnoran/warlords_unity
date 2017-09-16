using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestGUI : MonoBehaviour {

    public GameObject menu;
    public GameObject talents;
    public GameObject chat;
    public Button exit;
    public Button restart;
    private bool isShowing = false;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }

    bool isChatVisible()
    {
        // return !getChat().inputVisible;
        return false;
    }

    void OnGUI() {
        //GUILayout.Label("Minions left: " + ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions().Count);

        ////Hero hero = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero();

        //Minion enemy = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHeroEnemyTarget();
        //if(enemy != null) {
        //    GUILayout.Label("Enemy target: " + enemy.hp + "/" + enemy.maxHp);
        //}
        //else
        //{
        //  GUILayout.Label("No target selected");
        //}

        //Hero friendlyTarget = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHeroFriendlyTarget();
        //if (friendlyTarget != null) {
        //    GUILayout.Label("Friendly: " + friendlyTarget.hp + "/" + friendlyTarget.maxHp + " Class " + friendlyTarget.class_type);
        //}

        //if (getGameLogic().getMyHero() != null) {
        //    GUILayout.Label("Auto attacking : " + getGameLogic().getMyHero().getAutoAttacking());
        //}

        //bool allDead = true;
        //foreach(Hero heroHp in getGameLogic().getHeroes()) {
        //    if (heroHp.hp > 0) {
        //        allDead = false;
        //    }
        //}
        //if (getGameLogic().getHeroes() == null || getGameLogic().getHeroes().Count == 0) {
        //    // If we have yet recieved the heroes dont show menu
        //    allDead = false;
        //}
        //if (allDead) {
        //    menu.SetActive(true);
        //}else if(!allDead && isShowing) {
        //    menu.SetActive(true);
        //}else {
        //    menu.SetActive(false);
        //}
       
    }

    private void restartLevel() {
        Debug.Log("Sending restart level");
        getCommunication().restartLevel();
    }

    private void exitToLobby() {
        getCommunication().endGame();
        getCommunication().closeCommunication();

        Debug.Log("Exit to lobby screen");
        SceneManager.LoadScene("Lobby");
        getLobbyCommunication().getHeroes();
    }


    private long getMillis() {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
    }

    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    Chat getChat() {
        if (chat != null && chat.GetComponent(typeof(Chat)) != null) {
            return ((Chat)chat.GetComponent(typeof(Chat)));
        }
        return null;
    }

    ServerCommunication getCommunication() {
        if (GameObject.Find("Communication") != null) {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        } else {
            return null;
        }
    }

    LobbyCommunication getLobbyCommunication() {
        return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
    }
}
