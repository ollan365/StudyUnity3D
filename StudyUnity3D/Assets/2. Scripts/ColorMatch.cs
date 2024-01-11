using UnityEngine;

public class ColorMatch : MonoBehaviour
{
    // private readonly int WHITE = 0, RED = 1, BLUE = 2, GREEN = 3, ORANGE = 4, YELLOW = 5;
    
    private int[][] colorArray;

    private void Start()
    {
        colorArray = new int[6][];
        for (int i = 0; i < colorArray.Length; i++)
            colorArray[i] = new int[9];
    }
}
