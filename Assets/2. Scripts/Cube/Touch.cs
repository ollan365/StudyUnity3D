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
    public Transform ObjectPostion
    {
        get
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, direction);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 8)
                    return hit.collider.gameObject.transform.GetChild(0).transform;
            }
            return null;
        }
    }
    public Object Obj
    {
        get
        {
            if (ObjectPostion.childCount == 0) return null;
            else return ObjectPostion.GetChild(0).GetComponent<Object>();
        }
    }
}
