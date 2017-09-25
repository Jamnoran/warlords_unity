using Assets.scripts.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.vo
{
    public class Hero
    {
        public int id;
        public int user_id;
        public int xp;
        public int level;
        public int xpForLevel;
        public int topGameLvl;
        public int hp;
        public int maxHp;
        public int resource;
        public int maxResource;
        public bool alive = true; 
        public string class_type;
        public float positionX;
        public float positionZ;
        public float positionY;
        public float desiredPositionX;
        public float desiredPositionZ;
        public float desiredPositionY;
        public int targetEnemy = 0;
        public int targetFriendly = 0;
        public Transform trans;
        private bool autoAttacking = false;
        public float attackRange = 3.0f;
        public float baseMoveSpeed = 3.0f;
        public List<Buff> buffs = new List<Buff>();
        public List<Buff> deBuffs = new List<Buff>();
        public List<Talent> talents;
        private int totalTalentPoints = 0;

        public Hero() {

        }


        public Hero(string classType) {
            class_type = classType;
        }

        public void update() {
            ShieldLogic shieldLogic = (ShieldLogic)trans.GetComponent(typeof(ShieldLogic));
            if (buffs != null && buffs.Count > 0) 
            {
                for (int i = 0; i < buffs.Count; i++) 
                {
                    Buff buff = buffs[i];
                    //if ((buff.millisBuffStarted + buff.duration) < DeviceUtil.getMillis())
                    //{
                    //    buffs.Remove(buff);
                    //    int type = buff.type;
                    //    if (type == Buff.SPEED) 
                    //    {
                    //        calculateSpeed();
                    //    }else if (type == Buff.SHIELD)
                    //    {
                    //        Debug.Log("Removing shield for hero " + id);
                    //        shieldLogic.setVisibility(false);
                    //    }
                    //}
                }
            } else 
            {
                // Remove all buffs if they are visible

                // Handle shield
                if (shieldLogic != null && shieldLogic.shieldOn) {
                    shieldLogic.setVisibility(false);
                }

                // Handle speed buff
                calculateSpeed();
            }
        }

        public void setTotalTalentPoints(int points) {
            totalTalentPoints = points;
        }

        public int getTotalTalentPoints() {
            return totalTalentPoints;
        }

        public void setTrans(Transform transf) {
            trans = transf;
        }

        public Vector3 getTargetPosition() {
            return new Vector3(desiredPositionX, desiredPositionY, desiredPositionZ);
        }

        public Vector3 getDesiredPosition() {
            return new Vector3(desiredPositionX, desiredPositionY, desiredPositionZ);
        }

        public Vector3 getTransformPosition() {
            return new Vector3(trans.position.x, trans.position.y, trans.position.z);
        }

        public String getModelName() {
            if (class_type == "WARRIOR") {
                return "Nordstrom";
            } else if (class_type == "PRIEST")
            {
                return "Kachujin";
            }
            return null;
        }

        public void setHp(int newHp) {
            hp = newHp;

            int heroid = getLobbyCommunication().heroId;
            bool ownHero = true;
            if (id != heroid) {
                ownHero = false;
            }
            updateHealthBar(ownHero);
            if (hp <= 0)
            {
                getAnimation().setAlive(false);
            }
        }

        public void updateHealthBar(bool ownHero) {
            if (trans != null) {
                if (ownHero) {
                    ((HealthUpdate)GameObject.Find("Canvas").GetComponent(typeof(HealthUpdate))).setCurrentVal(hp);
                }
            }
        }

        public void initBars() {
            ((HealthUpdate)GameObject.Find("Canvas").GetComponent(typeof(HealthUpdate))).setMaxValue(maxHp);
            setHp(maxHp);
            if (class_type == "WARLOCK")
            {
                GameObject manaGlobe = GameObject.Find("mana globe");
                manaGlobe.SetActive(false);
            }
        }

        public void calculateSpeed() {
            float newCalculatedMoveSpeed = baseMoveSpeed;
            if (buffs != null && buffs.Count > 0) {
                foreach (var buff in buffs) {
                    if (buff.type == Buff.SPEED) {
                        newCalculatedMoveSpeed = newCalculatedMoveSpeed + buff.value;
                    }
                }
            }
            ((CharacterAnimations) trans.GetComponent(typeof(CharacterAnimations))).moveSpeed = newCalculatedMoveSpeed;
        }


        public bool getAutoAttacking()
        {
            return autoAttacking;
        }

        public void setAutoAttacking(bool value) {
            autoAttacking = value;
        }


        ServerCommunication getCommunication() {
            return ((ServerCommunication)GameObject.Find("Communication").GetComponent(typeof(ServerCommunication)));
        }

        LobbyCommunication getLobbyCommunication() {
            return ((LobbyCommunication)GameObject.Find("Communication").GetComponent(typeof(LobbyCommunication)));
        }
        CharacterAnimations getAnimation(){
            return (CharacterAnimations) trans.GetComponent(typeof(CharacterAnimations));
        }

    }
}
