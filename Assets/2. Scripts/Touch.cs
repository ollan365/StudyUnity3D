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
        Debug.Log($"{positionColor}의 {positionIndex}번째 큐브");
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
