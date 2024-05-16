using UnityEngine;

public class CubePosition : MonoBehaviour
{
    [SerializeField] private Transform child;
    [SerializeField] private Transform newChild;

    public void FindPriorChild()
    {
        child.parent = transform;
    }

    public void FindNewChild()
    {
        if (newChild == null) return;

        child = newChild;
        child.parent = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
            newChild = other.transform;
    }
}
