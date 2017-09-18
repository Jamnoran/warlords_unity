using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using DunGen.Graph;

namespace DunGen.Editor
{
	public static class MenuMethods
	{
        [MenuItem("Assets/Create/DunGen/Dungeon Archetype")]
        public static DungeonArchetype CreateArchetype()
        {
            return EditorUtil.CreateAsset<DungeonArchetype>();
        }

        [MenuItem("Assets/Create/DunGen/Dungeon Flow")]
        public static void CreateFlow()
        {
            var flow = EditorUtil.CreateAsset<DungeonFlow>();
            flow.Reset();
        }

        [MenuItem("Assets/Create/DunGen/Tile Set")]
        public static TileSet CreateTileSet()
        {
            return EditorUtil.CreateAsset<TileSet>();
        }

		[MenuItem("Assets/Create/DunGen/Key Manager")]
		public static KeyManager CreateKeyManager()
		{
			return EditorUtil.CreateAsset<KeyManager>();
		}
	}
}
