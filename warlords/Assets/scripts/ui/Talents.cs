using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Talents : MonoBehaviour {

    public Button generalButton;
    public Button spell1Button;
    public Button spell2Button;
    public Button saveButton;
    public GameObject talentsHolder;
    public GameObject talentsUiPrefab;
    private List<Talent> talents = new List<Talent>();
    private List<Ability> abilities = new List<Ability>();
    public int totalPoints = 10;
    private int calculationOfPoints = 0;
    public Text pointsLeft;

    // Use this for initialization
    void Start () {
        generalButton.onClick.AddListener(generalButtonClick);
        spell1Button.onClick.AddListener(spell1ButtonClick);
        spell2Button.onClick.AddListener(spell2ButtonClick);
        saveButton.onClick.AddListener(save);
        
    }

    // Update is called once per frame
    void Update () {
    }


    public void refresh() {
        Hero hero = getGameLogic().getMyHero();
        if (hero != null) {
            abilities = getGameLogic().getAbilities();
            talents = hero.talents;
            totalPoints = hero.getTotalTalentPoints();
        }

        setUpMenu();

        showTalentTree(0);

        calculatePoints();
    }

    private void setUpMenu() {
        int i = 0;
        foreach (var ability in abilities) {
            Sprite abilitySprite = Resources.Load<Sprite>("sprites/items/" + ability.image);
            if (i == 0) {

            }else if (i == 1) {
                spell1Button.GetComponent<Image>().sprite = abilitySprite;
            } else if (i == 2) {
                spell2Button.GetComponent<Image>().sprite = abilitySprite;
            }
            i++;
        }
    }
    
    void addTalent(Talent talent) {
        var talentHolder = Instantiate(talentsUiPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        talent.setGameObject(talentHolder);
        talentHolder.transform.SetParent(talentsHolder.transform, false);
        updateTalent(talent);
        foreach (var textHolder in talentHolder.GetComponentsInChildren<Text>()) {
            if (textHolder.name == "Description") {
                textHolder.text = talent.description;
            } else if (textHolder.name == "Points") {
                textHolder.text = "" + talent.pointAdded;
            }
        }
        foreach (var buttonHolder in talentHolder.GetComponentsInChildren<Button>()) {
            buttonHolder.onClick.AddListener(delegate { addPoint(talent.id); });
        }
    }

    void updateTalent(Talent talent) {
        var talentHolder = talent.getGameObject();
        if(talentHolder != null) {
            foreach (var textHolder in talentHolder.GetComponentsInChildren<Text>()) {
                if (textHolder.name == "Description") {
                    textHolder.text = talent.description;
                } else if (textHolder.name == "Points") {
                    textHolder.text = "" + talent.pointAdded;
                }
            }
        }
    }

    void addPoint(int talentId) {
        foreach (var talent in talents) {
            if (talent.id.Equals(talentId)) {
                talent.pointAdded = talent.pointAdded + 1;
                updateTalent(talent);
            }
        }
        calculatePoints();
    }

    private void calculatePoints() {
        calculationOfPoints = 0;
        foreach (var talent in talents) {
            calculationOfPoints = calculationOfPoints + talent.pointAdded;
        }
        pointsLeft.text = "Points left: " + (totalPoints - calculationOfPoints);
    }

    void showTalentTree(int spellId) {
        foreach (var talent in talents) {
            Destroy(talent.getGameObject());
        }
        foreach (var talent in talents) {
            if (talent.spellId == spellId) {
                addTalent(talent);
            }
        }
    }

    void save() {
        foreach (var talent in talents) {
            Debug.Log("Saving this talent : " + talent.description + " points : " + talent.pointAdded);
        }
        gameObject.SetActive(false);
    }

    void generalButtonClick() {
        showTalentTree(0);
    }

    void spell1ButtonClick() {
        showTalentTree(abilities[1].id);
    }

    void spell2ButtonClick() {
        showTalentTree(abilities[2].id);
    }






    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
