using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static Constants;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private GameObject[] whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray;
    private GameObject[][] colorCoverArray;

    [SerializeField] private Text[] bingoTexts;

    [SerializeField] private GameObject[] centerCubeArray;

    private GameObject selectedCharacter;
    private bool[] movableCube;
    private void Awake()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
        colorCoverArray = new GameObject[][] { whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray };
        movableCube = new bool[9];
    }

    public ObjectType CheckCubeObject(Colors color, int index)
    {
        return colorCheckCubeArray[color.ToInt()][index].GetComponent<ColorCheckCube>().GetObjectType();
    }
    public GameObject GetCubeObject(Colors color, int index)
    {
        return colorCheckCubeArray[color.ToInt()][index].GetComponent<ColorCheckCube>().GetObject();
    }
    public void CharacterSelect(GameObject character)
    {
        Debug.Log("character select!");
        selectedCharacter = character;
        MovableCubeSetting(selectedCharacter.GetComponent<Object>().GetPosition().Index);
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

        int selectedCharacterColor = selectedCharacter.GetComponent<Object>().GetPosition().Color.ToInt();
        for (int i = 0; i < 9; i++) // 이동 가능한 곳이면 cover
        {
            colorCoverArray[selectedCharacterColor][i].SetActive(false);

            if (colorCheckCubeArray[selectedCharacterColor][i].GetComponent<ColorCheckCube>().GetObjectType() == ObjectType.NULL)
                colorCoverArray[selectedCharacterColor][i].SetActive(movableCube[i]);
        }
    }
    public bool Move(Colors color, int index, bool wantMove)
    {
        if (color != selectedCharacter.GetComponent<Object>().GetPosition().Color) // 다른 면이면 이동 못함
            return false;
        if (!movableCube[index]) // 같은 면의 이동 불가능한 곳이면 이동 안 함
            return false;
        if (wantMove)
            StartCoroutine(MoveCoroutine(color, index));
        return true;
    }
    private IEnumerator MoveCoroutine(Colors color, int index)
    {
        Transform parent = colorCheckCubeArray[color.ToInt()][index].GetComponent<ColorCheckCube>().colorPointCube.transform.GetChild(0).transform;


        //selectedCharacter.transform.position = parent.position;

        //2024-03-06 Player Move Animation
        selectedCharacter.transform.parent = parent;

        Vector3 originPos = selectedCharacter.transform.localPosition;
        Vector3 middlePos = Vector3.Lerp(originPos, Vector3.zero, 0.5f);

        float travelTIme = 0f;
        

        while (travelTIme < 0.15f)
        {
            selectedCharacter.transform.localPosition = Vector3.Lerp(originPos, middlePos, travelTIme / 0.15f);
            travelTIme += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        while (travelTIme < 0.5f)
        {
            selectedCharacter.transform.localPosition = Vector3.Lerp(middlePos, Vector3.zero, travelTIme / 0.5f);
            travelTIme += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }


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

            if (count == 0)
                bingoTexts[centerCubeArray[i].layer - 8].text = "NO";
            else if (count == 6)
                bingoTexts[centerCubeArray[i].layer - 8].text = "ALL";
            else
                bingoTexts[centerCubeArray[i].layer - 8].text = "ONE";
        }
    }
}
