using UnityEngine;

public class CubePosition : MonoBehaviour
{
    Transform child;

    public void FindChild()
    {
        child.parent = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        child = other.transform;
    }
}
