using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static Constants;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private Text[] bingoTexts;
    private BingoStatus[] bingoStatus;

    private GameObject selectedCharacter;
    private bool[] movableCube;
    private void Awake()
    {
        bingoStatus = new BingoStatus[9];
        for (int i = 0; i < 9; i++)
            bingoStatus[i] = BingoStatus.DEFAULT;
        movableCube = new bool[9];
    }
    public void CharacterSelect(GameObject character)
    {
        Debug.Log("character select!");
        selectedCharacter = character;
        MovableCubeSetting(selectedCharacter.GetComponent<Object>().Index);
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

        int selectedCharacterColor = selectedCharacter.GetComponent<Object>().Color.ToInt();
        for (int i = 0; i < 9; i++) // �̵� ������ ���̸� cover
        {
            StageCube.Instance.coverArray[selectedCharacterColor][i].SetActive(false);

            ObjectType obj = StageCube.Instance.touchArray[selectedCharacterColor][i].Obj.Type;
            if (obj == ObjectType.NULL || obj == ObjectType.PORTAL || obj == ObjectType.TREASURE)
                StageCube.Instance.colorArray[selectedCharacterColor][i].SetActive(movableCube[i]);
        }
    }
    public bool Move(Colors color, int index, bool wantMove)
    {
        if (color != selectedCharacter.GetComponent<Object>().Color) // �ٸ� ���̸� �̵� ����
            return false;
        if (!movableCube[index]) // ���� ���� �̵� �Ұ����� ���̸� �̵� �� ��
            return false;
        if (wantMove)
            StartCoroutine(MoveCoroutine(color, index));
        return true;
    }
    private IEnumerator MoveCoroutine(Colors color, int index)
    {
        MovableCubeSetting(-1);

        Transform parent = StageCube.Instance.touchArray[color.ToInt()][index].ObjectPostion;

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
    public int BingoCheck(int color, bool turnChange)
    {
        return 1;
    }
    //{
    //    int count = 0;
    //    Colors[] colorOfSide = new Colors[9];
    //    int[] colorBingo = new int[9];

    //    for (int i = 0; i < 9; i++)
    //    {
    //        colorOfSide[i] = StageCube.Instance.touchArray[color][i].Color;
    //        colorBingo[i] = 0;
    //    }

    //    if (colorOfSide[0] == colorOfSide[1] && colorOfSide[1] == colorOfSide[2])
    //        colorBingo[colorOfSide[0].ToInt()]++;
    //    if (colorOfSide[3] == colorOfSide[4] && colorOfSide[4] == colorOfSide[5])
    //        colorBingo[colorOfSide[3].ToInt()]++;
    //    if (colorOfSide[6] == colorOfSide[7] && colorOfSide[7] == colorOfSide[8])
    //        colorBingo[colorOfSide[6].ToInt()]++;
    //    if (colorOfSide[0] == colorOfSide[3] && colorOfSide[3] == colorOfSide[6])
    //        colorBingo[colorOfSide[0].ToInt()]++;
    //    if (colorOfSide[1] == colorOfSide[4] && colorOfSide[4] == colorOfSide[7])
    //        colorBingo[colorOfSide[1].ToInt()]++;
    //    if (colorOfSide[2] == colorOfSide[5] && colorOfSide[5] == colorOfSide[8])
    //        colorBingo[colorOfSide[2].ToInt()]++;

    //    //if (!IsAllCoolTime(bingoStatus[color]) && count == 6)
    //    //    bingoTexts[centerCubeArray[color].layer - 8].text = "ALL";
    //    //else if (bingoStatus[color] != BingoStatus.DEFAULT)
    //    //    bingoTexts[centerCubeArray[color].layer - 8].text = "COOL TIME";
    //    //else if (count > 0)
    //    //    bingoTexts[centerCubeArray[color].layer - 8].text = "ONE";
    //    //else
    //    //    bingoTexts[centerCubeArray[color].layer - 8].text = "NO";

    //    //if (turnChange)
    //    //{
    //    //    if (!IsAllCoolTime(bingoStatus[color]) && count == 6)
    //    //    {
    //    //        bingoStatus[color] = BingoStatus.ALL_1;
    //    //        return BINGO_ALL;
    //    //    }
    //    //    else if(bingoStatus[color] == BingoStatus.DEFAULT && count > 0)
    //    //    {
    //    //        bingoStatus[color] = BingoStatus.ONE_1;
    //    //        return BINGO_ONE;
    //    //    }
    //    //    ToNext(bingoStatus[color]);
    //    //}
    //    //return BINGO_DEFAULT;
    //}
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


    //private IEnumerator MouseCheck()
    //{
    //    Colors selectedColor = selectedCharacter.GetComponent<Object>().GetPosition().Color;

    //    while (isCover)
    //    {
    //        // ���콺�� ��ũ�� ��ǥ�� ���� ��ǥ�� ��ȯ
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit[] hits = Physics.RaycastAll(ray);

    //        // ��� ���� �浹���� ���� �ݺ�

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
}