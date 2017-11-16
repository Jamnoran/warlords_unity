using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.vo
{
    public class Minion
    {
        public int id;
        public int level;
        public float hp;
        public float maxHp;
        public float positionX;
        public float positionZ;
        public float positionY;
        public float desiredPositionX;
        public float desiredPositionZ;
        public float desiredPositionY;
        public Transform minionTransform;
        public int heroTarget = 0;
        public bool alive = true;
        public int minionType = 1;
        public List<Buff> buffs = new List<Buff>();
        public List<Buff> deBuffs = new List<Buff>();

        internal void setTransform(Transform minTransform)
        {
            minionTransform = minTransform;
        }

        public Vector3 getDesiredPosition()
        {
            //Debug.Log("Returning new position : " + desiredPositionX + " x " + desiredPositionZ);
            return new Vector3(desiredPositionX, desiredPositionY, desiredPositionZ);
        }

        public Vector3 getTransformPosition()
        {
            return new Vector3(minionTransform.position.x, minionTransform.position.y, minionTransform.position.z);
        }

        public void setHp(float newHp)
        {
            hp = newHp;
            if(hp <= 0){
                alive = false;
            }
            if (minionTransform != null)
            {
                ((HealthUpdate)minionTransform.GetComponent(typeof(HealthUpdate))).setCurrentVal(hp);
            }
        }

        public void initBars()
        {
            ((HealthUpdate)minionTransform.GetComponent(typeof(HealthUpdate))).setMaxValue(maxHp);
        }

        public void setAlive(bool v)
        {
            alive = v;
        }
    }
}