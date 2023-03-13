using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;

public class HeroInfo : MonoBehaviour {
    public int heroId = 0;
    public bool alive = true;
    [SerializeField]
    public Hero hero;
    public bool isMyHero = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setHero(Hero updatedHero)
    {
        hero = updatedHero;
    }

    public Hero getHero()
    {
        return hero;
    }

    public void setHeroId(int id)
    {
        heroId = id;
    }

    public int getHeroId()
    {
        return heroId;
    }

    public void setAlive(bool updatedAliveStatus)
    {
        alive = updatedAliveStatus;
    }

    public bool isAlive()
    {
        return alive;
    }

    public void setSelected(bool sel)
    {
        Transform selectTarget = gameObject.transform.Find("SelectedTarget");
        selectTarget.gameObject.SetActive(sel);
    }
}
