using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DunGen
{
	[AddComponentMenu("DunGen/Culling/Basic Room Culling Camera")]
	public class BasicRoomCullingCamera : MonoBehaviour
	{
		public int AdjacentTileDepth = 1;
		public bool CullBehindClosedDoors = true;
		public Transform TargetOverride;

		protected bool isReady;
		protected bool isCulling;
		protected bool isDirty;
		protected DungeonGenerator generator;
		protected Tile currentTile;
		protected List<Tile> allTiles;
		protected List<Door> allDoors;
		protected List<Tile> visibleTiles;
		protected Dictionary<Tile, Dictionary<Renderer, bool>> rendererVisibilities = new Dictionary<Tile, Dictionary<Renderer, bool>>();


		protected virtual void Awake()
		{
			var runtimeDungeon = FindObjectOfType<RuntimeDungeon>();

			if (runtimeDungeon != null)
			{
				generator = runtimeDungeon.Generator;
				generator.OnGenerationStatusChanged += OnDungeonGenerationStatusChanged;
			}
		}

		protected virtual void OnDestroy()
		{
			if (generator != null)
				generator.OnGenerationStatusChanged -= OnDungeonGenerationStatusChanged;
		}

		protected virtual void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			if (status == GenerationStatus.Complete)
				SetDungeon(generator.CurrentDungeon);
			else if (status == GenerationStatus.Failed)
				ClearDungeon();
		}

		protected virtual void OnPreCull()
		{
			if (isReady)
				SetIsCulling(true);
		}

		protected virtual void OnPostRender()
		{
			if (isReady)
				SetIsCulling(false);
		}

		protected virtual void LateUpdate()
		{
			if (!isReady)
				return;

			Transform target = (TargetOverride != null) ? TargetOverride : transform;
			bool hasPositionChanged = currentTile == null || !currentTile.Bounds.Contains(target.position);

			if (hasPositionChanged)
			{
				// Update current tile
				foreach (var tile in allTiles)
				{
					if (tile == null)
						continue;

					if (tile.Bounds.Contains(target.position))
					{
						currentTile = tile;
						break;
					}
				}

				isDirty = true;
			}

			if (isDirty)
				UpdateCulling();

			// Update the list of renderers for tiles about to be culled
			foreach (var tile in allTiles)
				if (!visibleTiles.Contains(tile))
					UpdateRendererList(tile);
		}

		protected void UpdateRendererList(Tile tile)
		{
			Dictionary<Renderer, bool> renderers;

			if (!rendererVisibilities.TryGetValue(tile, out renderers))
				rendererVisibilities[tile] = renderers = new Dictionary<Renderer, bool>();

#if UNITY_5
			UpdateRenderersInChildren(tile.gameObject, ref renderers);
#else
			foreach (var renderer in tile.GetComponentsInChildren<Renderer>())
				renderers[renderer] = renderer.enabled;
#endif
		}

#if UNITY_5
		// A little verbose, but necessary to avoid generating runtime garbage
		private List<Renderer> rendererResults = new List<Renderer>();
		private void UpdateRenderersInChildren(GameObject obj, ref Dictionary<Renderer, bool> renderers)
		{
			rendererResults.Clear();
			obj.GetComponents(rendererResults);

			foreach (var renderer in rendererResults)
				renderers[renderer] = renderer.enabled;

			for (int i = 0; i < obj.transform.childCount; i++)
				UpdateRenderersInChildren(obj.transform.GetChild(i).gameObject, ref renderers);
		}
#endif

		protected void SetIsCulling(bool isCulling)
		{
			this.isCulling = isCulling;

			foreach (var tile in allTiles)
			{
				if (visibleTiles.Contains(tile))
					continue;

				Dictionary<Renderer, bool> renderers;
				if (rendererVisibilities.TryGetValue(tile, out renderers))
				{
					foreach (var renderer in renderers)
						if (renderer.Key != null)
							renderer.Key.enabled = (isCulling) ? false : renderer.Value;
				}
			}
		}

		protected void UpdateCulling()
		{
			visibleTiles.Clear();

			if (currentTile != null)
				visibleTiles.Add(currentTile);

			int processTileStart = 0;

			// Add neighbours down to RoomDepth (0 = just tiles containing characters, 1 = plus adjacent tiles, etc)
			for (int i = 0; i < AdjacentTileDepth; i++)
			{
				int processTileEnd = visibleTiles.Count;

				for (int t = processTileStart; t < processTileEnd; t++)
				{
					var tile = visibleTiles[t];

					// Get all connections to adjacent tiles
					foreach (var doorway in tile.Placement.UsedDoorways)
					{
						var adjacentTile = doorway.ConnectedDoorway.Tile;

						// Skip the tile if it's already visible
						if (visibleTiles.Contains(adjacentTile))
							continue;

						// No need to add adjacent rooms to the visible list when the door between them is closed
						if (CullBehindClosedDoors)
						{
							var door = doorway.DoorComponent;

							if (door != null && !door.IsOpen)
								continue;
						}

						visibleTiles.Add(adjacentTile);
					}
				}

				processTileStart = processTileEnd;
			}
		}

		public void SetDungeon(Dungeon dungeon)
		{
			if (isReady)
				ClearDungeon();

			if (dungeon == null)
				return;

			allTiles = new List<Tile>(dungeon.AllTiles);
			allDoors = new List<Door>(GetAllDoorsInDungeon(dungeon));
			visibleTiles = new List<Tile>(allTiles.Count);

			foreach (var door in GetAllDoorsInDungeon(dungeon))
				door.OnDoorStateChanged += OnDoorStateChanged;

			isReady = true;
			isDirty = true;
		}

		protected IEnumerable<Door> GetAllDoorsInDungeon(Dungeon dungeon)
		{
			foreach (var doorObj in dungeon.Doors)
			{
				if (doorObj == null)
					continue;

				var door = doorObj.GetComponent<Door>();

				if (door != null)
					yield return door;
			}
		}

		protected virtual void ClearDungeon()
		{
			foreach (var door in allDoors)
				door.OnDoorStateChanged -= OnDoorStateChanged;

			isReady = false;
		}

		protected virtual void OnDoorStateChanged(Door door, bool isOpen)
		{
			isDirty = true;
		}
	}
}
