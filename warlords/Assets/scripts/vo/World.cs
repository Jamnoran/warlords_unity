using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.scripts.vo
{
    public class World
    {
        public int worldLevel = 1;
        public int sizeX = 100;
        public int sizeZ = 100;
        public int worldType = 1; 
        public List<Point> spawnPoints = new List<Point>();
        public int seed = 0;

		public static int DUNGEON_CRAWLER = 1;
		public static int HORDE_MODE = 2;
		public static int GAUNTLET = 3;

    }
}