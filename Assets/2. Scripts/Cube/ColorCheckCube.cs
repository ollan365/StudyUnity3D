using UnityEngine;
using static Constants;

public class ColorCheckCube : MonoBehaviour
{
    public GameObject colorPointCube;
    [SerializeField] private Colors color;
    public Colors Color { get => color; }
    [SerializeField] private int index;
    public int Index { get => index; }
    public ObjectType GetObjectType()
    {
        Transform objectPosition = colorPointCube.transform.GetChild(0);

        if (objectPosition.childCount == 0) return ObjectType.NULL;
        return objectPosition.GetChild(0).GetComponent<Object>().Type;
    }
    public GameObject GetObject()
    {
        Transform objectPosition = colorPointCube.transform.GetChild(0);

        if (objectPosition.childCount == 0) return null;
        return objectPosition.GetChild(0).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        colorPointCube = other.gameObject;
        gameObject.layer = colorPointCube.layer;
    }
}
