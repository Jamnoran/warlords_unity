using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.vo
{
    [System.Serializable]
    public class Buff {
        public static int SPEED = 1;
	    public static int SHIELD = 2;
        public static int DOT = 3;

	    public int heroId = 0;
        public int target = 0;
        public int type = 0;
        public int value = 0;
        public int duration = 0;
        public string tickTime = "";
        public int ticks = 0;
        public long millisBuffStarted = 0;
    }
}