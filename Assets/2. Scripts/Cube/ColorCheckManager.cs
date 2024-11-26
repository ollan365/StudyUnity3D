using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using static Constants;
using TMPro;

public class ColorCheckManager : MonoBehaviour
{
    public static ColorCheckManager Instance { get; private set; }
    //[SerializeField] private TMP_Text[] bingoTexts;

    private GameObject selectedCharacter;
    public GameObject SelectedCharacter { get => selectedCharacter; }
    private bool[] movableCube;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        movableCube = new bool[9];
    }
    public void CharacterSelect(GameObject character)
    {
        // 보스 구속으로 인한 플레이어 이동 불가 상태 (경고창 띄우는 등의 이펙트 필요)
        if (Boss.Instance && Boss.Instance.playerCantMove && character.GetComponent<Object>().Type == ObjectType.PLAYER) return;

        selectedCharacter = character;
        MovableCubeSetting(selectedCharacter.GetComponent<Object>().Index);
    }
    public bool CharacterSelectCancel(GameObject character, bool mustChange)
    {
        if (!mustChange && selectedCharacter != character)
            return false;

        selectedCharacter = null;

        MovableCubeSetting(-1);
        return true;
    }

    public void MovableCubeSetting(int index)
    {
        if (selectedCharacter == null) index = -1;

        switch (index)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 7:
            case 8:
                movableCube = StageCube.Instance.Cross(index);
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
            if (obj != ObjectType.NULL && obj != ObjectType.PORTAL && obj != ObjectType.TRIGGER && obj != ObjectType.MERCHANT)
                movableCube[i] = false;

            if (StageManager.Instance.StatusOfStage == StageStatus.PLAYER || StageManager.Instance.StatusOfStage == StageStatus.END)
                StageCube.Instance.coverArray[selectedCharacterColor][i].SetActive(movableCube[i]);
        }
    }
    public bool Move(Colors color, int index, bool wantMove = true, bool characterSelectCancel = true)
    {
        if (selectedCharacter == null || color != selectedCharacter.GetComponent<Object>().Color || !movableCube[index]) return false;
        if (wantMove) StartCoroutine(MoveCoroutine(color, index, characterSelectCancel));
        return true;
    }
    public IEnumerator MoveCoroutine(Colors color, int index, bool characterSelectCancel)
    {
        GameObject obj = selectedCharacter;
        CharacterSelectCancel(null, true);
        MovableCubeSetting(-1);

        Transform parent = StageCube.Instance.touchArray[color.ToInt()][index].ObjectPostion;

        obj.transform.parent = parent;

        Vector3 originPos = obj.transform.localPosition;
        Vector3 middlePos = Vector3.Lerp(originPos, Vector3.zero, 0.5f);

        float travelTIme = 0f;

        //localPosition의 0,0 으로 이동하는 것이기 때문에 originPos - vector3.zero 를 lookRotation 함수의 인자로
        //전달해줘야 하지만 어차피 0을 뺀 값을 자기자신과 같기 때문에 그냥 originPos를 넣으면 된다.
        Quaternion rot = Quaternion.LookRotation(originPos);
        obj.transform.localRotation = rot;
        
        while (travelTIme < 0.15f)
        {
            obj.transform.localPosition = Vector3.Lerp(originPos, middlePos, travelTIme / 0.15f);
            travelTIme += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        while (travelTIme < 0.5f)
        {
            obj.transform.localPosition = Vector3.Lerp(middlePos, Vector3.zero, travelTIme / 0.5f);
            travelTIme += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForFixedUpdate();

        obj.transform.localPosition = Vector3.zero;
        obj.GetComponent<Object>().touchCube = StageCube.Instance.touchArray[color.ToInt()][index];

        MovableCubeSetting(index);

        if (!characterSelectCancel) CharacterSelect(obj);

        if (obj.GetComponent<Object>().Type == ObjectType.PLAYER) EventManager.Instance.BingoCheck();
        else if(obj.GetComponent<Object>().Type == ObjectType.ENEMY)
        {
            if (obj.GetComponent<Object>().touchCube.Obj.Type == ObjectType.TRIGGER)
            {
                if(obj.GetComponent<Object>().touchCube.Obj.Name == "Treasure") ObjectManager.Instance.ObjectDie(obj.GetComponent<Object>().touchCube.Obj.gameObject);
                else StageManager.Instance.StagePlayLogic.Trigger(obj.GetComponent<Object>().touchCube.Obj.gameObject);
            }
        }
    }
}