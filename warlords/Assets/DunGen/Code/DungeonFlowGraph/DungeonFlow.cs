using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen.Graph
{
    /// <summary>
    /// A graph representing the flow of a dungeon
    /// </summary>
    [Serializable]
	public class DungeonFlow : ScriptableObject
    {
        /// <summary>
        /// The minimum and maximum length of the dungeon
        /// </summary>
        public IntRange Length = new IntRange(5, 10);
        /// <summary>
        /// A list of IDs for global prop groups
        /// </summary>
        public List<int> GlobalPropGroupIDs = new List<int>();
        /// <summary>
        /// The number of each global prop group items allowed in a single dungeon
        /// </summary>
        public List<IntRange> GlobalPropRanges = new List<IntRange>();
		/// <summary>
		/// The asset that handles all of the keys that this dungeon needs to know about
		/// </summary>
		public KeyManager KeyManager;
		/// <summary>
		/// The percentage chance of two unconnected but overlapping doorways being connected (0-1)
		/// </summary>
		public float DoorwayConnectionChance;
		/// <summary>
		/// Simple rules for injecting special tiles into the dungeon generation process
		/// </summary>
		public List<TileInjectionRule> TileInjectionRules = new List<TileInjectionRule>();

        public List<GraphNode> Nodes = new List<GraphNode>();
        public List<GraphLine> Lines = new List<GraphLine>();


        /// <summary>
        /// Creates the default graph
        /// </summary>
        public void Reset()
        {
			var emptyTileSet = new TileSet[0];
			var emptyArchetype = new DungeonArchetype[0];

			var builder = new DungeonFlowBuilder(this)
				.AddNode(emptyTileSet, "Start")
				.AddLine(emptyArchetype, 1.0f)
				.AddNode(emptyTileSet, "Goal");

			builder.Complete();
        }

        public GraphLine GetLineAtDepth(float normalizedDepth)
        {
            normalizedDepth = Mathf.Clamp(normalizedDepth, 0, 1);

            if (normalizedDepth == 0)
                return Lines[0];
            else if (normalizedDepth == 1)
                return Lines[Lines.Count - 1];

            foreach (var line in Lines)
                if (normalizedDepth >= line.Position && normalizedDepth < line.Position + line.Length)
                    return line;

            Debug.LogError("GetLineAtDepth was unable to find a line at depth " + normalizedDepth + ". This shouldn't happen.");
            return null;
        }

        public DungeonArchetype[] GetUsedArchetypes()
        {
            return Lines.SelectMany(x => x.DungeonArchetypes).ToArray();
        }

        public TileSet[] GetUsedTileSets()
        {
            List<TileSet> tileSets = new List<TileSet>();

            foreach (var node in Nodes)
                tileSets.AddRange(node.TileSets);

            foreach(var line in Lines)
                foreach (var archetype in line.DungeonArchetypes)
                {
                    tileSets.AddRange(archetype.TileSets);
                    tileSets.AddRange(archetype.BranchCapTileSets);
                }

            return tileSets.ToArray();
            //return Nodes.SelectMany(x => x.TileSets).Concat(Lines.SelectMany(x => x.DungeonArchetypes).SelectMany(y => y.TileSets)).ToArray();
        }
    }
}
