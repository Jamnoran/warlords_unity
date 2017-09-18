using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using DunGen.Graph;
using System.IO;
using DunGen.Adapters;
using System.Reflection;

namespace DunGen.Editor
{
	public static class EditorUtil
    {
        /**
         * Utilities for drawing custom classes in the inspector
         */

        public static T CreateAsset<T>(string pathOverride = null, bool selectNewAsset = true) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            string path = (pathOverride != null) ? "Assets/" + pathOverride : AssetDatabase.GetAssetPath(Selection.activeObject);

            if (path == "")
                path = "Assets";
            else if (Path.GetExtension(path) != "")
                path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");

            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).Name + ".asset");
            string dir = Application.dataPath + "/" + path.TrimStart("Assets/".ToCharArray());

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();

            if (selectNewAsset)
            {
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }

            return asset;
        }

		/// <summary>
		/// Draws a GUI for a game object chance table. Allowing for addition/removal of rows and
		/// modification of values and weights
		/// </summary>
		/// <param name="table">The table to draw</param>
        public static void DrawGameObjectChanceTableGUI(string objectName, GameObjectChanceTable table, List<bool> showWeights, bool allowSceneObjects, bool allowAssetObjects)
        {
			string title = string.Format("{0} Weights ({1})", objectName, table.Weights.Count);
			EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            EditorGUI.indentLevel = 1;

            int toDeleteIndex = -1;
            GUILayout.BeginVertical("box");

            for (int i = 0; i < table.Weights.Count; i++)
            {
                var w = table.Weights[i];
                GUILayout.BeginVertical("box");
                EditorGUILayout.BeginHorizontal();

                var obj = (GameObject)EditorGUILayout.ObjectField("", w.Value, typeof(GameObject), allowSceneObjects);

				if (obj != null)
				{
					var prefabType = PrefabUtility.GetPrefabType(obj);
					bool isAsset = prefabType == PrefabType.ModelPrefab || prefabType == PrefabType.Prefab;

					if (allowAssetObjects && isAsset || allowSceneObjects && !isAsset)
						w.Value = obj;
				}
				else
					w.Value = null;

                if (GUILayout.Button("x", EditorStyles.miniButton, EditorConstants.SmallButtonWidth))
                    toDeleteIndex = i;

                EditorGUILayout.EndHorizontal();

                showWeights[i] = EditorGUILayout.Foldout(showWeights[i], "Weights");

                if (showWeights[i])
                {
                    w.MainPathWeight = EditorGUILayout.FloatField("Main Path", w.MainPathWeight);
                    w.BranchPathWeight = EditorGUILayout.FloatField("Branch Path", w.BranchPathWeight);

                    if (w.UseDepthScale)
                        w.DepthWeightScale = EditorGUILayout.CurveField("Depth Scale", w.DepthWeightScale, Color.white, new Rect(0, 0, 1, 1));
                }

                GUILayout.EndVertical();
            }

            if (toDeleteIndex >= 0)
            {
                table.Weights.RemoveAt(toDeleteIndex);
                showWeights.RemoveAt(toDeleteIndex);
            }

            if (GUILayout.Button("Add New " + objectName))
            {
                table.Weights.Add(new GameObjectChance() { UseDepthScale = true });
                showWeights.Add(false);
            }

            EditorGUILayout.EndVertical();
        }

		/// <summary>
		/// Draws a simple GUI for an IntRange
		/// </summary>
		/// <param name="name">A descriptive label</param>
		/// <param name="range">The range to modify</param>
        public static void DrawIntRange(string name, IntRange range)
        {
            EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(name, EditorConstants.LabelWidth);
            GUILayout.FlexibleSpace();
			range.Min = EditorGUILayout.IntField(range.Min, EditorConstants.IntFieldWidth);
			EditorGUILayout.LabelField("-", EditorConstants.SmallWidth);
			range.Max = EditorGUILayout.IntField(range.Max, EditorConstants.IntFieldWidth);

            EditorGUILayout.EndHorizontal();
        }

		/// <summary>
		/// Draws a min/max slider representing a float range
		/// </summary>
		/// <param name="name">A descriptive label</param>
		/// <param name="range">The range value</param>
		/// <param name="limitMin">The lowest value of the slider</param>
		/// <param name="limitMax">The highest value of the slider</param>
		public static void DrawLimitedFloatRange(string name, FloatRange range, float limitMin = 0, float limitMax = 1)
		{
			float min = range.Min;
			float max = range.Max;

			DrawLimitedFloatRange(name, ref min, ref max, limitMin, limitMax);

			range.Min = min;
			range.Max = max;
		}

		/// <summary>
		/// Draws a min/max slider representing a float range
		/// </summary>
		/// <param name="name">A descriptive label</param>
		/// <param name="min">The current minimum value</param>
		/// <param name="max">The current maximum value</param>
		/// <param name="limitMin">The lowest value of the slider</param>
		/// <param name="limitMax">The highest value of the slider</param>
		public static void DrawLimitedFloatRange(string name, ref float min, ref float max, float limitMin, float limitMax)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(name);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.MinMaxSlider(ref min, ref max, limitMin, limitMax);
			min = EditorGUILayout.FloatField(min, GUILayout.Width(50));
			max = EditorGUILayout.FloatField(max, GUILayout.Width(50));

			EditorGUILayout.EndHorizontal();
		}

		/// <summary>
		/// Draws the GUI for a list of Unity.Object. Allows users to add/remove/modify a specific type
		/// deriving from Unity.Object (such as GameObject, or a Component type)
		/// </summary>
		/// <param name="header">A descriptive header</param>
		/// <param name="objects">The object list to edit</param>
		/// <param name="allowedSelectionTypes">The types of objects that are allowed to be selected</param>
		/// <typeparam name="T">The type of object in the list</typeparam>
		public static void DrawObjectList<T>(string header, IList<T> objects, GameObjectSelectionTypes allowedSelectionTypes) where T : UnityEngine.Object
        {
			bool allowSceneSelection = (allowedSelectionTypes & GameObjectSelectionTypes.InScene) == GameObjectSelectionTypes.InScene;
			bool allowPrefabSelection = (allowedSelectionTypes & GameObjectSelectionTypes.Prefab) == GameObjectSelectionTypes.Prefab;

            EditorGUILayout.PrefixLabel(header);
            EditorGUI.indentLevel = 0;

            int toDeleteIndex = -1;
            GUILayout.BeginVertical("box");

            for (int i = 0; i < objects.Count; i++)
            {
                T obj = objects[i];
                EditorGUILayout.BeginHorizontal();

                T tempObj = (T)EditorGUILayout.ObjectField("", obj, typeof(T), allowSceneSelection);

				if(tempObj != null)
				{
					var prefabType = PrefabUtility.GetPrefabType(tempObj);

					if ((prefabType == PrefabType.Prefab && allowPrefabSelection) || (prefabType != PrefabType.Prefab && allowSceneSelection))
						objects[i] = tempObj;
				}

				if (GUILayout.Button("x", EditorStyles.miniButton, EditorConstants.SmallButtonWidth))
                    toDeleteIndex = i;

                EditorGUILayout.EndHorizontal();
            }

            if (toDeleteIndex >= 0)
                objects.RemoveAt(toDeleteIndex);

            if (GUILayout.Button("Add New"))
                objects.Add(default(T));

            EditorGUILayout.EndVertical();
        }

		public static void DrawKeySelection(string label, KeyManager manager, List<KeyLockPlacement> keys, bool includeRange)
		{
			if(manager == null)
				return;

			manager.ExposeKeyList();

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

			int toDeleteIndex = -1;
			string[] keyNames = manager.Keys.Select(x => x.Name).ToArray();

			for (int i = 0; i < keys.Count; i++)
			{
				EditorGUILayout.BeginVertical("box");

				var key = manager.GetKeyByID(keys[i].ID);

				EditorGUILayout.BeginHorizontal();

				int nameIndex = EditorGUILayout.Popup(Array.IndexOf(keyNames, key.Name), keyNames);
				keys[i].ID = manager.GetKeyByName(keyNames[nameIndex]).ID;

				if (GUILayout.Button("x", EditorStyles.miniButton, EditorConstants.SmallButtonWidth))
					toDeleteIndex = i;

				EditorGUILayout.EndHorizontal();

                if(includeRange)
				    EditorUtil.DrawIntRange("Count", keys[i].Range);

				EditorGUILayout.EndVertical();
			}

			if(toDeleteIndex > -1)
				keys.RemoveAt(toDeleteIndex);

			if(GUILayout.Button("Add"))
				keys.Add(new KeyLockPlacement() { ID = manager.Keys[0].ID });

			EditorGUILayout.EndVertical();
		}

        public static void DrawDungeonGenerator(DungeonGenerator generator, bool isRuntimeDungeon)
        {
            generator.DungeonFlow = (DungeonFlow)EditorGUILayout.ObjectField("Dungeon Flow", generator.DungeonFlow, typeof(DungeonFlow), false);

            generator.ShouldRandomizeSeed = EditorGUILayout.Toggle("Randomize Seed", generator.ShouldRandomizeSeed);

            if (!generator.ShouldRandomizeSeed)
                generator.Seed = EditorGUILayout.IntField("Seed", generator.Seed);

            generator.MaxAttemptCount = EditorGUILayout.IntField("Max Failed Attempts", generator.MaxAttemptCount);
			generator.LengthMultiplier = EditorGUILayout.FloatField("Length Multiplier", generator.LengthMultiplier);
            generator.IgnoreSpriteBounds = EditorGUILayout.Toggle("Ignore Sprite Bounds", generator.IgnoreSpriteBounds);
            generator.UpVector = EditorGUILayout.Vector3Field("Up Vector", generator.UpVector);

			if (generator.LengthMultiplier < 0)
				generator.LengthMultiplier = 0.0f;

            if (isRuntimeDungeon)
                generator.DebugRender = EditorGUILayout.Toggle("Debug Render", generator.DebugRender);

			generator.UseLegacyWeightCombineMethod = EditorGUILayout.Toggle(new GUIContent("Use Legacy Weighting", "Reverts to the old method of handling weight combination for multiple TileSets.\n\nNEW: Tiles are put into one pool and chosen from at random based on their weight.\n\nLEGACY: One TileSet is selected at random, then a Tile is chosen from that set based on their weight."), generator.UseLegacyWeightCombineMethod);
			generator.PlaceTileTriggers = EditorGUILayout.Toggle(new GUIContent("Place Tile Triggers", "Places trigger colliders around Tiles which can be used in conjunction with the DungenCharacter component to receieve events when changing rooms"), generator.PlaceTileTriggers);
			generator.TileTriggerLayer = EditorGUILayout.LayerField(new GUIContent("Trigger Layer", "The layer to place the tile root objects on if \"Place Tile Triggers\" is checked"), generator.TileTriggerLayer);

			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.LabelField("Global Overrides", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			DrawOverride("Allow Immediate Repeats", ref generator.OverrideAllowImmediateRepeats, ref generator.AllowImmediateRepeats);
			DrawOverride("Allow Tile Rotation", ref generator.OverrideAllowTileRotation, ref generator.AllowTileRotation);

			EditorGUILayout.EndVertical();

			DrawPortalCullingAdapter(generator, isRuntimeDungeon);
        }

		private static void DrawPortalCullingAdapter(DungeonGenerator generator, bool isRuntimeDungeon)
		{
			Type[] cullingTypes;
			string[] cullingTypeNames;
			bool isCullingAdapterAvailable = TypeUtil.GetAdapterTypesInfo(typeof(PortalCullingAdapter), out cullingTypes, out cullingTypeNames, true);

			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Portal Culling", EditorStyles.boldLabel);

			int chosenAdapterClassIndex = Array.IndexOf(cullingTypes, generator.PortalCullingAdapterClass.Type);

			bool previousEnabled = GUI.enabled;
			GUI.enabled = isCullingAdapterAvailable;
			chosenAdapterClassIndex = EditorGUILayout.Popup(chosenAdapterClassIndex, cullingTypeNames);
			GUI.enabled = previousEnabled;

			generator.PortalCullingAdapterClass = (chosenAdapterClassIndex > 0) ? new SerializableType(cullingTypes[chosenAdapterClassIndex]) : new SerializableType();

			if (generator.PortalCullingAdapterClass.Type == null && generator.Culling != null)
				generator.Culling = null;
			else if (generator.PortalCullingAdapterClass.Type != null)
			{
				if (generator.Culling == null || generator.Culling.GetType() != generator.PortalCullingAdapterClass.Type)
					generator.Culling = Activator.CreateInstance(generator.PortalCullingAdapterClass.Type) as PortalCullingAdapter;

				if (generator.Culling == null)
					Debug.Log("Culling is NULL");
				if (generator.Culling.GetType() != generator.PortalCullingAdapterClass.Type)
					Debug.Log("Culling adapter type mismatch");
			}

			EditorGUILayout.EndHorizontal();


			var culling = generator.Culling;

			if (culling != null)
			{
				generator.IsPortalCullingEnabled = EditorGUILayout.Toggle("Enabled", generator.IsPortalCullingEnabled);

				previousEnabled = GUI.enabled;
				GUI.enabled = generator.IsPortalCullingEnabled;
				culling.OnInspectorGUI(generator, isRuntimeDungeon);
				GUI.enabled = previousEnabled;
			}

			EditorGUILayout.EndVertical();
		}

		public static void DrawOverride(string label, ref bool shouldOverride, ref bool value)
		{
			EditorGUILayout.BeginHorizontal();
			shouldOverride = EditorGUILayout.Toggle(shouldOverride, GUILayout.Width(10));
			bool previousEnabled = GUI.enabled;
			GUI.enabled = shouldOverride;
			value = EditorGUILayout.Toggle(label, value);
			GUI.enabled = previousEnabled;
			EditorGUILayout.EndHorizontal();
		}
	}
}
