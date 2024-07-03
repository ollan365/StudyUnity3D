using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;
using DG.Tweening;

public class PlayLogic : MonoBehaviour
{
    [SerializeField] private ColorCheckManager colorCheckManager;
    private Sequence sequence;

    private void Start()
    {
        sequence = DOTween.Sequence();
    }

    public void OpenTreasureBox(GameObject obj)
    {
        int gold = obj.GetComponent<Object>().Damage;
        StaticManager.Instance.Gold += gold;
        ObjectManager.Instance.ObjectDie(obj);

        StartCoroutine(GoldText(gold));
    }
    private IEnumerator GoldText(int gold)
    {
        GameObject goldTextObj = colorCheckManager.SelectedCharacter.GetComponent<Object>().GoldText;
        Text goldText = goldTextObj.GetComponent<Text>();
        goldText.text = $"+{gold}";

        //골드 텍스트 띄우는 부분

        RectTransform rectTransform = goldTextObj.GetComponent<RectTransform>();
        //goldText.color = new Color(1, 1, 0, 1);
        //Vector3 originPosition =  rectTransform.position;

        //float current = 1;
        //while (current > 0)
        //{
        //    current -= Time.deltaTime;

        //    Color color = goldText.color;
        //    color.a = Mathf.Lerp(1, 0, current * 2);
        //    goldText.color = color;
        //    rectTransform.Translate(Vector3.up * Time.deltaTime);
        //    yield return new WaitForFixedUpdate();
        //}

        //goldText.color = new Color(1, 1, 0, 0);
        //rectTransform.position = originPosition;

        //DOTween으로 변경한 부분
        //일단 대충은 되는데 정확하게 의도한 대로는 안됨
        //내일 하면서 주기나, 위치 세부적으로 조정하기.
        Vector3 textPos = rectTransform.position;
        Color goldColor = goldText.color;

        textPos.y += 0.5f;
        goldColor.a = 1;
        sequence.Append(rectTransform.DOMove(textPos, 1.0f));
        sequence.Join(goldText.DOColor(goldColor, 1.0f));

        yield return new WaitForSeconds(1.0f);

        textPos.y -= 0.5f;
        goldColor.a = 0;
        sequence.Append(rectTransform.DOMove(textPos, 1.0f));
        sequence.Join(goldText.DOColor(new Color(1, 1, 0, 0), 1.0f));


        yield return new WaitForFixedUpdate();
    }
    public void UsePortion(int itemID, GameObject playerTeam)
    {
        GameObject p = StageManager.Instance.Player.gameObject;

        foreach (GameObject f in StageManager.Instance.FriendList)
            if (playerTeam == f)
                p = f;

        Portion portion = StaticManager.Instance.portionDatas[itemID];
        switch (portion.StatusEffectType)
        {
            case StatusEffect.HP:
                p.GetComponent<Object>().HP += portion.Value;
                p.GetComponent<Object>().HP_Percent(portion.HealRate);
                break;
        }
    }
}
