using UnityEngine;
using System.Collections;



public class basePriest : baseCharacter {
   
    //create out basic priest class, derived from baseCharacter class.
    public basePriest(){
        CharacterClassName        = "Priest";
        CharacterClassDescription = "The proud priests of the lettOhl guild are very apt at restoring health and inspiring their allies.";
        Stamina                   = 45;
        Endurance                 = 20;
        Strength                  = 1;
        Intellect                 = 20;
        Agility                   = 3;
        Armor                     = 10;
    }
	
}
