using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using DunGen.Analysis;
using DunGen.Graph;

namespace DunGen.Editor
{
	public sealed class DungeonAnalysisWindow : EditorWindow
	{
		private DungeonGenerator generator = new DungeonGenerator();
		private GenerationAnalysis analysis;
		private int iterationCount = 10000;
		private float maximumAnalysisTime = 20;


		// Not accessible yet- the runtime analyzer is orders of magnitude faster and less error-prone
		//[MenuItem("Window/DunGen/Analyze Dungeon")]
		private static void OpenWindow()
		{
			EditorWindow.GetWindow<DungeonAnalysisWindow>(false, "Analyze", true);
		}
		
		private void OnGUI()
		{
			generator.DungeonFlow = (DungeonFlow)EditorGUILayout.ObjectField("Dungeon Flow", generator.DungeonFlow, typeof(DungeonFlow), false);
			generator.MaxAttemptCount = EditorGUILayout.IntField("Max Failed Attempts", generator.MaxAttemptCount);
			iterationCount = EditorGUILayout.IntField("Iterations", iterationCount);
			maximumAnalysisTime = EditorGUILayout.FloatField("Max Analysis Time (sec)", maximumAnalysisTime);
			
			if (GUILayout.Button("Generate"))
				analysis = generator.RunAnalysis(iterationCount, maximumAnalysisTime * 1000);

			if(analysis == null)
				return;

			EditorGUILayout.BeginVertical("box");

			EditorGUILayout.LabelField("Dungeon successfully generated: " + Mathf.RoundToInt(analysis.SuccessPercentage).ToString() + "%");

			EditorGUILayout.Space();
			EditorGUILayout.Space();

			DrawNumberSetData("Main Path Room Count", analysis.MainPathRoomCount);
			DrawNumberSetData("Branch Path Room Count", analysis.BranchPathRoomCount);
			DrawNumberSetData("Total Room Count", analysis.TotalRoomCount);
			DrawNumberSetData("Max Branch Depth", analysis.MaxBranchDepth);
			DrawNumberSetData("Retry Count", analysis.TotalRetries);

			DrawNumberSetData("Pre-Process Time", analysis.PreProcessTime);
			DrawNumberSetData("Main Path Generation Time", analysis.MainPathGenerationTime);
			DrawNumberSetData("Branch Path Generation Time", analysis.BranchPathGenerationTime);
			DrawNumberSetData("Post-Process Time", analysis.PostProcessTime);
			DrawNumberSetData("Total Time", analysis.TotalTime);

			EditorGUILayout.EndVertical();
		}

		private void DrawNumberSetData(string label, NumberSetData data)
		{
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
			EditorGUILayout.LabelField(string.Format("[{0} - {1}] (avg. {2}) Standard Deviation: {3}", data.Min, data.Max, data.Average, data.StandardDeviation));
		}
	}
}