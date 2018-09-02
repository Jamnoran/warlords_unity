using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;
using UnityEngine.UI;
public class SpellShadow : MonoBehaviour
{


    private List<Ability> abilities;
    private bool flag = true;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
                if (this.transform.name == "original" + i.ToString())
                {
                    if ((abilities.Count > i - 1) && abilities[i] != null)
                    {
                        Sprite abilitySprite = Resources.Load<Sprite>("Spells/" + abilities[i].image);
                        this.GetComponent<Image>().sprite = abilitySprite;
                        var tmp = this.GetComponent<Image>().color;
                        tmp.a = 0.5f;
                        this.GetComponent<Image>().color = tmp;
                    }

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
