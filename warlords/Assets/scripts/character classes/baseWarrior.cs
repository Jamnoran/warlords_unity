using UnityEngine;
using System.Collections;


public class baseWarrior : baseCharacter {

    //create out basic warrior class, derived from baseCharacter class.
    public baseWarrior()
    {
        CharacterClassName        = "Warrior";
        CharacterClassDescription = "Forged from the fires of sala'bakke, the warrior is pround and sturdy.";
        Stamina                   = 100;
        Endurance                 = 10;
        Strength                  = 25;
        Intellect                 = 2;
        Agility                   = 4;
        Armor                     = 40;
    }
	
}
