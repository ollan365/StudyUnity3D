using UnityEngine;

public class ColorCheckCube : MonoBehaviour
{
    public GameObject colorPointCube;
    private void OnTriggerEnter(Collider other)
    {
        colorPointCube = other.gameObject;
        gameObject.layer = colorPointCube.layer;
    }
}
