using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace DunGen.Editor
{
	[CustomEditor(typeof(Doorway))]
	public class DoorwayInspector : UnityEditor.Editor
	{
		#region Constants

		private static readonly GUIContent socketGroupLabel = new GUIContent("Socket Group", "Determines if two doorways can connect. By default, only doorways with matching socket groups can be connected to one another");
		private static readonly GUIContent sizeLabel = new GUIContent("Size", "The size of the doorway, used for visualisation and in some integrations");
		private static readonly GUIContent hideConditionalObjectsLabel = new GUIContent("Hide Conditional Objects?", "If checked, any in-scene door or blocked objects will be hidden for the purpose of reducing clutter. Has no effect on the runtime results");
		private static readonly GUIContent addWhenInUseLabel = new GUIContent("Scene Objects", "In-scene objects to be KEPT when the doorway is in use (connected)");
		private static readonly GUIContent addWhenNotInUseLabel = new GUIContent("Scene Objects", "In-scene objects to be REMOVED when the doorway is in use (connected)");
		private static readonly GUIContent priorityLabel = new GUIContent("Priority", "When two doorways are connected, the one with the higher priority will have their door prefab used");
		private static readonly GUIContent doorPrefabLabel = new GUIContent("Random Prefab", "When this doorway is in use (connected), a prefab will be spawned from this list at random");
		private static readonly GUIContent blockerPrefabLabel = new GUIContent("Random Prefab", "When this doorway is NOT in use (unconnected), a prefab will be spawned from this list at random");
		private static readonly GUIContent avoidRotationLabel = new GUIContent("Avoid Rotation?", "If checked, the placed prefab will NOT be oriented to match the doorway");
		private static readonly GUIContent connectorsLabel = new GUIContent("Connectors", "In-scene objects and prefabs used when the doorway is in use (connected)");
		private static readonly GUIContent blockersLabel = new GUIContent("Blockers", "In-scene objects and prefabs used when the doorway is not in use (not connected)");

		#endregion

		private SerializedProperty socketGroupProp;
		private SerializedProperty sizeProp;
		private SerializedProperty hideConditionalObjectsProp;
		private SerializedProperty priorityProp;
		private SerializedProperty avoidDoorPrefabRotationProp;
		private SerializedProperty avoidBlockerPrefabRotationProp;
		private ReorderableList addWhenInUseList;
		private ReorderableList addWhenNotInUseList;
		private ReorderableList doorPrefabList;
		private ReorderableList blockerPrefabList;

		private bool showConnectors;
		private bool showBlockers;


		private void OnEnable()
		{
			socketGroupProp = serializedObject.FindProperty("SocketGroup");
			sizeProp = serializedObject.FindProperty("Size");
			hideConditionalObjectsProp = serializedObject.FindProperty("hideConditionalObjects");
			priorityProp = serializedObject.FindProperty("DoorPrefabPriority");
			avoidDoorPrefabRotationProp = serializedObject.FindProperty("AvoidRotatingDoorPrefab");
			avoidBlockerPrefabRotationProp = serializedObject.FindProperty("AvoidRotatingBlockerPrefab");


			addWhenInUseList = new ReorderableList(serializedObject, serializedObject.FindProperty("AddWhenInUse"), true, true, true, true);
			addWhenInUseList.drawElementCallback = (rect, index, isActive, isFocused) => DrawGameObjectInput(addWhenInUseList, rect, index, true);
			addWhenInUseList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, addWhenInUseLabel);

			addWhenNotInUseList = new ReorderableList(serializedObject, serializedObject.FindProperty("AddWhenNotInUse"), true, true, true, true);
			addWhenNotInUseList.drawElementCallback = (rect, index, isActive, isFocused) => DrawGameObjectInput(addWhenNotInUseList, rect, index, true);
			addWhenNotInUseList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, addWhenNotInUseLabel);

			doorPrefabList = new ReorderableList(serializedObject, serializedObject.FindProperty("DoorPrefabs"), true, true, true, true);
			doorPrefabList.drawElementCallback = (rect, index, isActive, isFocused) => DrawGameObjectInput(doorPrefabList, rect, index, false);
			doorPrefabList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, doorPrefabLabel);

			blockerPrefabList = new ReorderableList(serializedObject, serializedObject.FindProperty("BlockerPrefabs"), true, true, true, true);
			blockerPrefabList.drawElementCallback = (rect, index, isActive, isFocused) => DrawGameObjectInput(blockerPrefabList, rect, index, false);
			blockerPrefabList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, blockerPrefabLabel);
		}

		private void DrawGameObjectInput(ReorderableList list, Rect rect, int index, bool requireSceneObject)
		{
			rect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight);
			EditorGUI.BeginChangeCheck();

			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var newObject = EditorGUI.ObjectField(rect, element.objectReferenceValue, typeof(GameObject), requireSceneObject);

			bool isValidEntry = (newObject == null);

			if (newObject != null)
			{
				var prefabType = PrefabUtility.GetPrefabType(newObject);
				bool isPrefab = prefabType == PrefabType.Prefab || prefabType == PrefabType.ModelPrefab;
				isValidEntry = isPrefab != requireSceneObject;
			}

			if (EditorGUI.EndChangeCheck() && isValidEntry)
				element.objectReferenceValue = newObject;
		}

		public override void OnInspectorGUI()
		{
			var doorway = target as Doorway;
			serializedObject.Update();

			EditorGUILayout.PropertyField(socketGroupProp, socketGroupLabel);
			EditorGUILayout.PropertyField(sizeProp, sizeLabel);

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(hideConditionalObjectsProp, hideConditionalObjectsLabel);
			if (EditorGUI.EndChangeCheck())
				doorway.HideConditionalObjects = hideConditionalObjectsProp.boolValue;

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorGUI.indentLevel++;

			// Connectors
			EditorGUILayout.BeginVertical("box");
			showConnectors = EditorGUILayout.Foldout(showConnectors, connectorsLabel);
			if (showConnectors)
			{
				EditorGUILayout.PropertyField(priorityProp, priorityLabel);
				EditorGUILayout.PropertyField(avoidDoorPrefabRotationProp, avoidRotationLabel);

				EditorGUILayout.Space();

				doorPrefabList.DoLayoutList();

				EditorGUILayout.Space();

				addWhenInUseList.DoLayoutList();
			}
			EditorGUILayout.EndVertical();

			// Blockers
			EditorGUILayout.BeginVertical("box");
			showBlockers = EditorGUILayout.Foldout(showBlockers, blockersLabel);
			if (showBlockers)
			{
				EditorGUILayout.PropertyField(avoidBlockerPrefabRotationProp, avoidRotationLabel);

				EditorGUILayout.Space();

				blockerPrefabList.DoLayoutList();

				EditorGUILayout.Space();

				addWhenNotInUseList.DoLayoutList();
			}
			EditorGUILayout.EndVertical();
			EditorGUI.indentLevel--;

			serializedObject.ApplyModifiedProperties();
		}
	}
}