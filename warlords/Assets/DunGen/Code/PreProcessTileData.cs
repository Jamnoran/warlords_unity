using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
    public sealed class PreProcessTileData
    {
		public static Type ProBuilderObjectType { get; private set; }


		public GameObject Prefab { get; private set; }
        public GameObject Proxy { get; private set; }
        public readonly List<GameObject> ProxySockets = new List<GameObject>();

        public readonly List<DoorwaySocketType> DoorwaySockets = new List<DoorwaySocketType>();
        public readonly List<Doorway> Doorways = new List<Doorway>();


		//public static void FindProBuilderObjectType()
		//{
		//	if (ProBuilderObjectType != null)
		//		return;

		//	// Look through each of the loaded assemblies in our current AppDomain, looking for ProBuilder's pb_Object type
		//	foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
		//	{
		//		if (assembly.FullName.Contains("ProBuilder"))
		//		{
		//			ProBuilderObjectType = assembly.GetType("pb_Object");

		//			if (ProBuilderObjectType != null)
		//				break;
		//		}
		//	}
		//}

		public PreProcessTileData(GameObject prefab, bool ignoreSpriteRendererBounds, Vector3 upVector)
        {
            Prefab = prefab;
            Proxy = new GameObject(prefab.name + "_PROXY");

            // Reset prefab transforms
            prefab.transform.position = Vector3.zero;
            prefab.transform.rotation = Quaternion.identity;

            GetAllDoorways();

            // Copy all doors to the proxy object
            foreach(var door in Doorways)
            {
                var proxyDoor = new GameObject("ProxyDoor");
                proxyDoor.transform.position = door.transform.position;
                proxyDoor.transform.rotation = door.transform.rotation;

                proxyDoor.transform.parent = Proxy.transform;
                ProxySockets.Add(proxyDoor);
            }

            CalculateProxyBounds(ignoreSpriteRendererBounds, upVector);
        }

        public bool ChooseRandomDoorway(System.Random random, DoorwaySocketType? socketGroupFilter, Vector3? allowedDirection, out int doorwayIndex, out Doorway doorway)
        {
            doorwayIndex = -1;
            doorway = null;

            IEnumerable<Doorway> possibleDoorways = Doorways;

            if (socketGroupFilter.HasValue)
                possibleDoorways = possibleDoorways.Where(x => { return DoorwaySocket.IsMatchingSocket(x.SocketGroup, socketGroupFilter.Value); });
            if (allowedDirection.HasValue)
                possibleDoorways = possibleDoorways.Where(x => { return x.transform.forward == allowedDirection; });

            if (possibleDoorways.Count() == 0)
                return false;

            doorway = possibleDoorways.ElementAt(random.Next(0, possibleDoorways.Count()));
            doorwayIndex = Doorways.IndexOf(doorway);

            return true;
        }

        private void CalculateProxyBounds(bool ignoreSpriteRendererBounds, Vector3 upVector)
        {
            Bounds bounds = UnityUtil.CalculateObjectBounds(Prefab, true, ignoreSpriteRendererBounds);

			// Since ProBuilder objects don't have a mesh until they're instantiated, we have to calculate the bounds manually
			//if (ProBuilderObjectType != null)
			//{
			//	foreach (var pbObject in Prefab.GetComponentsInChildren(ProBuilderObjectType))
			//	{
			//		Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			//		Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			//		Vector3[] vertices = (Vector3[])ProBuilderObjectType.GetProperty("vertices").GetValue(pbObject, null);

			//		foreach (var vert in vertices)
			//		{
			//			min = Vector3.Min(min, vert);
			//			max = Vector3.Max(max, vert);
			//		}

			//		Vector3 size = Prefab.transform.TransformDirection(max - min);
			//		Vector3 center = Prefab.transform.TransformPoint(min) + size / 2;
			//		bounds.Encapsulate(new Bounds(center, size));
			//	}
			//}

			bounds = UnityUtil.CondenseBounds(bounds, Prefab.GetComponentsInChildren<Doorway>(true));
			bounds.size *= 0.99f;

            var collider = Proxy.AddComponent<BoxCollider>();
            collider.center = bounds.center;
            collider.size = bounds.size;
        }

        private void GetAllDoorways()
        {
            DoorwaySockets.Clear();

            foreach (var d in Prefab.GetComponentsInChildren<Doorway>(true))
            {
                Doorways.Add(d);

                if (!DoorwaySockets.Contains(d.SocketGroup))
                    DoorwaySockets.Add(d.SocketGroup);
            }
        }
    }
}
