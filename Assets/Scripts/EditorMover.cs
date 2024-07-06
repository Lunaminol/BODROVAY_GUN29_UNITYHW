using UnityEngine;

namespace DefaultNamespace
{
	
	[RequireComponent(typeof(PositionSaver))]
	public class EditorMover : MonoBehaviour
	{
		private PositionSaver _save;
		private float _currentDelay;

		//todo comment: Что произойдёт, если _delay > _duration?
		//Позиция не сохранится (если каждые _delay секунд в течении промежутка времени _duration мы записываем позицию перемещающегося объекта и _delay > _duration,
		//мы не сделаем ни одной записи).
		[SerializeField, Range(0.2f, 1f)]
		private float _delay = 0.5f;
		[SerializeField, Min(0.2f)]
		private float _duration = 5f;

		private void Start()
		{
			//todo comment: Почему этот поиск производится здесь, а не в начале метода Update?
			//
			//Метод Update вызывается много раз в течении runtime, а Start - только 1 раз. В Update мы бы выполняли поиск и удаляли записи снова и снова,
			//а нам это нужно сделать только 1 раз - в начале runtime. 
			if (_duration < _delay)
			{
				_duration = _delay * 5f;
			}
			_save = GetComponent<PositionSaver>();
			_save.Records.Clear();
		}

		private void Update()
		{
			_duration -= Time.deltaTime;
			if (_duration <= 0f)
			{
				enabled = false;
				Debug.Log($"<b>{name}</b> finished", this);
				return;
			}
			
			//todo comment: Почему не написать (_delay -= Time.deltaTime;) по аналогии с полем _duration?
			//_delay - это фиксированный интервал между сохранениями позиций, а _currentDelay - динамический способ отсчета времени до следующей точки сохранения.
			//Если использовать _delay - мы будем менять интервал сохранения позиций во время runtime. 
			_currentDelay -= Time.deltaTime;
			if (_currentDelay <= 0f)
			{
				_currentDelay = _delay;
				_save.Records.Add(new PositionSaver.Data
				{
					Position = transform.position,
					//todo comment: Для чего сохраняется значение игрового времени?
					//Для того чтобы сохранить точное время когда была записана позиция, что позволит воспроизвести эти условия в будущем.
					Time = Time.time,
				});
			}
			
		}
	}
}