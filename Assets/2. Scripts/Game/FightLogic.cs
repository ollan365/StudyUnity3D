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

        sequence = DOTween.Sequence();
        sequence.Append(src.transform.DOMove(dst.transform.position, 0.5f)).SetEase(Ease.InExpo)
                .SetDelay(0.3f)
                .Append(src.transform.DOLocalMove(Vector3.zero, 1.0f)).SetEase(Ease.Linear)
                .OnComplete(seqEnd);

    }

    private void seqEnd()
    {
        Debug.Log("LookAt End");
    }

    private IEnumerator Attack(Object attacker, List<GameObject> attacked)
    {
        Debug.Log("공격자: " + attacker.gameObject + " 피격자: " + attacked.Count);
        if (attacked.Count == 0)
        { attacking = false; yield break; }

        StartCoroutine(StageManager.Instance.CubeRotate(attacker.Color));
        while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        for(int i = 0; i < attacked.Count; i++)
        {
            //activate indicator
            attacker.Indicator.SetActive(true);
            attacked[i].GetComponent<Object>().Indicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            LookAt(attacker.gameObject, attacked[i]);

            if (attacker.Type == ObjectType.ENEMY)
                attacked[i].GetComponent<Object>().OnHit(StatusEffect.HP, attacker.Damage);
            else if (attacker.Type == ObjectType.PLAYER || attacker.Type == ObjectType.FRIEND)
                attacked[i].GetComponent<Object>().OnHit(StatusEffect.HP, attacker.Damage * Mathf.Pow(1.2f, attacked.Count - 1));
            yield return new WaitForFixedUpdate();

            //disable indicator
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
            if (weaponType == WeaponType.SWORD || weaponType == WeaponType.STAFF)
            {
                if (p.Color == color && AttackableRange(weaponType, index)[p.Index])
                    attackable.Add(StageManager.Instance.Player.gameObject);
            }
            else if (weaponType == WeaponType.HOLY)
            {
                if (p.Color != color && p.Index == index)
                    attackable.Add(StageManager.Instance.Player.gameObject);
            }
        }
        else if (objType == ObjectType.FRIEND)
        {
            for (int i = 0; i < 3; i++)
            {
                if (StageManager.Instance.FriendList[i] == null || StageManager.Instance.FriendList[i].GetComponent<Object>().HP <= 0) continue;

                Object f = StageManager.Instance.FriendList[i].GetComponent<Object>();
                if (weaponType == WeaponType.SWORD || weaponType == WeaponType.STAFF)
                {
                    if (f.Color == color && AttackableRange(weaponType, index)[f.Index])
                        attackable.Add(StageManager.Instance.FriendList[i]);
                }
                else if (weaponType == WeaponType.HOLY)
                {
                    if (f.Color != color && f.Index == index)
                        attackable.Add(StageManager.Instance.FriendList[i]);
                }
            }
        }
        else if (objType == ObjectType.ENEMY)
        {
            foreach (GameObject enemyObj in StageManager.Instance.EnemyList)
            {
                if (enemyObj.GetComponent<Object>().HP <= 0) continue;

                Object e = enemyObj.GetComponent<Object>();
                if (weaponType == WeaponType.SWORD || weaponType == WeaponType.STAFF)
                {
                    if (e.Color == color && AttackableRange(weaponType, index)[e.Index])
                        attackable.Add(enemyObj);
                } 
                else if (weaponType == WeaponType.HOLY)
                {
                    if (e.Color != color && e.Index == index)
                        attackable.Add(enemyObj);
                }
            }
        }
        return attackable;
    }

    
    private bool[] AttackableRange(WeaponType weaponType, int index)
    {
        bool[] attackable = new bool[9];
        for (int i = 0; i < 9; i++)
            attackable[i] = false;

        if (weaponType == WeaponType.SWORD)
        {
            attackable = StageCube.Instance.Cross(index);
        }
        else if (weaponType == WeaponType.STAFF)
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

        return attackable;
    }

}