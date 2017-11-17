using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetFrame : MonoBehaviour {

    public GameObject targetFrame;

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
                Hero heroTarget = getGameLogic().getHero(friendlyTarget);
                float hpPerc = (float)heroTarget.hp / (float)heroTarget.maxHp;
                updateInformation(targetFrame, hpPerc, 0.0f, true, heroTarget.class_type);
            }
            else
            {
                int targetEnemy = getGameLogic().getMyHero().targetEnemy;
                if (targetEnemy > 0)
                {
                    Minion minion = getGameLogic().getMinion(targetEnemy);
                    float hpPerc = (float)minion.hp / (float)minion.maxHp;
                    updateInformation(targetFrame, hpPerc, 0.0f, false, "" + minion.minionType);
                }
                else
                {
                    targetFrame.SetActive(false);
                }
            }
        }
    }

    private void updateInformation(GameObject frame, float hp, float resource, bool friendly, string classType)
    {
        frame.SetActive(true);
        frame.transform.Find("Red Border").gameObject.SetActive(!friendly);
        frame.transform.Find("HP Bar").GetComponent<UIProgressBar>().fillAmount = hp;
        setTextDescription(frame.transform.Find("HP Bar/Text").GetComponent<Text>(), hp);
    }


    private void setTextDescription(Text amountText, float amount)
    {
        if (amountText != null)
        {
            if (this.textVariant == TextVariant.Percent)
            {
                amountText.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
            }
            else if (this.textVariant == TextVariant.Value)
            {
                amountText.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat);
            }
            else if (this.textVariant == TextVariant.ValueMax)
            {
                amountText.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat) + "/" + this.m_TextValue;
            }
        }
    }


    GameLogic getGameLogic()
    {
        return ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic)));
    }
}
