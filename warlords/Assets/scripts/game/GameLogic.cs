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

    // Must have a prefab for communication in case of we want to start game scene without going through lobby (in that case the communication gameobject is not alive)
    public Transform communication;
    //hold our prefab for the first mob
    public Transform minion1;
    // Hold prefab for a warrior hero
    public Transform warrior;
    // Hold prefab for a priest hero
    public Transform priest;

    // Animation Effects
    public Transform healAnimation;

    public List<Ability> abilities = null;

    public Transform door;
    public Transform door90;
    public Transform wall;
    public Transform start;
    public Transform stairs;
    public Transform light;

    public bool isInGame = false;
    public World world;


    // Use this for initialization
    void Start()
    {
        Debug.Log("Game logic has started");
        if ((GameObject.Find("Communication")) == null)
        {
            Debug.Log("Communication is not set, create from prefab");
            Instantiate(communication, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            StartCoroutine(Example());
        }
        else
        {
            checkIfShouldJoinServer();
        }
    }

    IEnumerator Example()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Waited a second for server connection to start up");
        checkIfShouldJoinServer();
    }

    public void checkIfShouldJoinServer()
    {
        if (getCommunication().gameId == 0)
        {
            getCommunication().joinServer();
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("a"))
        {
            autoAttack();
        }
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

    public Hero getClosestHeroByPosition(Vector3 position)
    {
        Hero closestHero = null;
        float closesDistance = 100.0f;
        foreach (var hero in heroes)
        {
            float distance = Vector3.Distance(hero.getTransformPosition(), position);
            if (distance < closesDistance) {
                closesDistance = distance;
                closestHero = hero;
            }
        }
        if (closestHero != null) {
            return closestHero;
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

    public Minion getClosestMinionByPosition(Vector3 position)
    {
        Minion closestMinion = null;
        float closesDistance = 100.0f;
        foreach (var minion in minions)
        {
            float distance = Vector3.Distance(minion.getTransformPosition(), position);
            if (distance < closesDistance)
            {
                closesDistance = distance;
                closestMinion = minion;
            }
        }
        if (closestMinion != null)
        {
            return closestMinion;
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
                        minion.setHp(newMinion.hp);;
                        Debug.Log("Minions new hp = " + minion.hp);
                    }
                    minion.desiredPositionX = newMinion.desiredPositionX;
                    minion.desiredPositionZ = newMinion.desiredPositionZ;

                    MinionAnimations minionAnimations = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));
                    minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, 0f, newMinion.desiredPositionZ));
                    if (newMinion.heroTarget > 0) {
                        minionAnimations.heroTargetId = newMinion.heroTarget;
                    }
                }
            }
            if (!found)
            {
                // Initiate minion here
                Debug.Log("Initiate minion");
                Transform minionTransform = (Transform)Instantiate(minion1, new Vector3(newMinion.desiredPositionX, 0f, newMinion.desiredPositionZ), Quaternion.identity);
                newMinion.setTransform(minionTransform);
                newMinion.initBars();
                MinionAnimations minionAnimations = (MinionAnimations)minionTransform.GetComponent(typeof(MinionAnimations));
                minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, 0f, newMinion.desiredPositionZ));
                FieldOfView fieldOfView = ((FieldOfView) minionTransform.Find("mob1").GetComponent(typeof(FieldOfView)));
                fieldOfView.TYPE_OF_FIELD_OF_VIEW = 1;
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
                        Debug.Log("Heroes new hp = " + hero.hp);
                    }
                    // Dont change desired position for own hero
                    String heroid = getCommunication().getHeroId();
                    if (hero.id != Int32.Parse(heroid))
                    {
                        Debug.Log("Changing position for hero : " + hero.id + " To x[" + newHero.desiredPositionX + "] Z[" + newHero.desiredPositionZ + "]");
                        hero.desiredPositionX = newHero.desiredPositionX;
                        hero.desiredPositionZ = newHero.desiredPositionZ;
                        Vector3 target = new Vector3(newHero.desiredPositionX, 1.0f, newHero.desiredPositionZ);
                        CharacterAnimations animation = (CharacterAnimations)hero.trans.GetComponent(typeof(CharacterAnimations));
                        animation.setDesiredLocation(target);
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
                newHero.setTrans(heroTransform);
                String heroid = getCommunication().getHeroId();
                int hId = Int32.Parse(heroid);
                if (newHero.id == hId)
                {
                    ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).isMyHero = true;
                }
                ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).heroId = newHero.id;
                heroes.Add(newHero);
            }
        }
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
            if (gameAnimation.animation_type == "ATTACK")
            {
                Debug.Log("Attack anination");
                //Minion minion = getMinion(gameAnimation.target_id);
                Hero target = getHero(gameAnimation.source_id);
                CharacterAnimations anim = (CharacterAnimations)target.trans.GetComponent(typeof(CharacterAnimations));
                anim.attackAnimation();
            }
            if (gameAnimation.animation_type == "MINION_ATTACK")
            {
                Debug.Log("Minion attack anination");
                Minion minion = getMinion(gameAnimation.target_id);
                Hero target = getHero(gameAnimation.source_id);
                if (minion != null && minion.minionTransform != null)
                {
                    MinionAnimations anim = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));
                    anim.attackAnimation();
                }
                
            }
        }
    }

    public void clearWorld()
    {
        foreach (var minion in minions)
        {
            Destroy(minion.minionTransform.gameObject);
        }
        minions = new List<Minion>();
        foreach (var obstacles in world.obstacles)
        {
            if (obstacles.transform != null && obstacles.transform.gameObject != null)
            {
                Destroy(obstacles.transform.gameObject);
            }
            else {
                //Debug.Log("This obstacle has no gameobject : " + obstacles.type);
            }
            
        }
        world = null;
        Debug.Log("World is cleared");
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

    public void sendSpell(int spellId, List<int> enemies, List<int> friendly)
    {
        Debug.Log("Send spell " + spellId);
        getCommunication().sendSpell(spellId, enemies, friendly, getMyHero().getTargetPosition());
    }

    public List<Ability> getAbilities()
    {
        return abilities;
    }

    private void updateAbilities()
    {
        getCommunication().getAbilities();
    }

    public void setAbilities(List<Ability> updatedAbilities)
    {
        abilities = updatedAbilities;
        foreach (var ability in abilities)
        {
            Debug.Log("Ability : " + ability.name);
        }
    }

    public void updateCooldown(Ability ability)
    {
        Debug.Log("User got a new cooldown on this ability untill can use again : " + ability.name + " ");

    }

    public void autoAttack()
    {
        // Here you will need to check the id of the minion focused to send up to server.
        // This is a basic attack
        int minionId = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().targetEnemy;
        getCommunication().sendAutoAttack(minionId);

    }

    public Hero getMyHero()
    {
        foreach (var hero in heroes)
        {
            String heroid = getCommunication().getHeroId();
            int hId = Int32.Parse(heroid);
            if (hId == hero.id)
            {
                return hero;
            }
        }
        return null;
    }

    public void teleportHeroes(List<Hero> teleportHeroes)
    {
        foreach (var heroInList in teleportHeroes)
        {
            Hero hero = getHero(heroInList.id);
            hero.desiredPositionX = heroInList.desiredPositionX;
            hero.desiredPositionZ = heroInList.desiredPositionZ;
            hero.positionX = heroInList.positionX;
            hero.positionZ = heroInList.positionZ;
            Vector3 newPosition = new Vector3(hero.positionX, 1.0f, hero.positionZ);
            hero.trans.position = newPosition;
            Debug.Log("Moved hero: " + hero.id + " to position : " + newPosition);
        }
    }

    public void createWorld(ResponseWorld responseWorld)
    {
        Debug.Log("Server sent to create world");
        world = responseWorld.world;
        foreach (var obstacle in world.obstacles)
        {
            if (obstacle.type == 1) // Wall
            {
                obstacle.transform = (Transform) Instantiate(wall, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }else if (obstacle.type == 3) // Start
            {
                obstacle.transform = (Transform) Instantiate(start, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }
            else if (obstacle.type == 4) // Stairs down
            {
                obstacle.transform = (Transform) Instantiate(stairs, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }
            else if (obstacle.type == 5) // Light
            {
                obstacle.transform = (Transform)Instantiate(light, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
            }
        }
    }

    public void endGame()
    {
        foreach (var hero in heroes)
        {
            Destroy(hero.trans.gameObject);
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


    ServerCommunication getCommunication()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Communication");
        foreach (GameObject go in gos)
        {
            return (ServerCommunication)go.GetComponent(typeof(ServerCommunication));
        }
        return null;
    }

}