using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ScreenEffect : MonoBehaviour
{
    public static ScreenEffect Instance { get; private set; }
    private Image coverPanel;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        coverPanel = GetComponentInChildren<Image>();
    }

    public void Fade(float start, float end, float fadeTime, Image fadeObject = null)
    {
        StartCoroutine(OnFade(fadeObject, start, end, fadeTime));
    }
    private IEnumerator OnFade(Image fadeObject, float start, float end, float fadeTime)
    {
        if (fadeObject == null) fadeObject = coverPanel;

        if (!fadeObject.gameObject.activeSelf) fadeObject.gameObject.SetActive(true);
        Color newColor = fadeObject.color;
        newColor.a = start;
        fadeObject.color = newColor;

        float current = 0, percent = 0;

        while (percent < 1 && fadeTime != 0)
        {
            current += Time.deltaTime;
            percent = current / fadeTime;

            newColor.a = Mathf.Lerp(start, end, percent);
            fadeObject.color = newColor;

            yield return null;
        }
        newColor.a = end;
        fadeObject.color = newColor;

        // 투명해졌으면 끈다
        if (fadeObject == coverPanel && end == 0)
        {
            fadeObject.gameObject.SetActive(false);
        }
    }
}
