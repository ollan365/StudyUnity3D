using UnityEngine;
using static Constants;

public class Touch : MonoBehaviour
{
    [SerializeField] private CubeManager CM;

    [SerializeField] private Colors[] colors;
    public Colors[] TouchColors { get => colors; }
    [SerializeField] private int[] ints;
    public int[] TouchInts{ get => ints; }
    [SerializeField] private Colors AbsoluteColor;
    public Colors Color { get => AbsoluteColor; }
    [SerializeField] private int AbsoluteIndex;
    public int Index { get => AbsoluteIndex; }
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
    public Colors RelativeColor
    {
        get
        {
            switch(colorPointCube.tag)
            {
                case "White": return Colors.WHITE;
                case "Red": return Colors.RED;
                case "Blue": return Colors.BLUE;
                case "Green": return Colors.GREEN;
                case "Orange": return Colors.ORANGE;
                case "Yellow": return Colors.YELLOW;
                default: return Colors.NULL;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
            colorPointCube = other.gameObject;
    }

}
