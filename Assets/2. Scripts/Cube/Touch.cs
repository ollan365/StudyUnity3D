using UnityEngine;
using static Constants;

public class Touch : MonoBehaviour
{
    [SerializeField] private CubeManager CM;

    [SerializeField] private Colors[] colors;
    public Colors[] TouchColors { get => colors; }
    [SerializeField] private int[] ints;
    public int[] TouchInts{ get => ints; }
    [SerializeField] private Colors positionColor;
    public Colors Color { get => positionColor; }
    [SerializeField] private int positionIndex;
    public int Index { get => positionIndex; }
    [SerializeField] private Vector3 direction;
    public Transform ObjectPostion { get => colorPointCube.transform.GetChild(0).transform; }
    public Object Obj
    {
        get
        {
            if (ObjType == ObjectType.NULL) return null;
            else return ObjectPostion.GetChild(0).GetComponent<Object>();
        }
    }
    public ObjectType ObjType
    {
        get
        {
            if (ObjectPostion.childCount == 0) return ObjectType.NULL;
            else return ObjectPostion.GetChild(0).GetComponent<Object>().Type;
        }
    }
    [SerializeField] private GameObject colorPointCube;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
            colorPointCube = other.gameObject;
    }

}
