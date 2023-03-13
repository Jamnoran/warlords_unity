using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.scripts.vo;
using UnityEngine.UI;

public class foobar : MonoBehaviour {

    public Slider slider1;
    public Slider slider2;
    public Slider slider3;

    private float currentHealth1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



   public void UpdatePartyFrames(List<Hero> heroes)
    {
        var numberOfFriends = heroes.Count;

        if (numberOfFriends == 0)
        {
           
        }
        else if (numberOfFriends == 1)
        {
            slider1.maxValue = heroes[0].maxHp;
            slider1.value = heroes[0].hp;
        }
        else if (numberOfFriends == 2)
        {
            slider1.maxValue = heroes[0].maxHp;
            slider1.value = heroes[0].hp;
            slider2.maxValue = heroes[1].maxHp;
            slider2.value = heroes[1].hp;
        }
        else
        {
            slider1.maxValue = heroes[0].maxHp;
            slider1.value = heroes[0].hp;
            slider2.maxValue = heroes[1].maxHp;
            slider2.value = heroes[1].hp;
            slider3.maxValue = heroes[2].maxHp;
            slider3.value = heroes[2].hp;

        }

      

    }

  
}
