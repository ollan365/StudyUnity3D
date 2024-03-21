using UnityEngine;
using System.Collections;
using static Constants;

public class EnvLogic : MonoBehaviour
{
    [SerializeField] private ColorCheckManager colorCheckManager;
    public IEnumerator MoveEnemy()
    {
        foreach(GameObject e in StageManager.Instance.EnemyList)
        {
            colorCheckManager.CharacterSelect(e);

            Object enemyObj = e.GetComponent<Object>();

            int random = Random.Range(0, 9);
            if (colorCheckManager.Move(enemyObj.Color, random, true))
            {
                StageManager.Instance.CubeRotate(enemyObj.Color);

                if (StageCube.Instance.touchArray[enemyObj.Color.ToInt()][random].ObjType == ObjectType.TREASURE)
                    StageCube.Instance.touchArray[enemyObj.Color.ToInt()][random].Obj.OnHit(9999);
            }

            yield return new WaitForSeconds(1f); // move에 걸리는 시간
            colorCheckManager.CharacterSelectCancel(e);
        }
    }
}
