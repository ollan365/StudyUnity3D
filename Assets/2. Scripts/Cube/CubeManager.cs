using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;

public class CubeManager : MonoBehaviour
{
    // 2차원 배열이 inspector에서 할당이 안 돼서 만든 array들
    [SerializeField] private GameObject[] whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray;
    [SerializeField] private GameObject[] wyArray, roArray, bgArray;
    private GameObject[][] colorArray;

    [SerializeField] private GameObject[] turnPoints;

    [SerializeField] private float duration; // 회전에 걸리는 시간
    private Touch mouseStartTouchCube;
    private GameObject mouseStartObject;
    private enum PlayerTurnStatus { NORMAL, TURN, CHARACTER_SELECTED, SUMMONS_SELECTED }
    private PlayerTurnStatus playerTurnStatus;
    private int summonsIndex;

    [SerializeField] private StageManager stageManager;
    [SerializeField] private ObjectManager objectManager;

    [SerializeField] private Text stageCountText;
    private int rotateCount;
    private int moveCount;
    private int changeCount;

    [SerializeField] private GameObject shopPopup;
    private void Start()
    {
        playerTurnStatus = PlayerTurnStatus.NORMAL;
        colorArray = new GameObject[][] { whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray, wyArray, roArray, bgArray };
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && (stageManager.StatusOfStage == StageStatus.PLAYER || stageManager.StatusOfStage == StageStatus.END))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) // 큐브 너머의 오브젝트는 확인 안 함
                    break;

                Object script = hit.collider.gameObject.GetComponent<Object>();
                if (script != null) // 클릭된 객체들 중 Object 컴포넌트를 가진 객체가 있으면
                {
                    mouseStartObject = hit.collider.gameObject;
                    break;
                }
            }

            foreach (RaycastHit hit in hits)
            {
                Touch script = hit.collider.gameObject.GetComponent<Touch>();
                if (script != null) // 클릭된 객체들 중 Touch 컴포넌트를 가진 객체가 있으면
                {
                    MouseStart(script);
                    break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0) && (stageManager.StatusOfStage == StageStatus.PLAYER || stageManager.StatusOfStage == StageStatus.END))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) // 큐브 너머의 오브젝트는 확인 안 함
                    break;

                Object script = hit.collider.gameObject.GetComponent<Object>();
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                if (script != null && mouseStartObject == hit.collider.gameObject) // 오브젝트를 클릭했다면
                {
                    if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
                    {
                        Debug.Log("Already object");
                    }
                    else if(playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED && script.Type == ObjectType.MERCHANT)
                    {
                        if(GetComponent<ColorCheckManager>().Move(script.GetPosition().Color, script.GetPosition().Index, false))
                            OpenMerchantInventory();
                        else
                            Debug.Log($"{script.Type}");
                    }
                    else if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED && script.Type == ObjectType.TREASURE)
                    {
                        if (GetComponent<ColorCheckManager>().Move(script.GetPosition().Color, script.GetPosition().Index, true))
                            objectManager.OpenTreasureBox(script.gameObject);
                        else
                            Debug.Log($"{script.Type}");
                    }
                    else if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED && script.Type == ObjectType.PORTAL)
                    {
                        if (GetComponent<ColorCheckManager>().Move(script.GetPosition().Color, script.GetPosition().Index, true))
                            stageManager.NextStage();
                        else
                            Debug.Log($"{script.Type}");
                    }
                    else if(script.Type != ObjectType.PLAYER) // 일단은 player 말고는 이동이 불가능
                    {
                        Debug.Log($"{script.Type}");
                    }
                    else if (playerTurnStatus == PlayerTurnStatus.NORMAL)
                    {
                        playerTurnStatus = PlayerTurnStatus.CHARACTER_SELECTED;
                        gameObject.GetComponent<ColorCheckManager>().CharacterSelect(mouseStartObject);
                    }
                    else if(playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
                    {
                        playerTurnStatus = gameObject.GetComponent<ColorCheckManager>().CharacterSelectCancel(mouseStartObject)
                            ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.CHARACTER_SELECTED;
                    }
                    mouseStartObject = null;
                    return;
                }
            }
            foreach (RaycastHit hit in hits)
            {
                Touch touchScript = hit.collider.gameObject.GetComponent<Touch>();
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                if (touchScript != null)
                {
                    MouseEnd(touchScript);
                    break;
                }
            }
        }
    }
    public void MouseStart(Touch script)
    {
        if (playerTurnStatus == PlayerTurnStatus.TURN)
        {
            mouseStartObject = null;
            mouseStartTouchCube = null;
            return;
        }
        mouseStartTouchCube = script;
    }
    public void MouseEnd(Touch script)
    {
        if (mouseStartTouchCube == null) return;

        if (mouseStartTouchCube == script) // 같은 곳을 클릭했을 때
        {
            ObjectType type = GetComponent<ColorCheckManager>().CheckCubeObject(script.GetPositionColor(), script.GetPositionIndex());
            if (type == ObjectType.NULL)
            {
                if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
                    if (moveCount > 0 && GetComponent<ColorCheckManager>().Move(script.GetPositionColor(), script.GetPositionIndex(), true))
                        StageCountTextChange(rotateCount, moveCount - 1, changeCount);

                if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
                    playerTurnStatus = stageManager.SummonsFriend(script.GetPositionColor(), script.GetPositionIndex(), summonsIndex)
                        ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.SUMMONS_SELECTED;
            }
            else
            {
                if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
                {
                    Debug.Log("Already object");
                }
                else if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED && type == ObjectType.MERCHANT)
                {
                    if (GetComponent<ColorCheckManager>().Move(script.GetPositionColor(), script.GetPositionIndex(), false))
                        OpenMerchantInventory();
                    else
                        Debug.Log($"{type}");
                }
                else if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED && type == ObjectType.TREASURE)
                {
                    if (GetComponent<ColorCheckManager>().Move(script.GetPositionColor(), script.GetPositionIndex(), true))
                        objectManager.OpenTreasureBox(GetComponent<ColorCheckManager>().GetCubeObject(script.GetPositionColor(), script.GetPositionIndex()));
                    else
                        Debug.Log($"{type}");
                }
                else if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED && type == ObjectType.PORTAL)
                {
                    if (GetComponent<ColorCheckManager>().Move(script.GetPositionColor(), script.GetPositionIndex(), true))
                        stageManager.NextStage();
                    else
                        Debug.Log($"{type}");
                }
                else if (type != ObjectType.PLAYER)
                {
                    Debug.Log($"{type}");
                }
                else if (playerTurnStatus == PlayerTurnStatus.NORMAL)
                {
                    playerTurnStatus = PlayerTurnStatus.CHARACTER_SELECTED;
                    gameObject.GetComponent<ColorCheckManager>().CharacterSelect(GetComponent<ColorCheckManager>().GetCubeObject(script.GetPositionColor(), script.GetPositionIndex()));
                }
                else if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
                {
                    playerTurnStatus = gameObject.GetComponent<ColorCheckManager>().CharacterSelectCancel(GetComponent<ColorCheckManager>().GetCubeObject(script.GetPositionColor(), script.GetPositionIndex()))
                            ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.CHARACTER_SELECTED;
                }
                mouseStartObject = null;
            }
            return;
        }
        else if (playerTurnStatus != PlayerTurnStatus.NORMAL)
            return;

        Touch start = mouseStartTouchCube;
        Touch end = script;

        for (int i =0;i<start.GetTouchColors().Length;i++)
            for(int j=0;j<end.GetTouchColors().Length;j++)
                if(start.GetTouchColors()[i] == end.GetTouchColors()[j])
                {
                    int direction = end.GetTouchInts()[j] - start.GetTouchInts()[i];
                    if (direction < 0) Turn(end.GetTouchColors()[j], -1);
                    else if (direction > 0) Turn(end.GetTouchColors()[j], 1);
                    return;
                }
    }
    public void StageCountTextChange(int rotate, int move, int change)
    {
        rotateCount = rotate;
        moveCount = move;
        changeCount = change;

        stageCountText.text = $"{rotateCount} / {moveCount} / {changeCount}";
    }
    public bool CanChangeWeapon()
    {
        if (changeCount <= 0) return false;

        StageCountTextChange(rotateCount, moveCount, changeCount - 1);
        return true;
    }
    private void Turn(Colors color, int direction)
    {
        if (color == Colors.NULL || playerTurnStatus != PlayerTurnStatus.NORMAL || (rotateCount <= 0 && stageManager.StatusOfStage == StageStatus.PLAYER)) return;
        if (stageManager.StatusOfStage == StageStatus.PLAYER) StageCountTextChange(rotateCount - 1, moveCount, changeCount);
        playerTurnStatus = PlayerTurnStatus.TURN;

        GameObject turnPoint = turnPoints[color.ToInt()];
        GameObject[] array = colorArray[color.ToInt()];
        Vector3 rotation = Vector3.zero;
        switch (color.ToInt())
        {
            case WHITE:
            case WY:
                rotation.y += direction * 90;
                break;
            case RED:
            case RO:
                rotation.x += direction * 90;
                break;
            case BLUE:
            case BG:
                rotation.z += direction * 90;
                break;
            case GREEN:
                rotation.z -= direction * 90;
                break;
            case ORANGE:
                rotation.x -= direction * 90;
                break;
            case YELLOW:
                rotation.y -= direction * 90;
                break;
            default:
                rotation = Vector3.zero;
                break;
        }
        foreach (GameObject position in array) // 큐브의 부모를 turnPoint로 설정하여 함께 회전시킨다
        {
            position.transform.GetChild(0).parent = turnPoint.transform;
        }
        StartCoroutine(TurnEffect(turnPoint, rotation, array));

    }
    private IEnumerator TurnEffect(GameObject turnPoint, Vector3 rotation, GameObject[] array)
    {
        Quaternion startRotation = turnPoint.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(rotation) * startRotation;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            turnPoint.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 보정을 위해 최종 회전 각도로 설정
        turnPoint.transform.localRotation = endRotation;

        yield return new WaitForFixedUpdate();

        foreach (GameObject position in array)
            position.GetComponent<CubePosition>().FindChild();

        yield return new WaitForFixedUpdate(); // 이게 없으면 check cube의 layer가 바뀌기 전에 빙고 체크함

        gameObject.GetComponent<ColorCheckManager>().BingoCheck();

        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }

    public void StartRandomTurn(int randomCount)
    {
        StartCoroutine(RandomTurn(randomCount));
    }
    private IEnumerator RandomTurn(int randomCount)
    {
        for (int i = 0; i < randomCount; i++)
        {
            while (playerTurnStatus == PlayerTurnStatus.TURN)
                yield return new WaitForFixedUpdate();

            int direction = Random.Range(0, 2);
            direction = direction == 1 ? 1 : -1;
            int value = Random.Range(0, 9);

            Turn(value.ToColor(), direction);
        }
    }

    public void SelectSummonsButton(int friendIndex)
    {
        if (playerTurnStatus == PlayerTurnStatus.NORMAL && stageManager.StatusOfStage == StageStatus.PLAYER)
        {
            playerTurnStatus = PlayerTurnStatus.SUMMONS_SELECTED;
            summonsIndex = friendIndex;
            Debug.Log("summons btn selected!");
        }
        else if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
            playerTurnStatus = PlayerTurnStatus.NORMAL;
    }

    private void OpenMerchantInventory()
    {
        objectManager.ChangeShop();
        shopPopup.SetActive(true);
    }
}