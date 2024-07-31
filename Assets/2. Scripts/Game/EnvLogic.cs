using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static Constants;

public class EnvLogic : MonoBehaviour
{
    private bool isMoving = false;
    public IEnumerator EnvLogicStart()
    {
        foreach (GameObject e in StageManager.Instance.EnemyList)
        {
            if (e.GetComponent<Object>().HP <= 0) continue;

            Object enemyObj = e.GetComponent<Object>();
            ColorCheckManager.Instance.CharacterSelect(e);

            List<int> priority = GetPriorityMoveCube(enemyObj.AttackType);
            if (enemyObj.Color != StageManager.Instance.Player.Color)
                priority = new() { Random.Range(0, 9), Random.Range(0, 9), Random.Range(0, 9) };

            for (int i = 0; i < priority.Count; i++)
            {
                if (enemyObj.Index == priority[i]) break; // 본인의 현재 위치보다 우선순위가 낮아지면 이동 안함

                if (ColorCheckManager.Instance.Move(enemyObj.Color, priority[i], false))
                {
                    isMoving = true;
                    StartCoroutine(MoveEnemy(enemyObj, priority[i]));
                    break;
                }
            }

            while (isMoving) yield return new WaitForFixedUpdate();

            ColorCheckManager.Instance.CharacterSelectCancel(e, true);
        }
        yield return new WaitForFixedUpdate();
        StageManager.Instance.ChangeStatus(StageStatus.PLAYER);
    }
    private IEnumerator MoveEnemy(Object enemy, int index)
    {
        StartCoroutine(StageManager.Instance.CubeRotate(enemy.Color));
        yield return new WaitForSeconds(1f); // CubeRotate에 걸리는 시간

        enemy.Indicator.SetActive(true);
        yield return new WaitForSeconds(1f);

        ColorCheckManager.Instance.Move(enemy.Color, index, true);
        yield return new WaitForSeconds(1f); // Move에 걸리는 시간
        enemy.Indicator.SetActive(false);
        yield return new WaitForSeconds(1f);

        if (StageCube.Instance.touchArray[enemy.Color.ToInt()][index].ObjType == ObjectType.TRIGGER)
            StageCube.Instance.touchArray[enemy.Color.ToInt()][index].Obj.OnHit(StatusEffect.HP_PERCENT, 100);

        isMoving = false;
    }

    private List<int> GetPriorityMoveCube(WeaponType weaponType)
    {
        int playerIndex = StageManager.Instance.Player.Index;

        List<int> priorityMoveCube = new();

        if (weaponType == WeaponType.SWORD)
        {
            switch (playerIndex)
            {
                case 0:
                    priorityMoveCube = new() { 1, 3, 2, 4, 6, 5, 7, 8 };
                    break;
                case 1:
                    priorityMoveCube = new() { 0, 2, 4, 3, 5, 7, 6, 8 };
                    break;
                case 2:
                    priorityMoveCube = new() { 1, 5, 0, 4, 8, 3, 7, 6 };
                    break;
                case 3:
                    priorityMoveCube = new() { 0, 4, 6, 1, 5, 7, 2, 8 };
                    break;
                case 4:
                    priorityMoveCube = new() { 1, 3, 5, 7, 0, 2, 6, 8 };
                    break;
                case 5:
                    priorityMoveCube = new() { 2, 4, 8, 1, 3, 7, 0, 6 };
                    break;
                case 6:
                    priorityMoveCube = new() { 3, 7, 0, 4, 8, 1, 5, 2 };
                    break;
                case 7:
                    priorityMoveCube = new() { 4, 6, 8, 1, 3, 5, 0, 2 };
                    break;
                case 8:
                    priorityMoveCube = new() { 5, 7, 2, 4, 6, 1, 3, 0 };
                    break;
            }
        }
        else if (weaponType == WeaponType.STAFF)
        {
            switch (playerIndex)
            {
                case 0:
                    priorityMoveCube = new() { 4, 8, 7, 5, 6, 2, 3, 1 };
                    break;
                case 1:
                    priorityMoveCube = new() { 5, 3, 8, 6, 7, 4, 2, 0 };
                    break;
                case 2:
                    priorityMoveCube = new() { 4, 6, 7, 3, 8, 0, 5, 1 };
                    break;
                case 3:
                    priorityMoveCube = new() { 7, 1, 8, 2, 5, 6, 4, 0 };
                    break;
                case 4:
                    priorityMoveCube = new() { 8, 6, 2, 0, 7, 5, 3, 1 };
                    break;
                case 5:
                    priorityMoveCube = new() { 7, 1, 6, 0, 3, 8, 4, 2 };
                    break;
                case 6:
                    priorityMoveCube = new() { 4, 2, 5, 1, 8, 0, 7, 3 };
                    break;
                case 7:
                    priorityMoveCube = new() { 5, 3, 2, 0, 1, 8, 6, 4 };
                    break;
                case 8:
                    priorityMoveCube = new() { 4, 0, 3, 1, 6, 2, 7, 5 };
                    break;
            }
        }

        return priorityMoveCube;
    }

    private List<int> RandomArray(List<int> array)
    {
        // array를 랜덤하게 섞음
        for (int i = 0; i < array.Count; i++)
        {
            int temp = array[i];
            int randomIndex = Random.Range(i, array.Count);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }

        return array;
    }
}
