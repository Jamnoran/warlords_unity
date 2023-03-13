using System;
using UnityEngine;
using UnityEditor;

namespace DunGen.Editor
{
    [CustomEditor(typeof(Tile))]
    public class TileInspector : UnityEditor.Editor
    {
		#region Content & Styles

		private static class Content
		{
			public static readonly GUIContent AllowRotation = new GUIContent("Allow Rotation", "If checked, this tile is allowed to be rotated by the dungeon gennerator. This setting can be overriden globally in the dungeon generator settings");
			public static readonly GUIContent AllowImmediateRepeats = new GUIContent("Allow Immediate Repeats", "If checked, this tile can appear beside an identical copy in the dungeon layout. This setting can be overriden globally in the dungeon generator settings");
			public static readonly GUIContent OverrideAutomaticTileBounds = new GUIContent("Override Automatic Tile Bounds", "DunGen automatically calculates a bounding volume for tiles. Check this option if you're having problems with the automatically generated bounds.");
			public static readonly GUIContent OverriddenBounds = new GUIContent("Overridden Bounds", "If specified, DunGen will use these boundary values for this tile instead of automatically calculating its own");
			public static readonly GUIContent FitToTile = new GUIContent("Fit to Tile", "Uses DunGen's automatic bounds generating to try to fit the bounds to the tile.");
			public static readonly GUIContent Entrance = new GUIContent("Entrance", "If set, DunGen will always use this doorway as the entrance to this tile.");
			public static readonly GUIContent Exit = new GUIContent("Exit", "If set, DunGen will always use this doorway as the first exit from this tile");
		}

		#endregion


		public override void OnInspectorGUI()
        {
            Tile tile = target as Tile;

            if (tile == null)
                return;

            tile.AllowRotation = EditorGUILayout.Toggle(Content.AllowRotation, tile.AllowRotation);
			tile.AllowImmediateRepeats = EditorGUILayout.Toggle(Content.AllowImmediateRepeats, tile.AllowImmediateRepeats);
			tile.OverrideAutomaticTileBounds = EditorGUILayout.Toggle(Content.OverrideAutomaticTileBounds, tile.OverrideAutomaticTileBounds);

			if (tile.OverrideAutomaticTileBounds)
			{
				tile.TileBoundsOverride = EditorGUILayout.BoundsField(Content.OverriddenBounds, tile.TileBoundsOverride);

				if(GUILayout.Button(Content.FitToTile))
					tile.TileBoundsOverride = tile.transform.InverseTransformBounds(UnityUtil.CalculateObjectBounds(tile.gameObject, false, false));
			}

			EditorGUILayout.BeginVertical("box");
			EditorGUILayout.HelpBox("You can optionally designate doorways as the entrance / exit for this tile", MessageType.Info);

			tile.Entrance = EditorGUILayout.ObjectField(Content.Entrance, tile.Entrance, typeof(Doorway), true) as Doorway;
			tile.Exit = EditorGUILayout.ObjectField(Content.Exit, tile.Exit, typeof(Doorway), true) as Doorway;

			EditorGUILayout.EndVertical();

            if (GUI.changed)
                EditorUtility.SetDirty(tile);
        }
    }
}