using System;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(Tile))]
    public class TileInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            Tile tile = target as Tile;

            if (tile == null)
                return;

            tile.AllowRotation = EditorGUILayout.Toggle("Allow Rotation", tile.AllowRotation);
			tile.AllowImmediateRepeats = EditorGUILayout.Toggle("Allow Immediate Repeats", tile.AllowImmediateRepeats);

            if (GUI.changed)
                EditorUtility.SetDirty(tile);
        }
    }
}