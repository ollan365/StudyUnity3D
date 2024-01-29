using UnityEngine;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private ColorCheckCube[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private ColorCheckCube[][] colorCheckCubeArray;
    private bool[][] colorMatchCheckArray;

    private void Awake()
    {
        colorCheckCubeArray = new ColorCheckCube[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };

        colorMatchCheckArray = new bool[6][];
        for(int i = 0; i< colorMatchCheckArray.Length; i++)
        {
            colorMatchCheckArray[i] = new bool[9];
            for (int j = 0; j < colorMatchCheckArray[i].Length; j++)
                colorMatchCheckArray[i][j] = false;
        }
    }

    public void BingoCheck()
    {
        for (int i = 0; i < colorMatchCheckArray.Length; i++)
            for (int j = 0; j < colorMatchCheckArray[i].Length; j++)
                colorMatchCheckArray[i][j] = colorCheckCubeArray[i][j].IsColorMatch;

        for (int i = 0; i < colorMatchCheckArray.Length; i++)
        {
            int count = 0;

            for(int j = 0; j < colorMatchCheckArray[i].Length; j++)
            {
                if (!colorMatchCheckArray[i][j]) break;
                if(j == colorMatchCheckArray[i].Length - 1)
                {
                    Debug.Log($"Color: {i}  Bingo: All!");
                    return;
                }
            }

            if (colorMatchCheckArray[i][0] && colorMatchCheckArray[i][1] && colorMatchCheckArray[i][2])
                count++;
            if (colorMatchCheckArray[i][3] && colorMatchCheckArray[i][4] && colorMatchCheckArray[i][5])
                count++;
            if (colorMatchCheckArray[i][6] && colorMatchCheckArray[i][7] && colorMatchCheckArray[i][8])
                count++;
            if (colorMatchCheckArray[i][0] && colorMatchCheckArray[i][3] && colorMatchCheckArray[i][6])
                count++;
            if (colorMatchCheckArray[i][1] && colorMatchCheckArray[i][4] && colorMatchCheckArray[i][7])
                count++;
            if (colorMatchCheckArray[i][2] && colorMatchCheckArray[i][5] && colorMatchCheckArray[i][8])
                count++;

            if (count > 0)
                Debug.Log($"Color: {i}  Bingo: {count}");
        }
    }
}
