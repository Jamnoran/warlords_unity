using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(RuntimeDungeon))]
	public sealed class RuntimeDungeonInspector : UnityEditor.Editor
	{
        public override void OnInspectorGUI()
        {
            RuntimeDungeon dungeon = target as RuntimeDungeon;

            if (dungeon == null)
                return;

            dungeon.GenerateOnStart = EditorGUILayout.Toggle("Generate on Start", dungeon.GenerateOnStart);
			dungeon.Root = EditorGUILayout.ObjectField(new GUIContent("Root", "An optional root object for the dungeon to be parented to. If blank, a new root GameObject will be created named \"" + Constants.DefaultDungeonRootName + "\""), dungeon.Root, typeof(GameObject), true) as GameObject;

            EditorGUILayout.BeginVertical("box");
            EditorUtil.DrawDungeonGenerator(dungeon.Generator, true);
            EditorGUILayout.EndVertical();

            if (GUI.changed)
                EditorUtility.SetDirty(dungeon);
        }
	}
}
