using UnityEngine;
using System.Collections;

public class ObjectEffect : MonoBehaviour
{
    public static ObjectEffect Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void MakeSmall(GameObject obj)
    {
        StartCoroutine(Smalller(obj));
    }
    private IEnumerator Smalller(GameObject obj)
    {
        Vector3 originScale = obj.transform.localScale;
        Vector3 zeroScale = Vector3.zero;

        float elapsedTime = 0f;
        float shrinkDuration = 1f;

        while (elapsedTime < shrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / shrinkDuration;
            obj.transform.localScale = Vector3.Lerp(originScale, zeroScale, t);
            yield return null;
        }
        Destroy(obj);
    }
}
