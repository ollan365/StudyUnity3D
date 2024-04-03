using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Constants;
using static Excel;

public class FightLogic : MonoBehaviour
{
    [SerializeField] private ColorCheckManager colorCheckManager;
    public IEnumerator Attack()
    {
        for (int i = 0; i < 6; i++) // ���� Ȯ��
        {
            int bingoCnt = colorCheckManager.BingoTextChange(i);

            if (bingoCnt == 0) continue;

            StartCoroutine(StageManager.Instance.CubeRotate(i.ToColor())); // ���� �ϼ� �� �� ������ ȸ��
            while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();

            BingoReward(i, bingoCnt);

            yield return new WaitForSeconds(0.5f);
        }

        Debug.Log("Bingo End!");

        StartCoroutine(StageManager.Instance.CubeRotate(StageManager.Instance.Player.Color));
        while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        List<GameObject> attackableEnemy = AttackableObject(StageManager.Instance.Player.AttackType, StageManager.Instance.Player.Color, StageManager.Instance.Player.Index, ObjectType.ENEMY);
        foreach (GameObject enemy in attackableEnemy)
        {
            enemy.GetComponent<Object>().OnHit(StageManager.Instance.Player.Damage);
            yield return new WaitForFixedUpdate();

            yield return new WaitForSeconds(0.1f);
        }

        // ���� ������ ���� ���� ������ ���� List ����
        List<KeyValuePair<int, int>> enemyAttackOrder = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < StageManager.Instance.StageData(ENEMY_COUNT); i++)
        {
            Object enemyObject = StageManager.Instance.EnemyList[i].GetComponent<Object>();
            if (enemyObject.gameObject.activeSelf)
                enemyAttackOrder.Add(new KeyValuePair<int, int>(enemyObject.Damage, i));
            else // enemyAttackOrder�� ũ�Ⱑ friendAttackOrder ���� Ŀ���ϹǷ� ���� �ֵ鵵 �ϴ� list�� �ֱ�
                enemyAttackOrder.Add(new KeyValuePair<int, int>(0, i));
        }
        List<KeyValuePair<int, int>> friendAttackOrder = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < 3; i++)
        {
            if (StageManager.Instance.FriendList[i] == null) continue;
            Object friendObject = StageManager.Instance.FriendList[i].GetComponent<Object>();
            if (friendObject.gameObject.activeSelf)
                friendAttackOrder.Add(new KeyValuePair<int, int>(friendObject.Damage, i));
        }
        enemyAttackOrder = enemyAttackOrder.OrderByDescending(enemyAttackOrder => enemyAttackOrder.Key).ToList(); // ���ݷ� ������ �������� ����
        friendAttackOrder = friendAttackOrder.OrderByDescending(friendAttackOrder => friendAttackOrder.Key).ToList();

        if (!StageManager.Instance.EnemyList[enemyAttackOrder[0].Value].activeSelf) // ����ִ� enemy�� ������
        {
            StageManager.Instance.ClearStage();
            yield break;
        }

        for (int i = 0; i < enemyAttackOrder.Count; i++)
        {
            if (i < friendAttackOrder.Count && StageManager.Instance.FriendList[friendAttackOrder[i].Value].activeSelf) // ���ᵵ �ִٸ�
            {
                Object friendObj = StageManager.Instance.FriendList[friendAttackOrder[i].Value].GetComponent<Object>();
                attackableEnemy = AttackableObject(friendObj.AttackType, friendObj.Color, friendObj.Index, ObjectType.ENEMY);

                StartCoroutine(StageManager.Instance.CubeRotate(friendObj.Color));
                while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
                yield return new WaitForSeconds(1f);

                foreach (GameObject enemy in attackableEnemy)
                {
                    enemy.GetComponent<Object>().OnHit(friendAttackOrder[i].Key);
                    yield return new WaitForFixedUpdate();

                    yield return new WaitForSeconds(0.1f);
                }
            }

            // �� ����
            if (!StageManager.Instance.EnemyList[enemyAttackOrder[i].Value].activeSelf) continue;

            Object enemyObj = StageManager.Instance.EnemyList[enemyAttackOrder[i].Value].GetComponent<Object>();
            List<GameObject> attackablePlayerTeam;

            attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.PLAYER);

            StartCoroutine(StageManager.Instance.CubeRotate(enemyObj.Color));
            while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
            yield return new WaitForSeconds(1f);

            foreach (GameObject p in attackablePlayerTeam)
            {
                p.GetComponent<Object>().OnHit(enemyAttackOrder[i].Key);
                yield return new WaitForFixedUpdate();

                yield return new WaitForSeconds(0.1f);
            }

            if (!StageManager.Instance.Player.gameObject.activeSelf) // �÷��̾ ������ ���� ����
            {
                StageManager.Instance.GameOver();
                yield break;
            }

            attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.FRIEND);
            foreach (GameObject pTeam in attackablePlayerTeam)
            {
                pTeam.GetComponent<Object>().OnHit(enemyAttackOrder[i].Key);
                yield return new WaitForFixedUpdate();

                yield return new WaitForSeconds(0.1f);
            }
        }

        // statge statue�� �ٲ۴�
        StageManager.Instance.ChangeStatus();
    }
    
    private void BingoReward(int sideColor, int bingoCnt)
    {
        if (bingoCnt == 6) return; // �� ����
        if (bingoCnt > 0) return; // �� ���� ���� �ٸ� ���� ���� ���� �ϼ� ����
        //if (random == 0)
        //{
        //    if (bingo == BingoStatus.ALL || StageManager.Instance.Player.Color.ToInt() == i)
        //        StageManager.Instance.Player.HP_Percent(10);
        //    foreach (GameObject f in StageManager.Instance.FriendList)
        //    {
        //        if (f == null || !f.activeSelf) continue;

        //        if (bingo == BingoStatus.ALL || f.GetComponent<Object>().Color.ToInt() == i)
        //            f.GetComponent<Object>().HP_Percent(10);
        //    }
        //}
        //else
        //{
        //    // if (bingo == BingoStatus.ONE) additionalMoveCount++;
        //    // else changeCount++;
        //}
    }
    private List<GameObject> AttackableObject(WeaponType weaponType, Colors color, int index, ObjectType objType)
    {
        List<GameObject> attackable = new();

        if (objType == ObjectType.PLAYER && StageManager.Instance.Player.gameObject.activeSelf)
        {
            Object p = StageManager.Instance.Player;
            if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
            {
                if (p.Color == color && AttackableRange(weaponType, index)[p.Index])
                    attackable.Add(StageManager.Instance.Player.gameObject);
            }
            else if (weaponType == WeaponType.AP)
            {
                if (p.Color != color && p.Index == index)
                    attackable.Add(StageManager.Instance.Player.gameObject);
            }
        }
        else if (objType == ObjectType.FRIEND)
        {
            for (int i = 0; i < 3; i++)
            {
                if (StageManager.Instance.FriendList[i] == null || !StageManager.Instance.FriendList[i].activeSelf) continue;

                Object f = StageManager.Instance.FriendList[i].GetComponent<Object>();
                if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
                {
                    if (f.Color == color && AttackableRange(weaponType, index)[f.Index])
                        attackable.Add(StageManager.Instance.FriendList[i]);
                }
                else if (weaponType == WeaponType.AP)
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
                if (!enemyObj.activeSelf) continue;

                Object e = enemyObj.GetComponent<Object>();
                if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
                {
                    if (e.Color == color && AttackableRange(weaponType, index)[e.Index])
                        attackable.Add(enemyObj);
                }
                else if (weaponType == WeaponType.AP)
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

        if (weaponType == WeaponType.CAD)
        {
            switch (index)
            {
                case 0:
                    attackable[1] = true;
                    attackable[3] = true;
                    break;
                case 1:
                    attackable[0] = true;
                    attackable[2] = true;
                    attackable[4] = true;
                    break;
                case 2:
                    attackable[1] = true;
                    attackable[5] = true;
                    break;
                case 3:
                    attackable[0] = true;
                    attackable[4] = true;
                    attackable[6] = true;
                    break;
                case 4:
                    attackable[1] = true;
                    attackable[3] = true;
                    attackable[5] = true;
                    attackable[7] = true;
                    break;
                case 5:
                    attackable[2] = true;
                    attackable[4] = true;
                    attackable[8] = true;
                    break;
                case 6:
                    attackable[3] = true;
                    attackable[7] = true;
                    break;
                case 7:
                    attackable[4] = true;
                    attackable[6] = true;
                    attackable[8] = true;
                    break;
                case 8:
                    attackable[5] = true;
                    attackable[7] = true;
                    break;
                default:
                    break;
            }
        }
        else if (weaponType == WeaponType.LAD)
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