using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Assets.scripts.util;

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
    public Transform tauntAnimation;

    private List<Ability> abilities = null;

    public Transform door;
    public Transform door90;
    public Transform wall;
    public Transform start;
    public Transform stairs;
    public Transform light;
    public Transform floor;

    public bool isInGame = false;
    public World world;
    public GameObject playerHealth;


    // Use this for initialization
    void Start() {
        Debug.Log("Game logic has started");
        if ((GameObject.Find("Communication")) == null){
            Debug.Log("Go to connect screen.");
            SceneManager.LoadScene("Connect");
        }
    }

    IEnumerator Example()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("Waited a second for server connection to start up");
    }
    

    // Update is called once per frame
    void Update() {
        if (heroes != null && heroes.Count > 0) {
            foreach (var hero in heroes) {
                hero.update();
            }
        }
    }

   

    internal Hero getHero(int heroId) {
        foreach (var hero in heroes) {
            if (heroId == hero.id) {
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

    

    public void updateListOfMinions(List<Minion> newMinions) {
        foreach (var newMinion in newMinions) {
            bool found = false;
            foreach (var minion in minions) {
                if (newMinion.id == minion.id) {
                    found = true;
                    // Need to update all new information that comes from the server
                    if (minion.hp != newMinion.hp) {
                        //update minion hp
                        minion.setHp(newMinion.hp);
                        Debug.Log("Minions new hp = " + minion.hp);
                    }
                    MinionAnimations minionAnimations = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));

                    if (minion.desiredPositionX != newMinion.desiredPositionX || minion.desiredPositionZ != newMinion.desiredPositionZ) {
                        //Debug.Log("Minions desired position changed, update minion " + minion.desiredPositionX + " != " + newMinion.desiredPositionX);
                        minion.desiredPositionX = newMinion.desiredPositionX;
                        minion.desiredPositionZ = newMinion.desiredPositionZ;

                        minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, 0f, newMinion.desiredPositionZ));
                    }

                    if (newMinion.heroTarget > 0) {
                        minionAnimations.heroTargetId = newMinion.heroTarget;
                    }
                }
            }
            if (!found) {
                // Initiate minion here
                Debug.Log("Initiate minion at pos " + newMinion.desiredPositionX + "x" + newMinion.desiredPositionZ);
                Transform minionTransform = (Transform)Instantiate(minion1, new Vector3(newMinion.desiredPositionX, 1.0f, newMinion.desiredPositionZ), Quaternion.identity);
                newMinion.setTransform(minionTransform);
                newMinion.initBars();
                MinionAnimations minionAnimations = (MinionAnimations)minionTransform.GetComponent(typeof(MinionAnimations));
                minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, 1.0f, newMinion.desiredPositionZ));
                FieldOfView fieldOfView = ((FieldOfView) minionTransform.Find("mob1").GetComponent(typeof(FieldOfView)));
                minions.Add(newMinion);
            }
        }
    }

    public void updateListOfHeroes(List<Hero> newHeroes) {
        int heroid = getLobbyCommunication().heroId;
        foreach (var newHero in newHeroes) {
            bool found = false;
            foreach (var hero in heroes) {
                if (newHero.id == hero.id) {
                    found = true;
                    hero.setHp(newHero.hp);
                    hero.xp = newHero.xp;
                    hero.level = newHero.level;
                    hero.resource = newHero.resource;
                    // Dont change desired position for own hero
                    if (hero.id != heroid) {
                        //Debug.Log("Changing position for hero : " + hero.id + " To x[" + newHero.desiredPositionX + "] Z[" + newHero.desiredPositionZ + "]");
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
            if (!found) {
                // Initiate hero here
                Debug.Log("Initiate Hero");
                Transform prefabToUse = warrior;
                if (newHero.class_type == "PRIEST") {
                    prefabToUse = priest;
                }
                Transform heroTransform = (Transform) Instantiate(prefabToUse, new Vector3(newHero.desiredPositionX, 1.0f, newHero.desiredPositionZ), Quaternion.identity);
                heroTransform.name = prefabToUse.name;
                newHero.setTrans(heroTransform);
                if (newHero.id == heroid) {
                    Debug.Log("Setting hero id: " + newHero.id + " To my own hero");
                    ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).isMyHero = true;
                    newHero.updateHealthBar(true);
                }
                ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).heroId = newHero.id;
                heroes.Add(newHero);
                //set initial health for hero
                newHero.initBars();
            }
        }
    }
    public void updateAnimations(List<GameAnimation> gameAnimations) {
        foreach (var gameAnimation in gameAnimations) {
            if (gameAnimation.animation_type == "MINION_DIED") {
                Debug.Log("Minion died");
                Minion minion = getMinion(gameAnimation.target_id);
                Destroy(minion.minionTransform.gameObject);
                // This is wrong, shouldnt do it while in loop (find out correct way to do it) normally done by an iterator but not sure how to do it in c#
                minions.Remove(minion);
                foreach(Hero hero in heroes)
                {
                    if (minion.id == hero.targetEnemy)
                    {
                        hero.targetEnemy = 0;
                        hero.setAutoAttacking(false);
                    }
                }
            }
            if (gameAnimation.animation_type == "ATTACK") {
                //Debug.Log("Attack animation");
                //Minion minion = getMinion(gameAnimation.target_id);
                Hero target = getHero(gameAnimation.source_id);
                CharacterAnimations anim = (CharacterAnimations)target.trans.GetComponent(typeof(CharacterAnimations));
                anim.attackAnimation();
            }
            if (gameAnimation.animation_type == "HERO_RUN")
            {
                Debug.Log("Run animation");
                //Minion minion = getMinion(gameAnimation.target_id);
                Hero target = getHero(gameAnimation.source_id);
                CharacterAnimations anim = (CharacterAnimations)target.trans.GetComponent(typeof(CharacterAnimations));
                anim.runAnimation();
            }
            if (gameAnimation.animation_type == "HERO_IDLE") {
                //Debug.Log("Idle animation");
                Hero target = getHero(gameAnimation.source_id);
                CharacterAnimations anim = (CharacterAnimations)target.trans.GetComponent(typeof(CharacterAnimations));
                anim.idleAnimation();
            }
            if (gameAnimation.animation_type == "MINION_ATTACK") {
                //Debug.Log("Minion attack animation");
                Minion minion = getMinion(gameAnimation.source_id);
                if (minion != null && minion.minionTransform != null) {
                    MinionAnimations anim = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));
                    anim.attackAnimation();
                }
            }
            if (gameAnimation.animation_type == "MINION_RUN") {
                //Debug.Log("Minion run animation");
                Minion minion = getMinion(gameAnimation.source_id);
                if (minion != null && minion.minionTransform != null) {
                    MinionAnimations anim = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));
                    anim.runAnimation();
                }
            }


            // SPELLS
            if (gameAnimation.animation_type == "HEAL") {
                Debug.Log("Heal animnation");
                Hero target = getHero(gameAnimation.target_id);
                Instantiate(healAnimation, new Vector3(target.positionX, 0.3f, target.positionZ), Quaternion.identity);
            }

            if (gameAnimation.animation_type == "TAUNT") {
                Debug.Log("Taunt animnation");
                Hero source = getHero(gameAnimation.source_id);
                Instantiate(tauntAnimation, new Vector3(source.positionX, 0.3f, source.positionZ), Quaternion.identity);
            }
        }
    }



    public void handleHeroBuff(ResponseHeroBuff responseHeroBuff) {
        Hero hero;
        Minion minion;

        hero = getHero(responseHeroBuff.heroId);
        responseHeroBuff.millisBuffStarted = DeviceUtil.getMillis();
        hero.buffs.Add(responseHeroBuff);

        if (responseHeroBuff.minionId > 0 ) {
            minion = getMinion(responseHeroBuff.minionId);
        }
        if (responseHeroBuff.type == Buff.SPEED) {
            hero.calculateSpeed();
            if (!hero.getAutoAttacking()) {
                hero.setAutoAttacking(true);
            }
        }
    }


    public void stopHero(int heroId) {
        Debug.Log("Stopping hero.");
        Hero hero = getHero(heroId);
        hero.desiredPositionX = hero.trans.position.x;
        hero.desiredPositionZ = hero.trans.position.z;

        CharacterAnimations heroAnimation = (CharacterAnimations)hero.trans.GetComponent(typeof(CharacterAnimations));
        heroAnimation.stopMove();
        hero.setAutoAttacking(false);
    }

    public void clearWorld() {
        foreach (var minion in minions) {
            Destroy(minion.minionTransform.gameObject);
        }
        minions = new List<Minion>();
        //foreach (var obstacles in world.obstacles) {
        //    if (obstacles.transform != null && obstacles.transform.gameObject != null) {
        //        Destroy(obstacles.transform.gameObject);
        //    }  else {
        //        //Debug.Log("This obstacle has no gameobject : " + obstacles.type);
        //    }
        //}
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
        if (myHero != null) {
            myHero.targetFriendly = 0;
            myHero.targetEnemy = minion_id;
        }
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
        if (getAbility(ability.id) != null)
        {
            getAbility(ability.id).timeWhenOffCooldown = ability.timeWhenOffCooldown;
            getAbility(ability.id).waitingForCdResponse = false;
            Debug.Log("User got a new cooldown on this ability untill can use again : " + ability.name + " CD : " + ability.timeWhenOffCooldown);
        }
    }

    public Ability getAbility(int id)
    {
        foreach (var ability in abilities)
        {
            if (ability.id == id)
            {
                return ability;
            }
        }
        return null;
    }
    /// <summary>
    /// Get an abilities ID by name, this is usefull for example when sending ability ID to server but you only have access to the name (i.e action bar)
    /// </summary>
    /// <param name="name">The name of the ability you wish to fetch ID for</param>
    /// <returns>int - The corresponding ID for the ability name</returns>
    public int? getAbilityIdByAbilityName(string name)
    {
        foreach (var ability in abilities)
        {
            if (ability.name == name)
            {
                return ability.id;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get the description of the ability so we can show it to the user
    /// </summary>
    /// <param name="name">Name of the ability that we wish to find description for</param>
    /// <returns>String containing the description if one is found, otherwhise empty string</returns>
    public string getAbilityDescriptionByAbilityName(string name) {
        foreach (var ability in abilities)
        {
            if (ability.name == name && ability.description != null)
            {
                return ability.description;
            }
        }
        return "";
    }

    public void autoAttack(){
        // Here we will need to check the id of the minion focused to send up to server.
        // This is a basic attack
        int minionId = ((GameLogic)GameObject.Find("GameLogicObject").GetComponent(typeof(GameLogic))).getMyHero().targetEnemy;
        getCommunication().sendAutoAttack(minionId);
    }

    public Hero getMyHero() {
        if (getLobbyCommunication() != null) {
            int hId = getLobbyCommunication().heroId;
            foreach (var hero in heroes) {
                if (hId == hero.id) {
                    return hero;
                }
            }
        }
      
        return null;
    }

    public bool isMyHeroAlive()
    {
        if (getMyHero() != null)
        {
            return (getMyHero().hp > 0);
        }
        else
        {
            return false;
        }
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

        getGenerator().GenerateRandom(world.seed);

        //foreach (var obstacle in world.obstacles)
        //{
        //    if (obstacle.type == 1) // Wall
        //    {
        //        obstacle.transform = (Transform) Instantiate(wall, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
        //    }else if (obstacle.type == 3) // Start
        //   {
        //        obstacle.transform = (Transform) Instantiate(start, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
        //    }
        //    else if (obstacle.type == 4) // Stairs down
        //    {
        //        obstacle.transform = (Transform) Instantiate(stairs, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
        //    }
        //    else if (obstacle.type == 5) // Light
        //    {
        //        obstacle.transform = (Transform)Instantiate(light, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
        //    }
        //    else if (obstacle.type == 6) // Floor
        //    {
        //        obstacle.transform = (Transform)Instantiate(floor, new Vector3(obstacle.positionX, obstacle.positionY, obstacle.positionZ), Quaternion.identity);
        //    }
        //}
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
        //foreach (var obstacle in world.obstacles)
        //{
        //    if(obstacle.transform != null)
        //    {
        //        Destroy(obstacle.transform.gameObject);
        //    }
        //}
        world = null;
        minions = new List<Minion>();
        heroes = new List<Hero>();
        Debug.Log("Game ended");
    }
    
    ServerCommunication getCommunication() {
        return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
    }

    LobbyCommunication getLobbyCommunication() {
        if (GameObject.Find("Communication") != null) {
            return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
        } else {
            return null;
        }
    }

    DunGenerator getGenerator() {
        return ((DunGenerator)GameObject.Find("Generator").GetComponent(typeof(DunGenerator)));
    }
}