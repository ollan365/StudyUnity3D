using UnityEngine;
using static Constants;

public class Touch : MonoBehaviour
{
    [SerializeField] private CubeManager CM;

    [SerializeField] private Colors[] colors;
    [SerializeField] private int[] ints;
    public Colors[] GetTouchColors()
    {
        return colors;
    }
    public int[] GetTouchInts()
    {
        return ints;
    }
}
