using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;

public class PlayLogic : MonoBehaviour
{
    public void OpenTreasureBox(GameObject obj)
    {
        int gold = obj.GetComponent<Object>().Damage;
        StaticManager.Instance.Gold += gold;
        ObjectManager.Instance.ObjectDie(obj);

        StartCoroutine(GoldText(gold));
    }
    private IEnumerator GoldText(int gold)
    {
        GameObject goldTextObj = ColorCheckManager.Instance.SelectedCharacter.GetComponent<Object>().GoldText;
        Text goldText = goldTextObj.GetComponent<Text>();
        goldText.text = $"+{gold}";
        RectTransform rectTransform = goldTextObj.GetComponent<RectTransform>();
        goldText.color = new Color(1, 1, 0, 1);
        Vector3 originPosition =  rectTransform.position;

        float current = 1;
        while (current > 0)
        {
            current -= Time.deltaTime;

            Color color = goldText.color;
            color.a = Mathf.Lerp(1, 0, current * 2);
            goldText.color = color;
            rectTransform.Translate(Vector3.up * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        goldText.color = new Color(1, 1, 0, 0);
        rectTransform.position = originPosition;

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
                p.GetComponent<Object>().OnHit(StatusEffect.HP, -portion.Value);
                break;
            case StatusEffect.HP_PERCENT:
                p.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, portion.HealRate);
                break;
        }
    }
}
