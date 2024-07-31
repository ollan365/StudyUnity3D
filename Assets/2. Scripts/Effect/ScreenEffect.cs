using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using static Constants;
using DG.Tweening;

public class ScreenEffect : MonoBehaviour
{
    private Sequence sequence;


    [Header("Fight Turn Effect")]
    [SerializeField] private Image fightBackground;
    [SerializeField] private RectTransform leftSwd;
    [SerializeField] private RectTransform rightSwd;

    public static ScreenEffect Instance { get; private set; }
    [SerializeField] private Image coverPanel;
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

    public void StatusChangeEffect()
    {
        sequence = DOTween.Sequence();
        switch (StageManager.Instance.StatusOfStage)
        {
            case StageStatus.PLAYER:
                //플레이어 턴으로 넘어갈 때의 연출
                break;
            case StageStatus.FIGHT:
                //활성화
                fightBackground.gameObject.SetActive(true);
                leftSwd.gameObject.SetActive(true);
                rightSwd.gameObject.SetActive(true);

                //연출
                sequence.Append(fightBackground.DOFade(0.4f, 1.0f))
                        .Join(leftSwd.DOLocalMoveX(-120, 1.0f)).SetEase(Ease.InExpo)
                        .Join(rightSwd.DOLocalMoveX(120, 1.0f)).SetEase(Ease.InExpo)
                        .Join(leftSwd.DORotate(new Vector3(0f, 0f, 55f), 0.5f)).SetEase(Ease.InExpo)
                        .Join(rightSwd.DORotate(new Vector3(0f, 180f, 55f), 0.5f)).SetEase(Ease.InExpo)

                        .Append(leftSwd.DORotate(new Vector3(0f, 0f, 0f), 0.3f)).SetEase(Ease.Linear)
                        .Join(rightSwd.DORotate(new Vector3(0f,180f,0f), 0.3f)).SetEase(Ease.Linear)

                        .SetDelay(0.2f)
                        .Append(fightBackground.DOFade(0.0f, 1.0f))
                        .OnComplete(()=> offObjects(StageStatus.FIGHT));
                break;
            default:
                break;
        }
    }

    public void offObjects(StageStatus status)
    {
        switch (status)
        {
            case StageStatus.PLAYER:
                break;

            case StageStatus.FIGHT:
                fightBackground.gameObject.SetActive(false);
                leftSwd.gameObject.SetActive(false);
                rightSwd.gameObject.SetActive(false);
                leftSwd.localPosition = new Vector3(-950f, 0f, 0f);
                rightSwd.localPosition = new Vector3(950f, 0f, 0f);
                break;
            default:
                break;
        } 
    }
}
