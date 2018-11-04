using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroInfo : MonoBehaviour {
    public int heroId = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setHeroId(int id)
    {
        heroId = id;
    }

    public int getHeroId()
    {
        return heroId;
    }

    public void setSelected(bool sel)
    {
        Transform selectTarget = gameObject.transform.Find("SelectedTarget");
        selectTarget.gameObject.SetActive(sel);
    }
}
