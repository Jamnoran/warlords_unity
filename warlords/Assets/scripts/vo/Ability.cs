using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.scripts.vo
{
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

        public bool isReady() {
            if (waitingForCdResponse || (timeWhenOffCooldown != null && !timeWhenOffCooldown.Equals("") && (long.Parse(timeWhenOffCooldown) >= getMillis()))) {
                return false;
            }
            return true;
        }


        private long getMillis() {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(DateTime.UtcNow - epochStart).TotalMilliseconds;
        }

    }
}
