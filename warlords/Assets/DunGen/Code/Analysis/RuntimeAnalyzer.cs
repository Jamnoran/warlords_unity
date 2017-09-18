using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DunGen.Analysis;
using DunGen.Graph;

using Stopwatch = System.Diagnostics.Stopwatch;

namespace DunGen.Editor
{
	[AddComponentMenu("DunGen/Analysis/Runtime Analyzer")]
	public sealed class RuntimeAnalyzer : MonoBehaviour
	{
		public DungeonFlow DungeonFlow;
		public int Iterations = 100;
		public int MaxFailedAttempts = 20;
		public bool RunOnStart = true;
		public float MaximumAnalysisTime = 0;
		public float PerFrameAnalysisTime = 0.1f;

		private DungeonGenerator generator = new DungeonGenerator();
		private GenerationAnalysis analysis;
		private StringBuilder infoText = new StringBuilder();
		private int targetIterations;
		private int currentIterations;
		private double analysisTime;
        private bool finishedEarly;
        private bool prevShouldRandomizeSeed;
		

		private void Start()
		{
			if(RunOnStart)
				Analyze();
		}

		public void Analyze()
		{
			bool isValid = false;

			if(DungeonFlow == null)
				Debug.LogError("No DungeonFlow assigned to analyzer");
			else if(Iterations <= 0)
				Debug.LogError("Iteration count must be greater than 0");
			else if(MaxFailedAttempts <= 0)
				Debug.LogError("Max failed attempt count must be greater than 0");
			else
				isValid = true;

			if(!isValid)
				return;

            prevShouldRandomizeSeed = generator.ShouldRandomizeSeed;

			generator.isAnalysis = true;
			generator.DungeonFlow = DungeonFlow;
			generator.MaxAttemptCount = MaxFailedAttempts;
            generator.ShouldRandomizeSeed = true;
			analysis = new GenerationAnalysis(Iterations);
			analysisTime = 0;

			currentIterations = 0;
			targetIterations = Iterations;
		}

		private void Update()
		{
			if(targetIterations <= 0)
				return;

			Stopwatch sw = Stopwatch.StartNew();

			int iterationsThisFrame = 0;
			int remainingIterations = targetIterations - currentIterations;
			for (int i = 0; i < remainingIterations; i++)
			{
				if(sw.Elapsed.TotalSeconds >= PerFrameAnalysisTime)
					break;

				if(generator.Generate())
				{
					analysis.IncrementSuccessCount();
					analysis.Add(generator.GenerationStats);
				}

				currentIterations++;
				iterationsThisFrame++;
			}

			analysisTime += sw.Elapsed.TotalSeconds;

            if (MaximumAnalysisTime > 0 && analysisTime >= MaximumAnalysisTime)
            {
                targetIterations = currentIterations;
                finishedEarly = true;
            }

			if(currentIterations >= targetIterations)
			{
				targetIterations = 0;
				analysis.Analyze();

				UnityUtil.Destroy(generator.Root);
				OnAnalysisComplete();
			}
		}

		private void OnAnalysisComplete()
		{
            generator.ShouldRandomizeSeed = prevShouldRandomizeSeed;
			infoText.Length = 0;

            Debug.Log(analysis.MaxBranchDepth);

			if(finishedEarly)
				infoText.AppendLine("[ Reached maximum analysis time before the target number of iterations was reached ]");

			infoText.AppendFormat("Iterations: {0}, Max Failed Attempts: {1}", (finishedEarly) ? analysis.IterationCount : analysis.TargetIterationCount, MaxFailedAttempts);
			infoText.AppendFormat("\nTotal Analysis Time: {0:0.00} seconds", analysisTime);
			//infoText.AppendFormat("\n\tOf which spent generating dungeons: {0:0.00} seconds", analysis.AnalysisTime / 1000.0f);
			infoText.AppendFormat("\nDungeons successfully generated: {0}% ({1} failed)", Mathf.RoundToInt(analysis.SuccessPercentage), analysis.TargetIterationCount - analysis.SuccessCount);
			
			infoText.AppendLine();
			infoText.AppendLine();
			
			infoText.Append("## TIME TAKEN (in milliseconds) ##");
			infoText.AppendFormat("\n\tPre-Processing:\t\t\t\t\t{0}", analysis.PreProcessTime);
			infoText.AppendFormat("\n\tMain Path Generation:\t\t{0}", analysis.MainPathGenerationTime);
			infoText.AppendFormat("\n\tBranch Path Generation:\t\t{0}", analysis.BranchPathGenerationTime);
			infoText.AppendFormat("\n\tPost-Processing:\t\t\t\t{0}", analysis.PostProcessTime);
			infoText.Append("\n\t-------------------------------------------------------");
			infoText.AppendFormat("\n\tTotal:\t\t\t\t\t\t\t\t{0}", analysis.TotalTime);
			
			infoText.AppendLine();
			infoText.AppendLine();
			
			infoText.AppendLine("## ROOM DATA ##");
			infoText.AppendFormat("\n\tMain Path Rooms: {0}", analysis.MainPathRoomCount);
			infoText.AppendFormat("\n\tBranch Path Rooms: {0}", analysis.BranchPathRoomCount);
			infoText.Append("\n\t-------------------");
			infoText.AppendFormat("\n\tTotal: {0}", analysis.TotalRoomCount);
			
			infoText.AppendLine();
			infoText.AppendLine();
			
			infoText.AppendFormat("Retry Count: {0}", analysis.TotalRetries);
		}

		private void OnGUI()
		{
			if(analysis == null || infoText == null || infoText.Length == 0)
			{
				string failedGenerationsCountText = (analysis.SuccessCount < analysis.IterationCount) ? ("\nFailed Dungeons: " + (analysis.IterationCount - analysis.SuccessCount).ToString()) : "";

				GUILayout.Label(string.Format("Analyzing... {0} / {1} ({2:0.0}%){3}", currentIterations, targetIterations, (currentIterations / (float)targetIterations) * 100, failedGenerationsCountText));

				return;
			}

			GUILayout.Label(infoText.ToString());
		}
	}
}