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
    public Transform minion2;
    // Hold prefab for heroes
    public Transform warrior;
    public Transform priest;
	public Transform warlock;

    // Animation Effects
    public Transform healAnimation;
    public Transform tauntAnimation;
    public Transform shieldAnimation;

    private List<Ability> abilities = null;

    public bool isInGame = false;
    public World world;
    public GameObject playerHealth;

    private int thisHeroId;
    // Use this for initialization
    void Start() {
        Debug.Log("Game logic has started");
        if ((GameObject.Find("Communication")) == null){
            Debug.Log("Go to connect screen.");
            SceneManager.LoadScene("Connect");
        }

        if (getLobbyCommunication() != null) {
            try
            {
                thisHeroId = getLobbyCommunication().heroId;
            }
            catch (Exception e)
            {

                throw new Exception("Could not load lobbycommunication: " + e);
            }
        }

    }

    public Hero getHeroByTransform(Transform transform)
    {
        return getHero(((HeroInfo)transform.GetComponent(typeof(HeroInfo))).getHeroId());
    }



    public Minion getMinionByTransform(Transform transform)
    {
        MinionInfo minionInfo = (MinionInfo)transform.GetComponent(typeof(MinionInfo));
        return getMinion(minionInfo.getMinionId());
    }

    // Update is called once per frame
    void Update() {
        if (heroes != null && heroes.Count > 0) {
            foreach (var hero in heroes) {
                hero.update();
            }
        }
        if (Input.GetKeyUp("n"))
        {
            getCommunication().heroHasClickedPortal(getMyHero().id);
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

    public Hero getClosestHeroByPosition(Vector3 position) {
        Hero closestHero = null;
        float closesDistance = 100.0f;
        foreach (var hero in heroes) {
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

    public Minion getClosestMinionByPosition(Vector3 position) {
        Minion closestMinion = null;
        float closesDistance = 100.0f;
        foreach (var minion in minions) {
            float distance = Vector3.Distance(minion.getTransformPosition(), position);
            if (distance < closesDistance) {
                closesDistance = distance;
                closestMinion = minion;
            }
        }
        if (closestMinion != null) {
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
                        minion.desiredPositionY = newMinion.desiredPositionY;

                        minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, newMinion.desiredPositionY + 1.0f, newMinion.desiredPositionZ));
                    }

                    if (newMinion.heroTarget > 0) {
                        minionAnimations.heroTargetId = newMinion.heroTarget;
                    }
                }
            }
            if (!found) {
                initiateMinion(newMinion);
            }
        }
    }
        
    public void initiateMinion(Minion newMinion)
    {
        // Initiate minion here
        //Debug.Log("Initiate minion at pos " + newMinion.desiredPositionX + "x" + newMinion.desiredPositionZ + " y: " + newMinion.desiredPositionY);
        Transform prefabOfMinion = null;
        if (newMinion.minionType == 1)
        {
            prefabOfMinion = minion1;
        }else if (newMinion.minionType == 2)
        {
            prefabOfMinion = minion2;
        }
        Transform minionTransform = (Transform)Instantiate(prefabOfMinion, new Vector3(newMinion.desiredPositionX, newMinion.desiredPositionY + 1.0f, newMinion.desiredPositionZ), Quaternion.identity);
        newMinion.setTransform(minionTransform);
        newMinion.initBars();
        MinionAnimations minionAnimations = (MinionAnimations)minionTransform.GetComponent(typeof(MinionAnimations));
        minionAnimations.setDesiredLocation(new Vector3(newMinion.desiredPositionX, newMinion.desiredPositionY + 1.0f, newMinion.desiredPositionZ));
        FieldOfView fieldOfView = ((FieldOfView)minionTransform.Find("mob").GetComponent(typeof(FieldOfView)));
        MinionInfo minionInfo = (MinionInfo)minionTransform.GetComponent(typeof(MinionInfo));
        minionInfo.setMinionId(newMinion.id);
        minions.Add(newMinion);
    }


    public void sendMinionPostionsToServer(){
		if (getMinions ().Count > 0) {
			List<Minion> updatePositionMinions = new List<Minion> ();
			foreach (Minion minion in getMinions()) {
				Minion tempMin = new Minion ();
				tempMin.id = minion.id;
				Vector3 currPos = minion.getTransformPosition ();
				tempMin.positionX = currPos.x;
				tempMin.positionY = currPos.y;
				tempMin.positionZ = currPos.z;
				updatePositionMinions.Add (tempMin);
			}
			//Debug.Log ("Sending update of minions position of this many minions : " + updatePositionMinions.Count);
			getCommunication ().sendUpdateMinionPosition (getMyHero ().id, updatePositionMinions);
			updatePositionMinions = null; 
		}
	}

    public void updateListOfHeroes(List<Hero> newHeroes) {
        foreach (var newHero in newHeroes) {
            updateHero(newHero);
        }

        UpdateXpBar();
        UpdatePartyFrames(newHeroes);
    }

    private void UpdatePartyFrames(List<Hero> heroes)
    {
        try
        {
            heroes.RemoveAll(hero => hero.id == thisHeroId);
            if (getPartyFrame() != null)
            {
                getPartyFrame().UpdatePartyFrames(heroes);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Could not update party-frames: " + e);
        }
     
    }

    private void UpdateXpBar()
    {
        try
        {
            getPartyFrame().UpdateXpBar(getMyHero());
        }
        catch (Exception e)
        {
            throw new Exception("Could not update xp-bar: " + e);
        }
        
    }

    void updateHero(Hero newHero)
    {
        int heroid = getLobbyCommunication().heroId;
        bool found = false;
        foreach (var hero in heroes)
        {
            // Check if this is the corresponding hero to update
            if (newHero.id == hero.id)
            {
                found = true;
                hero.setHp(newHero.hp);
                hero.xp = newHero.xp;
                hero.level = newHero.level;
                hero.armor = newHero.armor;
                hero.magicResistance = newHero.magicResistance;
                hero.armorPenetration = newHero.armorPenetration;
                hero.magicPenetration = newHero.magicPenetration;
                hero.setResource(newHero.resource);
                // Dont change desired position for own hero
                if (hero.id != heroid)
                {
                    hero.desiredPositionX = newHero.desiredPositionX;
                    hero.desiredPositionZ = newHero.desiredPositionZ;
                    CharacterAnimations animation = (CharacterAnimations)hero.trans.GetComponent(typeof(CharacterAnimations));

                    Vector3 target = new Vector3(newHero.desiredPositionX, newHero.desiredPositionY + 1.0f, newHero.desiredPositionZ);
                    animation.setDesiredLocation(target);
                    Vector3 currentPositionFromServer = new Vector3(newHero.positionX, newHero.positionY + 1.0f, newHero.positionZ);
                    animation.setPositionFromServer(currentPositionFromServer);
                }
                hero.positionX = newHero.positionX;
                hero.positionZ = newHero.positionZ;

                updateHeroBuffs(newHero, hero);
            }
        }
        if (!found)
        {
            initiateHero(newHero);
        }
    }

    void updateHeroBuffs(Hero newHero, Hero hero)
    {
        List<Buff> oldBuffs = hero.buffs;
        hero.buffs = newHero.buffs;
        foreach (var buff in newHero.buffs)
        {
            foreach (var oldbuff in oldBuffs)
            {
                if (buff.type == oldbuff.type)
                {
                    buff.millisBuffStarted = oldbuff.millisBuffStarted;
                }
            }
            if (buff.millisBuffStarted == 0)
            {
                buff.millisBuffStarted = DeviceUtil.getMillis();
            }
            if (buff.type == Buff.SHIELD)
            {
                ShieldLogic shieldLogic = (ShieldLogic)hero.trans.GetComponent(typeof(ShieldLogic));
                if (!shieldLogic.shieldOn)
                {
                    Debug.Log("Shielding hero " + hero.id);
                    shieldLogic.setVisibility(true);
                }
            }
            if (buff.type == Buff.SPEED)
            {
                Debug.Log("Buff speed! Calculating new speed");
                hero.calculateSpeed();
                hero.setAutoAttacking(true);
            }
        }
    }

    public void updateAbilityInformation(Ability ability)
    {
        Debug.Log("We got an update of ability for use with castbar");
        if (ability.id > 0)
        {
            GetCooldown().setCooldown(ability.position, ability.timeWhenOffCooldown, ability.id);
        }
        else
        {
            getAbility(0).timeWhenOffCooldown = ability.timeWhenOffCooldown;
        }

    }

    void initiateHero(Hero newHero)
    {
        int heroid = getLobbyCommunication().heroId;
        // Initiate hero here
        Debug.Log("Initiate Hero");
        Transform prefabToUse = warrior;
        if (newHero.class_type == "PRIEST")
        {
            prefabToUse = priest;
        }
		if (newHero.class_type == "WARLOCK")
		{
			prefabToUse = warlock;
		}
        Transform heroTransform = (Transform)Instantiate(prefabToUse, new Vector3(newHero.desiredPositionX, newHero.desiredPositionY + 1.0f, newHero.desiredPositionZ), Quaternion.identity);
        heroTransform.name = prefabToUse.name;
        newHero.setTrans(heroTransform);
        if (newHero.id == heroid)
        {
            Debug.Log("Setting hero id: " + newHero.id + " To my own hero");
            ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).isMyHero = true;
            newHero.updateHealthBar(true);
        }
        ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).heroId = newHero.id;

        HeroInfo heroInfo = (HeroInfo)heroTransform.GetComponent(typeof(HeroInfo));
        heroInfo.setHeroId(newHero.id);

        heroes.Add(newHero);
        //set initial health for hero
        if (newHero.id == heroid)
        {
            newHero.initBars();
        }
        
    }

    internal void setTalents(ResponseTalents response) {
        getMyHero().talents = response.talents;
        getMyHero().setTotalTalentPoints(response.totalTalentPoints);
    }

    public void updateAnimations(List<GameAnimation> gameAnimations) {
        foreach (var gameAnimation in gameAnimations) {
            if (gameAnimation.animation_type == "MINION_DIED") {
                Debug.Log("Minion died");
                Minion minion = getMinion(gameAnimation.target_id);

                ((HealthUpdate)minion.minionTransform.GetComponent(typeof(HealthUpdate))).hideBar();
                Debug.Log("Minion died, disable everything!");

                ((FieldOfViewMinion)minion.minionTransform.Find("mob").GetComponent(typeof(FieldOfViewMinion))).enabled = false;
                // Disable capsle collider + Character animation
                ((CapsuleCollider)minion.minionTransform.GetComponent(typeof(CapsuleCollider))).enabled = false;
                ((CharacterController)minion.minionTransform.GetComponent(typeof(CharacterController))).enabled = false;

                //Destroy(minion.minionTransform.gameObject);
                MinionAnimations anim = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));
                anim.deadAnimation();
                minion.setAlive(false);

                // This is wrong, shouldnt do it while in loop (find out correct way to do it) normally done by an iterator but not sure how to do it in c#
                //minions.Remove(minion);
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
                //Debug.Log("Run animation");
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
            // TODO: Update if hero has animation as well
            if (gameAnimation.animation_type == "HEAL") {
                Debug.Log("Heal animation");
                Hero target = getHero(gameAnimation.target_id);
                Transform healAn = Instantiate(healAnimation, new Vector3(target.positionX, target.positionY, target.positionZ), Quaternion.identity);
                FollowGameObject anim = (FollowGameObject)healAn.GetComponent(typeof(FollowGameObject));
                anim.objectToFollow = target.trans;
            }
            if (gameAnimation.animation_type == "TAUNT") {
                Debug.Log("Taunt animation");
                Hero source = getHero(gameAnimation.source_id);
                Instantiate(tauntAnimation, new Vector3(source.positionX, source.positionY, source.positionZ), Quaternion.identity);
            }
            if (gameAnimation.animation_type == "DRAIN")
            {
                Debug.Log("Drain life animation");
                Hero source = getHero(gameAnimation.source_id);
                CharacterAnimations anim = (CharacterAnimations)source.trans.GetComponent(typeof(CharacterAnimations));
                anim.spellAnimation(gameAnimation.spellAnimationId);
            }
            if (gameAnimation.animation_type == "SMITE")
            {
                Debug.Log("Smite animation");
                Hero source = getHero(gameAnimation.source_id);
                CharacterAnimations anim = (CharacterAnimations)source.trans.GetComponent(typeof(CharacterAnimations));
                anim.spellAnimation(gameAnimation.spellAnimationId);
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
        //hero.setAutoAttacking(false);
    }

    public void clearWorld() {
        foreach (var minion in minions) {
            Destroy(minion.minionTransform.gameObject);
        }
        minions = new List<Minion>();

        world = null;
        Debug.Log("World is cleared");
    }

    public List<Minion> getMinions()
    {
        return minions;
    }

    public List<Minion> getAliveMinions()
    {
        List<Minion> minionsAlive = new List<Minion>();
        foreach (Minion min in minions)
        {
            if (min.hp > 0)
            {
                minionsAlive.Add(min);
            }
        }
        return minionsAlive;
    }
    

    public List<Hero> getHeroes()
    {
        return heroes;
    }

    public void setHeroTargetEnemy(int minion_id) {
        Hero myHero = getMyHero();
        if (myHero != null) {
            myHero.targetFriendly = 0;
            myHero.targetEnemy = minion_id;
        }
    }

    public void setHeroTargetFriendly(int hero_id) {
        Hero myHero = getMyHero();
        if (myHero != null)
        {
            myHero.targetEnemy = 0;
            myHero.targetFriendly = hero_id;
        }
    }

    public Minion getMyHeroEnemyTarget() {
        Hero myHero = getMyHero();
        if (myHero != null && myHero.targetEnemy > 0)
        {
            return getMinion(myHero.targetEnemy);
        }
        return null;
    }

    public Hero getMyHeroFriendlyTarget() {
        Hero myHero = getMyHero();
        if (myHero != null && myHero.targetFriendly > 0)
        {
            return getHero(myHero.targetFriendly);
        }
        return null;
    }

    public void sendSpell(int spellId, List<int> enemies, List<int> friendly) {
        Debug.Log("Send spell " + spellId);
        if (isMyHeroAlive())
        {
            Hero myHero = getMyHero();
            getCommunication().sendStopHero(myHero.id);
            // TODO: Turn hero towards target (either against aoe/enemy/friendly)
			getCommunication().sendSpell(myHero.id, spellId, enemies, friendly, myHero.getTargetPosition());
        }
    }

    public List<Ability> getAbilities() {
        return abilities;
    }

	private void updateAbilities() {
		Hero myHero = getMyHero();
		getCommunication().getAbilities(myHero.id);
    }

    public void setAbilities(List<Ability> updatedAbilities) {
        abilities = updatedAbilities;
        //foreach (var ability in abilities)
        //{
        //    Debug.Log("Ability : " + ability.name);
        //}
    }

    public void updateCooldown(Ability ability) {
        if (getAbility(ability.id) != null)
        {
            getAbility(ability.id).timeWhenOffCooldown = ability.timeWhenOffCooldown;
            getAbility(ability.id).waitingForCdResponse = false;
            Debug.Log("User got a new cooldown on this ability untill can use again : " + ability.name + " CD : " + ability.timeWhenOffCooldown);
        }
    }

    public Ability getAbility(int id) {
        foreach (var ability in abilities) {
            if (ability.id == id) {
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
    public int getAbilityIdByAbilityName(string name)
    {
        foreach (var ability in abilities)
        {
            if (ability.image == name)
            {
                return ability.id;
            }
        }
        return 9999;
    }
    public Ability getAbilityByAbilityName(string name)
    {
        foreach (var ability in abilities)
        {
            if (ability.image == name)
            {
                return ability;
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
        int minionId = getMyHero().targetEnemy;
        getCommunication().sendAutoAttack(minionId);
    }

    public Hero getMyHero() {
        if (getLobbyCommunication() != null && getLobbyCommunication().heroId > 0) {
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

	public bool isGameMode(int gameModeToCheck){
		if(world != null && world.worldType == gameModeToCheck){
			return true;
		}
		return false;
	}

    public void teleportHeroes(List<Hero> teleportHeroes) {
        foreach (var heroInList in teleportHeroes) {
            Hero hero = getHero(heroInList.id);
            hero.desiredPositionX = heroInList.desiredPositionX;
            hero.desiredPositionZ = heroInList.desiredPositionZ;
            hero.desiredPositionY = heroInList.desiredPositionY;
            hero.positionX = heroInList.desiredPositionX;
            hero.positionZ = heroInList.desiredPositionZ;
            hero.positionY = heroInList.desiredPositionY;
            Vector3 newPosition = new Vector3(hero.positionX, hero.positionY, hero.positionZ);
            if (hero.id == getMyHero().id) {
                CharacterAnimations heroAnimation = (CharacterAnimations)hero.trans.GetComponent(typeof(CharacterAnimations));
                heroAnimation.targetPosition = new Vector3(heroInList.desiredPositionX, heroInList.desiredPositionY, heroInList.desiredPositionZ);
            }
            hero.trans.position = newPosition;
            Debug.Log("Moved hero: " + hero.id + " to position : " + newPosition);
        }
    }

    public void createWorld(ResponseWorld responseWorld)
    {
        Debug.Log("Server sent to create world");
        world = responseWorld.world;

        if (!getLobbyCommunication().local)
        {
            getGenerator().GenerateRandom(world.seed, world.worldType);
        }

        getTestSpawn().startJobForSpawnPoints();

		if(isGameMode(World.HORDE_MODE)){
			StartCoroutine("startCheckIfLevelIsComplete");
		}
    }



	IEnumerator startCheckIfLevelIsComplete() {
		yield return new WaitForSeconds(5.0f);
		getHordeMode().currentMode = true;
        getNotificationhandler().startHordeMode();
		Debug.Log ("Current game mode is horde!");
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



    FriendlyFrames getPartyFrame()
    {
        return ((FriendlyFrames)GameObject.Find("GameLogicObject").GetComponent(typeof(FriendlyFrames)));
    }

    DunGenerator getGenerator() {
        return ((DunGenerator)GameObject.Find("Generator").GetComponent(typeof(DunGenerator)));
    }

    SpawnLocator getTestSpawn() {
        return ((SpawnLocator)GameObject.Find("GameLogicObject").GetComponent(typeof(SpawnLocator)));
    }

	HordeMode getHordeMode() {
		return ((HordeMode)GameObject.Find("GameLogicObject").GetComponent(typeof(HordeMode)));
	}

    NotificationHandler getNotificationhandler()
    {
        return ((NotificationHandler)GameObject.Find("GameLogicObject").GetComponent(typeof(NotificationHandler)));
    }

    Cooldown GetCooldown()
    {
        return ((Cooldown)GameObject.Find("GameLogicObject").GetComponent(typeof(Cooldown)));
    }
}