using UnityEngine;
using static Constants;

public class ColorCheckCube : MonoBehaviour
{
    public GameObject colorPointCube;

    public ObjectType GetObjectType()
    {
        Transform objectPosition = colorPointCube.transform.GetChild(0);

        if (objectPosition.childCount == 0) return ObjectType.Null;
        return objectPosition.GetChild(0).GetComponent<Object>().Type;
    }
    private void OnTriggerEnter(Collider other)
    {
        colorPointCube = other.gameObject;
        gameObject.layer = colorPointCube.layer;
    }
}
