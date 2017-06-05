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

    public GameObject generalContent;
    public GameObject spell1Content;

    public int totalPoints = 10;
    public Text pointsLeft;

    private List<Talent> talents = new List<Talent>();
    private List<Ability> abilities = new List<Ability>();
    private int calculationOfPoints = 0;

    // Use this for initialization
    void Start () {
        //generalButton.onClick.AddListener(generalButtonClick);
        //spell1Button.onClick.AddListener(spell1ButtonClick);
        //spell2Button.onClick.AddListener(spell2ButtonClick);
        //saveButton.onClick.AddListener(save);
        //refresh();

    }

    // Update is called once per frame
    void Update () {
        calculatePoints();
    }


    public void refresh() {
        Debug.Log("Refreshing the talents");
        abilities = getGameLogic().getAbilities();
        talents = getGameLogic().getMyHero().talents;
        totalPoints = getGameLogic().getMyHero().getTotalTalentPoints();

        showTalentTree(0);
        showTalentTree(1);
    }

    void generateTempAbilities()
    {
        //Hero hero = getGameLogic().getMyHero();
        Hero hero = null;
        if (hero != null)
        {
            abilities = getGameLogic().getAbilities();
            talents = hero.talents;
            totalPoints = hero.getTotalTalentPoints();
        }
        totalPoints = 10;
        talents = new List<Talent>();
        Talent talent = new Talent();
        talent.setId(2);
        talent.setTalentId(2);
        talent.setDescription("Increase hp");
        talent.setSpellId(0);
        talents.Add(talent);
        Talent talent2 = new Talent();
        talent2.setId(1);
        talent2.setTalentId(1);
        talent2.setDescription("Reduce cooldown");
        talent2.setSpellId(1);
        talents.Add(talent2);

        abilities = new List<Ability>();
        Ability general = new Ability();
        general.id = 0;
        general.image = null;
        abilities.Add(general);
        Ability ability = new Ability();
        ability.id = 1;
        ability.image = "cleave";
        abilities.Add(ability);

        showTalentTree(0);
        showTalentTree(1);

    }


    void showTalentTree(int spellId) {
        foreach (var talent in talents) {
            if (talent.spellId == spellId) {
                Destroy(talent.getGameObject());
            }
        }
        foreach (var talent in talents) {
            if (talent.spellId == spellId) {
                addTalent(talent);
            }
        }
    }

    void addTalent(Talent talent) {
        Debug.Log("Adding talent : " + talent.description + " id : " + talent.talentId + " for spell " + talent.spellId);
        var talentHolder = Instantiate(talentsUiPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        
        if (talent.spellId == 0) {
            talentHolder.transform.SetParent(generalContent.transform, false);
        } else if (talent.spellId == 1) {
            talentHolder.transform.SetParent(spell1Content.transform, false);
        }
        
        updateTalent(talent);

        foreach (var textHolder in talentHolder.GetComponentsInChildren<Text>()) {
            if (textHolder.name == "Spell Name Text") {
                textHolder.text = talent.description;
            } else if (textHolder.name == "Points") {
                textHolder.text = "" + talent.pointAdded;
            }
        }

        foreach (var buttonHolder in talentHolder.GetComponentsInChildren<Image>()) {
            if (buttonHolder.name.Equals("Talent Slot")) {
                UITalentSlot script = ((UITalentSlot)buttonHolder.GetComponent(typeof(UITalentSlot)));
                UISpellInfo spInfo = new UISpellInfo();
                spInfo.ID = talent.spellId;
                spInfo.Icon = Resources.Load<Sprite>("sprites/items/taunt");
                UITalentInfo taInfo = new UITalentInfo();
                taInfo.ID = talent.talentId;
                taInfo.maxPoints = 10;
                Debug.Log("Assigned : " + script.Assign(taInfo, spInfo));
            }
        }

        talent.setGameObject(talentHolder);
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
    
    public bool calculatePoints() {
        calculationOfPoints = 0;
        foreach (var talent in talents) {
            if (talent.getGameObject() != null) {
                foreach (var buttonHolder in talent.getGameObject().GetComponentsInChildren<Image>()) {
                    if (buttonHolder.name.Equals("Talent Slot")) {
                        UITalentSlot script = ((UITalentSlot)buttonHolder.GetComponent(typeof(UITalentSlot)));
                        //Debug.Log("We found UiTalentSlot : " + script.getCurrentPoints() + " For talent " + talent.talentId);
                        talent.setPointAdded(script.getCurrentPoints());
                        calculationOfPoints = calculationOfPoints + talent.pointAdded;
                    }
                }
            }
        }
        if ((totalPoints - calculationOfPoints) >= 0) {
            pointsLeft.text = "Points left: " + (totalPoints - calculationOfPoints);
            return true;
        } else {
            return false;
        }
    }
    

    public void saveTalents() {
        Debug.Log("Saving talents");

        foreach (var talent in talents) {
            if (talent.getGameObject() != null)
            {
                foreach (var buttonHolder in talent.getGameObject().GetComponentsInChildren<Image>()) {
                    if (buttonHolder.name.Equals("Talent Slot")) {
                        UITalentSlot script = ((UITalentSlot)buttonHolder.GetComponent(typeof(UITalentSlot)));
                        Debug.Log("We found UiTalentSlot : " + script.getCurrentPoints() + " For talent " + talent.talentId);
                        talent.setPointAdded(script.getCurrentPoints());
                    }
                }
            }
            else
            {
                Debug.Log("Talent had no gameObject, why? " + talent.description);
            }
        }

        getCommunication().updateTalents(talents);

        gameObject.SetActive(false);
    }

    public void dismissWindow() {
        gameObject.SetActive(false);
    }




    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
