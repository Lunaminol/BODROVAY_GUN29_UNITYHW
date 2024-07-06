using UnityEditor;
using UnityEngine;

namespace DefaultNamespace
{
    public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR

    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]

    public class ReadOnlyPropertyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
#endif
}
