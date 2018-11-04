using UnityEngine;
using System.Collections;
using Assets.scripts.vo;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using Assets.scripts.util;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    //list of minions currently alive in our universe.
    private List<Minion> minions = new List<Minion>();
    private List<Hero> heroes = new List<Hero>();

    //list of minions targeted by aoe
    private List<Minion> aoeMinions = new List<Minion>();

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
    // Priest
    public Transform healAnimation;
    public Transform smiteAnimation;
    public Transform shieldAnimation;
    // Warrior
    public Transform tauntAnimation;
    public Transform cleaveAnimation;
    // Warlock
    public Transform drainAnimation;
    public Transform hemmorageAnimation;
    public Transform restoreAnimation;
    public Transform bloodBoltAnimation;
    public Transform rainOfDeathAnimation;

    private List<Ability> abilities = null;
    public List<Item> heroItems = new List<Item>();
    public bool isInGame = false;
    public World world;
    private int thisHeroId;

    void Start() {
        Debug.Log("Game logic has started");
        if ((GameObject.Find("Communication")) == null){
            Debug.Log("Go to connect screen.");
            SceneManager.LoadScene("Connect", LoadSceneMode.Additive);
        }
        else
        {
            try
            {
                thisHeroId = getLobbyCommunication().heroId;
            }
            catch (Exception e)
            {

                throw new Exception("Could not load lobbycommunication: " + e);
            }
        }
		if (getGenerator() != null) {
			getGenerator().setReady (true);
		}

        GameSettings settings = new GameSettings();
        settings.loadAllSettings();
    }

    // Update is called once per frame
    void Update() {
        if (heroes != null && heroes.Count > 0) {
            foreach (var hero in heroes) {
                hero.update();
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

    public void sendEquipment(List<Item> updatedItems)
    {
        //Debug.Log("Sending these items to server");
        foreach(Item item in updatedItems)
        {
            //Debug.Log("Name: " + item.name + " Equiped: " + item.equipped);
            getCommunication().updateItem(thisHeroId, item.id, item.positionId, item.equipped);
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


    public List<Item> getHeroItems()
    {
        return heroItems;
    }
    
    public Item getHeroItemById(int id)
    {
        foreach (var item in heroItems)
        {
            if (id == item.id)
            {
                return item;
            }
        }
        return null;
    }

    public void AddAoeMinion(Transform minionTransform)
    {
        try
        {
            Minion minion = getMinionByTransform(minionTransform);
            if (!aoeMinions.Contains(minion))
            {
                aoeMinions.Add(minion);
                Debug.Log("Minion added to list, minions in cone: " + aoeMinions.Count);
            }
        }
        catch (Exception e)
        {

            throw new Exception("Could not add minion to list: " + e);
        }
      
    }

    public void RemoveAoeMinion(Transform minionTransform)
    {
        try
        {
            Minion minion = getMinionByTransform(minionTransform);
            if (aoeMinions.Contains(minion))
            {
                aoeMinions.Remove(minion);
                Debug.Log("Minion removed from list, minions in cone: " + aoeMinions.Count);
            }
        }
        catch (Exception e)
        {

            throw new Exception("Could not remove minion from list: " + e);
        }
       
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
                        //Debug.Log("Minions new hp = " + minion.hp);
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

        UpdatePartyFrames();
    }

    private void UpdatePartyFrames()
    {
        //heroes.RemoveAll(hero => hero.id == thisHeroId);
        if (getPartyFrame() != null)
        {
            getPartyFrame().UpdatePartyFrames(getHeroesWithoutSelf());
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
                hero.xpForLevel = newHero.xpForLevel;
                hero.setLevel(newHero.level);
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
                else
                {
                    // Logic for own hero
                    // Update xp bar
                    getXpBar().fillAmount = ((float)hero.xp / (float)hero.xpForLevel);
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

    public void updateHeroItems(List<Item> newItems)
    {
        heroItems = newItems;

    }

    public void combatText(ResponseCombatText responseCombatText)
    {
        if (responseCombatText.friendly)
        {
            Hero hero = getHero(responseCombatText.idOfTarget);
            Transform sctObject = hero.trans.Find("3dmodel/FriendlyCanvas/frame/SCTpoint");
            ScrollingCombatText sct = (ScrollingCombatText)sctObject.GetComponent(typeof(ScrollingCombatText));
            sct.showText(responseCombatText.amount, responseCombatText.crit, responseCombatText.color);
        }
        else
        {
            Minion minion = getMinion(responseCombatText.idOfTarget);
            Transform sctObject = minion.minionTransform.Find("mob/EnemyCanvas/frame/SCTpoint");
            ScrollingCombatText sct = (ScrollingCombatText)sctObject.GetComponent(typeof(ScrollingCombatText));
            sct.showText(responseCombatText.amount, responseCombatText.crit, responseCombatText.color);
        }
    }

    public void rotateTarget(ResponseRotateTarget responseRotateTarget)
    {
        CharacterAnimations anim = (CharacterAnimations)getMyHero().trans.GetComponent(typeof(CharacterAnimations));
        if (responseRotateTarget.getTargetPosition() != null && responseRotateTarget.getTargetPosition().x != 0 && responseRotateTarget.getTargetPosition().z != 0)
        {
            anim.rotateToTarget(responseRotateTarget.getTargetPosition());
        }
        else if (responseRotateTarget.isFriendly() && responseRotateTarget.getIdOfTarget() > 0)
        {
            anim.rotateToTarget(getHero(responseRotateTarget.getIdOfTarget()).getTransformPosition());
        }
        else
        {
            //anim.rotateToTarget(getMinion(responseRotateTarget.getIdOfTarget()).getTransformPosition());
            Debug.Log("Rotate to transform");
            if(anim != null && responseRotateTarget != null && getMinion(responseRotateTarget.getIdOfTarget()) != null){
                anim.rotateToTransform(getMinion(responseRotateTarget.getIdOfTarget()).minionTransform);
            }
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
                //Debug.Log("Buff speed! Calculating new speed");
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
            if (ability.calculatedCastTime > 0)
            {
                getCastbar().showSpell(ability);
            }
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
        float heroHeightStartPosition = 300.0f;
        Transform heroTransform = (Transform)Instantiate(prefabToUse, new Vector3(newHero.desiredPositionX, newHero.desiredPositionY + heroHeightStartPosition, newHero.desiredPositionZ), Quaternion.identity);
        heroTransform.name = prefabToUse.name;
        newHero.setTrans(heroTransform);
        if (newHero.id == heroid)
        {
            Debug.Log("Setting hero id: " + newHero.id + " To my own hero");
            ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).isMyHero = true;
            newHero.updateHealthBar(true);
            getKeyboardInput().setUpSplats(heroTransform);
        }
        ((clickToMove)heroTransform.GetComponent(typeof(clickToMove))).heroId = newHero.id;

        HeroInfo heroInfo = (HeroInfo)heroTransform.GetComponent(typeof(HeroInfo));
        heroInfo.setHeroId(newHero.id);

        heroes.Add(newHero);
        //set initial health for hero
        newHero.initBars();
    }

    internal void setTalents(ResponseTalents response) {
        getMyHero().talents = response.talents;
        getMyHero().setTotalTalentPoints(response.totalTalentPoints);
    }

    public void updateAnimations(List<GameAnimation> gameAnimations) {
        foreach (var gameAnimation in gameAnimations) {
            if (gameAnimation.animation_type == "MINION_DIED") {
                Minion minion = getMinion(gameAnimation.target_id);

                ((HealthUpdate)minion.minionTransform.GetComponent(typeof(HealthUpdate))).hideBar();

                ((FieldOfViewMinion)minion.minionTransform.Find("mob").GetComponent(typeof(FieldOfViewMinion))).enabled = false;
                // Disable capsle collider + Character animation
                ((CapsuleCollider)minion.minionTransform.GetComponent(typeof(CapsuleCollider))).enabled = false;
                ((CharacterController)minion.minionTransform.GetComponent(typeof(CharacterController))).enabled = false;

                //Destroy(minion.minionTransform.gameObject);
                MinionAnimations anim = (MinionAnimations)minion.minionTransform.GetComponent(typeof(MinionAnimations));
                anim.deadAnimation();
                minion.setAlive(false);

                Transform collider = minion.minionTransform.Find("minion_rigidbody");
                collider.gameObject.SetActive(false);

                if (getMyHero().targetEnemy == minion.id)
                {
                    deselectTarget(minion.id, false);
                }

                // This is wrong, shouldnt do it while in loop (find out correct way to do it) normally done by an iterator but not sure how to do it in c#
                //minions.Remove(minion);
                foreach (Hero hero in heroes)
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

            handleAnimation(gameAnimation);
        }
    }

    private void handleAnimation(GameAnimation gameAnimation)
    {
        //Debug.Log("Got animation " + gameAnimation.animation_type);
        Hero source = getHero(gameAnimation.source_id);
        //handleSpellPositioningAndRotation(Transform prefab, GameAnimation gameAnimation, bool followTarget, bool lookAt, bool spawnSource, bool friendly)
        // Priest
        if (gameAnimation.animation_type == "HEAL")
        {
            Debug.Log("Heal animation");
            handleSpellPositioningAndRotation(healAnimation, gameAnimation, true, true,true,true, false);
        }
        if (gameAnimation.animation_type == "SMITE")
        {
            Debug.Log("Smite animation");
            CharacterAnimations anim = (CharacterAnimations)source.trans.GetComponent(typeof(CharacterAnimations));
            anim.spellAnimation(gameAnimation.spellAnimationId);
        }
        else if (gameAnimation.animation_type == "SMITE_CAST")
        {
            Debug.Log("Smite cast animation");
            handleSpellPositioningAndRotation(smiteAnimation, gameAnimation, true, true, true, false, false);
        }

        // Warrior
        if (gameAnimation.animation_type == "TAUNT")
        {
            Debug.Log("Taunt animation");
            Transform animationTransform = handleSpellPositioningAndRotation(tauntAnimation, gameAnimation, true, false, true, false, true);
            SpellCollissionListener listener = animationTransform.GetComponentInChildren<SpellCollissionListener>();
            listener.setAbility(getAbilityByAbilityName(gameAnimation.animation_type));
        }
        if (gameAnimation.animation_type == "CLEAVE")
        {
            Debug.Log("Cleave animation");
            Transform animationTransform = handleSpellPositioningAndRotation(cleaveAnimation, gameAnimation, true, false, true, false, true);
            SpellCollissionListener listener = animationTransform.GetComponentInChildren<SpellCollissionListener>();
            listener.setAbility(getAbilityByAbilityName(gameAnimation.animation_type));
        }

        // Warlock
        if (gameAnimation.animation_type == "DRAIN")
        {
            Debug.Log("Drain life animation");
            CharacterAnimations anim = (CharacterAnimations)source.trans.GetComponent(typeof(CharacterAnimations));
            anim.spellAnimation(gameAnimation.spellAnimationId);
            handleSpellPositioningAndRotation(drainAnimation, gameAnimation, false, true, true, false, false);
        }
        if (gameAnimation.animation_type == "BLOOD_BOLT")
        {
            Debug.Log("Blood bolt animation");
            CharacterAnimations anim = (CharacterAnimations)source.trans.GetComponent(typeof(CharacterAnimations));
            anim.spellAnimation(gameAnimation.spellAnimationId);
            handleSpellPositioningAndRotation(bloodBoltAnimation, gameAnimation, false, true, true, false, false);
        }
        if (gameAnimation.animation_type == "HAEMORRHAGE")
        {
            Debug.Log("HAEMORRHAGE animation");
            handleSpellPositioningAndRotation(hemmorageAnimation, gameAnimation, true, false, false, false, false);
        }
        if (gameAnimation.animation_type == "RESTORE")
        {
            Debug.Log("Restore animation");
            handleSpellPositioningAndRotation(restoreAnimation, gameAnimation, false, true, true, true, false);
        }
    }

    // prefab: Prefab to animate
    // gameanimation: request to handle
    private Transform handleSpellPositioningAndRotation(Transform prefab, GameAnimation gameAnimation, bool followTarget, bool lookAt, bool spawnSource, bool friendly, bool towardsPosition)
    {
        Debug.Log("GameAnimation target : " + gameAnimation.target_id + " source: " + gameAnimation.source_id);
        Transform animationTransform = null;
        Hero source = getHero(gameAnimation.source_id);
        Transform target = null;
        Vector3 targetPos = new Vector3();
        if (!towardsPosition) {
            if (friendly)
            {
                target = getHero(gameAnimation.target_id).trans;
                targetPos = getHero(gameAnimation.target_id).getTransformPosition();
            }
            else
            {
                target = getMinion(gameAnimation.target_id).minionTransform;
                targetPos = getMinion(gameAnimation.target_id).getTransformPosition();
            }
            if (spawnSource)
            {

                animationTransform = Instantiate(prefab, source.getPosition(), Quaternion.identity);
            }
            else if (!spawnSource && !towardsPosition)
            {
                animationTransform = Instantiate(prefab, targetPos, Quaternion.identity);
            }
        }
        else
        {
            animationTransform = Instantiate(prefab, source.getPosition(), source.getRotation());
        }
      
        FollowGameObject followGameOjbect = ((FollowGameObject)animationTransform.GetComponent(typeof(FollowGameObject)));
        if (followTarget)
        {
            followGameOjbect.setObjectToFollow(target);
        }
        if (lookAt)
        {
            followGameOjbect.setObjectToLookAt(target);
        }
        if (towardsPosition)
        {
            Debug.Log("Setting position to Look at");
            followGameOjbect.setPositionToLookAt(new Vector3(gameAnimation.position_x, gameAnimation.position_y, gameAnimation.position_z));
        }
        return animationTransform;
    }

    public void stopHero(int heroId) {
        //Debug.Log("Stopping hero.");
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

    public List<Hero> getHeroesWithoutSelf()
    {
        
        List<Hero> heroesWithoutSelf = new List<Hero>();
        foreach(Hero hero in heroes)
        {
            if (hero.id != getLobbyCommunication().heroId)
            {
                heroesWithoutSelf.Add(hero);
            }
        }
        return heroesWithoutSelf;
    }

    public void setHeroTargetEnemy(int minionId) {
        Hero myHero = getMyHero();
        if (myHero != null) {
            if (myHero.targetEnemy > 0) {
                deselectTarget(myHero.targetEnemy, false);
            }
            if (myHero.targetFriendly > 0)
            {
                deselectTarget(myHero.targetFriendly, true);
            }
            if (minionId > 0)
            {
                myHero.targetFriendly = 0;
            }
            myHero.targetEnemy = minionId;
        }
    }


    public void setHeroTargetFriendly(int heroId) {
        Hero myHero = getMyHero();
        if (myHero != null)
        {
            if (myHero.targetEnemy > 0)
            {
                deselectTarget(myHero.targetFriendly, true);
            }
            if (myHero.targetFriendly > 0)
            {
                deselectTarget(myHero.targetFriendly, true);
            }
            if (heroId > 0)
            {
                myHero.targetEnemy = 0;
            }
            myHero.targetFriendly = heroId;
        }
    }

    private void deselectTarget(int targetId, bool friendly)
    {
        if (friendly)
        {
            Hero hero = getHero(targetId);
            if (hero != null)
            {
                HeroInfo heroInfo = hero.trans.GetComponent<HeroInfo>();
                heroInfo.setSelected(false);
            }
        }
        else
        {
            Minion min = getMinion(targetId);
            if (min != null) {
                MinionInfo minInfo = min.minionTransform.GetComponent<MinionInfo>();
                minInfo.setSelected(false);
            }
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


    public void sendSpell(int spellId, List<int> enemies, List<int> friendly)
    {
        Hero myHero = getMyHero();
        sendSpell(spellId, enemies, friendly, myHero.getTargetPosition(), true);
    }

    public void sendSpell(int spellId, List<int> enemies, List<int> friendly, Vector3 targetPosition, bool initialCast)
    {
        Debug.Log("Send spell " + spellId);
        if (isMyHeroAlive())
        {
            Hero myHero = getMyHero();
            getCommunication().sendStopHero(myHero.id);
            if (SpellUtil.getSpell(spellId).initialCast && initialCast)
            {
                getCommunication().sendSpell(myHero.id, spellId, enemies, friendly, targetPosition, true);
            }
            else
            {
                getCommunication().sendSpell(myHero.id, spellId, enemies, friendly, targetPosition);
            }
            
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
    }

    public void updateCooldown(Ability ability) {
        if (updateAbility(ability))
        {
            Debug.Log("User got a new cooldown on this ability untill can use again : " + ability.name + " CD : " + ability.timeWhenOffCooldown);
            getCastbar().showSpell(getAbility(ability.id));
        }
    }

    public bool updateAbility(Ability ability)
    {
        if(getAbility(ability.id) != null) { 
            getAbility(ability.id).timeWhenOffCooldown = ability.timeWhenOffCooldown;
            getAbility(ability.id).waitingForCdResponse = false;
            getAbility(ability.id).calculatedCastTime = ability.calculatedCastTime;
            return true;
        }
        return false;
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
            if (ability.image.ToLower() == name.ToLower())
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
            //Debug.Log("Moved hero: " + hero.id + " to position : " + newPosition);
        }
    }

    public void createWorld(ResponseWorld responseWorld)
    {
        if (getLoading() != null)
        {
            getLoading().showLoading();
        }
        world = responseWorld.world;

        getHideWalls().clearHiddenObjects();

		if (!getLobbyCommunication().local && getGenerator() != null)
        {
            getGenerator().GenerateRandom(world.seed, world.worldType);
        }

        getTestSpawn().startJobForSpawnPoints();

		if(isGameMode(World.HORDE_MODE))
        {
			StartCoroutine("hordeLogicStart");
        }
        else if (isGameMode(World.GAUNTLET))
        {
            StartCoroutine("gauntletLogicStart");
        }
        else
        {
            getNotificationhandler().setHordeMode(false);
            getNotificationhandler().showNotification(1, "");
            getGauntletMode().setMode(false);
            StartCoroutine("crawlerLogicStart");
        }
    }

    IEnumerator crawlerLogicStart()
    {
        yield return new WaitForSeconds(2.5f);
        getLoading().hideLoading();
    }

    IEnumerator hordeLogicStart()
    {
		yield return new WaitForSeconds(1.0f);
		getHordeMode().currentMode = true;
        getGauntletMode().setMode(false);
        getNotificationhandler().setHordeMode(true);
		Debug.Log ("Current game mode is horde!");
        getLoading().hideLoading();
    }

    IEnumerator gauntletLogicStart()
    {
        yield return new WaitForSeconds(1.0f);
        getGauntletMode().startTimer();
        getHordeMode().currentMode = false;
        getGauntletMode().setMode(true);
        getNotificationhandler().setHordeMode(false);
        Debug.Log("Current game mode is gauntlet!");
        getLoading().hideLoading();
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
		if (GameObject.Find ("Generator") != null) {
			return ((DunGenerator)GameObject.Find ("Generator").GetComponent (typeof(DunGenerator)));
		}
		return null;
    }

    SpawnLocator getTestSpawn() {
        return ((SpawnLocator)GameObject.Find("GameLogicObject").GetComponent(typeof(SpawnLocator)));
    }

	HordeMode getHordeMode() {
		return ((HordeMode)GameObject.Find("GameLogicObject").GetComponent(typeof(HordeMode)));
	}

    GauntletMode getGauntletMode()
    {
        return ((GauntletMode)GameObject.Find("GameLogicObject").GetComponent(typeof(GauntletMode)));
    }

    NotificationHandler getNotificationhandler()
    {
        return ((NotificationHandler)GameObject.Find("GameLogicObject").GetComponent(typeof(NotificationHandler)));
    }

    Cooldown GetCooldown()
    {
        return ((Cooldown)GameObject.Find("GameLogicObject").GetComponent(typeof(Cooldown)));
    }

    CastBar getCastbar()
    {
        return ((CastBar)GameObject.Find("GameLogicObject").GetComponent(typeof(CastBar)));
    }

    HideWalls getHideWalls()
    {
        return ((HideWalls)GameObject.Find("GameLogicObject").GetComponent(typeof(HideWalls)));
    }

    LoadingHandler getLoading()
    {
        return ((LoadingHandler)GameObject.Find("GameLogicObject").GetComponent(typeof(LoadingHandler)));
    }

    KeyboardInput getKeyboardInput()
    {
        return ((KeyboardInput)GameObject.Find("GameLogicObject").GetComponent(typeof(KeyboardInput)));
    }


    UIProgressBar getXpBar()
    {
        GameObject canvas = GameObject.Find("Canvas/Action Bar/XP Bar");
        return ((UIProgressBar) canvas.GetComponent(typeof(UIProgressBar)));
    }
}