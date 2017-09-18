using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(LocalPropSet))]
    public class LocalPropSetInspector : UnityEditor.Editor
    {
        private readonly List<bool> showWeights = new List<bool>();


        private void OnEnable()
        {
            for (int i = 0; i < (target as LocalPropSet).Props.Weights.Count; i++)
                showWeights.Add(false);
        }

        public override void OnInspectorGUI()
        {
            LocalPropSet propSet = target as LocalPropSet;

            if (propSet == null)
                return;

			propSet.CountMode = (LocalPropSetCountMode)EditorGUILayout.EnumPopup("Count Mode", propSet.CountMode);

			string countModeHelpText = "";
			switch (propSet.CountMode)
			{
				case LocalPropSetCountMode.Random:
					countModeHelpText = "A number of props will be chosen at random between the min & max count";
					break;

				case LocalPropSetCountMode.DepthBased:
					countModeHelpText = "A number of props will be chosen based on the current depth into the dungeon (read from the curve below). A value of zero on the graph will use the min count, a value of one will use the max count";
					break;

				case LocalPropSetCountMode.DepthMultiply:
					countModeHelpText = "A number of props will be chosen at random between the min & max count and then multiplied by the value read from the curve below";
					break;

				default:
					break;
			}

			EditorGUILayout.HelpBox(countModeHelpText, MessageType.Info);

			EditorUtil.DrawIntRange("Count", propSet.PropCount);

			if(propSet.CountMode == LocalPropSetCountMode.DepthBased || propSet.CountMode == LocalPropSetCountMode.DepthMultiply)
				propSet.CountDepthCurve = EditorGUILayout.CurveField("Count Depth Curve", propSet.CountDepthCurve, Color.white, new Rect(0, 0, 1, 1));

			EditorGUILayout.Space();
            EditorUtil.DrawGameObjectChanceTableGUI("Prop", propSet.Props, showWeights, true, false);

            if (GUILayout.Button("Add Selected"))
            {
                foreach (var go in Selection.gameObjects)
                    if (!propSet.Props.ContainsGameObject(go))
                    {
                        propSet.Props.Weights.Add(new GameObjectChance(go));
                        showWeights.Add(false);
                    }
            }

            if (GUI.changed)
                EditorUtility.SetDirty(propSet);
        }
    }
}

