using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CustomElement))]
public class CustomElementDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 요소의 displayName 필드를 읽음
        SerializedProperty displayNameProp = property.FindPropertyRelative("displayName");
        SerializedProperty valueProp = property.FindPropertyRelative("value");

        // 리스트 항목의 이름을 displayName으로 설정
        label.text = displayNameProp.stringValue;

        // 기본 필드 그리기
        EditorGUI.PropertyField(position, valueProp, label);
    }
}
