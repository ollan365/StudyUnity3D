using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReplaceLegacyTextEditor : EditorWindow
{
    public TMP_FontAsset newFontAsset;

    [MenuItem("Tools/Replace Legacy Text with TMP")]
    public static void ShowWindow()
    {
        GetWindow<ReplaceLegacyTextEditor>("Replace Legacy Text");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Legacy Text with TMP", EditorStyles.boldLabel);

        newFontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font Asset", newFontAsset, typeof(TMP_FontAsset), false);

        if (GUILayout.Button("Replace"))
        {
            ReplaceLegacyTexts();
        }
    }

    private void ReplaceLegacyTexts()
    {
        Text[] legacyTexts = FindObjectsOfType<Text>();

        foreach (Text legacyText in legacyTexts)
        {
            GameObject textObject = legacyText.gameObject;
            string textContent = legacyText.text;
            Color textColor = legacyText.color;
            FontStyles fontStyle = legacyText.fontStyle == FontStyle.Bold ? FontStyles.Bold : (legacyText.fontStyle == FontStyle.Italic ? FontStyles.Italic : FontStyles.Normal);
            float fontSize = legacyText.fontSize;

            Undo.RecordObject(textObject, "Replace Legacy Text with TMP");

            DestroyImmediate(legacyText);

            TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
            tmpText.text = textContent;
            tmpText.color = textColor;
            tmpText.fontSize = fontSize;
            tmpText.fontStyle = fontStyle;

            if (newFontAsset != null)
            {
                tmpText.font = newFontAsset;
            }

            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.alignment = TextAlignmentOptions.Midline;

            EditorUtility.SetDirty(textObject);
        }
    }
}
