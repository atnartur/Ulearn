using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using RunCsJob;
using RunCsJob.Api;
using uLearn.Model.Blocks;

namespace uLearn
{
	public class CourseValidator
	{
		private readonly Course course;
		private readonly string workDir;

		public event Action<string> InfoMessage;
		public event Action<string> Error;

		public CourseValidator(Course course, string workDir)
		{
			this.course = course;
			this.workDir = workDir;
		}

		public void ValidateExercises()
		{
			foreach (var slide in course.Slides.OfType<ExerciseSlide>())
			{
				LogSlideProcessing("Validate exercise", slide);
				EthalonSolutionsForExercises(slide);
			}
		}

		private void LogSlideProcessing(string prefix, Slide slide)
		{
			InfoMessage?.Invoke(prefix + " " + slide.Info.UnitName + " - " + slide.Title);
		}

		public void ValidateVideos()
		{
			var videos = GetVideos().ToLookup(d => d.Item2, d => d.Item1);
			foreach (var g in videos.Where(g => g.Count() > 1))
				ReportError("Duplicate videos on slides " + string.Join(", ", g));
			foreach (var g in videos)
			{
				Slide slide = g.First();
				LogSlideProcessing("Validate video", slide);
				var url = "https://www.youtube.com/oembed?format=json&url=http://www.youtube.com/watch?v=" + g.Key;
				try
				{
					new WebClient().DownloadData(url);
				}
				catch (Exception e)
				{
					ReportError("Slide " + slide + " contains not accessible video. " + e.Message);
				}
			}
		}

		private void ReportSlideError(Slide slide, string error)
		{
			ReportError(slide.Info.UnitName + ": " + slide.Title + ". " + error);
		}

		private void ReportError(string message)
		{
			Error?.Invoke(message);
		}

		public IEnumerable<Tuple<Slide, string>> GetVideos()
		{
			return course.Slides
				.SelectMany(slide =>
					slide.Blocks.OfType<YoutubeBlock>()
						.Select(b => Tuple.Create(slide, b.VideoId)));
		}

		private void InitialCodeIsNotSolutionForProjExercise(ExerciseSlide slide)
		{
			var exercise = (ProjectExerciseBlock)slide.Exercise;
			var directoryName = Path.Combine(exercise.SlideFolderPath, exercise.ExerciseDir);
			var excluded = (exercise.PathsToExcludeForChecker ?? new string[0]).Concat(new[] { "bin/*", "obj/*" }).ToList();
			var exerciseDir = new DirectoryInfo(directoryName);
			var bytes = exerciseDir.ToZip(excluded, new[]
			{
				new FileContent
				{
					Path = exercise.CsprojFileName,
					Data = ProjModifier.ModifyCsproj(exerciseDir.GetFile(exercise.CsprojFileName),
						proj => ProjModifier.PrepareCsprojBeforeZipping(proj, exercise))
				}
			});
			var pathToCompiler = Path.Combine(workDir, "Microsoft.Net.Compilers.1.3.2");
			var result = SandboxRunner.Run(pathToCompiler,
				new ProjRunnerSubmition
				{
					Id = slide.Id.ToString(),
					ZipFileData = bytes,
					ProjectFileName = exercise.CsprojFileName,
					Input = "",
					NeedRun = true
				});
			if (result.Verdict != Verdict.Ok)
				ReportSlideError(slide, "Exercise initial code verdict is not OK. RunResult = " + result);
			else if (result.Score >= 0.5)
				ReportSlideError(slide, "Exercise initial code (available to students) is solution!");
		}

		private void EthalonSolutionForSingleFileExercises(ExerciseSlide slide)
		{
			var exercise = (SingleFileExerciseBlock)slide.Exercise;
			var solution = exercise.BuildSolution(exercise.EthalonSolution);
			if (solution.HasErrors)
			{
				FailOnError(slide, solution, exercise.EthalonSolution);
				return;
			}

			var result = SandboxRunner.Run("", exercise.CreateSubmition(
				slide.Id.ToString(),
				exercise.EthalonSolution));

			var output = result.GetOutput().NormalizeEoln();

			var isRightAnswer = output.NormalizeEoln().Equals(slide.Exercise.ExpectedOutput.NormalizeEoln());
			if (!isRightAnswer)
			{
				ReportSlideError(slide,
					"ActualOutput: " + output.NormalizeEoln() + "\n" +
					"ExpectedOutput: " + slide.Exercise.ExpectedOutput.NormalizeEoln() + "\n" +
					"CompilationError: " + result.CompilationOutput + "\n" +
					"SourceCode: " + solution.SourceCode + "\n\n");
			}
		}

		private void EthalonSolutionsForExercises(ExerciseSlide slide)
		{
			if (slide.Exercise is ProjectExerciseBlock)
				InitialCodeIsNotSolutionForProjExercise(slide);
			else
				EthalonSolutionForSingleFileExercises(slide);
		}

		private void FailOnError(ExerciseSlide slide, SolutionBuildResult solution, string ethalonSolution)
		{
			ReportSlideError(slide, $@"Template solution: {ethalonSolution}
source code: {solution.SourceCode}
error: {solution.ErrorMessage}");
		}
	}
}