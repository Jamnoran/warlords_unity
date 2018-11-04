using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeTargetColor : MonoBehaviour {

    public static int RED = 1;
    public static int YELLOW = 2;
    public static int PURPLE = 3;
    public static int WHITE = 4;
    public Sprite redSprite;
    public Sprite yellowSprite;
    public Sprite purpleSprite;
    public Sprite whiteSprite;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setColor(int color)
    {
        Transform hpBar = gameObject.transform.Find("HP Bar");
        Image background = hpBar.GetComponent<Image>();
        if (color == YELLOW) {
            background.sprite = yellowSprite;
        } else if (color == PURPLE)
        {
            background.sprite = purpleSprite;
        }
        else if (color == WHITE)
        {
            background.sprite = whiteSprite;
        }
        else if (color == RED)
        {
            background.sprite = redSprite;
        }

    }

    public void clearColor()
    {
        Transform hpBar = gameObject.transform.Find("HP Bar");
        Image background = hpBar.GetComponent<Image>();
        background.sprite = redSprite;
    }
}
