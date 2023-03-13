using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CastBar : MonoBehaviour
{
    public UICastBar m_CastBar;

    public void start()
    {

    }

    public void update()
    {

    }

    public void showSpell(Ability ability)
    {
        UISpellInfo spellInfo = new UISpellInfo();
        // Calculate from millis to float
        float converted = ((float) ability.calculatedCastTime / 1000);
        Debug.Log("Calculated time : " + converted);
        spellInfo.CastTime = converted;
        spellInfo.Name = ability.name;
        spellInfo.Icon = Resources.Load<Sprite>("Spells/" + ability.image);

        this.m_CastBar.StartCasting(spellInfo, spellInfo.CastTime, (Time.time + spellInfo.CastTime));
    }
}
