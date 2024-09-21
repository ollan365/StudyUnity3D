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

        if (obj.GetComponent<Touch>())
        {
            particleObj = Instantiate(particlePrefabs[Int(particle)], obj.transform);
            particleObj.transform.rotation = obj.GetComponent<Touch>().ObjectPostion.rotation;
        }
        else
        {
            particleObj = Instantiate(particlePrefabs[Int(particle)], obj.transform);
            particleObj.transform.rotation = obj.transform.rotation;
        }

        Vector3 parentScale = obj.transform.localScale;
        float particle_X = 1 / parentScale.x, particle_Y = 1 / parentScale.y, particle_Z = 1 / parentScale.z;

        switch (particle)
        {
            case Particle.Enemy_Summon:
            case Particle.Friend_Summon:
            case Particle.Attack_PlayerTeam_BySword:
            case Particle.PlayerTeam_Sttaff_Charging:
            case Particle.Attack_Enemy_BySword:
            case Particle.Enemy_Sttaff_Charging:
                particle_X *= 0.5f;
                particle_Y *= 0.5f;
                particle_Z *= 0.5f;
                break;
            case Particle.Heal:
                particle_X *= 0.4f;
                particle_Y *= 0.4f;
                particle_Z *= 0.4f;
                break;
            case Particle.Enemy_INVINCIBILITY:
            case Particle.PlayerTeam_INVINCIBILITY:
                particle_X *= 0.2f;
                particle_Y *= 0.3f;
                particle_Z *= 0.2f;
                break;

            case Particle.PlayerTeam_Sttaff_Projectile:
            case Particle.Enemy_Sttaff_Projectile:
                particle_X *= 0.01f;
                particle_Y *= 0.01f;
                particle_Z *= 0.01f;
                break;
        }
        particleObj.transform.localScale = new Vector3(particle_X, particle_Y, particle_Z);

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
            return 0.5f;
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
        GameObject projectile = null;

        // 투사체 생성
        if (isEnemy) projectile = PlayParticle(src.GetComponent<Object>().touchCube.gameObject, Particle.Enemy_Sttaff_Projectile);
        else projectile = PlayParticle(src.GetComponent<Object>().touchCube.gameObject, Particle.PlayerTeam_Sttaff_Projectile);
        yield return new WaitForSeconds(0.5f);

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
