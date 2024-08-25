using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    [SerializeField] private GameObject[] particlePrefabs;
    [SerializeField] private GameObject[] testObjectForParticle;
    [SerializeField] private Particle testParticle;
    [SerializeField] private float projectileSpeed;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TestParticle()
    {
        foreach (GameObject obj in testObjectForParticle)
            PlayParticle(obj, testParticle);
    }
    public GameObject PlayParticle(GameObject obj, Particle particle)
    {
        GameObject particleObj = null;

        if (obj.GetComponent<Touch>()) particleObj = Instantiate(particlePrefabs[Int(particle)], obj.transform.position, obj.GetComponent<Touch>().ObjectPostion.transform.rotation);
        else particleObj = Instantiate(particlePrefabs[Int(particle)], obj.transform.position, obj.transform.rotation);
        
        switch (particle)
        {
            case Particle.Enemy_Summon:
            case Particle.Friend_Summon:
            case Particle.Attack_PlayerTeam_BySword:
            case Particle.PlayerTeam_Sttaff_Charging:
            case Particle.Attack_Enemy_BySword:
            case Particle.Enemy_Sttaff_Charging:
                particleObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case Particle.Heal:
                particleObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                break;
            case Particle.Enemy_INVINCIBILITY:
            case Particle.PlayerTeam_INVINCIBILITY:
                particleObj.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);
                break;

            case Particle.PlayerTeam_Sttaff_Projectile:
            case Particle.Enemy_Sttaff_Projectile:
                particleObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                break;
        }

        particleObj.GetComponent<ParticleSystem>().Play();
        return particleObj;
    }
    public float AttackParticle(Transform src, Transform dst)
    {
        bool isEnemyAttack = src.GetComponent<Object>().Type == ObjectType.ENEMY;
        if (src.GetComponent<Object>().AttackType == WeaponType.SWORD)
        {
            StartCoroutine(AttackSwordCoroutine(dst, isEnemyAttack));
        }
        else if (src.GetComponent<Object>().AttackType == WeaponType.STAFF)
        {
            StartCoroutine(AttackStaffCoroutine(src, dst, isEnemyAttack));
            return 2;
        }
        return Time.deltaTime;
    }
    private IEnumerator AttackSwordCoroutine(Transform dst, bool isEnemy)
    {
        yield return new WaitForSeconds(0.2f);

        if (!isEnemy) PlayParticle(dst.gameObject, Particle.Attack_PlayerTeam_BySword);
        else PlayParticle(dst.gameObject, Particle.Attack_Enemy_BySword);
    }
    private IEnumerator AttackStaffCoroutine(Transform src, Transform dst, bool isEnemy)
    {
        GameObject chargingObject = null, projectile = null;

        // 차징 시작
        if (isEnemy) chargingObject = PlayParticle(src.gameObject, Particle.Enemy_Sttaff_Charging);
        else chargingObject = PlayParticle(src.gameObject, Particle.PlayerTeam_Sttaff_Charging);
        yield return new WaitForSeconds(0.5f);

        // 투사체 생성
        if (isEnemy) projectile = PlayParticle(src.gameObject, Particle.Enemy_Sttaff_Projectile);
        else projectile = PlayParticle(src.gameObject, Particle.PlayerTeam_Sttaff_Projectile);
        yield return new WaitForSeconds(0.5f);

        // 차징 끝 -> 투사체 날림
        Destroy(chargingObject);

        Vector3 direction = (dst.transform.position - src.transform.position).normalized;
        projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
        yield return new WaitForSeconds(0.5f);

        Destroy(projectile);
    }
    private int Int(Particle text)
    {
        switch (text)
        {
            case Particle.Enemy_Summon: return 0;
            case Particle.Friend_Summon: return 1;
            case Particle.Heal: return 2;
            case Particle.Enemy_INVINCIBILITY: return 3;
            case Particle.PlayerTeam_INVINCIBILITY: return 4;
            case Particle.Attack_PlayerTeam_BySword: return 5;
            case Particle.PlayerTeam_Sttaff_Projectile: return 6;
            case Particle.PlayerTeam_Sttaff_Charging: return 7;
            case Particle.Attack_Enemy_BySword: return 8;
            case Particle.Enemy_Sttaff_Projectile: return 9;
            case Particle.Enemy_Sttaff_Charging: return 10;
            default: return -1;
        }
    }
}
