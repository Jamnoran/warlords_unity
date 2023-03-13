using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DunGen.Graph;

namespace DunGen
{
    [AddComponentMenu("DunGen/Tile")]
	public class Tile : MonoBehaviour
    {
        /// <summary>
        /// Should this tile be allowed to rotate to fit in place?
        /// </summary>
        public bool AllowRotation = true;

		/// <summary>
		/// Should this tile be allowed to be placed next to another instance of itself?
		/// </summary>
		public bool AllowImmediateRepeats = true;

		/// <summary>
		/// Should the automatically generated tile bounds be overriden with a user-defined value?
		/// </summary>
		public bool OverrideAutomaticTileBounds = false;

		/// <summary>
		/// Optional tile bounds to override the automatically calculated tile bounds
		/// </summary>
		public Bounds TileBoundsOverride = new Bounds(Vector3.zero, Vector3.one);

		public Doorway Entrance;
		public Doorway Exit;

		/// <summary>
		/// The calculated world-space bounds of this Tile
		/// </summary>
		[HideInInspector]
		public Bounds Bounds { get { return transform.TransformBounds(Placement.LocalBounds); } }

		/// <summary>
		/// Information about the tile's position in the generated dungeon
		/// </summary>
		public TilePlacementData Placement
		{
			get { return placement; }
			internal set { placement = value; }
		}
        /// <summary>
        /// The Dungeon Archetype that is assigned to this tile (only applicable if this tile lay on a graph line)
        /// </summary>
        public DungeonArchetype Archetype
        {
            get { return archetype; }
            internal set { archetype = value; }
        }
        /// <summary>
        /// The TileSet that is assigned to this tile
        /// </summary>
        public TileSet TileSet
        {
            get { return tileSet; }
            internal set { tileSet = value; }
        }
        /// <summary>
        /// The flow graph node this tile was spawned from (only applicable if this tile lay on a graph node)
        /// </summary>
        public GraphNode Node
        {
            get { return (node == null) ? null : node.Node; }
            internal set
            {
                if (value == null)
                    node = null;
                else
                    node = new FlowNodeReference(value.Graph, value);
            }
        }
        /// <summary>
        /// The flow graph line this tile was spawned from (only applicable if this tile lay on a graph line)
        /// </summary>
        public GraphLine Line
        {
            get { return (line == null) ? null : line.Line; }
            internal set
            {
                if (value == null)
                    line = null;
                else
                    line = new FlowLineReference(value.Graph, value);
            }
        }
        /// <summary>
        /// The dungeon that this tile belongs to
        /// </summary>
        public Dungeon Dungeon { get; internal set; }

        [SerializeField]
        private TilePlacementData placement;

        [SerializeField]
        private DungeonArchetype archetype;
        [SerializeField]
        private TileSet tileSet;
        [SerializeField]
        private FlowNodeReference node;
        [SerializeField]
        private FlowLineReference line;


        internal void AddTriggerVolume()
        {
            BoxCollider triggerVolume = gameObject.AddComponent<BoxCollider>();
			triggerVolume.center = Placement.LocalBounds.center;
			triggerVolume.size = Placement.LocalBounds.size;
            triggerVolume.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other == null)
                return;

            DungenCharacter character = other.gameObject.GetComponent<DungenCharacter>();

            if (character != null)
                character.HandleTileChange(this);
        }

        private void OnDrawGizmosSelected()
        {
			Gizmos.color = Color.red;
			Bounds? bounds = null;


			if (OverrideAutomaticTileBounds)
				bounds = transform.TransformBounds(TileBoundsOverride);
			else if (placement != null)
				bounds = Bounds;

			if(bounds.HasValue)
				Gizmos.DrawWireCube(bounds.Value.center, bounds.Value.size);
		}

        public IEnumerable<Tile> GetAdjactedTiles()
        {
            return Placement.UsedDoorways.Select(x => x.ConnectedDoorway.Tile).Distinct();
        }

        public bool IsAdjacentTo(Tile other)
        {
            foreach (var door in Placement.UsedDoorways)
                if (door.ConnectedDoorway.Tile == other)
                    return true;

            return false;
        }
	}
}
