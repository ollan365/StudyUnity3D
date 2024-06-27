using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Constants;
using static Excel;

public class FightLogic : MonoBehaviour
{
    public IEnumerator Attack()
    {
        StartCoroutine(StageManager.Instance.CubeRotate(StageManager.Instance.Player.Color));
        while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        List<GameObject> attackableEnemy = AttackableObject(StageManager.Instance.Player.AttackType, StageManager.Instance.Player.Color, StageManager.Instance.Player.Index, ObjectType.ENEMY);
        foreach (GameObject enemy in attackableEnemy)
        {
            //�÷��̾ �� ����
            LookAt(StageManager.Instance.Player.gameObject, enemy);
            enemy.GetComponent<Object>().OnHit(StatusEffect.HP, StageManager.Instance.Player.Damage);
            if (!enemy.activeSelf) {
                StageManager.Instance.SetStageTextValue(StageText.MONSTER, -1);
            }
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

        if (enemyAttackOrder.Count <= 0) // ����ִ� enemy�� ������
        {
            StageManager.Instance.ClearStage();
            yield break;
        }

        // ����
        int count = enemyAttackOrder.Count > friendAttackOrder.Count ? enemyAttackOrder.Count : friendAttackOrder.Count;
        for (int i = 0; i < count; i++)
        {
            // ���� ������
            if (i < friendAttackOrder.Count && StageManager.Instance.FriendList[friendAttackOrder[i].Value].activeSelf) // ���ᵵ �ִٸ�
            {
                Object friendObj = StageManager.Instance.FriendList[friendAttackOrder[i].Value].GetComponent<Object>();
                attackableEnemy = AttackableObject(friendObj.AttackType, friendObj.Color, friendObj.Index, ObjectType.ENEMY);

                // ������ ���� ���� ��
                if (attackableEnemy.Count > 0)
                {
                    StartCoroutine(StageManager.Instance.CubeRotate(friendObj.Color));
                    while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
                    yield return new WaitForSeconds(1f);

                    foreach (GameObject enemy in attackableEnemy)
                    {
                        //���ᰡ �� ����
                        LookAt(friendObj.gameObject, enemy);
                        enemy.GetComponent<Object>().OnHit(StatusEffect.HP, friendAttackOrder[i].Key);
                        if (!enemy.activeSelf)
                        {
                            StageManager.Instance.SetStageTextValue(StageText.MONSTER, -1);
                        }

                        yield return new WaitForSeconds(0.5f);
                    }
                }
            }

            // �� ����
            if (i < enemyAttackOrder.Count && StageManager.Instance.EnemyList[enemyAttackOrder[i].Value].activeSelf)
            {
                Object enemyObj = StageManager.Instance.EnemyList[enemyAttackOrder[i].Value].GetComponent<Object>();
                List<GameObject> attackablePlayerTeam;

                attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.PLAYER);
                attackablePlayerTeam.AddRange(AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.FRIEND));

                if (attackablePlayerTeam.Count == 0) continue;

                StartCoroutine(StageManager.Instance.CubeRotate(enemyObj.Color));
                while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
                yield return new WaitForSeconds(1f);

                foreach (GameObject p in attackablePlayerTeam)
                {
                    //���� �÷��̾� ���� ����
                    LookAt(enemyObj.gameObject, p);
                    p.GetComponent<Object>().OnHit(StatusEffect.HP, enemyAttackOrder[i].Key);
                   
                    yield return new WaitForSeconds(0.5f);
                }


                if (!StageManager.Instance.Player.gameObject.activeSelf) // �÷��̾ ������ ���� ����
                {
                    StageManager.Instance.GameOver();
                    yield break;
                }
            }

        }
        // statge statue�� �ٲ۴�
        StageManager.Instance.ChangeStatus();
    }

    private void LookAt(GameObject src, GameObject dst)
    {
        Vector3 direc = src.transform.position - dst.transform.position;
        Debug.Log("src: " + src + " " + src.transform.position);
        Debug.Log("dst: " + dst + " " + dst.transform.position);
        Debug.Log(direc.normalized);
        
        Quaternion rot = Quaternion.LookRotation(direc);
        Debug.Log(rot.eulerAngles);
        
        src.transform.rotation = rot;
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