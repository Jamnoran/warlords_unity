using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestGUI : MonoBehaviour {

    public GameObject menu;
    public Button exit;
    public Button restart;
    private bool isShowing = false;

    // Use this for initialization
    void Start () {
        menu.SetActive(false);
        restart.onClick.AddListener(() => { restartLevel(); });
        exit.onClick.AddListener(() => { exitToLobby(); });
    }
	
	// Update is called once per frame
	void Update () {


        // Handle auto attack, this sets a flag on the hero
        if (getGameLogic().isMyHeroAlive())
        {
            if (Input.GetKeyUp("a"))
            {
                bool autoAttacking = getGameLogic().getMyHero().getAutoAttacking();
                Debug.Log("Hero is now attacking : " + !autoAttacking);
                getGameLogic().getMyHero().setAutoAttacking(!autoAttacking);
            }else if (Input.GetKeyUp("s"))
            {
                getCommunication().sendStopHero(getGameLogic().getMyHero().id);
            }
        }
        if (Input.GetKeyDown("escape"))
        {
            isShowing = !isShowing;
        }
    }

    void OnGUI()
    {

        GUILayout.Label("Minions left: " + ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions().Count);

        Hero hero = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero();
        if (hero != null)
        {
            GUILayout.Label("Hero: " + hero.hp + "/" + hero.maxHp);

            GUILayout.Label("XP: " + hero.xp + " Level: " + hero.level);
        }

        Minion enemy = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHeroEnemyTarget();
        if(enemy != null)
        {
            GUILayout.Label("Enemy target: " + enemy.hp + "/" + enemy.maxHp);
        }

        Hero friendlyTarget = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHeroFriendlyTarget();
        if (friendlyTarget != null)
        {
            GUILayout.Label("Friendly: " + friendlyTarget.hp + "/" + friendlyTarget.maxHp + " Class " + friendlyTarget.class_type);
        }

        // When pressing p (or ability icon in UI) turn this bool to true else false (stopping this part to take much reasorces since it only checks a bool while running game instead of things against gamelogic)
        bool abilityTabOpen = true;
        if (abilityTabOpen)
        {
            if(getGameLogic().getAbilities() != null) { 
                foreach (var ability in getGameLogic().getAbilities())
                {
                    string coolDownText = "Ready";
                    var time = getMillis();
                    if (ability.timeWhenOffCooldown != null && !ability.timeWhenOffCooldown.Equals("") && (long.Parse(ability.timeWhenOffCooldown) >= time))
                    {
                        coolDownText = "" + (long.Parse(ability.timeWhenOffCooldown) - time);
                        
                    }
                    GUILayout.Label("Ability : " + ability.name + " CD: " + coolDownText);
                }
            }
        }

        bool allDead = true;
        foreach(Hero heroHp in getGameLogic().getHeroes())
        {
            if (heroHp.hp > 0)
            {
                allDead = false;
            }
        }
        if (getGameLogic().getHeroes() == null || getGameLogic().getHeroes().Count == 0)
        {
            // If we have yet recieved the heroes dont show menu
            allDead = false;
        }
        if (allDead)
        {
            menu.SetActive(true);
        }else if(!allDead && isShowing) {
            menu.SetActive(true);
        }else
        {
            menu.SetActive(false);
        }
       
    }

    private void restartLevel()
    {
        Debug.Log("Sending restart level");
        getCommunication().restartLevel();
    }

    private void exitToLobby()
    {
        Debug.Log("Exit to lobby screen");
        SceneManager.LoadScene("Lobby");
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
}
