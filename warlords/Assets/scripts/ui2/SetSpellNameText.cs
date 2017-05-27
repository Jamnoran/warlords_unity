using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.scripts.vo;

public class SetSpellNameText : MonoBehaviour {

    private bool flag = true;
    private List<Ability> abilities;
    public int spellIdFromAbilityList;

	void Update () {
        if (abilities == null)
        {
            abilities = getGameLogic().getAbilities();
        }
        if (abilities != null && flag)
        {
            flag = false;
            for (int i = 0; i < 12; i++)
            {
                var foo = this.transform.name;
                if (spellIdFromAbilityList == i)
                {
                    var tmpTextObject = this.GetComponent<Text>();
                    tmpTextObject.text = abilities[i].name;

                }

            }

        }
    }

    GameLogic getGameLogic()
    {
        if (GameObject.Find("GameLogicObject") != null)
        {
            return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
        }
        else
        {
            return null;
        }
    }
}
