using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static Constants;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private GameObject[] whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray;
    private GameObject[][] colorCheckCubeArray;

    [SerializeField] private GameObject[] whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray;
    private GameObject[][] colorCoverArray;

    [SerializeField] private Text[] bingoTexts;
    private BingoStatus[] bingoStatus;

    [SerializeField] private GameObject[] centerCubeArray;

    private GameObject selectedCharacter;
    private bool[] movableCube;
    private void Awake()
    {
        colorCheckCubeArray = new GameObject[][] { whiteCheckCubeArray, redCheckCubeArray, blueCheckCubeArray, greenCheckCubeArray, orangeCheckCubeArray, yellowCheckCubeArray };
        colorCoverArray = new GameObject[][] { whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray };
        bingoStatus = new BingoStatus[9];
        for (int i = 0; i < 9; i++)
            bingoStatus[i] = BingoStatus.DEFAULT;
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
    //private IEnumerator MouseCheck()
    //{
    //    Colors selectedColor = selectedCharacter.GetComponent<Object>().GetPosition().Color;

    //    while (isCover)
    //    {
    //        // 마우스의 스크린 좌표를 월드 좌표로 변환
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit[] hits = Physics.RaycastAll(ray);

    //        // 모든 레이 충돌점에 대해 반복
            
    //        foreach (GameObject cube in nowCover)
    //        {
    //            bool isMouse = false;
    //            foreach(RaycastHit hit in hits)
    //            {
    //                if(cube == hit.collider.gameObject)
    //                {
    //                    isMouse = true; break;
    //                }
    //            }
    //            if (!isMouse)
    //            {
    //                switch (selectedColor)
    //                {
    //                    case Colors.WHITE:
    //                    case Colors.YELLOW:
    //                        cube.transform.localScale = new(1f, 0.01f, 1f);
    //                        break;
    //                    case Colors.RED:
    //                    case Colors.ORANGE:
    //                        cube.transform.localScale = new(0.01f, 1f, 1f);
    //                        break;
    //                    case Colors.BLUE:
    //                    case Colors.GREEN:
    //                        cube.transform.localScale = new(1f, 1f, 0.01f);
    //                        break;
    //                }
    //                continue;
    //            }

    //            switch (selectedColor)
    //            {
    //                case Colors.WHITE:
    //                case Colors.YELLOW:
    //                    cube.transform.localScale = new(1.1f, 0.03f, 1.1f);
    //                    break;
    //                case Colors.RED:
    //                case Colors.ORANGE:
    //                    cube.transform.localScale = new(0.03f, 1.1f, 1.1f);
    //                    break;
    //                case Colors.BLUE:
    //                case Colors.GREEN:
    //                    cube.transform.localScale = new(1.1f, 1.1f, 0.03f);
    //                    break;
    //            }
    //            break;
    //        }
            
    //        yield return new WaitForFixedUpdate();
    //    }
    //    foreach(GameObject cube in nowCover)
    //        switch (selectedColor)
    //        {
    //            case Colors.WHITE:
    //            case Colors.YELLOW:
    //                cube.transform.localScale = new(1f, 0.01f, 1f);
    //                break;
    //            case Colors.RED:
    //            case Colors.ORANGE:
    //                cube.transform.localScale = new(0.01f, 1f, 1f);
    //                break;
    //            case Colors.BLUE:
    //            case Colors.GREEN:
    //                cube.transform.localScale = new(1f, 1f, 0.01f);
    //                break;
    //        }
    //    yield return new WaitForFixedUpdate();
    //}
    public int BingoCheck(int color, bool turnChange)
    {
        int count = 0;
        bool[] isColorMatch = new bool[9];

        for (int j = 0; j < 9; j++)
            isColorMatch[j] = centerCubeArray[color].layer == colorCheckCubeArray[color][j].layer;

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

        if (!IsAllCoolTime(bingoStatus[color]) && count == 6)
            bingoTexts[centerCubeArray[color].layer - 8].text = "ALL";
        else if (bingoStatus[color] != BingoStatus.DEFAULT)
            bingoTexts[centerCubeArray[color].layer - 8].text = "COOL TIME";
        else if (count > 0)
            bingoTexts[centerCubeArray[color].layer - 8].text = "ONE";
        else
            bingoTexts[centerCubeArray[color].layer - 8].text = "NO";

        if (turnChange)
        {
            if (!IsAllCoolTime(bingoStatus[color]) && count == 6)
            {
                bingoStatus[color] = BingoStatus.ALL_1;
                return BINGO_ALL;
            }
            else if(bingoStatus[color] == BingoStatus.DEFAULT && count > 0)
            {
                bingoStatus[color] = BingoStatus.ONE_1;
                return BINGO_ONE;
            }
            ToNext(bingoStatus[color]);
        }
        return BINGO_DEFAULT;
    }
    private BingoStatus ToNext(BingoStatus bingoStatus)
    {
        switch (bingoStatus)
        {
            case BingoStatus.ONE_1: return BingoStatus.ONE_2;
            case BingoStatus.ONE_2: return BingoStatus.ONE_2;
            case BingoStatus.ONE_3: return BingoStatus.DEFAULT;

            case BingoStatus.ALL_1: return BingoStatus.ALL_2;
            case BingoStatus.ALL_2: return BingoStatus.ALL_3;
            case BingoStatus.ALL_3: return BingoStatus.DEFAULT;

            default: return BingoStatus.DEFAULT;
        }
    }
    private bool IsAllCoolTime(BingoStatus bingoStatus)
    {
        switch (bingoStatus)
        {
            case BingoStatus.ALL_1:
            case BingoStatus.ALL_2:
            case BingoStatus.ALL_3: return true;

            default: return false;
        }
    }
}