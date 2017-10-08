using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.scripts.vo;

[System.Serializable]
public class ResponseCombatText
{
    public string response_type = "HERO_BUFF";
    public bool friendly = true;
    public int idOfTarget = 0;
    public string amount;
    public bool crit = false;
    public string color;


    public ResponseCombatText() { }

    public string toString()
    {
        return "CombatTextResponse{" +
                "response_type='" + response_type + '\'' +
                ", friendly=" + friendly +
                ", idOfTarget=" + idOfTarget +
                ", amount='" + amount + '\'' +
                ", crit=" + crit +
                ", color='" + color + '\'' +
                '}';
    }
}