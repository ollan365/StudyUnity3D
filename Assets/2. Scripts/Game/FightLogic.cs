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
        Time.timeScale = StageManager.Instance.TimeScaleValue;

        List<KeyValuePair<float, Object>> objectAttackOrder = ObjectAttackOrder();

        for (int i = 0; i < objectAttackOrder.Count; i++)
        {
            if (objectAttackOrder[i].Value.HP > 0)
            {
                List<GameObject> attackableEnemy = new();
                if (objectAttackOrder[i].Value.Type != ObjectType.ENEMY)
                    attackableEnemy = AttackableObject(objectAttackOrder[i].Value.AttackType, objectAttackOrder[i].Value.Color, objectAttackOrder[i].Value.Index, ObjectType.ENEMY);
                else
                {
                    attackableEnemy = AttackableObject(objectAttackOrder[i].Value.AttackType, objectAttackOrder[i].Value.Color, objectAttackOrder[i].Value.Index, ObjectType.PLAYER);
                    attackableEnemy.AddRange(AttackableObject(objectAttackOrder[i].Value.AttackType, objectAttackOrder[i].Value.Color, objectAttackOrder[i].Value.Index, ObjectType.FRIEND));
                }
                attacking = true;
                StartCoroutine(Attack(objectAttackOrder[i].Value, objectAttackOrder[i].Key, attackableEnemy));
                while (attacking) yield return new WaitForFixedUpdate();
            }
        }
        
        //딜교가 종료되면, 일정시간 기다렸다가 ENV 턴으로 넘어간다.
        yield return new WaitForSeconds(2.0f);
        // statge statue를 바꾼다
        if (StageManager.Instance.StatusOfStage != StageStatus.END)
            StageManager.Instance.ChangeStatus(StageStatus.ENV);

        Time.timeScale = 1;
    }
    private List<KeyValuePair<float, Object>> ObjectAttackOrder()
    {
        List<KeyValuePair<float, Object>> objectAttackOrder = new();

        // 플레이어가 가장 먼저 공격
        objectAttackOrder.Add(new KeyValuePair<float, Object>(StageManager.Instance.Player.Damage, StageManager.Instance.Player));

        // 적과 동료의 공격 순서 결정을 위해 List 생성
        List<KeyValuePair<float, Object>> enemyAttackOrder = new List<KeyValuePair<float, Object>>();
        for (int i = 0; i < StageManager.Instance.StageData(ENEMY_COUNT); i++)
        {
            Object enemyObject = StageManager.Instance.EnemyList[i].GetComponent<Object>();
            if (enemyObject.gameObject.activeSelf && enemyObject.HP > 0)
                enemyAttackOrder.Add(new KeyValuePair<float, Object>(enemyObject.Damage, enemyObject));
        }
        List<KeyValuePair<float, Object>> friendAttackOrder = new List<KeyValuePair<float, Object>>();
        for (int i = 0; i < 3; i++)
        {
            if (StageManager.Instance.FriendList[i] == null) continue;
            Object friendObject = StageManager.Instance.FriendList[i].GetComponent<Object>();
            if (friendObject.gameObject.activeSelf && friendObject.HP > 0)
                friendAttackOrder.Add(new KeyValuePair<float, Object>(friendObject.Damage, friendObject));
        }
        enemyAttackOrder = enemyAttackOrder.OrderByDescending(enemyAttackOrder => enemyAttackOrder.Key).ToList(); // 공격력 순으로 내림차순 정렬
        friendAttackOrder = friendAttackOrder.OrderByDescending(friendAttackOrder => friendAttackOrder.Key).ToList();

        int count = enemyAttackOrder.Count > friendAttackOrder.Count ? enemyAttackOrder.Count : friendAttackOrder.Count;

        for (int i = 0; i < count; i++)
        {
            if (friendAttackOrder.Count > i) objectAttackOrder.Add(friendAttackOrder[i]);
            if (enemyAttackOrder.Count > i) objectAttackOrder.Add(enemyAttackOrder[i]);
        }

        return objectAttackOrder;
    }
    private IEnumerator Attack(Object attacker, float damage, List<GameObject> attacked)
    {
        if (attacked.Count == 0)
        { attacking = false; yield break; }

        StartCoroutine(StageManager.Instance.CubeRotate(attacker.Color));
        while (StageManager.Instance.isCubeMove) yield return new WaitForFixedUpdate();
        yield return new WaitForSeconds(1f);

        // 보스 관련하여 일반 공격을 하는지 판단
        if (BossLogic(attacker)) yield break;

        // 일반 공격 시작
        // activate indicator
        attacker.Indicator.SetActive(true);

        GameObject chargingObject = null;
        if (attacker.AttackType == WeaponType.STAFF || attacker.AttackType == WeaponType.DUAL_STAFF)
        {
            // 차징 시작
            if (attacker.Type == ObjectType.ENEMY) chargingObject = ParticleManager.Instance.PlayParticle(attacker.gameObject, Particle.Enemy_Sttaff_Charging);
            else chargingObject = ParticleManager.Instance.PlayParticle(attacker.gameObject, Particle.PlayerTeam_Sttaff_Charging);
            yield return new WaitForSeconds(0.5f)  ;
        }

        // 일반 공격
        for (int i = 0; i < attacked.Count; i++)
        {
            attacked[i].GetComponent<Object>().Indicator.SetActive(true);
            yield return new WaitForSeconds(0.2f);

            LookAt(attacker.gameObject, attacked[i]);

            yield return new WaitForSeconds(AttackProduction(attacker.transform, attacked[i].transform, damage, Mathf.Pow(1.2f, i)));

            attacked[i].GetComponent<Object>().Indicator.SetActive(false);
            yield return new WaitForSeconds(0.1f);
        }

        if (chargingObject != null) Destroy(chargingObject);

        yield return new WaitForSeconds(0.5f);

        // disable indicator
        attacker.Indicator.SetActive(false);

        attacking = false;
    }
    private bool BossLogic(Object attacker)
    {
        if (!Boss.Instance) return false;

        if (Boss.Instance && Boss.Instance.slienceObject == attacker.gameObject)
        { attacking = false; return true; }

        if (attacker.GetComponent<Boss>() && attacker.GetComponent<Boss>().UseSkill())
        {
            StartCoroutine(BossSkill(attacker));
            return true;
        }

        return false;
    }
    private IEnumerator BossSkill(Object attacker)
    {
        attacker.Indicator.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        yield return new WaitForSeconds(attacker.GetComponent<Boss>().skillEffectTime + 0.5f);

        attacker.Indicator.SetActive(false);
        yield return new WaitForSeconds(0.1f);

        attacking = false;
        yield break;
    }

    private float AttackProduction(Transform src, Transform dst, float damage, float weight)
    {
        sequence = DOTween.Sequence();

        if (src.GetComponent<Object>().Type == ObjectType.ENEMY)
        {
            if (Boss.Instance && Boss.Instance.enemyPowerful) damage *= 1.1f;
        }
        else damage *= weight;


        // OnHit()을 기다렸다가 시키면, turn 넘어갈 때, 죽었는지 살았는지 판단할 때, 공격 판단 등 너무 많이 꼬이므로
        // 체력바가 닳고, 데미지 텍스트가 뜨는 건 미루더라도 HP 자체는 먼저 닳아야 함
        switch (src.GetComponent<Object>().AttackType)
        {
            case WeaponType.SWORD:
            case WeaponType.DUAL_SWORD:
                Vector3 knockback = dst.transform.position + (dst.transform.position - src.transform.position) / 6;
                dst.GetComponent<Object>().OnHit(StatusEffect.HP, damage, 0.2f);

                sequence.Append(src.transform.DOMove(dst.transform.position, 0.3f).SetEase(Ease.InExpo))
                        .Append(src.transform.DOLocalMove(Vector3.zero, 0.6f)).SetEase(Ease.Linear)
                        .Join(dst.transform.DOMove(knockback, 0.1f))
                        .Insert(0.4f, dst.transform.DOLocalMove(Vector3.zero, 0.05f)).SetEase(Ease.Linear);

                return ParticleManager.Instance.AttackParticle(src, dst);

            case WeaponType.STAFF:
            case WeaponType.DUAL_STAFF:
                float time = ParticleManager.Instance.AttackParticle(src, dst);
                dst.GetComponent<Object>().OnHit(StatusEffect.HP, damage, time);
                return time;

            case WeaponType.NULL:
                return 0;
            default:
                return 0;
        }
    }
    private void LookAt(GameObject src, GameObject dst)
    {
        Vector3 direc = src.transform.position - dst.transform.position;
        Quaternion sRot = Quaternion.LookRotation(direc);
        Quaternion dRot = Quaternion.LookRotation(-direc);
        src.transform.rotation = sRot;
        dst.transform.rotation = dRot;
    }

    private List<GameObject> AttackableObject(WeaponType weaponType, Colors color, int index, ObjectType objType)
    {
        List<GameObject> attackable = new();

        foreach (int i in AttackableOrder(weaponType, color, index))
        {
            Object obj = StageCube.Instance.touchArray[color.ToInt()][i].Obj;
            if (obj && obj.Type == objType) attackable.Add(obj.gameObject);
        }
        return attackable;
    }

    
    private List<int> AttackableOrder(WeaponType weaponType, Colors color, int index)
    {
        if (color == Colors.GREEN || color == Colors.ORANGE || color == Colors.YELLOW)
        {
            if (index == 0 || index == 3 || index == 6) index += 2;
            else if (index == 2 || index == 5 || index == 8) index -= 2;
        }

        List<int> attackOrder = new();
        string order = "";

        if (weaponType == WeaponType.STAFF)
        {
            switch (index)
            {
                case 0:
                    order = "4";
                    break;
                case 1:
                    order = "35";
                    break;
                case 2:
                    order = "4";
                    break;
                case 3:
                    order = "17";
                    break;
                case 4:
                    order = "6028";
                    break;
                case 5:
                    order = "71";
                    break;
                case 6:
                    order = "4";
                    break;
                case 7:
                    order = "35";
                    break;
                case 8:
                    order = "4";
                    break;
                default:
                    break;
            }
        }
        if (weaponType == WeaponType.SWORD)
        {
            switch (index)
            {
                case 0:
                    order = "13";
                    break;
                case 1:
                    order = "024";
                    break;
                case 2:
                    order = "15";
                    break;
                case 3:
                    order = "046";
                    break;
                case 4:
                    order = "3157";
                    break;
                case 5:
                    order = "428";
                    break;
                case 6:
                    order = "37";
                    break;
                case 7:
                    order = "648";
                    break;
                case 8:
                    order = "75";
                    break;
                default:
                    break;
            }
        }
        if (weaponType == WeaponType.DUAL_SWORD || weaponType == WeaponType.DUAL_STAFF)
        {
            switch (index)
            {
                case 0:
                    order = "143";
                    break;
                case 1:
                    order = "30254";
                    break;
                case 2:
                    order = "415";
                    break;
                case 3:
                    order = "01476";
                    break;
                case 4:
                    order = "63012587";
                    break;
                case 5:
                    order = "74128";
                    break;
                case 6:
                    order = "347";
                    break;
                case 7:
                    order = "63458";
                    break;
                case 8:
                    order = "745";
                    break;
                default:
                    break;
            }
        }

        foreach(char c in order)
        {
            int value = c - '0';
            if (color == Colors.GREEN || color == Colors.ORANGE || color == Colors.YELLOW)
            {
                if (value == 0 || value == 3 || value == 6) value += 2;
                else if (value == 2 || value == 5 || value == 8) value -= 2;
            }
            attackOrder.Add(value);
        }

        return attackOrder;
    }

}