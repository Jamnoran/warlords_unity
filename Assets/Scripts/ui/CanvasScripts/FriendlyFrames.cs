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
    public TextVariant textVariant = TextVariant.Percent;
    public int m_TextValue = 100;
    public string m_TextValueFormat = "0";

    private float hp1;
    private float hp2;
    private float hp3;

    public enum TextVariant
    {
        Percent,
        Value,
        ValueMax
    }

    // Use this for initialization
    void Start()
    {
        friendlyFrame1.SetActive(false);
        friendlyFrame2.SetActive(false);
        friendlyFrame3.SetActive(false);
    }

    public void UpdatePartyFrames(List<Hero> heroes)
    {
        for (int i = 0 ; i < heroes.Count ; i++)
        {
            GameObject targetFrame = null;
            switch (i)
            {
                case 0:
                    targetFrame = friendlyFrame1;
                    break;
                case 1:
                    targetFrame = friendlyFrame1;
                    break;
                case 2:
                    targetFrame = friendlyFrame1;
                    break;
            }
            float hpPerc = (float)heroes[i].hp / (float)heroes[i].maxHp;
            updateInformation(targetFrame, hpPerc, 0.0f, true, heroes[i].class_type, heroes[i].class_type);
        }
    }


    private void updateInformation(GameObject frame, float hp, float resource, bool friendly, string classType, string charName)
    {
        frame.SetActive(true);
        frame.transform.Find("HP Bar").GetComponent<UIProgressBar>().fillAmount = hp;
        frame.transform.Find("Unit Name").GetComponent<Text>().text = charName;
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

}
