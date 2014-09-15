﻿using System;
using uLearn;

namespace Slide08
{
	[Slide("Массивы массивов", "{D0DAB02D-BDB6-4BD0-AC43-AA98305D6A17}")]
    public class Program
    {
		//#video _L6_YCruwHs
		/*
		[Иллюстрация карты памяти](_S090_Memory.odp)
		*/
		/*
		## Заметки по лекции
		*/
        public static void Main()
        {
            //Это - массив массивов. Поскольку массив является типом (как int или string),
            //то можно делать массивы этого типа, т.е. - массивы массивов
            int[][] array;
            array = new int[2][];

            //Так можно - array инициализирован
            Console.WriteLine(array.Length);

            //Выдаст исключение, ведь нулевой массив в этом массиве массивов не инициализирован
            Console.WriteLine(array[0].Length);

            array[0] = new int[3];
            //Теперь это сработает, потому что мы проинициализировали нулевой элемент
            Console.WriteLine(array[0].Length);
            //А это по-прежнему вызовет исключение, потому что первый элемент не инициализирован
            Console.WriteLine(array[1].Length);
            
            //В отличие от многомерного массива, индексация производится двумя парами скобок
            array[0][0] = 1;

            //В отличие от многомерного массива, являющегося прямоугольником или параллелепипедом,
            //в массиве массивов все хранимые массивы могут быть разной длины
            array[1] = new int[10];

            //Могут быть массивы массивов массивов...
            int[][][] array1=new int[2][][];

            //или массивы двумерных массивов
            int[][,] array2=new int[2][,];
            
            //а также двумерные массивы массивов
            int[,][] array3 = new int[2, 3][];

            //Самое главное - если есть тип, 
           //то из него можно сделать массив, который тоже будет типом.

        }
    }
}