using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static Constants;

public class Boss : MonoBehaviour
{
    public static Boss Instance { get; private set; }

    [SerializeField] private int[] phasePercents;
    private bool[] useSkill;
    private float minHPpercent;
    private Object thisObject;
    public float skillEffectTime = 0;

    [Header("보스 스킬 효과들")]
    public bool playerCantMove = false;
    public int playerWeaken = 0;
    public GameObject slienceObject = null;
    public bool enemyPowerful = false;
    public Colors shadowColor = Colors.NULL;
    public bool invincibility = false;
    public List<GameObject> invincibilityObjects;
    public bool blindness = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        thisObject = GetComponent<Object>();

        useSkill = new bool[phasePercents.Length];
        for (int i = 0; i < useSkill.Length; i++) useSkill[i] = false;
        minHPpercent = 100;

        StageManager.Instance.isBossStage = true;
    }
    public void FightTurn()
    {
        playerCantMove = false;
        playerWeaken--;
        slienceObject = null;
        enemyPowerful = false;
        shadowColor = Colors.NULL;
        invincibility = false;
        if (blindness)
        {
            blindness = false;
            foreach (GameObject enemeyObject in StageManager.Instance.EnemyList)
                enemeyObject.SetActive(true);
        }
    }
    public bool UseSkill()
    {
        if (minHPpercent > thisObject.HP / thisObject.MaxHp * 100)
            minHPpercent = thisObject.HP / thisObject.MaxHp * 100;

        for (int i = 0; i < useSkill.Length; i++)
        {
            if (minHPpercent <= phasePercents[i] && !useSkill[i])
            {
                useSkill[i] = true;
                skillEffectTime = Skill(i);
                return skillEffectTime != -1;
            }
        }
        return false;
    }
    public float ReceivedDamageChange(Object obj)
    {
        if (obj.Type == ObjectType.PLAYER && playerWeaken > 0) return 1.3f;
        if (obj.Type == ObjectType.ENEMY)
        {
            if (invincibility)
            {
                foreach (GameObject invincibilityObject in invincibilityObjects)
                    if (invincibilityObject == obj.gameObject) return 0;
            }

            if (obj.Color == shadowColor) return 0;
        }
        return 1;
    }
    private float Skill(int phase)
    {
        phase++;

        switch (thisObject.ID)
        {
            case 100010:
                StartCoroutine(Uriel(phase));
                if (phase == 1) return 0.5f;
                if (phase == 2) return 0.5f;
                if (phase == 3) return 0.5f;
                break;
            case 100011:
                StartCoroutine(Raphael(phase));
                if (phase == 1) return 0.5f;
                if (phase == 2) return 0.5f;
                if (phase == 3 && EventManager.Instance.RandomDeadEnemy()) return 0.5f;
                break;
            case 100012:
                StartCoroutine(Gabriel(phase));
                if (phase == 1) return 0.5f;
                if (phase == 2) return 0.5f;
                if (phase == 3) return 0.5f;
                break;
            case 100013:
                StartCoroutine(Michael(phase));
                int randomSkill = Random.Range(0, 3);

                if (randomSkill == 0) { StartCoroutine(Uriel(phase)); return 0.5f; }
                if (randomSkill == 1) { StartCoroutine(Raphael(phase)); return 0.5f; }
                if (randomSkill == 2) { StartCoroutine(Gabriel(phase)); return 0.5f; }

                if (phase == 3) return 0.5f;
                break;
            case 100014:
                StartCoroutine(Lucifer(phase));
                if (phase == 1) return 0.5f;
                if (phase == 2) return 0.5f;
                if (phase == 3) return 0.5f;
                break;
        }
        return -1;
    }

    private IEnumerator Uriel(int phase)
    {
        if (phase == 1)
        {
            Object randomObj = EventManager.Instance.RandomObejectOfPlayerTeam(true);
            ParticleManager.Instance.PlayParticle(randomObj.gameObject, Particle.Attack_Enemy_BySword);
            yield return new WaitForSeconds(0.5f);
            randomObj.OnHit(StatusEffect.HP_PERCENT, 30);
        }
        if (phase == 2)
        {
            ParticleManager.Instance.PlayParticle(thisObject.gameObject, Particle.Heal);
            yield return new WaitForSeconds(0.5f);
            thisObject.OnHit(StatusEffect.HP_PERCENT, -20);
        }
        if (phase == 3)
        {
            Object randomObj = EventManager.Instance.RandomObejectOfPlayerTeam(true);
            ParticleManager.Instance.PlayParticle(randomObj.gameObject, Particle.Attack_Enemy_BySword);
            yield return new WaitForSeconds(0.5f);
            slienceObject = randomObj.gameObject;
        }
    }
    private IEnumerator Raphael(int phase)
    {
        if (phase == 1)
        {
            ParticleManager.Instance.PlayParticle(StageManager.Instance.Player.gameObject, Particle.Attack_Enemy_BySword);
            yield return new WaitForSeconds(0.5f);
            playerWeaken = 3;
        }
        if (phase == 2)
        {
            foreach (GameObject obj in StageManager.Instance.EnemyList)
                ParticleManager.Instance.PlayParticle(obj.gameObject, Particle.Attack_Enemy_BySword);
            yield return new WaitForSeconds(0.5f);

            foreach (GameObject obj in StageManager.Instance.EnemyList)
                obj.GetComponent<Object>().OnHit(StatusEffect.HP_PERCENT, -20);
        }
        if (phase == 3)
        {
            Object obj = EventManager.Instance.RandomDeadEnemy();
            StartCoroutine(EventManager.Instance.Revive(obj, 50));
            yield return new WaitForSeconds(0.5f);

            ParticleManager.Instance.PlayParticle(obj.gameObject, Particle.Attack_Enemy_BySword);
        }
    }
    private IEnumerator Gabriel(int phase)
    {
        if (phase == 1)
        {
            int count = EventManager.Instance.EnemyCount / 2;

            // GameObject 배열을 List로 변환합니다.
            List<GameObject> gameObjectList = StageManager.Instance.EnemyList.ToList();

            // Fisher-Yates 셔플 알고리즘으로 리스트를 랜덤하게 섞습니다.
            for (int i = gameObjectList.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);
                GameObject temp = gameObjectList[i];
                gameObjectList[i] = gameObjectList[randomIndex];
                gameObjectList[randomIndex] = temp;
            }

            // 섞인 리스트에서 앞쪽 절반을 선택합니다.
            invincibilityObjects = gameObjectList.Take(count).ToList();

            foreach (GameObject obj in invincibilityObjects)
            {
                ParticleManager.Instance.PlayParticle(obj.gameObject, Particle.Enemy_INVINCIBILITY);
            }
            yield return new WaitForSeconds(0.5f);
        }
        if (phase == 2)
        {
            ParticleManager.Instance.PlayParticle(thisObject.gameObject, Particle.Heal);
            yield return new WaitForSeconds(0.5f);
            thisObject.OnHit(StatusEffect.HP_PERCENT, -30);
        }
        if (phase == 3)
        {
            ParticleManager.Instance.PlayParticle(StageManager.Instance.Player.gameObject, Particle.Attack_Enemy_BySword);
            yield return new WaitForSeconds(0.5f);
            foreach (GameObject enemeyObject in StageManager.Instance.EnemyList)
                enemeyObject.SetActive(false);
        }
    }
    private IEnumerator Michael(int phase)
    {
        if (phase == 3)
        {
            int[] randomEnemyID = new int[4] { 100003, 100004, 100008, 100009 };
            Touch[] touchCubes = new Touch[3];
            for (int i = 0; i < 3; i++)
            {
                while (true)
                {
                    touchCubes[i] = StageCube.Instance.touchArray[Random.Range(0, 6)][Random.Range(0, 9)];
                    if (touchCubes[i].Obj == null)
                    {
                        ParticleManager.Instance.PlayParticle(touchCubes[i].gameObject, Particle.Enemy_Summon);
                        break;
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 3; i++)
            {
                ObjectManager.Instance.Summons(null, ObjectType.ENEMY, randomEnemyID[Random.Range(0, 4)]);
            }
        }
    }
    private IEnumerator Lucifer(int phase)
    {
        if (phase == 1)
        {
            ParticleManager.Instance.PlayParticle(StageManager.Instance.Player.gameObject, Particle.Enemy_Summon);
            yield return new WaitForSeconds(0.5f);
            playerCantMove = true;
        }
        if (phase == 2)
        {
            ParticleManager.Instance.PlayParticle(StageManager.Instance.Player.gameObject, Particle.Enemy_Summon);
            yield return new WaitForSeconds(0.5f);
            shadowColor = thisObject.Color;
        }
        if (phase == 3)
        {
            foreach (GameObject obj in StageManager.Instance.EnemyList)
                ParticleManager.Instance.PlayParticle(obj.gameObject, Particle.Enemy_INVINCIBILITY);
            yield return new WaitForSeconds(0.5f);
            enemyPowerful = true;
        }
    }
}
