using UnityEngine;

public class ColorCheckCube : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        gameObject.layer = other.gameObject.layer;
    }
}
