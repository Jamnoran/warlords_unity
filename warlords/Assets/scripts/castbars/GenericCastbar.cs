using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenericCastbar : MonoBehaviour {

    public Image castBarFiller;
    public GameObject textToUpdate;
    private Text tmpTxt;
    private bool isCasting;
    private float castTime;
    private float timeLeft;

    private void Start()
    {

        tmpTxt = textToUpdate.GetComponent<Text>();
        //instantiate casting to false, just in case something wierd sets it to true
        isCasting = false;
        //set fillammount to 0, seing how we allways start casting from scratch
        castBarFiller.fillAmount = 0;
      

        //mock casttime:
        castTime = 5.0f;
        timeLeft = castTime;
    }

    void Update () {

        //temp key so we can test this 
        if (Input.GetKeyDown("h"))
        {
            isCasting = true;
        }

        if (isCasting && castBarFiller.fillAmount < 1.0f)
        {
            castBarFiller.fillAmount += 1.0f / castTime * Time.deltaTime;
            timeLeft = timeLeft - Time.deltaTime;
            tmpTxt.text = (Math.Round(timeLeft,2)).ToString();
        }
        else
        {
            resetCastBar();
        }
	}

    private void resetCastBar()
    {
        tmpTxt.text = "";
        isCasting = false;
        castBarFiller.fillAmount = 0;
        timeLeft = castTime;
    }
}
