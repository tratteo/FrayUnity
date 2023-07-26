using Fray.FX;
using UnityEditor;
using UnityEngine;

namespace Fray.Editor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(VfxHandler))]
    [CustomPropertyDrawer(typeof(SfxHandler))]
    public class FxHandlerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var prop = property.FindPropertyRelative("fx");
            EditorGUI.PropertyField(position, prop, label, true);
            EditorGUI.EndProperty();
        }
    }
}