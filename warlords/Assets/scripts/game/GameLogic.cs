using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System.Collections.Generic;
using System;

public class GameLogic : MonoBehaviour
{
    //list of minions currently alive in our universe.
    private List<Minion> minions = new List<Minion>();
    private List<Hero> heroes = new List<Hero>();
    //hold our prefab for the first mob
    public Transform mob1;
    int currentMinionInList;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Game logic has started");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateListOfMinions(List<Minion> newMinions)
    {
        foreach (var newMinion in newMinions)
        {
            bool found = false;
            foreach (var minion in minions)
            {
                if (newMinion.id == minion.id)
                {
                    found = true;
                    if (minion.hp != newMinion.hp)
                    {
                        minion.hp = newMinion.hp;
                        Debug.Log("Minions new hp = " + minion.hp);
                    }
                }
            }
            if (!found)
            {
                // Initiate minion here
                Debug.Log("Initiate minion");
                Instantiate(mob1, new Vector3(newMinion.desiredPositionX, 0, newMinion.desiredPositionZ), Quaternion.identity);
                minions.Add(newMinion);
            }
        }
    }

    public void updateListOfHeroes(List<Hero> newHeroes)
    {
        foreach (var newHero in newHeroes)
        {
            bool found = false;
            foreach (var hero in heroes)
            {
                if (newHero.id == hero.id)
                {
                    found = true;
                    if (hero.hp != newHero.hp)
                    {
                        hero.hp = newHero.hp;
                        Debug.Log("Minions new hp = " + hero.hp);
                    }
                }
            }
            if (!found)
            {
                // Initiate minion here
                Debug.Log("Initiate Hero");
                //Instantiate(mob1, new Vector3(newHero.desiredPositionX, 0, newHero.desiredPositionZ), Quaternion.identity);
                heroes.Add(newHero);
            }
        }
    }

    public List<Minion> getMinions()
    {
        return minions;
    }

    public void setHeroTargetEnemy(int minion_id)
    {
        Hero myHero = getMyHero();
        myHero.targetEnemy = minion_id;
    }


    public Hero getMyHero()
    {
        foreach (var hero in heroes)
        {
            String heroid = ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).getHeroId();
            int hId = Int32.Parse(heroid);
            if (hId == hero.id)
            {
                return hero;
            }
        }
        return null;
    }
}