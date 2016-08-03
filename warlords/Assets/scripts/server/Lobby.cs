using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.scripts.vo;
using System.Collections.Generic;

public class Lobby : MonoBehaviour {
    // Create games buttons
    public Button changeHero = null;
    public Button quickGame = null;
    // Your heroes
    public Button topLeftButton = null;
    public Text topLeftButtonText = null;
    public Button topRightButton = null;
    public Text topRightButtonText = null;
    public Button botLeftButton = null;
    public Text botLeftButtonText = null;
    public Button botRightButton = null;
    public Text botRightButtonText = null;
    // Create heroes buttons
    public Button createWarrior = null;
    public Button createPriest = null;
    public Button createWarlock = null;
    public Button createRougue = null;

    public Text heroChooserText = null;

    private Hero currentHero = null;

    private List<Hero> heroes = new List<Hero>();

    // Canvaces
    public GameObject mainChat;
    public GameObject heroesLayout;
    public GameObject createHeroLayout;

    // Use this for initialization
    void Start ()
    {
        mainChat.SetActive(true);
        heroesLayout.SetActive(false);
        createHeroLayout.SetActive(false);
        changeHero.onClick.AddListener(() => { showheroDialog(); });
        quickGame.onClick.AddListener(() => { startQuickGame(); });
        topLeftButton.onClick.AddListener(() => { heroButtonPressed(0); });
        topRightButton.onClick.AddListener(() => { heroButtonPressed(1); });
        botLeftButton.onClick.AddListener(() => { heroButtonPressed(2); });
        botRightButton.onClick.AddListener(() => { heroButtonPressed(3); });
        createWarrior.onClick.AddListener(() => { createHeroButtonPressed(0); });
        createPriest.onClick.AddListener(() => { createHeroButtonPressed(1); });
        createWarlock.onClick.AddListener(() => { createHeroButtonPressed(2); });
        createRougue.onClick.AddListener(() => { createHeroButtonPressed(3); });

        Hero warrior = new Hero();
        warrior.class_type = "WARRIOR";
        warrior.level = 3;
        heroes.Add(warrior);
        currentHero = warrior;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentHero != null)
        {
            heroChooserText.text = currentHero.class_type + " \n Level " + currentHero.level;
        }
        int position = 0;
        foreach (var hero in heroes)
        {
            if (position == 0)
            {
                topLeftButtonText.text = hero.class_type + " \n Level " + hero.level;
            }
            else if (position == 1)
            {
                topRightButtonText.text = hero.class_type + " \n Level " + hero.level;
            }
            else if (position == 2)
            {
                botLeftButtonText.text = hero.class_type + " \n Level " + hero.level;
            }
            else if (position == 3)
            {
                botRightButtonText.text = hero.class_type + " \n Level " + hero.level;
            }
            position = position + 1;
        }
    }

    void showheroDialog() {
        Debug.Log("Showing hero dialog");
        heroesLayout.SetActive(true);
        mainChat.SetActive(false);
    }

    void startQuickGame()
    {
        Debug.Log("Starting a quick game");
    }
    void heroButtonPressed(int button)
    {
        Debug.Log("Button pressed: " + button);
        int position = 0;
        bool foundHero = false;
        foreach (var hero in heroes)
        {
            if (position == button)
            {
                currentHero = hero;
                foundHero = true;
            }
            position = position + 1;
        }
        heroesLayout.SetActive(false);
        if (foundHero)
        {
            mainChat.SetActive(true);
        }
        else
        {
            // Show create hero screen
            createHeroLayout.SetActive(true);
        }
    }

    void createHeroButtonPressed(int button)
    {
        Debug.Log("Creating hero pressed: " + button);
        Hero hero = new Hero();
        if (button == 0)
        {
            hero.class_type = "WARRIOR";
        }
        else if(button == 1)
        {
            hero.class_type = "PRIEST";
        }
        else if (button == 2)
        {
            hero.class_type = "PRIEST";
        }
        else if (button == 3)
        {
            hero.class_type = "PRIEST";
        }
        hero.level = 1;
        heroes.Add(hero);
        //currentHero = hero;
        heroesLayout.SetActive(true);
        createHeroLayout.SetActive(false);
    }
}
