using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
	[CustomEditor(typeof(Doorway))]
	public class DoorwayInspector : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			Doorway door = target as Doorway;

			if(door == null)
				return;

            door.SocketGroup = (DoorwaySocketType)EditorGUILayout.EnumPopup("Socket Group", door.SocketGroup);
            door.Size = EditorGUILayout.Vector2Field("Size", door.Size);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

			door.HideConditionalObjects = EditorGUILayout.ToggleLeft(new GUIContent("Hide Conditional Objects?", "Hides any objects in the \"Add when in use\" and \"Add when NOT in use\" lists for the purpose of reducing clutter at design-time. Has no effect on the runtime results"), door.HideConditionalObjects);

			EditorGUILayout.HelpBox("When this doorway is in use (another tile is connected using this doorway), the selected objects below will appear in the scene, otherwise, they will be removed", MessageType.Info);
			EditorUtil.DrawObjectList("Add when in use", door.AddWhenInUse, GameObjectSelectionTypes.InScene);

			EditorGUILayout.HelpBox("When this doorway is in NOT in use (the doorway is closed and no other tile is connected using this doorway), the selected objects below will appear in the scene, otherwise, they will be removed", MessageType.Info);
			EditorUtil.DrawObjectList("Add when NOT in use", door.AddWhenNotInUse, GameObjectSelectionTypes.InScene);

            EditorGUILayout.HelpBox("When this doorway is in use (another tile is connected using this doorway), a random prefab will be selected from this list to be spawned at the doorway location. One per doorway pair.", MessageType.Info);
			door.DoorPrefabPriority = EditorGUILayout.IntField(new GUIContent("Priority", "When a door prefab is to be placed, the doorway with the higher priority will have their prefab used"), door.DoorPrefabPriority);
            EditorUtil.DrawObjectList("Door Prefab", door.DoorPrefabs, GameObjectSelectionTypes.Prefab);

			door.AvoidRotatingDoorPrefab = EditorGUILayout.Toggle(new GUIContent("Avoid Door Prefab Rotation?", "If true, the chosen Door prefab will not be oriented to match the rotation of the doorway"), door.AvoidRotatingDoorPrefab);

			EditorGUILayout.HelpBox("When this doorway is NOT in use (no tile is connected to this doorway), a random prefab will be selected from this list to be spawned at the doorway location. One per doorway.", MessageType.Info);
			EditorUtil.DrawObjectList("Blocker Prefab", door.BlockerPrefabs, GameObjectSelectionTypes.Prefab);

			door.AvoidRotatingBlockerPrefab = EditorGUILayout.Toggle(new GUIContent("Avoid Blocker Prefab Rotation?", "If true, the chosen Blocker prefab will not be oriented to match the rotation of the doorway"), door.AvoidRotatingBlockerPrefab);

			if (GUI.changed)
			{
				door.HideConditionalObjects = door.HideConditionalObjects; // Set active state for newly added objects
				EditorUtility.SetDirty(door);
			}
		}
	}
}

