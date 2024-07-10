using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;

public class PlayLogic : MonoBehaviour
{
    [SerializeField] private ColorCheckManager colorCheckManager;

    public void OpenTreasureBox(GameObject obj)
    {
        int gold = (int)obj.GetComponent<Object>().Damage;
        StaticManager.Instance.Gold += gold;
        ObjectManager.Instance.ObjectDie(obj);

        StartCoroutine(ColorCheckManager.Instance.SelectedCharacter.GetComponent<Object>().PoppingText($"+{gold}", Color.red));
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
