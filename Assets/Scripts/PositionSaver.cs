using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
	public class PositionSaver : MonoBehaviour
	{
        [Serializable]
        public struct Data 
		{
			public Vector3 Position;
			public float Time;
		}

		[field: SerializeField, ReadOnly, Tooltip("Для заполнения поля воспользуйтесь контекстным меню в инспекторе и командой \"Create File\"")]
		private TextAsset _json;

		[field: SerializeField, HideInInspector]
		public List<Data> Records { get; private set; }

		private void Awake()
		{
			//todo comment: Что будет, если в теле этого условия не сделать выход из метода?
			//Код после блока if будет продолжать выполняться, возникнет ошибка т.к. отсутствуют ожидаемые данные (_json.text)
			if (_json == null)
			{
				gameObject.SetActive(false);
				Debug.LogError("Please, create TextAsset and add in field _json");

				return;
			}

			JsonUtility.FromJsonOverwrite(_json.text, this);
			//todo comment: Для чего нужна эта проверка (что она позволяет избежать)?
			//Эта проверка помогает гарантировать инициализацию списка и избежать ошибки NullReferenceException после его десериализации
			if (Records == null)
				Records = new List<Data>(10);
		}

		private void OnDrawGizmos()
		{
            //todo comment: Зачем нужны эти проверки (что они позволяют избежать)?
            //Проверка на null позволяет избежать ошибки NullReferenceException, проверка на Count позволяет убедиться что в Record есть хотя бы один элемент
            if (Records == null || Records.Count == 0) return;
			var data = Records;
			var prev = data[0].Position;
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(prev, 0.3f);
			//todo comment: Почему итерация начинается не с нулевого элемента?
			//Потому что в каждой итерации цикла мы линии между previous and current позициями, т.е. нам нужны и предыдущая и следующая позиции, если бы отчет начинался с 0,
			//то не было бы предыдущей позиции и мы не смогли бы нарисовать первую линию
			for (int i = 1; i < data.Count; i++)
			{
				var curr = data[i].Position;
				Gizmos.DrawWireSphere(curr, 0.3f);
				Gizmos.DrawLine(prev, curr);
				prev = curr;
			}
		}
		
#if UNITY_EDITOR
		[ContextMenu("Create File")]
		private void CreateFile()
		{
			//todo comment: Что происходит в этой строке?
			//Создание нового текстового файла внутри папки Assets нашего проекта
			var stream = File.Create(Path.Combine(Application.dataPath, "Path.txt"));
			//todo comment: Подумайте для чего нужна эта строка? (а потом проверьте догадку, закомментировав) 
			//Для освобождения системных ресурсов и избежания дальнейших проблем при работе с файлом и его изменении 
			stream.Dispose();
			UnityEditor.AssetDatabase.Refresh();
			//В Unity можно искать объекты по их типу, для этого используется префикс "t:"
			//После нахождения, Юнити возвращает массив гуидов (которые в мета-файлах задаются, например)
			var guids = UnityEditor.AssetDatabase.FindAssets("t:TextAsset");
			foreach (var guid in guids)
			{
				//Этой командой можно получить путь к ассету через его гуид
				var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
				//Этой командой можно загрузить сам ассет
				var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);
				//todo comment: Для чего нужны эти проверки?
				//Проверка на !null показывает успешно ли загружен ассет, проверка на имя показывает что был загруженный ассет имеет имя Path
				if(asset != null && asset.name == "Path")
				{
					_json = asset;
					UnityEditor.EditorUtility.SetDirty(this);
					UnityEditor.AssetDatabase.SaveAssets();
					UnityEditor.AssetDatabase.Refresh();
					//todo comment: Почему мы здесь выходим, а не продолжаем итерироваться?
					//Если найденный ассет корректен и операции выполнились дальнейшая итерация не нужна
					return;
				}
			}
		}

		private void OnDestroy()
		{
			//todo logic...
			if (_json == null) return;
            var text = JsonUtility.ToJson(Records, true);
            var path = UnityEditor.AssetDatabase.GetAssetPath(_json);
			File.WriteAllText(path, text);
			UnityEditor.EditorUtility.SetDirty(_json);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
        }
#endif
	}
}