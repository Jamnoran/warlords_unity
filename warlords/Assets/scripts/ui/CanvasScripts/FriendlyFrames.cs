using Assets.scripts.vo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendlyFrames : MonoBehaviour
{

    public GameObject friendlyFrame1;
    public GameObject friendlyFrame2;
    public GameObject friendlyFrame3;
    public UIProgressBar hpBar1;
    public UIProgressBar hpBar2;
    public UIProgressBar hpBar3;
    public Text hp_Text1;
    public Text hp_Text2;
    public Text hp_Text3;
    public TextVariant textVariant = TextVariant.Percent;
    public int m_TextValue = 100;
    public string m_TextValueFormat = "0";
    public GameObject enemyRedBorder;

    private float hp1;
    private float hp2;
    private float hp3;

    public enum TextVariant
    {
        Percent,
        Value,
        ValueMax
    }

   public void UpdatePartyFrames(List<Hero> heroes)
    {
        var allies = heroes.Count;
        switch (allies)
        {
            case 1:
                friendlyFrame2.SetActive(false);
                friendlyFrame3.SetActive(false);
                hp1 = (float)heroes[0].hp / (float)heroes[0].maxHp;
                SetFillAmount(hpBar1, hp_Text1, hp1);
                break;
            case 2:
                friendlyFrame3.SetActive(false);
                hp1 = (float)heroes[0].hp / (float)heroes[0].maxHp;
                hp2 = (float)heroes[1].hp / (float)heroes[1].maxHp;
                SetFillAmount(hpBar1, hp_Text1, hp1);
                SetFillAmount(hpBar2, hp_Text2, hp2);
                break;
            case 3:
                hp1 = (float)heroes[0].hp / (float)heroes[0].maxHp;
                hp2 = (float)heroes[1].hp / (float)heroes[1].maxHp;
                hp2 = (float)heroes[2].hp / (float)heroes[2].maxHp;
                SetFillAmount(hpBar1, hp_Text1, hp1);
                SetFillAmount(hpBar2, hp_Text2, hp2);
                SetFillAmount(hpBar3, hp_Text3, hp3);
            break;
            default:
                break;
        }
      
        
        
      
    }


    protected void SetFillAmount(UIProgressBar hpBar, Text hp_Text, float amount)
    {
        if (hpBar == null)
            return;

        hpBar.fillAmount = amount;

        if (hp_Text != null)
        {
            if (this.textVariant == TextVariant.Percent)
            {
                hp_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
            }
            else if (this.textVariant == TextVariant.Value)
            {
                hp_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat);
            }
            else if (this.textVariant == TextVariant.ValueMax)
            {
                hp_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat) + "/" + this.m_TextValue;
            }
        }
    }

}
