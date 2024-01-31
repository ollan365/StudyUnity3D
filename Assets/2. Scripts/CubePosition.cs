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
        if (other.gameObject.layer == 3)
            child = other.transform;
    }
}
