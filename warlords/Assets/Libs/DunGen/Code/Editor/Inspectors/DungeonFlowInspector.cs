using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using DunGen.Graph;
using UnityEngine;

namespace DunGen.Editor
{
    [CustomEditor(typeof(DungeonFlow))]
	public sealed class DungeonFlowInspector : UnityEditor.Editor
	{
        private bool showPropGroups = false;
		private bool showTileInjectionRules = false;


		private void OnEnable()
		{
			DungeonFlow flow = target as DungeonFlow;

            if (flow != null)
            {
                foreach (var line in flow.Lines)
                    line.Graph = flow;
                foreach (var node in flow.Nodes)
                    node.Graph = flow;
            }
		}

        public override void OnInspectorGUI()
        {
            DungeonFlow data = target as DungeonFlow;

            if (data == null)
                return;

			data.KeyManager = (KeyManager)EditorGUILayout.ObjectField("Key Manager", data.KeyManager, typeof(KeyManager), false);
			EditorUtil.DrawIntRange("Length", data.Length);

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.HelpBox("The percentage chance that an unconnected but overlapping set of doorways will be connected", MessageType.Info);
			data.DoorwayConnectionChance = EditorGUILayout.Slider("Connection Chance", data.DoorwayConnectionChance, 0, 1);
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();

            if (GUILayout.Button("Open Flow Editor"))
                DungeonFlowEditorWindow.Open(data);

			DrawTileInjectionRules(data);

			EditorGUILayout.Space();

            showPropGroups = EditorGUILayout.Foldout(showPropGroups, "Global Props");

            if (showPropGroups)
            {
                int toDeleteIndex = -1;
                GUILayout.BeginVertical("box");

                for (int i = 0; i < data.GlobalPropGroupIDs.Count; i++)
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.BeginHorizontal();
                    data.GlobalPropGroupIDs[i] = EditorGUILayout.IntField("Group ID", data.GlobalPropGroupIDs[i]);

                    if (GUILayout.Button("×", EditorStyles.miniButton, GUILayout.Width(19)))
                        toDeleteIndex = i;

                    EditorGUILayout.EndHorizontal();

                    EditorUtil.DrawIntRange("Count", data.GlobalPropRanges[i]);
                    EditorGUILayout.EndVertical();
                }

                if (toDeleteIndex >= 0)
                {
                    data.GlobalPropGroupIDs.RemoveAt(toDeleteIndex);
                    data.GlobalPropRanges.RemoveAt(toDeleteIndex);
                }

                if (GUILayout.Button("Add New"))
                {
                    data.GlobalPropGroupIDs.Add(0);
                    data.GlobalPropRanges.Add(new IntRange(0, 1));
                }

                EditorGUILayout.EndVertical();
            }

            if (GUI.changed)
                EditorUtility.SetDirty(data);
        }

		private void DrawTileInjectionRules(DungeonFlow data)
		{
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			showTileInjectionRules = EditorGUILayout.Foldout(showTileInjectionRules, "Special Tile Injection");

			if (!showTileInjectionRules)
				return;

			int indexToRemove = -1;

			EditorGUILayout.BeginVertical("box");

			for (int i = 0; i < data.TileInjectionRules.Count; i++ )
			{
				var rule = data.TileInjectionRules[i];
				EditorGUILayout.BeginVertical("box");

				EditorGUILayout.BeginHorizontal();

				//string title = (rule.TileSet == null) ? "None" : rule.TileSet.name;
				//EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
				rule.TileSet = EditorGUILayout.ObjectField(rule.TileSet, typeof(TileSet), false) as TileSet;

				if (GUILayout.Button("x", EditorStyles.miniButton, GUILayout.Width(20)))
					indexToRemove = i;

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();

				rule.IsRequired = EditorGUILayout.ToggleLeft("Is Required?", rule.IsRequired);
				rule.CanAppearOnMainPath = EditorGUILayout.ToggleLeft("Can appear on Main Path?", rule.CanAppearOnMainPath);
				rule.CanAppearOnBranchPath = EditorGUILayout.ToggleLeft("Can appear on Branch Path?", rule.CanAppearOnBranchPath);

				EditorGUILayout.Space();

				EditorUtil.DrawLimitedFloatRange("Path Depth", rule.NormalizedPathDepth);

				bool previousEnabled = GUI.enabled;
				GUI.enabled = rule.CanAppearOnBranchPath;

				EditorUtil.DrawLimitedFloatRange("Branch Depth", rule.NormalizedBranchDepth);
				GUI.enabled = previousEnabled;

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}

			if (indexToRemove > -1)
				data.TileInjectionRules.RemoveAt(indexToRemove);

			if (GUILayout.Button("Add New Rule"))
				data.TileInjectionRules.Add(new TileInjectionRule());

			EditorGUILayout.EndVertical();
		}
	}
}
