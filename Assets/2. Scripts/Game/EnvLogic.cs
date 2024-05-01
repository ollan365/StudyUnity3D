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

            if (enemyObj.Color != StageManager.Instance.Player.Color) // �÷��̾�� �ٸ� ���� ��
            {
                for (int i = 0; i < 3; i++) // �̵��� ���� ���� �õ�
                {
                    int random = Random.Range(0, 9);

                    if (colorCheckManager.Move(enemyObj.Color, random, false))
                    {
                        Debug.Log($"{enemyObj.Color}: {random}");

                        StartCoroutine(StageManager.Instance.CubeRotate(enemyObj.Color));
                        yield return new WaitForSeconds(2f); // CubeRotate�� �ɸ��� �ð�

                        colorCheckManager.Move(enemyObj.Color, random, true); // �̵�
                        yield return new WaitForSeconds(2f); // Move�� �ɸ��� �ð�

                        if (StageCube.Instance.touchArray[enemyObj.Color.ToInt()][random].ObjType == ObjectType.TREASURE)
                            StageCube.Instance.touchArray[enemyObj.Color.ToInt()][random].Obj.OnHit(9999);

                        break;
                    }
                }
            }
            else // �÷��̾�� ���� ���� ��
            {
                List<int> priority = GetPriorityMoveCube(enemyObj.AttackType);

                for (int i = 0; i < priority.Count; i++)
                {
                    if (enemyObj.Index == priority[i]) break; // ������ ���� ��ġ���� �켱������ �������� �̵� ����

                    if (colorCheckManager.Move(enemyObj.Color, priority[i], false))
                    {
                        Debug.Log($"{priority[i]}");

                        StartCoroutine(StageManager.Instance.CubeRotate(enemyObj.Color));
                        yield return new WaitForSeconds(2f); // CubeRotate�� �ɸ��� �ð�

                        colorCheckManager.Move(enemyObj.Color, priority[i], true);
                        yield return new WaitForSeconds(2f); // Move�� �ɸ��� �ð�

                        if (StageCube.Instance.touchArray[enemyObj.Color.ToInt()][i].ObjType == ObjectType.TREASURE)
                            StageCube.Instance.touchArray[enemyObj.Color.ToInt()][i].Obj.OnHit(9999);
                        break;
                    }
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

        if (weaponType == WeaponType.CAD)
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
        else if (weaponType == WeaponType.LAD)
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
        // array�� �����ϰ� ����
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
