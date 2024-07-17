using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ExceptionHandler : MonoBehaviour
{
    [SerializeField] private Image errorPanel;
    [SerializeField] private Text errorText;

    public static ExceptionHandler Instance { get; private set; }
    private Sequence sequence;
    private Color panelColor;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    public void ExceptionHandling(string text)
    {
        errorText.text = text;
        errorPanel.gameObject.SetActive(true);
        panelColor = errorPanel.color;

        sequence = DOTween.Sequence();

        sequence.Append(errorPanel.DOFade(0.7f, 1.0f))
            .Join(errorText.DOFade(1.0f, 1.0f))
            .Append(errorPanel.DOFade(0.0f, 1.0f))
            .Join(errorText.DOFade(0.0f, 1.0f));

    }
}
