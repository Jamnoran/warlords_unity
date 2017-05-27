using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyLogic : MonoBehaviour {

    private Hero currentHero = null;
    private int characterSelector = 0;

    private List<Hero> heroes = new List<Hero>();

    public Text characterClass;
    public Text characterLevel;
    public Button customGame;
    public Button quickGame;
    public Button leftArrow;
    public Button rightArrow;

    public GameObject player1;


    // Use this for initialization
    void Start () {
        //quickGame.onClick.AddListener(() => { startQuickGame(); });
        //customGame.onClick.AddListener(() => { startCustomGame(); });
        if (getLobbyCommunication() == null || getLobbyCommunication().userId == 0) {
            SceneManager.LoadScene("Connect");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}



    public void updateHeroes(List<Hero> newListOfHeroes) {
        heroes = newListOfHeroes;
        if (currentHero == null && heroes.Count > 0) {
            currentHero = heroes[0];
            getLobbyCommunication().heroId = currentHero.id;
            Debug.Log("Setting hero id : " + currentHero.id);
        }
        updateCurrentHeroInformation();
    }

    void updateCurrentHeroInformation() {
        if (currentHero != null && currentHero.id > 0) {
            characterClass.text = currentHero.class_type;
            characterLevel.text = "Level " + currentHero.level;
        }
        updatePlayer(0, currentHero);
    }


    void updatePlayer(int position, Hero hero) {
        if (position == 0) {
            Text[] texts = player1.GetComponentsInChildren<Text>();
            foreach (Text text in texts) {
                text.text = hero.class_type;
            }
        }
    }

    public void createHero() {
        Debug.Log("Creating a hero");
        SceneManager.LoadScene("CreateHero");
    }


    public void choosePreviousHero() {
        Debug.Log("Previous hero");
        characterSelector--;
        if (characterSelector < 0) {
            characterSelector = heroes.Count -1;
        }
        currentHero = heroes[characterSelector];
        updateCurrentHeroInformation();
    }

    public void chooseNextHero() {
        Debug.Log("Next hero");
        characterSelector++;
        if (characterSelector >= heroes.Count) {
            characterSelector = 0;
        }
        currentHero = heroes[characterSelector];
        updateCurrentHeroInformation();
    }


    public void startCustomGame() {
        Debug.Log("Starting a custom game");
        getLobbyCommunication().findCustomGame();
    }

    public void startQuickGame() {
        Debug.Log("Starting a quick game");
        getLobbyCommunication().findQuickGame();
    }


    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    LobbyCommunication getLobbyCommunication() {
        if(GameObject.Find("Communication") != null) {
            return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
        } else {
            return null;
        }
    }

}
