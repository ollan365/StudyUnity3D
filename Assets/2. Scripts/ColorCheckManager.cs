using UnityEngine;
using System.Collections;
using static Constants;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private GameObject[] whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray;
    private GameObject[][] colorCoverArray;

    [SerializeField] private GameObject[] centerCubeArray;

    private GameObject selectedCharacter;
    private bool[] movableCube;
    private void Awake()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
        colorCoverArray = new GameObject[][] { whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray };
        movableCube = new bool[9];
    }

    public void CharacterSelect(GameObject character)
    {
        Debug.Log("character select!");
        selectedCharacter = character;
        MovableCubeSetting(selectedCharacter.GetComponent<Object>().GetPosition().GetPositionIndex());
    }
    public bool CharacterSelectCancel(GameObject character)
    {
        if (selectedCharacter != character)
            return false;

        MovableCubeSetting(-1);
        Debug.Log("character select cancel!");
        return true;
    }
    private void MovableCubeSetting(int index)
    {
        for (int i = 0; i < 9; i++)
            movableCube[i] = false;

        switch (index)
        {
            case 0:
                movableCube[1] = true;
                movableCube[3] = true;
                break;
            case 1:
                movableCube[0] = true;
                movableCube[2] = true;
                movableCube[4] = true;
                break;
            case 2:
                movableCube[1] = true;
                movableCube[5] = true;
                break;
            case 3:
                movableCube[0] = true;
                movableCube[4] = true;
                movableCube[6] = true;
                break;
            case 4:
                movableCube[1] = true;
                movableCube[3] = true;
                movableCube[5] = true;
                movableCube[7] = true;
                break;
            case 5:
                movableCube[2] = true;
                movableCube[4] = true;
                movableCube[8] = true;
                break;
            case 6:
                movableCube[3] = true;
                movableCube[7] = true;
                break;
            case 7:
                movableCube[4] = true;
                movableCube[6] = true;
                movableCube[8] = true;
                break;
            case 8:
                movableCube[5] = true;
                movableCube[7] = true;
                break;

            default:
                break;
        }

        int selectedCharacterColor = selectedCharacter.GetComponent<Object>().GetPosition().GetPositionColor().ToInt();
        for (int i = 0; i < 9; i++) // 이동 가능한 곳이면 cover
        {
            colorCoverArray[selectedCharacterColor][i].SetActive(false);

            if (colorCheckCubeArray[selectedCharacterColor][i].GetComponent<ColorCheckCube>().GetObjectType() == ObjectType.NULL)
                colorCoverArray[selectedCharacterColor][i].SetActive(movableCube[i]);
        }
    }
    public void Move(Colors color, int index)
    {
        if (color != selectedCharacter.GetComponent<Object>().GetPosition().GetPositionColor()) // 다른 면이면 이동 못함
            return;
        if (!movableCube[index]) // 같은 면의 이동 불가능한 곳이면 이동 안 함
            return;
        StartCoroutine(MoveCoroutine(color, index));
    }
    private IEnumerator MoveCoroutine(Colors color, int index)
    {
        Transform parent = colorCheckCubeArray[color.ToInt()][index].GetComponent<ColorCheckCube>().colorPointCube.transform.GetChild(0).transform;
        
        selectedCharacter.transform.position = parent.position;
        selectedCharacter.transform.parent = parent;

        yield return new WaitForFixedUpdate();

        MovableCubeSetting(index);
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
