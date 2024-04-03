using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;

public class PlayLogic : MonoBehaviour
{
    [SerializeField] private Text playerGoldText;
    public void OpenTreasureBox(GameObject obj)
    {
        StaticManager.Instance.Gold += obj.GetComponent<Object>().Damage;
        ObjectManager.Instance.ObjectDie(obj);
    }
    public IEnumerator GoldText()
    {
        yield return null;
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
                break;
        }
    }
}
