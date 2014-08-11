﻿using System;

namespace uLearn.Courses.BasicProgramming.Slides
{
	[Slide("Переменные", "{fe3a32fb-9f61-4a58-a2f4-547d4c8fac77}")]
	public class S08_LocalAndGlobalVarsVideo
	{
		//#video //www.youtube.com/embed/_-rHn9x-0ho?rel=0

		/*
		## Заметки по лекции
		*/
		class Program
		{
			static string globalVariable = "Global variable";

			static void MethodA()
			{
				if (globalVariable == "")
				{
					string temporalVariable = "Temporal variable";
					Console.WriteLine(temporalVariable);
				}

				string localVariable = "Local variable";
				Console.WriteLine(localVariable); // Так можно: эта переменная используется в той же области, где и объявлена
				// Console.WriteLine(temporalVariable); // Так нельзя: temporalVariable определена только внутри блока if
			}

			static void MethodB()
			{
				// Console.WriteLine(localVariable); // Так нельзя: переменная определена в другом методе.
				Console.WriteLine(globalVariable); //Так можно: это глобальная переменная

			}
		}
		/* 
		Переменная доступна (грубо) внутри тех фигурных скобок, где она определена.
		Области кода, из которого допустимо обращение к переменной, называется "областью видимости"
		*/
	}
}