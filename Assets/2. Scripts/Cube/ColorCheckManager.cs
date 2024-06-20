using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static Constants;

public class ColorCheckManager : MonoBehaviour
{
    [SerializeField] private Text[] bingoTexts;
    private BingoStatus[] bingoStatus;

    private GameObject selectedCharacter;
    public GameObject SelectedCharacter { get => selectedCharacter; }
    private bool[] movableCube;

    private void Awake()
    {
        bingoStatus = new BingoStatus[6];
        for (int i = 0; i < 6; i++) bingoStatus[i] = BingoStatus.NONE;
        movableCube = new bool[9];
    }
    public void CharacterSelect(GameObject character)
    {
        selectedCharacter = character;
        MovableCubeSetting(selectedCharacter.GetComponent<Object>().Index);
    }
    public bool CharacterSelectCancel(GameObject character, bool mustChange)
    {
        if (!mustChange && selectedCharacter != character)
            return false;

        MovableCubeSetting(-1);
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
                for (int i = 0; i < 6; i++)
                    for (int j = 0; j < 9; j++)
                        StageCube.Instance.coverArray[i][j].SetActive(false);
                return;
        }

        int selectedCharacterColor = selectedCharacter.GetComponent<Object>().Color.ToInt();
        for (int i = 0; i < 9; i++) // �̵� ������ ���̸� cover
        {
            StageCube.Instance.coverArray[selectedCharacterColor][i].SetActive(false);

            ObjectType obj = StageCube.Instance.touchArray[selectedCharacterColor][i].ObjType;
            if (obj != ObjectType.NULL && obj != ObjectType.PORTAL && obj != ObjectType.TREASURE && obj != ObjectType.MERCHANT)
                movableCube[i] = false;

            if (selectedCharacter.GetComponent<Object>().Type != ObjectType.ENEMY)
                StageCube.Instance.coverArray[selectedCharacterColor][i].SetActive(movableCube[i]);
        }
    }
    public bool Move(Colors color, int index, bool wantMove)
    {
        if (color != selectedCharacter.GetComponent<Object>().Color || !movableCube[index]) return false;
        if (wantMove) StartCoroutine(MoveCoroutine(color, index));
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

        //localPosition의 0,0 으로 이동하는 것이기 때문에 originPos - vector3.zero 를 lookRotation 함수의 인자로
        //전달해줘야 하지만 어차피 0을 뺀 값을 자기자신과 같기 때문에 그냥 originPos를 넣으면 된다.
        Quaternion rot = Quaternion.LookRotation(originPos);
        selectedCharacter.transform.localRotation = rot;
        
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


    public int BingoCheck()
    {
        int[][] bingoNums = new int[6][];
        for (int i = 0; i < 6; i++)
        {
            int[] colorOfSide = new int[9];
            int[] bingoNum = new int[6];

            for (int j = 0; j < 9; j++)
            {
                colorOfSide[j] = StageCube.Instance.touchArray[i][j].RelativeColor.ToInt();
                bingoNum[j % 6] = 0; // %는 그냥 배열 크기가 6이라 에러 안 나도록 해둔거
            }

            if (colorOfSide[0] == colorOfSide[1] && colorOfSide[1] == colorOfSide[2])
                bingoNum[colorOfSide[0]]++;
            if (colorOfSide[3] == colorOfSide[4] && colorOfSide[4] == colorOfSide[5])
                bingoNum[colorOfSide[3]]++;
            if (colorOfSide[6] == colorOfSide[7] && colorOfSide[7] == colorOfSide[8])
                bingoNum[colorOfSide[6]]++;
            if (colorOfSide[0] == colorOfSide[3] && colorOfSide[3] == colorOfSide[6])
                bingoNum[colorOfSide[0]]++;
            if (colorOfSide[1] == colorOfSide[4] && colorOfSide[4] == colorOfSide[7])
                bingoNum[colorOfSide[1]]++;
            if (colorOfSide[2] == colorOfSide[5] && colorOfSide[5] == colorOfSide[8])
                bingoNum[colorOfSide[2]]++;

            bingoNums[i] = bingoNum;

        }

        bool[] playerTeam = new bool[6] { false, false, false, false, false, false };
        playerTeam[StageManager.Instance.Player.Color.ToInt()] = true;
        foreach (GameObject f in StageManager.Instance.FriendList)
            playerTeam[f.GetComponent<Object>().Color.ToInt()] = true;

        for (int j = 0; j < 6; j++) // 각각의 색
        {
            for (int i = 0; i < 6; i++) // 각각의 면
            {
                if (!playerTeam[i]) continue;

                if (bingoNums[i][j] == 6 && bingoStatus[j] != BingoStatus.ALL)
                {
                    bingoStatus[j] = BingoStatus.ALL;
                    BingoReward(BingoStatus.ALL);
                }
                else if (bingoNums[i][j] > 0 & bingoStatus[j] == BingoStatus.NONE)
                {
                    bingoStatus[j] = BingoStatus.ONE;
                    BingoReward(BingoStatus.ONE);
                }
            }
        }

        return 0;
    }

    private void BingoReward(BingoStatus bingo)
    {
        int random = Random.Range(0, 5);

        switch (random)
        {
            case 0: // 최소 공격력 10 / 50% 증가 -> 츨레이어 or 동료
                break;
            case 1: // 최대 공격력 증가 10 / 50% 증가 -> 츨레이어 or 동료
                break;
            case 2: // 해당 턴에 큐브 회전 횟수 4 / 6를 회복
                break;
            case 3: // 유닛의 체력을 30 / 100 최대 체력의 회복 -> 츨레이어 or 동료
                break;
            case 4: // 몬스터 공격력 50 / 100 감소 -> 몬스터
                break;
        }
    }
}