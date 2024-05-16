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
    public int Direction(Vector3 startPosition)
    {
        // ���콺 �̵� ����
        Vector3 mouseDirection = Input.mousePosition - startPosition;

        // �� ���� ����
        Vector3[] directions = { transform.forward, -transform.forward, -transform.right, transform.right };

        // �� ���� ���Ϳ��� ���� ���� ���
        float[] dot = new float[4];
        for (int i = 0; i < 4; i++) dot[i] = Vector3.Dot(mouseDirection.normalized, directions[i]);
        
        // ���� ���� ���� ū ���� ���ϱ�
        float maxDot = Mathf.Max(dot[0], dot[1], dot[2], dot[3]);

        Debug.Log($"{dot[0]} / {dot[1]} / {dot[2]} / {dot[3]}");

        // ���콺�� �̵� ����� ���� ������ ������ �ε��� ��ȯ
        for (int i = 0; i < 4; i++) if (maxDot == dot[i]) return i;

        return -1;
    }
    public void DrawRay() // ���� �ص� ��
    {
        Vector3[] direction = new Vector3[4] { transform.forward, -transform.forward, transform.right, -transform.right };
        Color[] color = new Color[4] { UnityEngine.Color.red, UnityEngine.Color.yellow, UnityEngine.Color.green, UnityEngine.Color.blue };
        for (int i = 0; i < 4; i++)
        {
            Ray ray = new Ray(transform.position, direction[i]);
            Debug.DrawRay(ray.origin, ray.direction, color[i], 5f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
            colorPointCube = other.gameObject;
    }

}
