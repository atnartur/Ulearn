﻿using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using uLearn.CSharp;
using uLearn.Model;
using Ulearn.Common.Extensions;

namespace uLearn.Utilities
{
	public class LessonToXmlConvertor
	{
		private readonly XmlSerializer lessonSerializer = new XmlSerializer(typeof(Lesson));

		[Test]
		[Explicit]
		public void ConvertSlidesFromDirectory()
		{
			var slidesDirectory = new DirectoryInfo(@"Your path to slides");
			var unit = new Unit(UnitSettings.CreateByTitle("u1", CourseSettings.DefaultSettings), slidesDirectory.GetSubdirectory("u1"));
			foreach (var slideFile in slidesDirectory.GetFiles("S*.cs"))
			{
				var slide = new CSharpSlideLoader().Load(slideFile, unit, 0, "Test", CourseSettings.DefaultSettings);
				ConvertSlide(slide);
			}
		}

		[Test]
		[Explicit]
		public void CovertLessonSlidesToXml()
		{
			var coursesDirectory = new DirectoryInfo(@"Your path to course");
			var courseDirectories = coursesDirectory.GetDirectories("Slides", SearchOption.AllDirectories);
			foreach (var courseDirectory in courseDirectories)
			{
				var course = new CourseLoader().LoadCourse(courseDirectory);
				Console.WriteLine($"course {course.Id}");
				foreach (var slide in course.Slides)
				{
					ConvertSlide(slide);
				}
			}
		}

		private void ConvertSlide(Slide slide)
		{
			if (slide.ShouldBeSolved)
				return;
			Console.WriteLine(slide.Info.SlideFile.FullName);
			var lesson = new Lesson(slide.Title, slide.Id, slide.Blocks);
			var path = Path.ChangeExtension(slide.Info.SlideFile.FullName, "lesson.xml");
			using (var writer = new StreamWriter(path, false, Encoding.UTF8))
				lessonSerializer.Serialize(writer, lesson);
			slide.Info.SlideFile.Delete();
		}
	}
}