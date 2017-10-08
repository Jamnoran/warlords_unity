using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingCombatText : MonoBehaviour {

    public Canvas enemyCanvas;
    public GameObject sctPrefab;
    public float timeToDie = 0.5f;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void showText(string damage, bool crit, string color)
    {
        GameObject temp = Instantiate(sctPrefab) as GameObject;
        RectTransform tempRect = temp.GetComponent<RectTransform>();
        temp.transform.SetParent(enemyCanvas.transform);
        tempRect.transform.localPosition = sctPrefab.transform.localPosition;
        tempRect.transform.localScale = sctPrefab.transform.localScale;
        tempRect.transform.localRotation = sctPrefab.transform.localRotation;
        string damageInDisplayFormat = "" + Math.Round(float.Parse(damage));

        if (color == "#FF00FF00")
        {
            temp.GetComponent<Text>().color = Color.green;
        }
        else if (color == "#FFFF0000")
        {
            temp.GetComponent<Text>().color = Color.red;
        }
        if (crit)
        {
            damageInDisplayFormat = "*" + damage + "*";
            temp.GetComponent<Text>().fontStyle = FontStyle.Bold;
        }
        temp.GetComponent<Text>().text = damageInDisplayFormat;
        Destroy(temp, timeToDie);
    }
}
