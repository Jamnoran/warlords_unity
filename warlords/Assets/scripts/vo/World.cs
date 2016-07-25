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
        public int worldType = 1; // 1 = normal, 2 = time limit, 3 = horde, 4 = tanky? 5 = boss
        public List<Vector3> spawnPoints = new List<Vector3>();
        public List<Obstacle> obstacles = new List<Obstacle>();


    }
}