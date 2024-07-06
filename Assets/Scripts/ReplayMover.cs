using System;
using UnityEngine;

namespace DefaultNamespace
{
	[RequireComponent(typeof(PositionSaver))]
	public class ReplayMover : MonoBehaviour
	{
		private PositionSaver _save;

		private int _index;
		private PositionSaver.Data _prev;
		private float _duration;

		private void Start()
		{
            //todo comment: зачем нужны эти проверки?
            //Чтобы избежать ArgumentOutOfRangeException при выполнении кода в Update 
            if (!TryGetComponent(out _save) || _save.Records.Count == 0)
			{
				Debug.LogError("Records incorrect value", this);
				//todo comment: Для чего выключается этот компонент?
				//Чтобы не выполнять код в этом скрипте если отсутствует файл _save
				enabled = false;
			}
		}

		private void Update()
		{
			var curr = _save.Records[_index];
			//todo comment: Что проверяет это условие (с какой целью)? 
			//Проверяем превысило ли текущее время значение времени в сохраненных Records, для того чтобы убедиться что прошел определенный промежуток времени и можно перейти к следующей записи
			//таким образом точно повторяя движение объекта которое мы записали
			if (Time.time > curr.Time)
			{
				_prev = curr;
				_index++;
				//todo comment: Для чего нужна эта проверка?
				//Для завершения компонента, проверяем что все записи обработаны
				if (_index >= _save.Records.Count)
				{
					enabled = false;
					Debug.Log($"<b>{name}</b> finished", this);
				}
			}
			//todo comment: Для чего производятся эти вычисления (как в дальнейшем они применяются)?
			//Нормализуем текущее время и вычисляем насколько мы продвинулись от предыдущей записи к следующей, нужен для дальнейшей интерполяции - вычисления промежуточных позиций между нашими записями
			var delta = (Time.time - _prev.Time) / (curr.Time - _prev.Time);
            //todo comment: Зачем нужна эта проверка?
            //Чтобы избежать ошибки в случае деления на 0 если curr.Time и _prev.Time будут иметь одинаковые значения 
			//(?) в последней итерации curr и prev будут иметь одинаковые значения т.к. Record.Count будет на 1 больше _index? 
            if (float.IsNaN(delta)) delta = 0f;
			//todo comment: Опишите, что происходит в этой строчке так подробно, насколько это возможно
			//Мы меняем позицию объекта с помощью метода линейной интерполяции Vector3.Lerp, для которого требуется начальная позиция откуда мы начнем двигаться (_prev),
			//конечная позиция где мы окажемся (сurr) и параметер delta который определяет насколько мы продвинулись между этими двумя позициями (0 - 1, от 0 до 100%) 
			//Полученное промежуточное значение присваеваеся текущей позиции объекта, таким образом мы меняем его положение в сцене
			transform.position = Vector3.Lerp(_prev.Position, curr.Position, delta);
		}
	}
}