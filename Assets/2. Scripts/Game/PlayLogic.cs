using UnityEngine;
using static Constants;

public class PlayLogic : MonoBehaviour
{
    public void OpenTreasureBox(GameObject obj)
    {
        StaticManager.Instance.Gold += obj.GetComponent<Object>().Damage;
        ObjectManager.Instance.ObjectDie(obj);
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
