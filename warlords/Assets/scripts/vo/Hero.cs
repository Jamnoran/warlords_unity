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
        public float desiredPositionX;
        public float desiredPositionZ;
        public int targetEnemy = 0;
        public int targetFriendly = 0;
        public Transform trans;
        private bool autoAttacking = false;

    
        public void setTrans(Transform transf)
        {
            trans = transf;
        }

        public Vector3 getTargetPosition()
        {
            return new Vector3(desiredPositionX, 0, desiredPositionZ);
        }
        public Vector3 getDesiredPosition()
        {
            return new Vector3(desiredPositionX, 0, desiredPositionZ);
        }

        public Vector3 getTransformPosition()
        {
            return new Vector3(trans.position.x, trans.position.y, trans.position.z);
        }

        public String getModelName()
        {
            if (class_type == "WARRIOR") {
                return "Nordstrom";
            } else if (class_type == "PRIEST")
            {
                return "Kachujin";
            }
            return null;
        }

        public void setHp(int newHp)
        {
            hp = newHp;


            String heroid = getCommunication().getHeroId();
            bool ownHero = true;
            if (id != Int32.Parse(heroid))
            {
                ownHero = false;
            }
            updateHealthBar(ownHero);
        }

        public void updateHealthBar(bool ownHero)
        {
            if (trans != null)
            {
                if (ownHero)
                {
                    ((HealthUpdate)GameObject.Find("Canvas").GetComponent(typeof(HealthUpdate))).setCurrentVal(hp);
                }
            }
        }

        public void initBars()
        {
            ((HealthUpdate)GameObject.Find("Canvas").GetComponent(typeof(HealthUpdate))).setMaxValue(maxHp);
            setHp(maxHp);
        }

        public bool getAutoAttacking()
        {
            return autoAttacking;
        }

        public void setAutoAttacking(bool value)
        {
            autoAttacking = value;
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
}
