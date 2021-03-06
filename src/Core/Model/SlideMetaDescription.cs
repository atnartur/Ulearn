﻿using System;
using System.IO;
using System.Xml.Serialization;
using uLearn.Configuration;

namespace uLearn.Model
{
	public class SlideMetaDescription
	{
		[XmlElement("image")]
		// ReSharper disable once InconsistentNaming
		public string _image { get; set; }
		
		[XmlIgnore]
		public string Image { get; set; }

		[XmlElement("keywords")]
		public string Keywords { get; set; }
		
		[XmlElement("description")]
		public string Description { get; set; }

		public void FixPaths(FileInfo slideFile)
		{
			if (string.IsNullOrEmpty(_image))
				return;

			string relativeUrl;
			try
			{
				relativeUrl = CourseUnitUtils.GetDirectoryRelativeWebPath(slideFile);
			}
			catch (Exception e)
			{
				/* It's ok if courses web directory is not found, i.e. when we run this code on course tool
				 * Just show error as warning to console and set relativeUrl to ""
				 */
				Console.WriteLine("Warning: " + e.Message);
				Console.WriteLine("It's ok if you are using course tool, not production ulearn web server.");
				relativeUrl = "";
			}
			
			var imagePath = Path.Combine(relativeUrl, _image);
			var configuration = ApplicationConfiguration.Read<UlearnConfiguration>();

			Image = configuration.BaseUrl + imagePath;
		}
	}
}