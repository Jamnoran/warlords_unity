using Assets.scripts.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.vo
{
    [System.Serializable]
    public class Ability
    {
        public int id;
        public String classType;
        public String name;
        public int levelReq;
        public String damageType;
        public String description;
        public String image;
        public int baseDamage;
	    public int topDamage;
        public int value;
        public int crittable;
        public String targetType;
        public int baseCD;
        public String timeWhenOffCooldown = "0";
        public bool waitingForCdResponse = false;
        public int position = 0;
        public int castTime = 0;
        public int calculatedCastTime = 0;
        public bool isCasting = false;
        public int resourceCost = 0;
        public float range;
        public bool initialCast = false;

        public bool isReady() {
            if ((timeWhenOffCooldown != null && !timeWhenOffCooldown.Equals("") && (long.Parse(timeWhenOffCooldown) >= DeviceUtil.getMillis()))) {
                return false;
            }
            return true;
        }


    }
}
