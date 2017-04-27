using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DunGen.Adapters;

namespace DunGen
{
    [AddComponentMenu("DunGen/Runtime Dungeon")]
	public class RuntimeDungeon : MonoBehaviour
	{
        public DungeonGenerator Generator = new DungeonGenerator();
        public bool GenerateOnStart = true;
		public GameObject Root;


        protected virtual void Start()
        {
			Generator.OnGenerationStatusChanged += OnDungeonGenerationStatusChanged;

			if (GenerateOnStart)
				Generate();
		}

		public void Generate()
		{
			if(Root != null)
				Generator.Root = Root;

			Generator.Generate();
		}

		protected virtual void OnDungeonGenerationStatusChanged(DungeonGenerator generator, GenerationStatus status)
		{
			// Detect any NavMeshAdapters that are attached and use them to generate a NavMesh once the dungeon is done
			if (status == GenerationStatus.Complete)
			{
				var navMeshGenerator = GetComponent<NavMeshAdapter>();

				if (navMeshGenerator != null)
					navMeshGenerator.Generate(generator.CurrentDungeon);
			}
		}
	}
}
