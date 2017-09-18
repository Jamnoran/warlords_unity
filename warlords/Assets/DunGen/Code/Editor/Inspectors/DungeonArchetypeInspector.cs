using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(DungeonArchetype))]
	public sealed class DungeonArchetypeInspector : UnityEditor.Editor
	{
        public override void OnInspectorGUI()
        {
            DungeonArchetype archetype = target as DungeonArchetype;

            if (archetype == null)
                return;

            EditorGUILayout.BeginVertical("box");

            EditorUtil.DrawIntRange("Branching Depth", archetype.BranchingDepth);
            EditorUtil.DrawIntRange("Branch Count", archetype.BranchCount);
            archetype.StraightenChance = EditorGUILayout.Slider("Straighten", archetype.StraightenChance, 0.0f, 1.0f);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorUtil.DrawObjectList<TileSet>("Tile Sets", archetype.TileSets, GameObjectSelectionTypes.Prefab);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox("Specific tiles can be set to appear at the end of branch paths. The cap type can be used to control if these tiles are used \"instead of\" or \"aswell as\" the standard tile sets listed above", MessageType.Info);

            archetype.BranchCapType = (BranchCapType)EditorGUILayout.EnumPopup("Branch Cap Type", archetype.BranchCapType);
            EditorUtil.DrawObjectList<TileSet>("Branch-Cap Tile Sets", archetype.BranchCapTileSets, GameObjectSelectionTypes.Prefab);

			if(GUI.changed)
				EditorUtility.SetDirty(archetype);
        }
	}
}
