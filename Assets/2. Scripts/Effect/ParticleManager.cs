using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance { get; private set; }
    [SerializeField] private GameObject[] particlePrefabs;
    [SerializeField] private GameObject testObjectForParticle;
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
    public void TestParticle(int particleIndex)
    {
        PlayParticle(testObjectForParticle, Particle.Enemy_Summon, particleIndex);
    }
    public void PlayParticle(GameObject obj, Particle particle, int testIndex = -1)
    {
        int index = testIndex == -1 ? Int(particle) : testIndex;
        GameObject particleObj = Instantiate(particlePrefabs[index], obj.transform.position, obj.transform.rotation);

        switch (particle)
        {
            case Particle.Enemy_Summon:
                particleObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case Particle.Friend_Summon:
                particleObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
            default: return -1;
        }
    }
}
