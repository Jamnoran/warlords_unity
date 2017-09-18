using UnityEngine;
using System.Collections;

namespace DunGen.Adapters
{
	public abstract class NavMeshAdapter : MonoBehaviour
	{
		#region Helpers

		public struct NavMeshGenerationProgress
		{
			public float Percentage;
			public string Description;
		}

		public delegate void OnNavMeshGenerationProgress(NavMeshGenerationProgress progress);

		#endregion

		public OnNavMeshGenerationProgress OnProgress;

		public abstract void Generate(Dungeon dungeon);
	}
}