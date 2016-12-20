using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System;

public class TestGUI : MonoBehaviour {

    //private baseCharacter class1 = new basePriest();
    //private baseCharacter class2 = new baseWarrior();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        //fetch our priest base stats and display on GUI
        //GUILayout.Label(class1.CharacterClassName);
        //GUILayout.Label(class1.CharacterClassDescription);
        //GUILayout.Label("stamina: "   + class1.Stamina);
        //GUILayout.Label("endurance: " + class1.Endurance);
        //GUILayout.Label("strength: "  + class1.Strength);
        //GUILayout.Label("intellect: " + class1.Intellect);
        //GUILayout.Label("agility: "   + class1.Agility);
        //GUILayout.Label("armor: "     + class1.Armor);

        //fetch our warrior base stats and display on GUI
        //GUILayout.Label(class2.CharacterClassName);
        //GUILayout.Label(class2.CharacterClassDescription);
        //GUILayout.Label("stamina: "   + class2.Stamina);
        //GUILayout.Label("endurance: " + class2.Endurance);
        //GUILayout.Label("strength: "  + class2.Strength);
        //GUILayout.Label("intellect: " + class2.Intellect);
        //GUILayout.Label("agility: "   + class2.Agility);
        //GUILayout.Label("armor: "     + class2.Armor);
        

        GUILayout.Label("Minions left: " + ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMinions().Count);

        Hero hero = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero();
        if (hero != null)
        {
            GUILayout.Label("Hero: " + hero.hp + "/" + hero.maxHp);
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
}
