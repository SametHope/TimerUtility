using UnityEngine;

namespace SametHope.TimerUtility
{
    /// <summary>
    /// Makes the property non-editable from the inspector.
    /// <para>Note: This will not play well with types that have special drawers such as lists.</para>
    /// </summary>
    public class TimerEditorReadOnlyAttribute : PropertyAttribute { }
}

#if UNITY_EDITOR
namespace SametHope.TimerUtility.Editor
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(TimerEditorReadOnlyAttribute))]
    public class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool previousGUIState = GUI.enabled;
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = previousGUIState;
        }
    }
}
#endif