using UnityEngine;
using System.Collections;

public class TestGUI : MonoBehaviour {

    private baseCharacter class1 = new basePriest();
    private baseCharacter class2 = new baseWarrior();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnGUI()
    {
        //fetch our priest base stats and display on GUI
        GUILayout.Label(class1.CharacterClassName);
        GUILayout.Label(class1.CharacterClassDescription);
        GUILayout.Label("stamina: "   + class1.Stamina);
        GUILayout.Label("endurance: " + class1.Endurance);
        GUILayout.Label("strength: "  + class1.Strength);
        GUILayout.Label("intellect: " + class1.Intellect);
        GUILayout.Label("agility: "   + class1.Agility);
        GUILayout.Label("armor: "     + class1.Armor);

        //fetch our warrior base stats and display on GUI
        GUILayout.Label(class2.CharacterClassName);
        GUILayout.Label(class2.CharacterClassDescription);
        GUILayout.Label("stamina: "   + class2.Stamina);
        GUILayout.Label("endurance: " + class2.Endurance);
        GUILayout.Label("strength: "  + class2.Strength);
        GUILayout.Label("intellect: " + class2.Intellect);
        GUILayout.Label("agility: "   + class2.Agility);
        GUILayout.Label("armor: "     + class2.Armor);


    }
}
