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
    // Hold prefab for a warrior hero
    public Transform warrior;
    // Hold prefab for a priest hero
    public Transform priest;

    // Animation Effects
    public Transform healAnimation;


    public Transform door;
    public Transform door90;
    public Transform wall;
    public Transform start;
    public Transform stairs;

    public bool isInGame = false;
    public World world;


    // Use this for initialization
    void Start()
    {
        Debug.Log("Game logic has started");
    }

    // Update is called once per frame
    void Update()
    {
    }
    

    internal Hero getHero(int heroId)
    {
        foreach (var hero in heroes)
        {
            if (heroId == hero.id)
            {
                return hero;
            }
        }
        return null;
    }

    public Minion getMinion(int minionId)
    {
        foreach (var minion in minions)
        {
            if (minionId == minion.id)
            {
                return minion;
            }
        }
        return null;
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
                    // Need too update all new information that comes from the server
                    if (minion.hp != newMinion.hp)
                    {
                        minion.hp = newMinion.hp;
                        Debug.Log("Minions new hp = " + minion.hp);
                    }
                    minion.desiredPositionX = newMinion.desiredPositionX;
                    minion.desiredPositionZ = newMinion.desiredPositionZ;
                }
            }
            if (!found)
            {
                // Initiate minion here
                Debug.Log("Initiate minion");
                Transform minionTransform = (Transform)Instantiate(mob1, new Vector3(newMinion.desiredPositionX, 2, newMinion.desiredPositionZ), Quaternion.identity);
                newMinion.setTransform(minionTransform);
                //((MinionMove)minionTransform.GetComponent(typeof(MinionMove))).minionId = newMinion.id;
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
                    // Dont change desired position for own hero
                    String heroid = ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).getHeroId();
                    if (hero.id != Int32.Parse(heroid))
                    {
                        hero.desiredPositionX = newHero.desiredPositionX;
                        hero.desiredPositionZ = newHero.desiredPositionZ;
                    }
                    hero.positionX = newHero.positionX;
                    hero.positionZ = newHero.positionZ;

                }
            }
            if (!found)
            {
                // Initiate hero here
                Debug.Log("Initiate Hero");
                Transform prefabToUse = warrior;
                if (newHero.class_type == "PRIEST") {
                    prefabToUse = priest;
                }
                Transform heroTransform = (Transform) Instantiate(prefabToUse, new Vector3(newHero.desiredPositionX, 1.0f, newHero.desiredPositionZ), Quaternion.identity);
                heroTransform.name = prefabToUse.name;
                newHero.setTransform(heroTransform);
                String heroid = ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).getHeroId();
                int hId = Int32.Parse(heroid);
                if (newHero.id == hId)
                {
                    ((move)heroTransform.GetComponent(typeof(move))).isMyHero = true;
                    //((revealFogOnMove)GetComponent(typeof(revealFogOnMove))).setHero(heroTransform);
                }
                ((move)heroTransform.GetComponent(typeof(move))).heroId = newHero.id;
                heroes.Add(newHero);
            }
        }
    }

    public List<Minion> getMinions()
    {
        return minions;
    }

    public List<Hero> getHeroes()
    {
        return heroes;
    }

    public void setHeroTargetEnemy(int minion_id)
    {
        Hero myHero = getMyHero();
        myHero.targetFriendly = 0;
        myHero.targetEnemy = minion_id;
    }
    public void setHeroTargetFriendly(int hero_id)
    {
        Hero myHero = getMyHero();
        myHero.targetEnemy = 0;
        myHero.targetFriendly = hero_id;
    }

    public Minion getMyHeroEnemyTarget()
    {
        Hero myHero = getMyHero();
        if (myHero != null && myHero.targetEnemy > 0)
        {
            return getMinion(myHero.targetEnemy);
        }
        return null;
    }

    public Hero getMyHeroFriendlyTarget()
    {
        Hero myHero = getMyHero();
        if (myHero != null && myHero.targetFriendly > 0)
        {
            return getHero(myHero.targetFriendly);
        }
        return null;
    }

    public void sendSpell(int spellId)
    {
        Debug.Log("Send spell " + spellId);
        ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication))).sendSpell(spellId, getMyHero().targetEnemy, getMyHero().targetFriendly, getMyHero().getTargetPosition());
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

    public void updateAnimations(List<GameAnimation> gameAnimations)
    {
        foreach (var gameAnimation in gameAnimations)
        {
            if (gameAnimation.animation_type == "MINION_DIED")
            {
                Debug.Log("Minion died");
                Minion minion = getMinion(gameAnimation.target_id);
                Destroy(minion.minionTransform.gameObject);
                // This is wrong, shouldnt do it while in loop (find out correct way to do it) normally done by an iterator but not sure how to do it in c#
                minions.Remove(minion);
            }
            if (gameAnimation.animation_type == "HEAL")
            {
                Debug.Log("Heal anination");
                Hero target = getHero(gameAnimation.target_id);
                Instantiate(healAnimation, new Vector3(target.positionX, 0.3f, target.positionZ), Quaternion.identity);
            }
        } 
    }

    public void createWorld(ResponseWorld responseWorld)
    {
        Debug.Log("Server sent to create world");
        world = responseWorld.world;
        foreach (var obstacle in world.obstacles)
        {
            if (obstacle.type == 1)
            {
                obstacle.transform = (Transform)Instantiate(wall, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }else if (obstacle.type == 3) // Start
            {
                obstacle.transform = (Transform)Instantiate(start, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }
            else if (obstacle.type == 4) // Stairs down
            {
                obstacle.transform = (Transform)Instantiate(stairs, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }
            else if (obstacle.type == 5) // Light
            {
                //obstacle.transform = (Transform)Instantiate(light, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }
        }
    }

    public void endGame()
    {
        foreach (var hero in heroes)
        {
            Destroy(hero.transform.gameObject);
        }
        foreach (var minion in minions)
        {
            Destroy(minion.minionTransform.gameObject);
        }
        foreach (var obstacle in world.obstacles)
        {
            if(obstacle.transform != null)
            {
                Destroy(obstacle.transform.gameObject);
            }
            
        }
        world = null;
        minions = new List<Minion>();
        heroes = new List<Hero>();
        Debug.Log("Game ended");
    }

}