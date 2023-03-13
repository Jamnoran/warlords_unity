using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(RandomPrefab))]
    public class RandomPrefabInspector : UnityEditor.Editor
    {
        private readonly List<bool> showWeights = new List<bool>();


        private void OnEnable()
        {
            for (int i = 0; i < (target as RandomPrefab).Props.Weights.Count; i++)
                showWeights.Add(false);
        }

        public override void OnInspectorGUI()
        {
            RandomPrefab prop = target as RandomPrefab;
            if (prop == null)
                return;

			prop.ZeroPosition = EditorGUILayout.Toggle(new GUIContent("Zero Position", "Snaps the spawned prop to this GameObject's position. Otherwise, the prefab's position will be used as an offset."), prop.ZeroPosition);
			prop.ZeroRotation = EditorGUILayout.Toggle(new GUIContent("Zero Rotation", "Snaps the spawned prop to this GameObject's rotation. Otherwise, the prefab's rotation will be used as an offset."), prop.ZeroRotation);

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			EditorUtil.DrawGameObjectChanceTableGUI("Prefab", prop.Props, showWeights, false, true);

            if (GUI.changed)
                EditorUtility.SetDirty(prop);
        }
    }
}

