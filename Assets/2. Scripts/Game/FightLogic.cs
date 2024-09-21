using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Constants;
using static Excel;
using DG.Tweening;

public class FightLogic : MonoBehaviour
{
    private bool attacking = false;
    private Sequence sequence;

    public IEnumerator FightLogicStart()
    {
        Object player = StageManager.Instance.Player;
        List<GameObject> attackableEnemy = AttackableObject(player.AttackType, player.Color, player.Index, ObjectType.ENEMY);

        attacking = true;
        StartCoroutine(Attack(player, attackableEnemy));
        while (attacking) yield return new WaitForFixedUpdate();

        // 적과 동료의 공격 순서 결정을 위해 List 생성
        List<KeyValuePair<float, int>> enemyAttackOrder = new List<KeyValuePair<float, int>>();
        for (int i = 0; i < StageManager.Instance.StageData(ENEMY_COUNT); i++)
        {
            Object enemyObject = StageManager.Instance.EnemyList[i].GetComponent<Object>();
            if (enemyObject.gameObject.activeSelf && enemyObject.HP > 0)
                enemyAttackOrder.Add(new KeyValuePair<float, int>(enemyObject.Damage, i));
        }
        List<KeyValuePair<float, int>> friendAttackOrder = new List<KeyValuePair<float, int>>();
        for (int i = 0; i < 3; i++)
        {
            if (StageManager.Instance.FriendList[i] == null) continue;
            Object friendObject = StageManager.Instance.FriendList[i].GetComponent<Object>();
            if (friendObject.gameObject.activeSelf && friendObject.HP > 0)
                friendAttackOrder.Add(new KeyValuePair<float, int>(friendObject.Damage, i));
        }
        enemyAttackOrder = enemyAttackOrder.OrderByDescending(enemyAttackOrder => enemyAttackOrder.Key).ToList(); // 공격력 순으로 내림차순 정렬
        friendAttackOrder = friendAttackOrder.OrderByDescending(friendAttackOrder => friendAttackOrder.Key).ToList();

        // 공격
        int count = enemyAttackOrder.Count > friendAttackOrder.Count ? enemyAttackOrder.Count : friendAttackOrder.Count;
        for (int i = 0; i < count; i++)
        {
            // 동료 선공격
            if (i < friendAttackOrder.Count && StageManager.Instance.FriendList[friendAttackOrder[i].Value].activeSelf) // 동료도 있다면
            {
                Object friendObj = StageManager.Instance.FriendList[friendAttackOrder[i].Value].GetComponent<Object>();
                attackableEnemy = AttackableObject(friendObj.AttackType, friendObj.Color, friendObj.Index, ObjectType.ENEMY);

                attacking = true;
                StartCoroutine(Attack(friendObj, attackableEnemy));
                while (attacking) yield return new WaitForFixedUpdate();
            }

            // 적 공격
            if (i < enemyAttackOrder.Count && StageManager.Instance.EnemyList[enemyAttackOrder[i].Value].activeSelf)
            {
                Object enemyObj = StageManager.Instance.EnemyList[enemyAttackOrder[i].Value].GetComponent<Object>();
                List<GameObject> attackablePlayerTeam;

                attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.PLAYER);
                attackablePlayerTeam.AddRange(AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.FRIEND));

                attacking = true;
                StartCoroutine(Attack(enemyObj, attackablePlayerTeam));
                while (attacking) yield return new WaitForFixedUpdate();
            }

        }
        
        //딜교가 종료되면, 일정시간 기다렸다가 ENV 턴으로 넘어간다.
        yield return new WaitForSeconds(2.0f);
        // statge statue를 바꾼다
        if (StageManager.Instance.StatusOfStage != StageStatus.END)
            StageManager.Instance.ChangeStatus(StageStatus.ENV);
    }

    private void LookAt(GameObject src, GameObject dst)
    {
        Vector3 direc = src.transform.position - dst.transform.position;
        Quaternion sRot = Quaternion.LookRotation(direc);
        Quaternion dRot = Quaternion.LookRotation(-direc);
        src.transform.rotation = sRot;
        dst.transform.rotation = dRot;
    }

    private float AttackProduction(Transform src, Transform dst, float weight = 1)
    {
        //공격자의 공격 타입을 얻고, 그에 따른 연출
        sequence = DOTween.Sequence();
        Vector3 knockback = dst.transform.position + (dst.transform.position - src.transform.position) / 6;

        // 보스 스킬 효과에 따라 weight 달라짐
        if(src.GetComponent<Object>().Type == ObjectType.ENEMY)
        {
            if (Boss.Instance && Boss.Instance.enemyPowerful) weight = 1.1f;
            else weight = 1;
        }  

        // OnHit()을 기다렸다가 시키면, turn 넘어갈 때, 죽었는지 살았는지 판단할 때, 공격 판단 등 너무 많이 꼬이므로
        // 체력바가 닳고, 데미지 텍스트가 뜨는 건 미루더라도 HP 자체는 먼저 닳아야 함
        switch (src.GetComponent<Object>().AttackType)
        {
            case WeaponType.SWORD:
                dst.GetComponent<Object>().OnHit(StatusEffect.HP, src.GetComponent<Object>().Damage * weight, 0.2f);

                sequence.Append(src.transform.DOMove(dst.transform.position, 0.3f).SetEase(Ease.InExpo))
                        .Append(src.transform.DOLocalMove(Vector3.zero, 0.6f)).SetEase(Ease.Linear)
                        .Join(dst.transform.DOMove(knockback, 0.1f))
                        .Insert(0.4f, dst.transform.DOLocalMove(Vector3.zero, 0.05f)).SetEase(Ease.Linear);

                return ParticleManager.Instance.AttackParticle(src, dst);

            case WeaponType.STAFF:
                float time = ParticleManager.Instance.AttackParticle(src, dst);
                dst.GetComponent<Object>().OnHit(StatusEffect.HP, src.GetComponent<Object>().Damage * weight, time);
                return time;

            case WeaponType.DUAL:
                return 0;

            case WeaponType.NULL:
                return 0;
            default:
                return 0;
        }
    }

    private IEnumerator Attack(Object attacker, List<GameObject> attacked)
    {
        if (attacker.GetComponent<Boss>() && attacker.GetComponent<Boss>().UseSkill())
        {
            attacker.Indicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            yield return new WaitForSeconds(attacker.GetComponent<Boss>().skillEffectTime + 0.5f);

            attacker.Indicator.SetActive(false);
            yield return new WaitForSeconds(0.1f);

            attacking = false;
            yield break;
        }

        if (attacked.Count == 0)
        { attacking = false; yield break; }

        StartCoroutine(StageManager.Instance.CubeRotate(attacker.Color));
        while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        if (Boss.Instance && Boss.Instance.slienceObject == attacker.gameObject)
        { attacking = false; yield break; }

        for(int i = 0; i < attacked.Count; i++)
        {
            // activate indicator
            attacker.Indicator.SetActive(true);
            attacked[i].GetComponent<Object>().Indicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            LookAt(attacker.gameObject, attacked[i]);

            yield return new WaitForSeconds(AttackProduction(attacker.transform, attacked[i].transform, Mathf.Pow(1.2f, i + 1)));

            // disable indicator
            attacker.Indicator.SetActive(false);
            attacked[i].GetComponent<Object>().Indicator.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        attacking = false;
    }
    private List<GameObject> AttackableObject(WeaponType weaponType, Colors color, int index, ObjectType objType)
    {
        List<GameObject> attackable = new();

        if (objType == ObjectType.PLAYER && StageManager.Instance.Player.gameObject.activeSelf)
        {
            Object p = StageManager.Instance.Player;

            if (p.Color == color && AttackableRange(weaponType, index)[p.Index])
                attackable.Add(StageManager.Instance.Player.gameObject);

        }
        else if (objType == ObjectType.FRIEND)
        {
            for (int i = 0; i < 3; i++)
            {
                if (StageManager.Instance.FriendList[i] == null || StageManager.Instance.FriendList[i].GetComponent<Object>().HP <= 0) continue;

                Object f = StageManager.Instance.FriendList[i].GetComponent<Object>();

                if (f.Color == color && AttackableRange(weaponType, index)[f.Index])
                    attackable.Add(StageManager.Instance.FriendList[i]);
            }
        }
        else if (objType == ObjectType.ENEMY)
        {
            foreach (GameObject enemyObj in StageManager.Instance.EnemyList)
            {
                if (enemyObj.GetComponent<Object>().HP <= 0) continue;

                Object e = enemyObj.GetComponent<Object>();

                if (e.Color == color && AttackableRange(weaponType, index)[e.Index])
                    attackable.Add(enemyObj);
            }
        }
        return attackable;
    }


    private bool[] AttackableRange(WeaponType weaponType, int index)
    {
        bool[] attackable = new bool[9];
        for (int i = 0; i < 9; i++)
            attackable[i] = false;

        if (weaponType == WeaponType.STAFF || weaponType == WeaponType.DUAL)
        {
            switch (index)
            {
                case 0:
                    attackable[5] = true;
                    break;
                case 1:
                    attackable[3] = true;
                    attackable[5] = true;
                    break;
                case 2:
                    attackable[4] = true;
                    break;
                case 3:
                    attackable[1] = true;
                    attackable[7] = true;
                    break;
                case 4:
                    attackable[0] = true;
                    attackable[2] = true;
                    attackable[6] = true;
                    attackable[8] = true;
                    break;
                case 5:
                    attackable[1] = true;
                    attackable[7] = true;
                    break;
                case 6:
                    attackable[4] = true;
                    break;
                case 7:
                    attackable[3] = true;
                    attackable[5] = true;
                    break;
                case 8:
                    attackable[4] = true;
                    break;
                default:
                    break;
            }
        }

        if (weaponType == WeaponType.SWORD)
        {
            attackable = StageCube.Instance.Cross(index);
        }

        if (weaponType == WeaponType.DUAL)
        {
            for (int i = 0; i < StageCube.Instance.Cross(index).Length; i++)
            {
                if (StageCube.Instance.Cross(index)[i])
                    attackable[i] = true;
            }
        }

        return attackable;
    }

}