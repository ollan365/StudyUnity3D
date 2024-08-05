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
    public void PlayParticle(GameObject obj, Particle particle)
    {
        GameObject particleObj = null;

        if (obj.GetComponent<Touch>()) particleObj = Instantiate(particlePrefabs[Int(particle)], obj.transform.position, obj.GetComponent<Touch>().ObjectPostion.transform.rotation);
        else particleObj = Instantiate(particlePrefabs[Int(particle)], obj.transform.position, obj.transform.rotation);
        
        switch (particle)
        {
            case Particle.Enemy_Summon:
            case Particle.Friend_Summon:
                particleObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case Particle.Heal:
                particleObj.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                break;
        }

        particleObj.GetComponent<ParticleSystem>().Play();
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
