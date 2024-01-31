using UnityEngine;
using static Constants;

public class Touch : MonoBehaviour
{
    [SerializeField] private CubeManager CM;

    [SerializeField] private Colors[] colors;
    [SerializeField] private int[] ints;

    [SerializeField] private Colors positionColor;
    [SerializeField] private int positionIndex;
    public void PrintInfo()
    {
        Debug.Log($"{positionColor}�� {positionIndex}��° ť��");
    }
    public Colors[] GetTouchColors()
    {
        return colors;
    }
    public int[] GetTouchInts()
    {
        return ints;
    }
}
