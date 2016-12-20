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
        public int hp;
        public int maxHp;
        public int resource;
        public int maxResource;
        public string class_type;
        public float positionX;
        public float positionZ;
        public float desiredPositionX;
        public float desiredPositionZ;
        public int targetEnemy = 0;
        public int targetFriendly = 0;
        public Transform trans;

    
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
            if (trans != null)
            {
                ((HealthUpdate)GameObject.Find("Canvas").GetComponent(typeof(HealthUpdate))).setCurrentVal(hp);
                ((HealthUpdate)trans.GetComponent(typeof(HealthUpdate))).setCurrentVal(hp);
            }
        }

        public void initBars()
        {
            ((HealthUpdate)trans.GetComponent(typeof(HealthUpdate))).setMaxValue(maxHp);
        }

    }
}
