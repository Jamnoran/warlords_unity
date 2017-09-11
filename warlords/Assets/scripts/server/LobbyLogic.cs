using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LobbyLogic : MonoBehaviour {

    private Hero currentHero = null;
    private int characterSelector = 0;

    private List<Hero> heroes = new List<Hero>();

    public Text characterClass;
    public Text characterLevel;
    public Button customGame;
    public Button quickGame;
    public Button startGame;
    public Button cancelGame;
    public Button createHeroButton;
    public Button deleteHeroButton;

    public Button leftArrow;
    public Button rightArrow;

    public GameObject player1;
    public GameObject player2;
    public GameObject player3;
    public GameObject player4;
    public GameObject playerSolo;
    public GameObject loadingText;
    public LFG group;

    // Use this for initialization
    void Start () {
        if (getLobbyCommunication() == null || getLobbyCommunication().userId == 0) {
            SceneManager.LoadScene("Connect");
        }
        showGroup(false);
    }
	
	// Update is called once per frame
	void Update () {
	}


    void showGroup(bool show) {
        player1.SetActive(show);
        player2.SetActive(show);
        player3.SetActive(show);
        player4.SetActive(show);
        playerSolo.SetActive(!show);
        startGame.gameObject.SetActive(show);
        cancelGame.gameObject.SetActive(show);
        customGame.gameObject.SetActive(!show);
        quickGame.gameObject.SetActive(!show);
    }

    public void setGroup(LFG lfg) {
        group = lfg;
        showGroup(true);
        if (group.getHeroClass1() != null) {
            updatePlayer(1, new Hero(group.getHeroClass1()));
        }
        if (group.getHeroClass2() != null) {
            updatePlayer(2, new Hero(group.getHeroClass2()));
        }
        if (group.getHeroClass3() != null) {
            updatePlayer(3, new Hero(group.getHeroClass3()));
        }
        if (group.getHeroClass4() != null) {
            updatePlayer(4, new Hero(group.getHeroClass4()));
        }
    }

    public void updateHeroes(List<Hero> newListOfHeroes) {
        heroes = newListOfHeroes;
        if (currentHero == null && heroes.Count > 0)
        {
            // Check what the last hero was that the user played
            int lastHeroIdPlayed = PlayerPrefs.GetInt("HERO_ID_LAST_USED");
            Debug.Log("Last hero id played : " + lastHeroIdPlayed);
            if (lastHeroIdPlayed > 0)
            {
                foreach (Hero hero in heroes)
                {
                    if (hero.id == lastHeroIdPlayed)
                    {
                        currentHero = hero;
                        Debug.Log("Found lasta hero played");
                    }
                }
            }
            else
            {
                currentHero = heroes[0];
            }
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
            updateText(playerSolo, hero);
            updateImage(playerSolo, hero);
        }
        if (position == 1) {
            updateText(player1, hero);
            updateImage(player1, hero);
        }
        if (position == 2) {
            updateText(player2, hero);
            updateImage(player2, hero);
        }
        if (position == 3) {
            updateText(player3, hero);
            updateImage(player3, hero);
        }
        if (position == 4) {
            updateText(player4, hero);
            updateImage(player4, hero);
        }
    }

    void updateText(GameObject holder, Hero hero) {
        Text[] texts = holder.GetComponentsInChildren<Text>();
        foreach (Text text in texts) {
            if (hero != null) {
                text.text = hero.class_type;
            } else {
                text.text = "Waiting for player";
            }
        }
    }

    void updateImage(GameObject holder, Hero hero) {
        if (hero != null) {
            Debug.Log("Updating image with hero : " + hero.class_type);
        }
        Image[] images = holder.GetComponentsInChildren<Image>();
        foreach (Image image in images) {
            bool hasHero = false;
            if (hero != null) {
                hasHero = true;
            }
            if (image.tag.Equals("CharacterHolder")) {
                image.enabled = !hasHero;
            } else if (image.tag.Equals("CharacterImage")) {
                image.enabled = hasHero;
                if (hasHero) {
                    if (hero.class_type.Equals("WARRIOR")) {
                        image.sprite = Resources.Load<Sprite>("WarriorFrame");
                        Debug.Log("Setting image to warrior");
                    }else if (hero.class_type.Equals("PRIEST")) {
                        image.sprite = Resources.Load<Sprite>("PriestFrame");
                        Debug.Log("Setting image to priest");
                    }
                    else if (hero.class_type.Equals("Warlock"))
                    {
                        image.sprite = Resources.Load<Sprite>("WarlockFrame");
                        Debug.Log("Setting image to warlock");
                    }
                }
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
        getLobbyCommunication().heroId = currentHero.id;
    }

    public void chooseNextHero() {
        Debug.Log("Next hero");
        characterSelector++;
        if (characterSelector >= heroes.Count) {
            characterSelector = 0;
        }
        currentHero = heroes[characterSelector];
        updateCurrentHeroInformation();
        getLobbyCommunication().heroId = currentHero.id;
    }


    public void startCustomGame() {
        Debug.Log("Starting a custom game");
        getLobbyCommunication().findCustomGame();
        PlayerPrefs.SetInt("HERO_ID_LAST_USED", currentHero.id);
    }

    public void startQuickGame() {
        Debug.Log("Starting a quick game");
        getLobbyCommunication().findQuickGame();
        PlayerPrefs.SetInt("HERO_ID_LAST_USED", currentHero.id);
    }

    public void sendStartGame() {
        Debug.Log("sendStartGame");
        getLobbyCommunication().sendStartGame(group);
    }

    public void sendCancelGame() {
        Debug.Log("sendCancelGame");
        getLobbyCommunication().sendCancelGameSearch(group);
        group = null;
        showGroup(false);
    }

    public void showLoadingText() {
        loadingText.SetActive(true);
        player1.SetActive(false);
        player2.SetActive(false);
        player3.SetActive(false);
        player4.SetActive(false);
        playerSolo.SetActive(false);
        startGame.gameObject.SetActive(false);
        cancelGame.gameObject.SetActive(false);
        customGame.gameObject.SetActive(false);
        quickGame.gameObject.SetActive(false);
        leftArrow.gameObject.SetActive(false);
        rightArrow.gameObject.SetActive(false);
        characterClass.gameObject.SetActive(false);
        characterLevel.gameObject.SetActive(false);
        createHeroButton.gameObject.SetActive(false);
        deleteHeroButton.gameObject.SetActive(false);

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
