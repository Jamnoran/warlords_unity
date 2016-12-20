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
        public int hp;
        public int maxHp;
        public float positionX;
        public float positionZ;
        public float desiredPositionX;
        public float desiredPositionZ;
        public Transform minionTransform;
        public float hightOfTerrain = 2;
        public int heroTarget = 0;
        
        internal void setTransform(Transform minTransform)
        {
            minionTransform = minTransform;
        }

        public Vector3 getDesiredPosition()
        {
            //Debug.Log("Returning new position : " + desiredPositionX + " x " + desiredPositionZ);
            return new Vector3(desiredPositionX, hightOfTerrain, desiredPositionZ);
        }

        public Vector3 getTransformPosition()
        {
            return new Vector3(minionTransform.position.x, minionTransform.position.y, minionTransform.position.z);
        }

        public void setHp(int newHp)
        {
            hp = newHp;
            if (minionTransform != null)
            {
                ((HealthUpdate)minionTransform.GetComponent(typeof(HealthUpdate))).setCurrentVal(hp);
            }
        }

        public void initBars()
        {
            ((HealthUpdate)minionTransform.GetComponent(typeof(HealthUpdate))).setMaxValue(maxHp);
        }
    }
}