using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Assets.scripts.vo
{
    public class Obstacle
    {
        public static int WALL = 1;

        public float positionX;
        public float positionY;
        public float positionZ;
        public float rotation;
        public int type;
        public Transform transform;
    }
}