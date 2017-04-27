using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace DunGen
{
	public static class UnityUtil
	{
		public static void Destroy(UnityEngine.Object obj)
		{
			if (Application.isPlaying)
				UnityEngine.Object.Destroy(obj);
			else
				UnityEngine.Object.DestroyImmediate(obj);
		}

		public static string GetUniqueName(string name, IEnumerable<string> usedNames)
		{
			if(string.IsNullOrEmpty(name))
				return GetUniqueName("New", usedNames);
			
			string baseName = name;
			int number = 0;
			bool hasNumber = false;

			int indexOfLastSeperator = name.LastIndexOf(' ');

			if(indexOfLastSeperator > -1)
			{
				baseName = name.Substring(0, indexOfLastSeperator);
				hasNumber = int.TryParse(name.Substring(indexOfLastSeperator + 1), out number);
				number++;
			}

			foreach(var n in usedNames)
			{
				if(n == name)
				{
					if(hasNumber)
						return GetUniqueName(baseName + " " + number.ToString(), usedNames);
					else
						return GetUniqueName(name + " 2", usedNames);
				}
			}
			
			return name;
		}

		public static Bounds CombineBounds(params Bounds[] bounds)
		{
			if (bounds.Length == 0)
				return new Bounds();
			else if (bounds.Length == 1)
				return bounds[0];

			Bounds combinedBounds = bounds[0];

			for (int i = 1; i < bounds.Length; i++)
				combinedBounds.Encapsulate(bounds[i]);

			return combinedBounds;
		}

        public static Bounds CalculateObjectBounds(GameObject obj, bool includeInactive, bool ignoreSpriteRenderers, bool ignoreTriggerColliders = true)
        {
            Bounds bounds = new Bounds();
            bool hasBounds = false;

            foreach (var renderer in obj.GetComponentsInChildren<Renderer>(includeInactive))
            {
                bool considerRenderer = (renderer is MeshRenderer) || ((renderer is SpriteRenderer) && !ignoreSpriteRenderers);

                if (!considerRenderer)
                    continue;

                if (hasBounds)
                    bounds.Encapsulate(renderer.bounds);
                else
                    bounds = renderer.bounds;

                hasBounds = true;
            }

            foreach (var collider in obj.GetComponentsInChildren<Collider>(includeInactive))
            {
				if (collider.isTrigger && ignoreTriggerColliders)
					continue;

                if (hasBounds)
                    bounds.Encapsulate(collider.bounds);
                else
                    bounds = collider.bounds;

                hasBounds = true;
            }

            return bounds;
        }

        /// <summary>
        /// Positions an object by aligning one of it's own sockets to the socket of another object
        /// </summary>
        /// <param name="objectA">The object to move</param>
        /// <param name="socketA">A socket for the object that we want to move (must be a child somewhere in the object's hierarchy)</param>
        /// <param name="socketB">The socket we want to attach the object to (must not be a child in the object's hierarchy)</param>
        public static void PositionObjectBySocket(GameObject objectA, GameObject socketA, GameObject socketB)
        {
            PositionObjectBySocket(objectA.transform, socketA.transform, socketB.transform);
        }

        /// <summary>
        /// Positions an object by aligning one of it's own sockets to the socket of another object
        /// </summary>
        /// <param name="objectA">The object to move</param>
        /// <param name="socketA">A socket for the object that we want to move (must be a child somewhere in the object's hierarchy)</param>
        /// <param name="socketB">The socket we want to attach the object to (must not be a child in the object's hierarchy)</param>
        public static void PositionObjectBySocket(Transform objectA, Transform socketA, Transform socketB)
        {
            Quaternion targetRotation = Quaternion.LookRotation(-socketB.forward, socketB.up);
            objectA.rotation = targetRotation * Quaternion.Inverse(Quaternion.Inverse(objectA.rotation) * socketA.rotation);

            Vector3 targetPosition = socketB.position;
            objectA.position = targetPosition - (socketA.position - objectA.position);
        }

        public static Vector3 GetCardinalDirection(Vector3 direction, out float magnitude)
        {
            float absX = Math.Abs(direction.x);
            float absY = Math.Abs(direction.y);
            float absZ = Math.Abs(direction.z);

            float dirX = direction.x / absX;
            float dirY = direction.y / absY;
            float dirZ = direction.z / absZ;

            if (absX > absY && absX > absZ)
            {
                magnitude = dirX;
                return new Vector3(dirX, 0, 0);
            }
            else if (absY > absX && absY > absZ)
            {
                magnitude = dirY;
                return new Vector3(0, dirY, 0);
            }
            else if (absZ > absX && absZ > absY)
            {
                magnitude = dirZ;
                return new Vector3(0, 0, dirZ);
            }
            else
            {
                magnitude = dirX;
                return new Vector3(dirX, 0, 0);
            }
        }

        public static Vector3 VectorAbs(Vector3 vector)
        {
            return new Vector3(Math.Abs(vector.x), Math.Abs(vector.y), Math.Abs(vector.z));
        }

        public static void SetVector3Masked(ref Vector3 input, Vector3 value, Vector3 mask)
        {
            if (mask.x != 0)
                input.x = value.x;
            if (mask.y != 0)
                input.y = value.y;
            if (mask.z != 0)
                input.z = value.z;
        }

        public static Bounds CondenseBounds(Bounds bounds, IEnumerable<Doorway> doorways)
        {
            Vector3 min = bounds.center - bounds.extents;
            Vector3 max = bounds.center + bounds.extents;

            foreach(var doorway in doorways)
            {
                float magnitude;
                Vector3 dir = UnityUtil.GetCardinalDirection(doorway.transform.forward, out magnitude);

                if (magnitude < 0)
                    SetVector3Masked(ref min, doorway.transform.position, dir);
                else
                    SetVector3Masked(ref max, doorway.transform.position, dir);
            }

            Vector3 size = max - min;
            Vector3 center = min + (size / 2);

            return new Bounds(center, size);
        }

        public static IEnumerable<T> GetComponentsInParents<T>(GameObject obj, bool includeInactive = false) where T : Component
        {
            if (obj.activeSelf || includeInactive)
            {
                foreach (var comp in obj.GetComponents<T>())
                    yield return comp;
            }

            if (obj.transform.parent != null)
                foreach (var comp in GetComponentsInParents<T>(obj.transform.parent.gameObject, includeInactive))
                    yield return comp;
        }

        public static T GetComponentInParents<T>(GameObject obj, bool includeInactive = false) where T : Component
        {
            if (obj.activeSelf || includeInactive)
            {
                foreach (var comp in obj.GetComponents<T>())
                    return comp;
            }

            if (obj.transform.parent != null)
                return GetComponentInParents<T>(obj.transform.parent.gameObject, includeInactive);
            else
                return null;
        }
	}
}
