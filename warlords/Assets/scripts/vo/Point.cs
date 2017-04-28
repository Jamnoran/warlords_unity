using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.scripts.vo {
    public class Point {
        public static int SPAWN_POINT = 1;
        public static int ENEMY_POINT = 2;

        private Vector3 location;
        public float posX;
        public float posZ;
        public int pointType;
        public bool used = false;

        public Point() {
        }

        public Point(Vector3 loc, int type) {
            posX = loc.x;
            posZ = loc.z;
            location = loc;
            pointType = type;
        }

        public float getPosX() {
            return posX;
        }

        public float getPosZ() {
            return posZ;
        }

        public Vector3 getLocation() {
            return location;
        }

        public void setLocation(Vector3 location) {
            this.location = location;
        }

        public int getPointType() {
            return pointType;
        }

        public void setPointType(int pointType) {
            this.pointType = pointType;
        }

        public bool isUsed() {
            return used;
        }

        public void setUsed(bool used) {
            this.used = used;
        }
        
    }
}
