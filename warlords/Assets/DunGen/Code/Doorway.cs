using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DunGen
{
    /// <summary>
    /// A component to handle doorway placement and behaviour
    /// </summary>
    [AddComponentMenu("DunGen/Doorway")]
    public class Doorway : MonoBehaviour
    {
        /// <summary>
        /// The socket group this doorway belongs to. Allows you to use different sized doorways and have them connect correctly
        /// </summary>
        public DoorwaySocketType SocketGroup;
		/// <summary>
		/// When placing a door prefab, the doorway with the higher priority will have their prefab used
		/// </summary>
		public int DoorPrefabPriority;
        /// <summary>
        /// When this doorway is in use, a prefab will be picked at random from this list and is spawned at the doorway location - one per doorways pair (connection)
        /// </summary>
        public List<GameObject> DoorPrefabs = new List<GameObject>();
		/// <summary>
		/// When this doorway is NOT in use, a prefab will be picked at random from this list and is spawned at the doorway location - one per doorway
		/// </summary>
		public List<GameObject> BlockerPrefabs = new List<GameObject>();
		/// <summary>
		/// If true, the chosen Door prefab will not be oriented to match the rotation of the doorway it is placed on
		/// </summary>
		public bool AvoidRotatingDoorPrefab;
		/// <summary>
		/// If true, the chosen Blocker prefab will not be oriented to match the rotation of the doorway it is placed on
		/// </summary>
		public bool AvoidRotatingBlockerPrefab;
		/// <summary>
		/// When this doorway is in use, objects in this list will remain in the scene, otherwise, they are destroyed
		/// </summary>
		public List<GameObject> AddWhenInUse = new List<GameObject>();
        /// <summary>
        /// When this doorway is NOT in use, objects in this list will remain in the scene, otherwise, they are destroyed
        /// </summary>
        public List<GameObject> AddWhenNotInUse = new List<GameObject>();
        /// <summary>
        /// The size of the doorway, for use with portal culling
        /// </summary>
        public Vector2 Size = new Vector2(1, 2);
        /// <summary>
        /// The Tile that this doorway belongs to
        /// </summary>
        public Tile Tile { get { return tile; } internal set { tile = value; } }
        /// <summary>
        /// The ID of the key used to unlock this door
        /// </summary>
        public int? LockID;
        /// <summary>
        /// Gets the lock status of the door
        /// </summary>
        public bool IsLocked { get { return LockID.HasValue; } }
        /// <summary>
        /// Does this doorway have a prefab object placed as a door?
        /// </summary>
        public bool HasDoorPrefab { get { return doorPrefab != null; } }
        /// <summary>
        /// The prefab that has been placed as a door for this doorway
        /// </summary>
        public GameObject UsedDoorPrefab { get { return doorPrefab; } }
		/// <summary>
		/// The Door component that has been assigned to the door prefab instance (if any)
		/// </summary>
		public Door DoorComponent { get { return doorComponent; } }
        /// <summary>
        /// The dungeon that this doorway belongs to
        /// </summary>
        public Dungeon Dungeon { get; internal set; }
        /// <summary>
        /// The doorway that this is connected to
        /// </summary>
        public Doorway ConnectedDoorway { get { return connectedDoorway; } internal set { connectedDoorway = value; } }
		/// <summary>
		/// Allows for hiding of any GameObject in the "AddWhenInUse" and "AddWhenNotInUse" lists - used to remove clutter at design-time; should not be used at runtime
		/// </summary>
		public bool HideConditionalObjects
		{
			get { return hideConditionalObjects; }
			set
			{
				hideConditionalObjects = value;

				foreach (var obj in AddWhenInUse)
					if (obj != null)
						obj.SetActive(!hideConditionalObjects);

				foreach (var obj in AddWhenNotInUse)
					if (obj != null)
						obj.SetActive(!hideConditionalObjects);
			}
		}

        [SerializeField]
        [HideInInspector]
        private GameObject doorPrefab;
		[SerializeField]
		[HideInInspector]
		private Door doorComponent;
		[SerializeField]
        private Tile tile;
        [SerializeField]
        private Doorway connectedDoorway;
		[SerializeField]
		private bool hideConditionalObjects;

        internal bool placedByGenerator;


        private void OnDrawGizmos()
        {
            if (!placedByGenerator)
                DebugDraw();
        }

        internal void SetUsedPrefab(GameObject doorPrefab)
        {
            this.doorPrefab = doorPrefab;

			if (doorPrefab != null)
				doorComponent = doorPrefab.GetComponent<Door>();
        }

        internal void RemoveUsedPrefab()
        {
            if (doorPrefab != null)
                UnityUtil.Destroy(doorPrefab);

            doorPrefab = null;
        }

        internal void DebugDraw()
        {
            Vector2 halfSize = Size * 0.5f;

            Gizmos.color = EditorConstants.DoorDirectionColour;
            float lineLength = Mathf.Min(Size.x, Size.y);
            Gizmos.DrawLine(transform.position + transform.up * halfSize.y, transform.position + transform.up * halfSize.y + transform.forward * lineLength);

            Gizmos.color = EditorConstants.DoorRectColour;
            Vector3 topLeft = transform.position - (transform.right * halfSize.x) + (transform.up * Size.y);
            Vector3 topRight = transform.position + (transform.right * halfSize.x) + (transform.up * Size.y);
            Vector3 bottomLeft = transform.position - (transform.right * halfSize.x);
            Vector3 bottomRight = transform.position + (transform.right * halfSize.x);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}