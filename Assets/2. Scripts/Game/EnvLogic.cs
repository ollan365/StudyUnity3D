using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Constants;

public class EnvLogic : MonoBehaviour
{
    [SerializeField] private ColorCheckManager colorCheckManager;
    public IEnumerator MoveEnemy()
    {
        foreach(GameObject e in StageManager.Instance.EnemyList)
        {
            if (!e.activeSelf) continue;

            Object enemyObj = e.GetComponent<Object>();
            colorCheckManager.CharacterSelect(e);

            List<int> priority = GetPriorityMoveCube(enemyObj.AttackType);

            for(int i = 0; i < priority.Count; i++)
            {
                if (enemyObj.Index == priority[i]) break; // 본인의 현재 위치보다 우선순위가 낮아지면 이동 안함

                if (colorCheckManager.Move(enemyObj.Color, priority[i], true))
                {
                    StartCoroutine(StageManager.Instance.CubeRotate(enemyObj.Color));
                    yield return new WaitForSeconds(2f); // CubeRotate에 걸리는 시간
                     
                    if (StageCube.Instance.touchArray[enemyObj.Color.ToInt()][i].ObjType == ObjectType.TREASURE)
                        StageCube.Instance.touchArray[enemyObj.Color.ToInt()][i].Obj.OnHit(9999);
                    break;
                }
            }
            colorCheckManager.CharacterSelectCancel(e, true);
        }
        StageManager.Instance.ChangeStatus();
    }

    private List<int> GetPriorityMoveCube(WeaponType weaponType)
    {
        int playerIndex = StageManager.Instance.Player.Index;

        List<int> priorityMoveCube = new();

        List<int> first = new(), second = new(), third = new(), fourth = new(), fifth = new();
        
        // ============== 구석 ================ //
        if (playerIndex == 0 || playerIndex == 8)
        {
            first = RandomArray(new List<int> { 0 });
            second = RandomArray(new List<int> { 1, 3 });
            third = RandomArray(new List<int> { 2, 4, 6 });
            fourth = RandomArray(new List<int> { 5, 7 });
            fifth = RandomArray(new List<int> { 8 });
        }
        else if (playerIndex == 2 || playerIndex == 6)
        {
            first = RandomArray(new List<int> { 2 });
            second = RandomArray(new List<int> { 1, 5 });
            third = RandomArray(new List<int> { 0, 4, 8 });
            fourth = RandomArray(new List<int> { 3, 7 });
            fifth = RandomArray(new List<int> { 6 });
        }

        if ((weaponType == WeaponType.CAD && (playerIndex == 0 || playerIndex == 2))
            || (weaponType == WeaponType.LAD && (playerIndex == 8 || playerIndex == 6)))
        {
            if (weaponType == WeaponType.LAD) priorityMoveCube.Add(4);

            priorityMoveCube.AddRange(first);
            priorityMoveCube.AddRange(second);
            priorityMoveCube.AddRange(third);
            priorityMoveCube.AddRange(fourth);
            priorityMoveCube.AddRange(fifth);
        }
        else if ((weaponType == WeaponType.CAD && (playerIndex == 8 || playerIndex == 6))
            || (weaponType == WeaponType.LAD && (playerIndex == 0 || playerIndex == 2)))
        {
            if (weaponType == WeaponType.LAD) priorityMoveCube.Add(4);

            priorityMoveCube.AddRange(fifth);
            priorityMoveCube.AddRange(fourth);
            priorityMoveCube.AddRange(third);
            priorityMoveCube.AddRange(second);
            priorityMoveCube.AddRange(first);
        }

        // ============== 가장자리 가운데 ================ //
        if (playerIndex == 1)
        {
            first = RandomArray(new List<int> { 0, 4, 2 });
            second = RandomArray(new List<int> { 3, 7, 5 });
            third = RandomArray(new List<int> { 6, 8 });
        }
        else if(playerIndex == 3)
        {
            first = RandomArray(new List<int> { 0, 4, 6 });
            second = RandomArray(new List<int> { 1, 5, 7 });
            third = RandomArray(new List<int> { 2, 8 });
        }
        else if (playerIndex == 5)
        {
            first = RandomArray(new List<int> { 2, 4, 8 });
            second = RandomArray(new List<int> { 1, 3, 7 });
            third = RandomArray(new List<int> { 0, 6 });
        }
        else if (playerIndex == 7)
        {
            first = RandomArray(new List<int> { 6, 4, 8 });
            second = RandomArray(new List<int> { 3, 1, 5 });
            third = RandomArray(new List<int> { 0, 2 });
        }
        if(playerIndex == 1 && playerIndex == 3 && playerIndex == 5 && playerIndex == 7)
        {
            if(weaponType == WeaponType.LAD)
            {
                if (playerIndex == 1 || playerIndex == 7)
                    priorityMoveCube.AddRange(RandomArray(new List<int> { 3, 5 }));
                else if (playerIndex == 3 || playerIndex == 5)
                    priorityMoveCube.AddRange(RandomArray(new List<int> { 1, 7 }));
            }
            priorityMoveCube.AddRange(first);
            priorityMoveCube.AddRange(second);
            priorityMoveCube.AddRange(third);
        }

        // ============== 가운데 ================ //
        if(playerIndex == 4)
        {
            first = RandomArray(new List<int> { 1, 3, 5, 7 });
            second = RandomArray(new List<int> { 2, 4, 6, 8 });

            if (weaponType == WeaponType.CAD)
            {
                priorityMoveCube.AddRange(first);
                priorityMoveCube.AddRange(second);
            }
            else if (weaponType == WeaponType.LAD)
            {
                priorityMoveCube.AddRange(second);
                priorityMoveCube.AddRange(first);
            }
        }
        return priorityMoveCube;
    }

    private List<int> RandomArray(List<int> array)
    {
        List<int> output = new List<int>();

        while (array.Count > 2)
        {
            int random = Random.Range(0, array.Count);
            output.Add(array[random]);
            array.RemoveAt(random);
        }

        if (array.Count == 2)
        {
            int random = Random.Range(0, 2);

            if (random == 0) { output.Add(array[0]); output.Add(array[1]); }
            else if (random == 1) { output.Add(array[1]); output.Add(array[0]); }
        }
        else return array; // 한개짜리 리스트

        return output;
    }
}
