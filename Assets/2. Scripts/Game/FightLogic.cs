//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static Constants;

//public class FightLogic : MonoBehaviour
//{
//    [SerializeField] private ColorCheckManager colorCheckManager;
//    [SerializeField] private StageManager stageManager;
//    private IEnumerator Attack()
//    {
//        additionalMoveCount = 0;
//        for (int i = 0; i < 6; i++) // 빙고 확인
//        {
//            BingoStatus bingo = colorCheckManager.BingoCheck(i, true);
//            int random = Random.Range(0, 2);

//            if (bingo == BingoStatus.DEFAULT) continue;

//            StartCoroutine(stageManager.CubeRotate(i.ToColor())); // 빙고 완성 시 그 면으로 회전
//            while (isCubeMove) yield return new WaitForFixedUpdate();

//            if (random == 0)
//            {
//                if (bingo == BingoStatus.ALL || player.GetComponent<Object>().Color.ToInt() == i)
//                    player.GetComponent<Object>().HP_Percent(10);
//                foreach (GameObject f in friend)
//                {
//                    if (f == null || !f.activeSelf) continue;

//                    if (bingo == BingoStatus.ALL || f.GetComponent<Object>().Color.ToInt() == i)
//                        f.GetComponent<Object>().HP_Percent(10);
//                }
//            }
//            else
//            {
//                if (bingo == BingoStatus.ONE) additionalMoveCount++;
//                else changeCount++;
//            }

//            yield return new WaitForSeconds(0.5f);
//        }

//        // 플레이어부터 공격
//        Object playerObj = player.GetComponent<Object>();

//        StartCoroutine(CubeRotate(playerObj.Color));
//        while (isCubeMove) yield return new WaitForFixedUpdate();

//        List<GameObject> attackableEnemy = AttackableObject(playerObj.AttackType, playerObj.Color, playerObj.Index, ObjectType.ENEMY);
//        foreach (GameObject enemy in attackableEnemy)
//        {
//            enemy.GetComponent<Object>().OnHit(playerObj.Damage);
//            yield return new WaitForFixedUpdate();

//            yield return new WaitForSeconds(0.1f);
//        }

//        // 적과 동료의 공격 순서 결정을 위해 List 생성
//        List<KeyValuePair<int, int>> enemyAttackOrder = new List<KeyValuePair<int, int>>();
//        for (int i = 0; i < stageDatas[ENEMY_COUNT]; i++)
//        {
//            Object enemyObject = enemy[i].GetComponent<Object>();
//            if (enemyObject.gameObject.activeSelf)
//                enemyAttackOrder.Add(new KeyValuePair<int, int>(enemyObject.Damage, i));
//            else // enemyAttackOrder의 크기가 friendAttackOrder 보다 커야하므로 죽은 애들도 일단 list에 넣기
//                enemyAttackOrder.Add(new KeyValuePair<int, int>(0, i));
//        }
//        List<KeyValuePair<int, int>> friendAttackOrder = new List<KeyValuePair<int, int>>();
//        for (int i = 0; i < 3; i++)
//        {
//            if (friend[i] == null) continue;
//            Object friendObject = friend[i].GetComponent<Object>();
//            if (friendObject.gameObject.activeSelf)
//                friendAttackOrder.Add(new KeyValuePair<int, int>(friendObject.Damage, i));
//        }
//        enemyAttackOrder = enemyAttackOrder.OrderByDescending(enemyAttackOrder => enemyAttackOrder.Key).ToList(); // 공격력 순으로 내림차순 정렬
//        friendAttackOrder = friendAttackOrder.OrderByDescending(friendAttackOrder => friendAttackOrder.Key).ToList();

//        if (!enemy[enemyAttackOrder[0].Value].activeSelf) // 살아있는 enemy가 없으면
//        {
//            ClearStage();
//            yield break;
//        }

//        for (int i = 0; i < enemyAttackOrder.Count; i++)
//        {
//            if (i < friendAttackOrder.Count && friend[friendAttackOrder[i].Value].activeSelf) // 동료도 있다면
//            {
//                Object friendObj = friend[friendAttackOrder[i].Value].GetComponent<Object>();
//                attackableEnemy = AttackableObject(friendObj.AttackType, friendObj.Color, friendObj.Index, ObjectType.ENEMY);

//                StartCoroutine(CubeRotate(friendObj.Color));
//                while (isCubeMove) yield return new WaitForFixedUpdate();

//                foreach (GameObject enemy in attackableEnemy)
//                {
//                    enemy.GetComponent<Object>().OnHit(friendAttackOrder[i].Key);
//                    yield return new WaitForFixedUpdate();

//                    yield return new WaitForSeconds(0.1f);
//                }
//            }

//            // 적 공격
//            if (!enemy[enemyAttackOrder[i].Value].activeSelf) continue;

//            Object enemyObj = enemy[enemyAttackOrder[i].Value].GetComponent<Object>();
//            List<GameObject> attackablePlayerTeam;

//            attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.PLAYER);

//            StartCoroutine(CubeRotate(enemyObj.Color));
//            while (isCubeMove) yield return new WaitForFixedUpdate();

//            foreach (GameObject p in attackablePlayerTeam)
//            {
//                p.GetComponent<Object>().OnHit(enemyAttackOrder[i].Key);
//                yield return new WaitForFixedUpdate();

//                yield return new WaitForSeconds(0.1f);
//            }

//            if (!player.activeSelf) // 플레이어가 죽으면 게임 종료
//            {
//                GameOver();
//                yield break;
//            }

//            attackablePlayerTeam = AttackableObject(enemyObj.AttackType, enemyObj.Color, enemyObj.Index, ObjectType.FRIEND);
//            foreach (GameObject pTeam in attackablePlayerTeam)
//            {
//                pTeam.GetComponent<Object>().OnHit(enemyAttackOrder[i].Key);
//                yield return new WaitForFixedUpdate();

//                yield return new WaitForSeconds(0.1f);
//            }
//        }

//        // statge statue를 바꾼다
//        StatusOfStage = StageStatus.PLAYER;
//    }
//    private List<GameObject> AttackableObject(WeaponType weaponType, Colors color, int index, ObjectType objType)
//    {
//        List<GameObject> attackable = new List<GameObject>();

//        if (objType == ObjectType.PLAYER && player.activeSelf)
//        {
//            Object p = player.GetComponent<Object>();
//            if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
//            {
//                if (p.Color == color && AttackableRange(weaponType, index)[p.Index])
//                    attackable.Add(player);
//            }
//            else if (weaponType == WeaponType.AP)
//            {
//                if (p.Color != color && p.Index == index)
//                    attackable.Add(player);
//            }
//        }
//        else if (objType == ObjectType.FRIEND)
//        {
//            for (int i = 0; i < 3; i++)
//            {
//                if (friend[i] == null || !friend[i].activeSelf) continue;

//                Object f = friend[i].GetComponent<Object>();
//                if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
//                {
//                    if (f.Color == color && AttackableRange(weaponType, index)[f.Index])
//                        attackable.Add(friend[i]);
//                }
//                else if (weaponType == WeaponType.AP)
//                {
//                    if (f.Color != color && f.Index == index)
//                        attackable.Add(friend[i]);
//                }
//            }
//        }
//        else if (objType == ObjectType.ENEMY)
//        {
//            foreach (GameObject enemyObj in enemy)
//            {
//                if (!enemyObj.activeSelf) continue;

//                Object e = enemyObj.GetComponent<Object>();
//                if (weaponType == WeaponType.CAD || weaponType == WeaponType.LAD)
//                {
//                    if (e.Color == color && AttackableRange(weaponType, index)[e.Index])
//                        attackable.Add(enemyObj);
//                }
//                else if (weaponType == WeaponType.AP)
//                {
//                    if (e.Color != color && e.Index == index)
//                        attackable.Add(enemyObj);
//                }
//            }
//        }
//        return attackable;
//    }
//    private bool[] AttackableRange(WeaponType weaponType, int index)
//    {
//        bool[] attackable = new bool[9];
//        for (int i = 0; i < 9; i++)
//            attackable[i] = false;

//        if (weaponType == WeaponType.CAD)
//        {
//            switch (index)
//            {
//                case 0:
//                    attackable[1] = true;
//                    attackable[3] = true;
//                    break;
//                case 1:
//                    attackable[0] = true;
//                    attackable[2] = true;
//                    attackable[4] = true;
//                    break;
//                case 2:
//                    attackable[1] = true;
//                    attackable[5] = true;
//                    break;
//                case 3:
//                    attackable[0] = true;
//                    attackable[4] = true;
//                    attackable[6] = true;
//                    break;
//                case 4:
//                    attackable[1] = true;
//                    attackable[3] = true;
//                    attackable[5] = true;
//                    attackable[7] = true;
//                    break;
//                case 5:
//                    attackable[2] = true;
//                    attackable[4] = true;
//                    attackable[8] = true;
//                    break;
//                case 6:
//                    attackable[3] = true;
//                    attackable[7] = true;
//                    break;
//                case 7:
//                    attackable[4] = true;
//                    attackable[6] = true;
//                    attackable[8] = true;
//                    break;
//                case 8:
//                    attackable[5] = true;
//                    attackable[7] = true;
//                    break;
//                default:
//                    break;
//            }
//        }
//        else if (weaponType == WeaponType.LAD)
//        {
//            switch (index)
//            {
//                case 0:
//                    attackable[5] = true;
//                    break;
//                case 1:
//                    attackable[3] = true;
//                    attackable[5] = true;
//                    break;
//                case 2:
//                    attackable[4] = true;
//                    break;
//                case 3:
//                    attackable[1] = true;
//                    attackable[7] = true;
//                    break;
//                case 4:
//                    attackable[0] = true;
//                    attackable[2] = true;
//                    attackable[6] = true;
//                    attackable[8] = true;
//                    break;
//                case 5:
//                    attackable[1] = true;
//                    attackable[7] = true;
//                    break;
//                case 6:
//                    attackable[4] = true;
//                    break;
//                case 7:
//                    attackable[3] = true;
//                    attackable[5] = true;
//                    break;
//                case 8:
//                    attackable[4] = true;
//                    break;
//                default:
//                    break;
//            }
//        }

//        return attackable;
//    }

//}
