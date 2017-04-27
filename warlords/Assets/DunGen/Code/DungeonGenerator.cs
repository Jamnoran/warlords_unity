using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using DunGen.Graph;
using DunGen.Analysis;

using Random = System.Random;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Collections;
using DunGen.Adapters;

namespace DunGen
{
	public delegate void TileInjectionDelegate(Random randomStream, ref List<InjectedTile> tilesToInject);

	[Serializable]
	public class DungeonGenerator
	{
		public int Seed;
		public bool ShouldRandomizeSeed = true;
		public Random RandomStream { get; protected set; }
		public int MaxAttemptCount = 20;
		public bool IgnoreSpriteBounds = false;
		public Vector3 UpVector = Vector3.up;
		public bool OverrideAllowImmediateRepeats = false;
		public bool AllowImmediateRepeats = false;
		public bool OverrideAllowTileRotation = false;
		public bool AllowTileRotation = false;
		public bool DebugRender = false;
		public bool AllowBacktracking = true;
		public float LengthMultiplier = 1.0f;
		public bool UseLegacyWeightCombineMethod = false;
		public bool PlaceTileTriggers = true;
		public int TileTriggerLayer = 2;

		public GameObject Root;
		public DungeonFlow DungeonFlow;
		public event GenerationStatusDelegate OnGenerationStatusChanged;
		public event TileInjectionDelegate TileInjectionMethods;
		public GenerationStatus Status { get; private set; }
		public GenerationStats GenerationStats { get; private set; }
		public int ChosenSeed { get; protected set; }
		public Dungeon CurrentDungeon { get { return currentDungeon; } }

		protected int retryCount;
		protected int roomRetryCount;
		protected Dungeon currentDungeon;
		protected readonly Dictionary<TilePlacementResult, int> tilePlacementResultCounters = new Dictionary<TilePlacementResult, int>();
		protected readonly List<PreProcessTileData> preProcessData = new List<PreProcessTileData>();
		protected readonly List<GameObject> useableTiles = new List<GameObject>();
		protected int targetLength;
		protected List<InjectedTile> tilesPendingInjection;

		private int nextNodeIndex;
		private DungeonArchetype currentArchetype;
		private GraphLine previousLineSegment;
		public bool isAnalysis;
		private GameObject lastTilePrefabUsed;
		private List<Doorway> allDoorways = new List<Doorway>();

		// Append dungeon
		private Dungeon appendedToDungeon;
		private bool allowAppendedDungeonIntersection;
		private Doorway appendedToDoorway;

		// Portal Culling
		public SerializableType PortalCullingAdapterClass = new SerializableType();
		public PortalCullingAdapter Culling;

		// We have to store portal culling parameters here for now since Unity annoyingly doesn't support serializing derived types - ideally this would go in SECTRPortalCullingAdapter
		public bool IsPortalCullingEnabled = true;
		public Light DirShadowCaster;
		public float ExtraBounds = 0.01f;
		public bool CullEachChild = false;


		public DungeonGenerator()
		{
			GenerationStats = new GenerationStats();
		}

		public DungeonGenerator(GameObject root)
			: this()
		{
			Root = root;
		}

		protected bool OuterGenerate(int? seed)
		{
			ShouldRandomizeSeed = !seed.HasValue;

			if (seed.HasValue)
				Seed = seed.Value;

			return Generate();
		}

		/// <summary>
		/// EXPERIMENTAL. Generate a dungeon that is appended to another. This is NOT supported functionality - it is intended as a starting point for potentially implementing pseudo-infinite dungeons
		/// </summary>
		/// <param name="appendTo">The previous dungeon</param>
		/// <param name="allowIntersection">Are intersections allowed? NOTE: Setting this to false will likely cause the generation to fail. If true, you'll need some way of handling overlaps;
		/// Portal culling such as SECTR VIS will handle the visual side, but you'll still collide with overlapping Tiles.</param>
		/// <returns></returns>
		public bool GenerateAppended(Dungeon appendTo, bool allowIntersection)
		{
			if (appendTo == currentDungeon)
				DetachDungeon();

			appendedToDungeon = appendTo;
			allowAppendedDungeonIntersection = allowIntersection;

			bool wasSuccess = Generate();

			if (wasSuccess)
			{
				// Clear the end doorway of the previous dungeon
				foreach (var obj in appendedToDoorway.AddWhenNotInUse)
					if (obj != null)
						UnityUtil.Destroy(obj);
			}

			return wasSuccess;
		}

		public bool Generate()
		{
			isAnalysis = false;
			return OuterGenerate();
		}

		public Dungeon DetachDungeon()
		{
			if (currentDungeon == null)
				return null;

			Dungeon dungeon = currentDungeon;
			currentDungeon = null;
			Root = null;
			Clear();

			return dungeon;
		}

		protected virtual bool OuterGenerate()
		{
			Clear();

			Status = GenerationStatus.NotStarted;
			DungeonArchetypeValidator validator = new DungeonArchetypeValidator(DungeonFlow);

			if (!validator.IsValid())
			{
				ChangeStatus(GenerationStatus.Failed);
				return false;
			}

			ChosenSeed = (ShouldRandomizeSeed) ? new Random().Next() : Seed;
			RandomStream = new Random(ChosenSeed);

			if (Root == null)
				Root = new GameObject(Constants.DefaultDungeonRootName);

			bool success = InnerGenerate(false);

			if (!success)
				Clear();

			return success;
		}

		public GenerationAnalysis RunAnalysis(int iterations, float maximumAnalysisTime)
		{
			DungeonArchetypeValidator validator = new DungeonArchetypeValidator(DungeonFlow);

			// No need to validate outside of the editor
			if (Application.isEditor)
			{
				if (!validator.IsValid())
				{
					ChangeStatus(GenerationStatus.Failed);
					return null;
				}
			}

			bool prevShouldRandomizeSeed = ShouldRandomizeSeed;

			isAnalysis = true;
			ShouldRandomizeSeed = true;
			GenerationAnalysis analysis = new GenerationAnalysis(iterations);
			Stopwatch sw = Stopwatch.StartNew();

			for (int i = 0; i < iterations; i++)
			{
				if (maximumAnalysisTime > 0 && sw.Elapsed.TotalMilliseconds >= maximumAnalysisTime)
					break;

				if (OuterGenerate())
				{
					analysis.IncrementSuccessCount();
					analysis.Add(GenerationStats);
				}
			}

			Clear();

			analysis.Analyze();
			ShouldRandomizeSeed = prevShouldRandomizeSeed;

			return analysis;
		}

		public void RandomizeSeed()
		{
			Seed = new Random().Next();
		}

		protected virtual bool InnerGenerate(bool isRetry)
		{
			if (isRetry)
			{
				if (retryCount >= MaxAttemptCount && Application.isEditor)
				{
					string errorText =	"Failed to generate the dungeon " + MaxAttemptCount + " times.\n" +
										"This could indicate a problem with the way the tiles are set up. Try to make sure most rooms have more than one doorway and that all doorways are easily accessible.\n" +
										"Here are a list of all reasons a tile placement had to be retried:";
					
					foreach (var pair in tilePlacementResultCounters)
						if (pair.Value > 0)
							errorText += "\n" + pair.Key + " (x" + pair.Value + ")";

					Debug.LogError(errorText);
					ChangeStatus(GenerationStatus.Failed);
					return false;
				}

				retryCount++;
				GenerationStats.IncrementRetryCount();
			}
			else
			{
				retryCount = 0;
				GenerationStats.Clear();
			}

			currentDungeon = Root.GetComponent<Dungeon>();
			if (currentDungeon == null)
				currentDungeon = Root.AddComponent<Dungeon>();

			currentDungeon.DebugRender = DebugRender;
			currentDungeon.PreGenerateDungeon(this);

			Clear();
			targetLength = Mathf.RoundToInt(DungeonFlow.Length.GetRandom(RandomStream) * LengthMultiplier);
			targetLength = Mathf.Max(targetLength, 2);

			// Tile Injection
			GenerationStats.BeginTime(GenerationStatus.TileInjection);

			if (tilesPendingInjection == null)
				tilesPendingInjection = new List<InjectedTile>();
			else
				tilesPendingInjection.Clear();

			GatherTilesToInject();

			// Pre-Processing
			GenerationStats.BeginTime(GenerationStatus.PreProcessing);
			PreProcess();

			// Main Path Generation
			GenerationStats.BeginTime(GenerationStatus.MainPath);
			if (!GenerateMainPath())
			{
				ChosenSeed = RandomStream.Next();
				RandomStream = new Random(ChosenSeed);

				return InnerGenerate(true);
			}

			// Branch Paths Generation
			GenerationStats.BeginTime(GenerationStatus.Branching);
			GenerateBranchPaths();

			// If there are any required tiles missing from the tile injection stage, the generation process should fail
			foreach (var tileInjection in tilesPendingInjection)
				if (tileInjection.IsRequired)
				{
					ChosenSeed = RandomStream.Next();
					RandomStream = new Random(ChosenSeed);

					return InnerGenerate(true);
				}

			// Post-Processing
			GenerationStats.BeginTime(GenerationStatus.PostProcessing);
			PostProcess();
			GenerationStats.EndTime();

			// Activate all door gameobjects that were added to doorways
			foreach (var door in currentDungeon.Doors)
				if (door != null)
					door.SetActive(true);

			CurrentDungeon.StartCoroutine(NotifyGenerationComplete());
			return true;
		}

		private IEnumerator NotifyGenerationComplete()
		{
			yield return null;

			ChangeStatus(GenerationStatus.Complete);

			// Let DungenCharacters know that they should re-check the Tile they're in
			foreach (var character in Component.FindObjectsOfType<DungenCharacter>())
				character.ForceRecheckTile();
		}

		public virtual void Clear()
		{
			if (currentDungeon != null)
				currentDungeon.Clear();

			foreach (var p in preProcessData)
				UnityUtil.Destroy(p.Proxy);

			useableTiles.Clear();
			preProcessData.Clear();

			if (Culling != null)
				Culling.Clear();

			tilePlacementResultCounters.Clear();
			allDoorways.Clear();
		}

		private void ChangeStatus(GenerationStatus status)
		{
			var previousStatus = Status;
			Status = status;

			if (previousStatus != status && OnGenerationStatusChanged != null)
				OnGenerationStatusChanged(this, status);
		}

		protected virtual void PreProcess()
		{
			if (preProcessData.Count > 0)
				return;

			ChangeStatus(GenerationStatus.PreProcessing);

			var usedTileSets = DungeonFlow.GetUsedTileSets().Concat(tilesPendingInjection.Select(x => x.TileSet)).Distinct();

			foreach (var tileSet in usedTileSets)
				foreach (var tile in tileSet.TileWeights.Weights)
				{
					if (tile.Value != null)
					{
						useableTiles.Add(tile.Value);
						tile.TileSet = tileSet;
					}
				}


			// Portal Culling
			if(Culling == null && PortalCullingAdapterClass.Type != null)
				Culling = Activator.CreateInstance(PortalCullingAdapterClass.Type) as PortalCullingAdapter;
		}

		protected virtual void GatherTilesToInject()
		{
			Random injectionRandomStream = new Random(ChosenSeed);

			// Gather from DungeonFlow
			foreach(var rule in DungeonFlow.TileInjectionRules)
			{
				// Ignore invalid rules
				if (rule.TileSet == null || (!rule.CanAppearOnMainPath && !rule.CanAppearOnBranchPath))
					continue;

				bool isOnMainPath = (!rule.CanAppearOnBranchPath) ? true : (!rule.CanAppearOnMainPath) ? false : injectionRandomStream.NextDouble() > 0.5;

				InjectedTile injectedTile = new InjectedTile(	rule.TileSet,
																isOnMainPath,
																rule.NormalizedPathDepth.GetRandom(injectionRandomStream),
																rule.NormalizedBranchDepth.GetRandom(injectionRandomStream),
																rule.IsRequired);

				tilesPendingInjection.Add(injectedTile);
			}

			// Gather from external delegates
			if (TileInjectionMethods != null)
				TileInjectionMethods(injectionRandomStream, ref tilesPendingInjection);
		}

		protected virtual bool GenerateMainPath()
		{
			ChangeStatus(GenerationStatus.MainPath);
			nextNodeIndex = 0;
			List<GraphNode> handledNodes = new List<GraphNode>(DungeonFlow.Nodes.Count);
			bool isDone = false;
			int i = 0;

			// Keep track of these now, we'll need them later when we know the actual length of the dungeon
			List<List<TileSet>> tiles = new List<List<TileSet>>(targetLength);
			List<DungeonArchetype> archetypes = new List<DungeonArchetype>(targetLength);
			List<GraphNode> nodes = new List<GraphNode>(targetLength);
			List<GraphLine> lines = new List<GraphLine>(targetLength);

			// We can't rigidly stick to the target length since we need at least one room for each node and that might be more than targetLength
			while (!isDone)
			{
				float depth = Mathf.Clamp(i / (float)(targetLength - 1), 0, 1);
				GraphLine lineSegment = DungeonFlow.GetLineAtDepth(depth);

				// This should never happen
				if (lineSegment == null)
					return false;

				// We're on a new line segment, change the current archetype
				if (lineSegment != previousLineSegment)
				{
					currentArchetype = lineSegment.DungeonArchetypes[RandomStream.Next(0, lineSegment.DungeonArchetypes.Count)];
					previousLineSegment = lineSegment;
				}

				List<TileSet> useableTileSets = null;
				GraphNode nextNode = null;
				var orderedNodes = DungeonFlow.Nodes.OrderBy(x => x.Position).ToArray();

				// Determine which node comes next
				foreach (var node in orderedNodes)
				{
					if (depth >= node.Position && !handledNodes.Contains(node))
					{
						nextNode = node;
						handledNodes.Add(node);
						break;
					}
				}

				// Assign the TileSets to use based on whether we're on a node or a line segment
				if (nextNode != null)
				{
					useableTileSets = nextNode.TileSets;
					nextNodeIndex = (nextNodeIndex >= orderedNodes.Length - 1) ? -1 : nextNodeIndex + 1;
					archetypes.Add(null);
					lines.Add(null);
					nodes.Add(nextNode);

					if (nextNode == orderedNodes[orderedNodes.Length - 1])
						isDone = true;
				}
				else
				{
					useableTileSets = currentArchetype.TileSets;
					archetypes.Add(currentArchetype);
					lines.Add(lineSegment);
					nodes.Add(null);
				}

				tiles.Add(useableTileSets);

				i++;
			}

			if (AllowBacktracking)
			{
				int tileRetryCount = 0;
				int totalForLoopRetryCount = 0;

				for (int j = 0; j < tiles.Count; j++)
				{
					var tile = AddTile((j == 0) ? null : currentDungeon.MainPathTiles[j - 1],
										tiles[j],
										j / (float)(tiles.Count - 1),
										archetypes[j]);

					// if no tile could be generated delete last successful tile and retry from previous index
					// else return false
					if (j > 5 && tile == null && tileRetryCount < 5 && totalForLoopRetryCount < 20)
					{
						Tile previousTile = currentDungeon.MainPathTiles[j - 1];

						foreach (var doorway in previousTile.Placement.AllDoorways)
							allDoorways.Remove(doorway);

						currentDungeon.RemoveLastConnection();
						currentDungeon.RemoveTile(previousTile);
						UnityUtil.Destroy(previousTile.gameObject);

						j -= 2; // -2 because loop adds 1
						tileRetryCount++;
						totalForLoopRetryCount++;
					}
					else if (tile == null)
						return false;
					else
					{
						tile.Node = nodes[j];
						tile.Line = lines[j];
						tileRetryCount = 0;
					}
				}
			}
			else
			{
				for (int j = 0; j < tiles.Count; j++)
				{
					var tile = AddTile((j == 0) ? null : currentDungeon.MainPathTiles[j - 1],
										tiles[j],
										j / (float)(tiles.Count - 1),
										archetypes[j]);

					// Return false if no tile could be generated
					if (tile == null)
						return false;
					else
					{
						tile.Node = nodes[j];
						tile.Line = lines[j];
					}
				}
			}

			return true;
		}

		protected virtual void GenerateBranchPaths()
		{
			ChangeStatus(GenerationStatus.Branching);

			foreach (var tile in currentDungeon.MainPathTiles)
			{
				// This tile was created from a graph node, there should be no branching
				if (tile.Archetype == null)
					continue;

				int branchCount = tile.Archetype.BranchCount.GetRandom(RandomStream);
				branchCount = Mathf.Min(branchCount, tile.Placement.UnusedDoorways.Count);

				if (branchCount == 0)
					continue;

				for (int i = 0; i < branchCount; i++)
				{
					Tile previousTile = tile;
					int branchDepth = tile.Archetype.BranchingDepth.GetRandom(RandomStream);

					for (int j = 0; j < branchDepth; j++)
					{
						List<TileSet> useableTileSets;

						if (j == (branchDepth - 1) && tile.Archetype.GetHasValidBranchCapTiles())
						{
							if (tile.Archetype.BranchCapType == BranchCapType.InsteadOf)
								useableTileSets = tile.Archetype.BranchCapTileSets;
							else
								useableTileSets = tile.Archetype.TileSets.Concat(tile.Archetype.BranchCapTileSets).ToList();
						}
						else
							useableTileSets = tile.Archetype.TileSets;

						float normalizedDepth = (branchDepth <= 1) ? 1 : j / (float)(branchDepth - 1);
						Tile newTile = AddTile(previousTile, useableTileSets, normalizedDepth, tile.Archetype);

						if (newTile == null)
							continue;

						newTile.Placement.BranchDepth = j;
						newTile.Placement.NormalizedBranchDepth = normalizedDepth;
						newTile.Node = previousTile.Node;
						newTile.Line = previousTile.Line;
						previousTile = newTile;
					}
				}
			}
		}

		protected virtual Tile AddTile(Tile attachTo, IList<TileSet> useableTileSets, float normalizedDepth, DungeonArchetype archetype, TilePlacementResult result = TilePlacementResult.None)
		{
			// This is a new attempt, reset the retry counter
			if (result == TilePlacementResult.None)
				roomRetryCount = 0;
			else
			{
				int currentResultCount = 0;
				tilePlacementResultCounters.TryGetValue(result, out currentResultCount);
				tilePlacementResultCounters[result] = currentResultCount + 1;

				roomRetryCount++;

				if (roomRetryCount > MaxAttemptCount)
					return null;
			}

			bool isOnMainPath = (Status == GenerationStatus.MainPath);

			// Use the previous dungeon's goal as our start tile if we're appending
			if (attachTo == null && appendedToDungeon != null)
				attachTo = appendedToDungeon.MainPathTiles[appendedToDungeon.MainPathTiles.Count - 1];

			// Check list of tiles to inject
			InjectedTile chosenInjectedTile = null;
			int injectedTileIndexToRemove = -1;

			bool isPlacingSpecificRoom = isOnMainPath && (archetype == null);

			if (tilesPendingInjection != null && !isPlacingSpecificRoom)
			{
				float pathDepth = (isOnMainPath) ? normalizedDepth : attachTo.Placement.PathDepth / (float)(targetLength - 1);
				float branchDepth = (isOnMainPath) ? 0 : normalizedDepth;

				for (int i = 0; i < tilesPendingInjection.Count; i++)
				{
					var injectedTile = tilesPendingInjection[i];

					if (injectedTile.ShouldInjectTileAtPoint(isOnMainPath, pathDepth, branchDepth))
					{
						chosenInjectedTile = injectedTile;
						injectedTileIndexToRemove = i;

						break;
					}
				}
			}

			Doorway fromDoorway = (attachTo == null) ? null : attachTo.Placement.PickRandomDoorway(RandomStream, true, archetype);

			if (attachTo != null && fromDoorway == null)
				return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.NoFromDoorway);

			if (appendedToDungeon != null && appendedToDoorway == null)
				appendedToDoorway = fromDoorway;


			// Select appropriate tile weights
			GameObjectChanceTable tileWeights = null;

			if (chosenInjectedTile != null)
				tileWeights = chosenInjectedTile.TileSet.TileWeights.Clone();
			else
			{
				if (UseLegacyWeightCombineMethod)
				{
					var chosenTileSet = useableTileSets[RandomStream.Next(0, useableTileSets.Count)];
					tileWeights = chosenTileSet.TileWeights.Clone();
				}
				else
				{
					var tables = useableTileSets.Select(x => x.TileWeights).ToArray();
					tileWeights = GameObjectChanceTable.Combine(tables);
				}
			}


			if (attachTo != null)
			{
				for (int i = tileWeights.Weights.Count - 1; i >= 0; i--)
				{
					var c = tileWeights.Weights[i];
					var cTemplate = GetTileTemplate(c.Value);

					if (cTemplate == null || !cTemplate.DoorwaySockets.Contains(fromDoorway.SocketGroup))
						tileWeights.Weights.RemoveAt(i);
				}
			}

			if (tileWeights.Weights.Count == 0)
				return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.NoTilesWithMatchingDoorway);

			bool allowRepeatTile = (attachTo != null) ? attachTo.AllowImmediateRepeats : true;

			if (OverrideAllowImmediateRepeats)
				allowRepeatTile = AllowImmediateRepeats;

			GameObjectChance chosenEntry = tileWeights.GetRandom(RandomStream, isOnMainPath, normalizedDepth, lastTilePrefabUsed, allowRepeatTile);

			// Couldn't find a valid Tile to use
			if (chosenEntry == null)
				return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.NoValidTile);

			GameObject tilePrefab = chosenEntry.Value;
			var toTemplate = GetTileTemplate(tilePrefab);

			if (toTemplate == null)
				return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.TemplateIsNull);

			int toDoorwayIndex = 0;
			Doorway toDoorway = null;

			if (fromDoorway != null)
			{
				Tile toTile = toTemplate.Prefab.GetComponent<Tile>();
				Vector3? allowedDirection;

				bool shouldAllowRotation = (toTile == null) ? true : toTile.AllowRotation;

				if (OverrideAllowTileRotation)
					shouldAllowRotation = AllowTileRotation;

				if (shouldAllowRotation)
				{
					// Enforce facing direction for vertical doorways
					if (fromDoorway.transform.forward == UpVector)
						allowedDirection = -UpVector;
					else if (fromDoorway.transform.forward == -UpVector)
						allowedDirection = UpVector;
					else
						allowedDirection = null;
				}
				else
					allowedDirection = -fromDoorway.transform.forward;

				if (!toTemplate.ChooseRandomDoorway(RandomStream, fromDoorway.SocketGroup, allowedDirection, out toDoorwayIndex, out toDoorway))
					return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.NoMatchingDoorwayInTile);

				// Move the proxy object into position
				GameObject toProxyDoor = toTemplate.ProxySockets[toDoorwayIndex];
				UnityUtil.PositionObjectBySocket(toTemplate.Proxy, toProxyDoor, fromDoorway.gameObject);

				if (IsCollidingWithAnyTile(toTemplate.Proxy))
					return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.TileIsColliding);
			}

			TilePlacementData newTile = new TilePlacementData(toTemplate, (Status == GenerationStatus.MainPath), archetype, chosenEntry.TileSet, currentDungeon);

			if (newTile == null)
				return AddTile(attachTo, useableTileSets, normalizedDepth, archetype, TilePlacementResult.NewTileIsNull);

			if (newTile.IsOnMainPath)
			{
				if (attachTo != null)
					newTile.PathDepth = attachTo.Placement.PathDepth + 1;
			}
			else
			{
				newTile.PathDepth = attachTo.Placement.PathDepth;
				newTile.BranchDepth = (attachTo.Placement.IsOnMainPath) ? 0 : attachTo.Placement.BranchDepth + 1;
			}

			if (fromDoorway != null)
			{
				// Moving enabled objects is very slow in the editor so we disable it first
				if (!Application.isPlaying)
					newTile.Root.SetActive(false);

				newTile.Root.transform.parent = Root.transform;
				toDoorway = newTile.AllDoorways[toDoorwayIndex];

				UnityUtil.PositionObjectBySocket(newTile.Root, toDoorway.gameObject, fromDoorway.gameObject);

				// Remember to re-enable any object we disabled earlier
				if (!Application.isPlaying)
					newTile.Root.SetActive(true);

				currentDungeon.MakeConnection(fromDoorway, toDoorway, RandomStream);
			}
			else
			{
				newTile.Root.transform.parent = Root.transform;
				newTile.Root.transform.localPosition = Vector3.zero;
			}

			// We've successfully injected the tile, so we can remove it from the pending list now
			if (chosenInjectedTile != null)
			{
				tilesPendingInjection.RemoveAt(injectedTileIndexToRemove);

				if (isOnMainPath)
					targetLength++;
			}

			currentDungeon.AddTile(newTile.Tile);
			newTile.RecalculateBounds(IgnoreSpriteBounds, UpVector);
			lastTilePrefabUsed = tilePrefab;

			if (PlaceTileTriggers)
			{
				newTile.Tile.AddTriggerVolume();
				newTile.Root.layer = TileTriggerLayer;
			}

			allDoorways.AddRange(newTile.AllDoorways);
			return newTile.Tile;
		}

		protected PreProcessTileData GetTileTemplate(GameObject prefab)
		{
			var template = preProcessData.Where(x => { return x.Prefab == prefab; }).FirstOrDefault();

			// No proxy has been loaded yet, we should create one
			if (template == null)
			{
				template = new PreProcessTileData(prefab, IgnoreSpriteBounds, UpVector);
				preProcessData.Add(template);
			}

			return template;
		}

		protected PreProcessTileData PickRandomTemplate(DoorwaySocketType? socketGroupFilter)
		{
			// Pick a random tile
			var tile = useableTiles[RandomStream.Next(0, useableTiles.Count)];
			var template = GetTileTemplate(tile);

			// If there's a socket group filter and the chosen Tile doesn't have a socket of this type, try again
			if (socketGroupFilter.HasValue && !template.DoorwaySockets.Contains(socketGroupFilter.Value))
				return PickRandomTemplate(socketGroupFilter);

			return template;
		}

		protected int NormalizedDepthToIndex(float normalizedDepth)
		{
			return Mathf.RoundToInt(normalizedDepth * (targetLength - 1));
		}

		protected float IndexToNormalizedDepth(int index)
		{
			return index / (float)targetLength;
		}

		protected bool IsCollidingWithAnyTile(GameObject proxy)
		{
			foreach (var r in currentDungeon.AllTiles)
				if (r.Placement.Bounds.Intersects(proxy.GetComponent<Collider>().bounds))
					return true;

			// Check previous dungeon for intersections
			if (appendedToDungeon != null && !allowAppendedDungeonIntersection)
			{
				foreach (var r in appendedToDungeon.AllTiles)
					if (r.Placement.Bounds.Intersects(proxy.GetComponent<Collider>().bounds))
						return true;
			}

			return false;
		}

		protected void ClearPreProcessData()
		{
			foreach (var p in preProcessData)
				UnityUtil.Destroy(p.Proxy);

			preProcessData.Clear();
		}

		protected virtual void ConnectOverlappingDoorways(float percentageChance)
		{
			if (percentageChance <= 0)
				return;

			List<Doorway> processedDoorways = new List<Doorway>(allDoorways.Count);

			const float epsilon = 0.00001f;

			foreach (var a in allDoorways)
			{
				foreach (var b in allDoorways)
				{
					if (a == b)
						continue;

					if (a.Tile == b.Tile)
						continue;

					if (processedDoorways.Contains(b))
						continue;

					if (!DoorwaySocket.IsMatchingSocket(a.SocketGroup, b.SocketGroup))
						continue;

					float distanceSqrd = (a.transform.position - b.transform.position).sqrMagnitude;

					if (distanceSqrd < epsilon)
					{
						if (RandomStream.NextDouble() < percentageChance)
							currentDungeon.MakeConnection(a, b, RandomStream);
					}
				}

				processedDoorways.Add(a);
			}
		}

		protected virtual void PostProcess()
		{
			ChangeStatus(GenerationStatus.PostProcessing);

			foreach (var tile in currentDungeon.AllTiles)
				tile.gameObject.SetActive(true);

			int length = currentDungeon.MainPathTiles.Count;

			//int maxBranchDepth = currentDungeon.BranchPathTiles.OrderByDescending(x => x.Placement.BranchDepth).Select(x => x.Placement.BranchDepth).FirstOrDefault();

			//
			// Need to sort list manually to avoid compilation problems on iOS
			int maxBranchDepth = 0;

			if (currentDungeon.BranchPathTiles.Count > 0)
			{
				List<Tile> branchTiles = currentDungeon.BranchPathTiles.ToList();
				branchTiles.Sort((a, b) =>
				{
					return b.Placement.BranchDepth.CompareTo(a.Placement.BranchDepth);
				}
				);

				maxBranchDepth = branchTiles[0].Placement.BranchDepth;
			}
			// End calculate max branch depth
			//

			if (!isAnalysis)
			{
				ConnectOverlappingDoorways(DungeonFlow.DoorwayConnectionChance);

				foreach (var tile in currentDungeon.AllTiles)
				{
					tile.Placement.NormalizedPathDepth = tile.Placement.PathDepth / (float)(length - 1);
					tile.Placement.ProcessDoorways(RandomStream);
				}

				currentDungeon.PostGenerateDungeon(this);

				if(DungeonFlow.KeyManager != null)
					PlaceLocksAndKeys();

				// Process random props
				foreach (var tile in currentDungeon.AllTiles)
					foreach (var prop in tile.GetComponentsInChildren<RandomProp>())
						prop.Process(RandomStream, tile);

				ProcessGlobalProps();
			}

			GenerationStats.SetRoomStatistics(currentDungeon.MainPathTiles.Count, currentDungeon.BranchPathTiles.Count, maxBranchDepth);
			ClearPreProcessData();

			// Handle portal culling setup if enabled
            if (Culling != null && IsPortalCullingEnabled && !isAnalysis)
            {
                Culling.Clear();

				Culling.PrepareForCulling(this, currentDungeon);
				CurrentDungeon.Culling = Culling.Clone();
            }

			// Ensure all culling states are properly set for doorways
			foreach (var door in Component.FindObjectsOfType<Door>())
				door.IsOpen = door.IsOpen;
		}

		protected virtual void ProcessGlobalProps()
		{
			Dictionary<int, GameObjectChanceTable> globalPropWeights = new Dictionary<int, GameObjectChanceTable>();

			foreach (var tile in currentDungeon.AllTiles)
			{
				foreach (var prop in tile.GetComponentsInChildren<GlobalProp>())
				{
					GameObjectChanceTable table = null;

					if (!globalPropWeights.TryGetValue(prop.PropGroupID, out table))
					{
						table = new GameObjectChanceTable();
						globalPropWeights[prop.PropGroupID] = table;
					}

					float weight = (tile.Placement.IsOnMainPath) ? prop.MainPathWeight : prop.BranchPathWeight;
					weight *= prop.DepthWeightScale.Evaluate(tile.Placement.NormalizedDepth);

					table.Weights.Add(new GameObjectChance(prop.gameObject, weight, 0, null));
				}
			}

			foreach (var chanceTable in globalPropWeights.Values)
				foreach (var weight in chanceTable.Weights)
					weight.Value.SetActive(false);

			List<int> processedPropGroups = new List<int>(globalPropWeights.Count);

			foreach (var pair in globalPropWeights)
			{
				if (processedPropGroups.Contains(pair.Key))
				{
					Debug.LogWarning("Dungeon Flow contains multiple entries for the global prop group ID: " + pair.Key + ". Only the first entry will be used.");
					continue;
				}

				int index = DungeonFlow.GlobalPropGroupIDs.IndexOf(pair.Key);

				if (index == -1)
					continue;

				IntRange range = DungeonFlow.GlobalPropRanges[index];

				var weights = pair.Value.Clone();
				int propCount = range.GetRandom(RandomStream);
				propCount = Mathf.Clamp(propCount, 0, weights.Weights.Count);

				// Commented code allows Global props to be deleted rather than just disabled
				//List<GameObject> chosenEntries = new List<GameObject>(propCount);

				for (int i = 0; i < propCount; i++)
				{
					var chosenEntry = weights.GetRandom(RandomStream, true, 0, null, true, true);

					if (chosenEntry != null && chosenEntry.Value != null)
					{
						chosenEntry.Value.SetActive(true);
						//chosenEntries.Add(chosenEntry.Value);
					}
				}

				//foreach (var w in weights.Weights)
				//	if (!chosenEntries.Contains(w.Value))
				//		GameObject.Destroy(w.Value);

				processedPropGroups.Add(pair.Key);
			}
		}

		protected virtual void PlaceLocksAndKeys()
		{
			var nodes = currentDungeon.ConnectionGraph.Nodes.Select(x => x.Tile.Node).Where(x => { return x != null; }).Distinct().ToArray();
			var lines = currentDungeon.ConnectionGraph.Nodes.Select(x => x.Tile.Line).Where(x => { return x != null; }).Distinct().ToArray();

			Dictionary<Doorway, Key> lockedDoorways = new Dictionary<Doorway, Key>();

			// Lock doorways on nodes
			foreach (var node in nodes)
			{
				foreach (var l in node.Locks)
				{
					var tile = currentDungeon.AllTiles.Where(x => { return x.Node == node; }).FirstOrDefault();
					var connections = currentDungeon.ConnectionGraph.Nodes.Where(x => { return x.Tile == tile; }).FirstOrDefault().Connections;
					Doorway entrance = null;
					Doorway exit = null;

					foreach (var conn in connections)
					{
						if (conn.DoorwayA.Tile == tile)
							exit = conn.DoorwayA;
						else if (conn.DoorwayB.Tile == tile)
							entrance = conn.DoorwayB;
					}

					var key = node.Graph.KeyManager.GetKeyByID(l.ID);

					if (key.Prefab != null)
					{
						if (entrance != null && (node.LockPlacement & NodeLockPlacement.Entrance) == NodeLockPlacement.Entrance)
							lockedDoorways.Add(entrance, key);

						if (exit != null && (node.LockPlacement & NodeLockPlacement.Exit) == NodeLockPlacement.Exit)
							lockedDoorways.Add(exit, key);
					}
					else
						Debug.LogError("Key with ID " + l.ID + " does not have a prefab to place");
				}
			}

			// Lock doorways on lines
			foreach (var line in lines)
			{
				var doorways = currentDungeon.ConnectionGraph.Connections.Where(x =>
				{
					bool isDoorwayAlreadyLocked = lockedDoorways.ContainsKey(x.DoorwayA) || lockedDoorways.ContainsKey(x.DoorwayB);
					bool doorwayHasLockPrefabs = x.DoorwayA.Tile.TileSet.LockPrefabs.Count > 0;

					return x.DoorwayA.Tile.Line == line &&
							x.DoorwayB.Tile.Line == line &&
							!isDoorwayAlreadyLocked &&
							doorwayHasLockPrefabs;

				}).Select(x => x.DoorwayA).ToList();

				if (doorways.Count == 0)
					continue;

				foreach (var l in line.Locks)
				{
					int lockCount = l.Range.GetRandom(RandomStream);
					lockCount = Mathf.Clamp(lockCount, 0, doorways.Count);

					for (int i = 0; i < lockCount; i++)
					{
						if (doorways.Count == 0)
							break;

						var doorway = doorways[RandomStream.Next(0, doorways.Count)];
						doorways.Remove(doorway);

						if (lockedDoorways.ContainsKey(doorway))
							continue;

						var key = line.Graph.KeyManager.GetKeyByID(l.ID);
						lockedDoorways.Add(doorway, key);
					}
				}
			}

			List<Doorway> locksToRemove = new List<Doorway>();

			foreach (var pair in lockedDoorways)
			{
				var door = pair.Key;
				var key = pair.Value;
				List<Tile> possibleSpawnTiles = new List<Tile>();

				foreach (var t in currentDungeon.AllTiles)
				{
					if (t.Placement.NormalizedPathDepth >= door.Tile.Placement.NormalizedPathDepth)
						continue;

					bool canPlaceKey = false;

					if (t.Node != null && t.Node.Keys.Where(x => { return x.ID == key.ID; }).Count() > 0)
						canPlaceKey = true;
					else if (t.Line != null && t.Line.Keys.Where(x => { return x.ID == key.ID; }).Count() > 0)
						canPlaceKey = true;

					if (!canPlaceKey)
						continue;

					//if (!door.Tile.Placement.IsOnMainPath)
					//{
					//    if (t.Placement.NormalizedBranchDepth >= door.Tile.Placement.NormalizedBranchDepth)
					//        continue;
					//}

					possibleSpawnTiles.Add(t);
				}

				var possibleSpawnComponents = possibleSpawnTiles.SelectMany(x => x.GetComponentsInChildren<Component>().OfType<IKeySpawnable>()).ToList();

				if (possibleSpawnComponents.Count == 0)
					locksToRemove.Add(door);
				else
				{
					int keysToSpawn = key.KeysPerLock.GetRandom(RandomStream);
					keysToSpawn = Math.Min(keysToSpawn, possibleSpawnComponents.Count);

					for (int i = 0; i < keysToSpawn; i++)
					{
						int chosenCompID = RandomStream.Next(0, possibleSpawnComponents.Count);
						var comp = possibleSpawnComponents[chosenCompID];
						comp.SpawnKey(key, DungeonFlow.KeyManager);

						foreach (var k in (comp as Component).GetComponentsInChildren<Component>().OfType<IKeyLock>())
							k.OnKeyAssigned(key, DungeonFlow.KeyManager);

						possibleSpawnComponents.RemoveAt(chosenCompID);
					}
				}
			}

			foreach (var door in locksToRemove)
				lockedDoorways.Remove(door);

			foreach (var pair in lockedDoorways)
			{
				pair.Key.RemoveUsedPrefab();
				LockDoorway(pair.Key, pair.Value, DungeonFlow.KeyManager);
			}
		}

		protected virtual void LockDoorway(Doorway doorway, Key key, KeyManager keyManager)
		{
			var placement = doorway.Tile.Placement;
			var prefabs = doorway.Tile.TileSet.LockPrefabs.Where(x => { return DoorwaySocket.IsMatchingSocket(x.SocketGroup, doorway.SocketGroup); }).Select(x => x.LockPrefabs).ToArray();

			if (prefabs.Length == 0)
				return;

			var chosenEntry = prefabs[RandomStream.Next(0, prefabs.Length)].GetRandom(RandomStream, placement.IsOnMainPath, placement.NormalizedDepth, null, true);
			var prefab = chosenEntry.Value;

			GameObject doorObj = (GameObject)GameObject.Instantiate(prefab);
			doorObj.transform.parent = Root.transform;
			doorObj.transform.position = doorway.transform.position;
			doorObj.transform.rotation = doorway.transform.rotation;

			// Set this locked door as the current door prefab
			doorway.SetUsedPrefab(doorObj);
			doorway.ConnectedDoorway.SetUsedPrefab(doorObj);

			DungeonUtil.AddAndSetupDoorComponent(CurrentDungeon, doorObj, doorway);

			foreach (var keylock in doorObj.GetComponentsInChildren<Component>().OfType<IKeyLock>())
				keylock.OnKeyAssigned(key, keyManager);
		}
	}
}
