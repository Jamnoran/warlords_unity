using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class Talents : MonoBehaviour {
    public GameObject talentsUiPrefab;
    public GameObject generalContent;
    public int totalPoints = 0;
    public Text pointsLeft;
    public Text title;

    private List<Talent> talents = new List<Talent>();
    private List<Ability> abilities = new List<Ability>();
    private int calculationOfPoints = 0;

    // Use this for initialization
    void Start () {
    }


    // Update is called once per frame
    void Update()
    {
        if (getUIWindow().IsVisible)
        {
            calculatePoints();
        }
    }

	public void toggleTalents(){
		if (getUIWindow ().IsVisible) {
			getUIWindow ().Hide ();
		} else {
			getUIWindow().Show();
			refresh();
		}
	}

    public bool IsVisible()
    {
        return getUIWindow().IsVisible;
    }
    public void Show(){
		getUIWindow ().Show ();
        refresh();
    }

	public void Hide(){
		getUIWindow ().Hide ();
	}


    public void talentTreePressed(int position)
    {
        Debug.Log("Talent tree pressed from : " + position);
        showTalentTree(position);
    }

    public void refresh() {
        Debug.Log("Refreshing the talents");
        abilities = getGameLogic().getAbilities();
        talents = getGameLogic().getMyHero().talents;
        totalPoints = getGameLogic().getMyHero().getTotalTalentPoints();

        showTalentTree(0);
        int i = 0;
        foreach (Ability ability in abilities)
        {
            // Set all icons in menu bar
            //Debug.Log("Trying to find talent icon on position : " + ability.position + " With ability id: " + ability.id + " i : " + i + " ability name: " + ability.name);
            GameObject spellicon = GameObject.Find("TalentMenuIcon (" + i  + ")");
            UISpellSlot script = ((UISpellSlot)spellicon.GetComponent(typeof(UISpellSlot)));
            UISpellInfo spInfo = new UISpellInfo();
            spInfo.ID = ability.id;
            if (i == 0)
            {
                spInfo.Icon = Resources.Load<Sprite>("sprites/items/general");
            }
            else
            {
                spInfo.Icon = Resources.Load<Sprite>("Spells/" + ability.image);
            }
            script.Assign(spInfo);
            i++;
        }
    }

    void showTalentTree(int position) {
        // Change title
        Ability spell = null;
        for(int o = 0; o < abilities.Count ; o++)
        {
            if (o == position)
            {
                spell = abilities[o];
            }
        }
        if (position == 0)
        {
            title.text = "General talents";
        }
        else
        {
            title.text = spell.name + " talents";
        }
        

        foreach (var talent in talents) {
            Destroy(talent.getGameObject());
        }
        foreach (var talent in talents) {
            if (talent.spellId == spell.id) {
                addTalent(talent);
            }
        }
    }

    void addTalent(Talent talent) {
        Debug.Log("Adding talent : " + talent.description + " id : " + talent.talentId + " for spell " + talent.spellId + " Points added : " + talent.getPointAdded());
        var talentHolder = Instantiate(talentsUiPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);

        talentHolder.transform.SetParent(generalContent.transform, false);
      
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
                spInfo.Icon = Resources.Load<Sprite>("Spells/taunt");
                UITalentInfo taInfo = new UITalentInfo();
                taInfo.ID = talent.talentId;
                taInfo.maxPoints = talent.getMaxPoints();

                script.setCurrentPoints(talent.getPointAdded());

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
                    //textHolder.text = "" + talent.pointAdded;
                }
            }
        }
    }
    
    public bool calculatePoints() {
        int tempCalc = 0;
        foreach (var talent in talents) {
            if (talent.getGameObject() != null) {
                foreach (var buttonHolder in talent.getGameObject().GetComponentsInChildren<Image>()) {
                    if (buttonHolder.name.Equals("Talent Slot")) {
                        UITalentSlot script = ((UITalentSlot)buttonHolder.GetComponent(typeof(UITalentSlot)));
                        talent.setPointAdded(script.getCurrentPoints());
                    }
                }
            }
        }

        foreach (var talent in talents)
        {
            tempCalc = tempCalc + talent.pointAdded;
        }
        calculationOfPoints = tempCalc;
        if ((totalPoints - calculationOfPoints) >= 0) {
            pointsLeft.text = "Points left: " + (totalPoints - calculationOfPoints);
            return true;
        } else {
            //Debug.Log("Not enough points to add more");
            return false;
        }
    }

    public bool hasPointsLeft()
    {
        return (totalPoints - calculationOfPoints) > 0;
    }
    

    public void saveTalents() {
        Debug.Log("Saving talents");
        List<Talent> talentsToSend = new List<Talent>();
        foreach (var talent in talents) {
            if (talent.getGameObject() != null)
            {
                foreach (var buttonHolder in talent.getGameObject().GetComponentsInChildren<Image>()) {
                    if (buttonHolder.name.Equals("Talent Slot")) {
                        UITalentSlot script = ((UITalentSlot)buttonHolder.GetComponent(typeof(UITalentSlot)));
                        Debug.Log("We found UiTalentSlot : " + script.getCurrentPoints() + " For talent " + talent.talentId);
                        talent.setPointAdded(script.getCurrentPoints());
                        if (talent.getPointAdded() > 0)
                        {
                            talentsToSend.Add(talent);
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Talent had no gameObject, why? " + talent.description);
            }
        }

		Hero myHero = getGameLogic().getMyHero();
		getCommunication().updateTalents(myHero.id, talentsToSend);

        dismissWindow();
    }

    public void dismissWindow()
    {
        getUIWindow().Hide();
    }
    

    UIWindow getUIWindow()
    {
        return ((UIWindow)transform.GetComponent(typeof(UIWindow)));
    }

    ServerCommunication getCommunication()
    {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    GameLogic getGameLogic() {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }

    Chat getChat()
    {
        return ((Chat)GameObject.Find("GameLogicObject").GetComponent(typeof(Chat)));
    }
}
