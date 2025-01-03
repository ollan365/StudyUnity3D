using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using static Constants;

public class PlayLogic : MonoBehaviour
{
    private Sequence sequence;
    private Animator animator;

    private void Start()
    {
        sequence = DOTween.Sequence();
    }

    public void Trigger (GameObject obj, bool isEvent = false)
    {
        Object triggerObj = obj.GetComponent<Object>();
        Debug.Log("Trigger name: " + triggerObj.name);

        switch (triggerObj.Name)
        {
            case "Treasure": StartCoroutine(OpenTreasure(triggerObj, isEvent)); break;
            case "ForbiddenFruit": StartCoroutine(EatForbiddenFruit(triggerObj)); break;
            case "Thunder": StartCoroutine(StrikeThunder(triggerObj)); break;
            case "HealBead": StartCoroutine(HealBead(triggerObj)); break;
        }
    }
    private IEnumerator OpenTreasure(Object obj, bool isEvent = false)
    {
        GameObject selectedCharacter = ColorCheckManager.Instance.SelectedCharacter;
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);

        //플레이어가 이동해서 상자를 오픈하는 경우
        if (!isEvent)
        {
            Vector3 direc = StageManager.Instance.Player.transform.position - obj.transform.position;

            Quaternion pRot = Quaternion.LookRotation(direc);
            Quaternion oRot = Quaternion.LookRotation(direc);

            StageManager.Instance.Player.transform.rotation = pRot;
            obj.transform.rotation = oRot;

            StageManager.Instance.Player.transform.localEulerAngles = new Vector3(0, StageManager.Instance.Player.transform.localEulerAngles.y, 0);
            obj.transform.localEulerAngles = new Vector3(0, obj.transform.localEulerAngles.y, 0);
        }

        //보물상자 여는 애니메이션
        animator = obj.GetComponent<Animator>();
        animator.SetBool("Open", true);

        //골드 획득 연출
        int gold = (int)obj.Damage;
        StaticManager.Instance.Gold += gold;
        if (ColorCheckManager.Instance.SelectedCharacter)
            ColorCheckManager.Instance.SelectedCharacter.GetComponent<Object>().PoppingText($"+{gold}", Color.yellow);
        else StageManager.Instance.Player.PoppingText($"+{gold}", Color.yellow);
        yield return new WaitForSeconds(2f);

        //상자 제거
        ObjectManager.Instance.ObjectDie(obj.gameObject);
        if (!isEvent)
        {
            ColorCheckManager.Instance.CharacterSelect(selectedCharacter);
            ColorCheckManager.Instance.Move(obj.Color, obj.Index, true, false); //상자로 이동
        }
        yield break;
    }
    private IEnumerator EatForbiddenFruit(Object obj)
    {
        ObjectManager.Instance.ObjectDie(obj.gameObject);

        int random = Random.Range(0, 100);

        Object selected = ColorCheckManager.Instance.SelectedCharacter.GetComponent<Object>();

        if (random < 50) selected.OnHit(StatusEffect.HP_PERCENT, -20);
        else selected.OnHit(StatusEffect.HP_PERCENT, 20);

        Debug.Log($"ForbiddenFruit: {selected} {random}");

        yield return new WaitForFixedUpdate();
    }
    private IEnumerator StrikeThunder(Object obj)
    {
        bool[] attackable = StageCube.Instance.Cross(obj.Index);

        // 이동한 당사자도 데미지를 받는다
        ColorCheckManager.Instance.SelectedCharacter.GetComponent<Object>().OnHit(StatusEffect.HP, 10 * StaticManager.Instance.Stage);

        for (int i = 0; i < 9; i++)
        {
            if (!attackable[i]) continue;

            Touch touch = StageCube.Instance.touchArray[obj.Color.ToInt()][i];
            if (touch.ObjType == ObjectType.NULL || touch.ObjType == ObjectType.TRIGGER) continue;

            touch.Obj.OnHit(StatusEffect.HP, 10 * StaticManager.Instance.Stage);
            Debug.Log($"Thunder: {touch.Obj} {10 * StaticManager.Instance.Stage}");
        }

        ObjectManager.Instance.ObjectDie(obj.gameObject);

        yield return new WaitForFixedUpdate();
    }
    private IEnumerator HealBead(Object obj)
    {
        ObjectManager.Instance.ObjectDie(obj.gameObject);

        int random = Random.Range(0, 100);

        Object selected = ColorCheckManager.Instance.SelectedCharacter.GetComponent<Object>();
        selected.OnHit(StatusEffect.HP_PERCENT, -50);

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
                p.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, -portion.HealRate);
                break;
        }
    }
}
