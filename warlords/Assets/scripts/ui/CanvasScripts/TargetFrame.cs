using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetFrame : MonoBehaviour {

    public GameObject targetFrame;
    public UIProgressBar hpBar;
    public Text hp_Text;
    public TextVariant textVariant = TextVariant.Percent;
    public int m_TextValue = 100;
    public string m_TextValueFormat = "0";
    public GameObject enemyRedBorder;

    public enum TextVariant
    {
        Percent,
        Value,
        ValueMax
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (getGameLogic().getMyHero() != null && targetFrame != null)
        {
            // Friendly take priority
            int friendlyTarget = getGameLogic().getMyHero().targetFriendly;

            if (friendlyTarget > 0)
            {
                targetFrame.SetActive(true);
                Hero heroTarget = getGameLogic().getHero(friendlyTarget);
                float hpPerc = (float)heroTarget.hp / (float)heroTarget.maxHp;
                SetFillAmount(hpPerc);
                enemyRedBorder.SetActive(false);
            }
            else
            {
                int targetEnemy = getGameLogic().getMyHero().targetEnemy;
                if (targetEnemy > 0)
                {
                    targetFrame.SetActive(true);
                    Minion minion = getGameLogic().getMinion(targetEnemy);
                    float hpPerc = (float)minion.hp / (float)minion.maxHp;
                    SetFillAmount(hpPerc);
                    enemyRedBorder.SetActive(true);
                }
                else
                {
                    targetFrame.SetActive(false);
                }
            }
        }
    }


    protected void SetFillAmount(float amount)
    {
        if (this.hpBar == null)
            return;

        this.hpBar.fillAmount = amount;

        if (this.hp_Text != null)
        {
            if (this.textVariant == TextVariant.Percent)
            {
                this.hp_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
            }
            else if (this.textVariant == TextVariant.Value)
            {
                this.hp_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat);
            }
            else if (this.textVariant == TextVariant.ValueMax)
            {
                this.hp_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat) + "/" + this.m_TextValue;
            }
        }
    }



    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
