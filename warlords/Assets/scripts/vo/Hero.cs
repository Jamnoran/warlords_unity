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
        public List<ResponseHeroBuff> buffs = new List<ResponseHeroBuff>();
        public List<Talent> talents;
        private int totalTalentPoints = 0;

        public void update() {
            if (buffs != null && buffs.Count > 0) {
                foreach (var buff in buffs) {
                    if ((buff.millisBuffStarted + buff.durationMillis) < DeviceUtil.getMillis()) {
                        int type = buff.type;
                        buffs.Remove(buff);
                        if (type == Buff.SPEED) {
                            calculateSpeed();
                        }
                    }
                }
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
