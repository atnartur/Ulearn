﻿using uLearn;

namespace OOP.Slides.U10_NoIfs
{
	[Slide("Задача: Визуализация", "D583B7E81AF54357988D4AC9F9A52631")]
	public class S040_Визуализация
	{
		/*
		Создайте отдельный класс визуализатора с одной операцией — нарисовать текущее состояние леса и лесных жителей на консоли.

		В этой задаче все еще запрещено использовать конструкции `if`, `switch` и `: ?` для явного или косвенного сравнения типов объектов.

		Доработайте класс Лес так, чтобы он генерировал событие на каждое изменение мира.
		Создайте простое консольное приложение, в котором визуализатор перерисовывает мир на каждое его изменение.

		Замените 1/0/L/K на наиболее визуально подходящие символы.

		Каждый лесной житель отмечается на изображении леса некоторым уникальным символом — маркером (например, латинскими буквами по порядку).

		После вывода игрового поля следует легенда с расшифровкой символов,
		для каждого лесного жителя приведено имя и количество оставшихся жизней.

		Пример вывода в простом случае:

			████  
			█T █  
			█ ♥█  
			████  

			█ — заросли
			♥ - жизнь
			T - лесной житель Thranduil (2 жизни)

		### Дополнительное усложнение

		Возможно в будущем появится несколько разных визуализаторов. Например консольный цветной, консольный черно-белый, и визуализатор в графическом интерфейсе.
		Постарайтесь спроектировать код, готовый к этому изменению.

		*/
	}
}