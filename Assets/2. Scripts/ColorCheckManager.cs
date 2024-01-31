using UnityEngine;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private GameObject[] centerCubeArray;

    private void Awake()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
    }

    public void BingoCheck()
    {
        for (int i = 0; i < 6; i++)
        {
            int count = 0;
            bool[] isColorMatch = new bool[9];

            for (int j = 0; j < 9; j++)
                isColorMatch[j] = centerCubeArray[i].layer == colorCheckCubeArray[i][j].layer;

            if (isColorMatch[0] && isColorMatch[1] && isColorMatch[2])
                count++;
            if (isColorMatch[3] && isColorMatch[4] && isColorMatch[5])
                count++;
            if (isColorMatch[6] && isColorMatch[7] && isColorMatch[8])
                count++;
            if (isColorMatch[0] && isColorMatch[3] && isColorMatch[6])
                count++;
            if (isColorMatch[1] && isColorMatch[4] && isColorMatch[7])
                count++;
            if (isColorMatch[2] && isColorMatch[5] && isColorMatch[8])
                count++;

            if(count == 0)
                Debug.Log($"Color: {centerCubeArray[i].layer - 8}  No Bingo...");
            else if (count == 6)
                Debug.Log($"Color: {centerCubeArray[i].layer - 8}  All Bingo!!!");
            else
                Debug.Log($"Color: {centerCubeArray[i].layer - 8}  Bingo: {count}");
        }
    }
}
