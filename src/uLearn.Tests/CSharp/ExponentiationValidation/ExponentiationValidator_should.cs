﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace uLearn.CSharp.ExponentiationValidation
{
	[TestFixture]
	public class ExponentiationValidator_should: ValidatorTestBase
	{
		private static readonly DirectoryInfo testDataDir = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..",
			"..", "CSharp", "ExponentiationValidation", "TestData"));
		private static readonly DirectoryInfo incorrectTestDataDir = testDataDir.GetDirectories("Incorrect").Single();
		private static readonly DirectoryInfo correctTestDataDir = testDataDir.GetDirectories("Correct").Single();

		private static IEnumerable<FileInfo> correctFiles = correctTestDataDir.EnumerateFiles();
		private static IEnumerable<FileInfo> incorrectFiles = incorrectTestDataDir.EnumerateFiles();	

		private static readonly ExponentiationStyleValidator validator = new ExponentiationStyleValidator();

		protected override BaseStyleValidator Validator => validator;

		[TestCaseSource(nameof(incorrectFiles))]
		public void FindErrors(FileInfo file)
		{
			CheckThatErrorsAreFound(file);
		}

		[TestCaseSource(nameof(correctFiles))]
		public void NotFindErrors(FileInfo file)
		{
			CheckThatErrorsAreNotFound(file);
		}

		[Explicit]
		[TestCaseSource(nameof(BasicProgrammingFiles))]
		public void NotFindErrors_InBasicProgramming(FileInfo file)
		{
			CheckErrorsAreNotFound_InBasicProgramming(file);
		}

		[Explicit]
		[TestCaseSource(nameof(SubmissionsFiles))]
		public void NotFindErrors_InCheckAcceptedFiles(FileInfo file)
		{
			CheckThatErrorsAreNotFound_InAcceptedFiles(file);
		}
	}
}