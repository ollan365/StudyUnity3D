using UnityEngine;
using System.Collections;
using static Constants;

public class CubeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] turnPoints;

    [SerializeField] private float duration; // ȸ���� �ɸ��� �ð�
    [SerializeField] private float rotateSpeed; //Cube Object Rotation Speed
    private Touch mouseStartTouchCube;
    private GameObject mouseStartObject;
    private Vector3 startPosition;
    private float value;
    private enum PlayerTurnStatus { NORMAL, TURN, CHARACTER_SELECTED, PORTION_SELECTED, SUMMONS_SELECTED }
    [SerializeField] private PlayerTurnStatus playerTurnStatus;
    private int itemID;


    private void Awake()
    {
        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }
    private void Update()
    {
        if (Input.GetMouseButton(2) && (StageManager.Instance.StatusOfStage != StageStatus.INIT && StageManager.Instance.StatusOfStage != StageStatus.FIGHT))
        {
            transform.Rotate(0f, -Input.GetAxis("Mouse X") * rotateSpeed, 0f, Space.World);
            transform.Rotate(Input.GetAxis("Mouse Y") * rotateSpeed, 0f, Input.GetAxis("Mouse Y") * rotateSpeed, Space.World);
        }

        if (Input.GetMouseButtonDown(0) && (StageManager.Instance.StatusOfStage == StageStatus.PLAYER || StageManager.Instance.StatusOfStage == StageStatus.END))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) // ť�� �ʸ��� ������Ʈ�� Ȯ�� �� ��
                    break;

                Object script = hit.collider.gameObject.GetComponent<Object>();
                if (script != null) // Ŭ���� ��ü�� �� Object ������Ʈ�� ���� ��ü�� ������
                {
                    mouseStartObject = hit.collider.gameObject;
                    break;
                }
            }

            foreach (RaycastHit hit in hits)
            {
                Touch script = hit.collider.gameObject.GetComponent<Touch>();
                if (script != null) // Ŭ���� ��ü�� �� Touch ������Ʈ�� ���� ��ü�� ������
                {
                    script.DrawRay();
                    MouseStart(script);
                    break;
                }
            }
        }

        // �߰� ��
        //if (Input.GetMouseButton(0) && StageManager.Instance.StatusOfStage == StageStatus.PLAYER || StageManager.Instance.StatusOfStage == StageStatus.END)
        //{
        //    if (mouseStartTouchCube != null)
        //    {
        //        value = Vector3.Distance(Input.mousePosition, startPosition) / 2000;
        //        Debug.Log($"{value}");
        //        int index = mouseStartTouchCube.Direction(startPosition);
        //        Turn(mouseStartTouchCube.TouchColors[index], mouseStartTouchCube.TouchInts[index], false);
        //    }
        //}

        if (Input.GetMouseButtonUp(0) && (StageManager.Instance.StatusOfStage == StageStatus.PLAYER || StageManager.Instance.StatusOfStage == StageStatus.END))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) // ť�� �ʸ��� ������Ʈ�� Ȯ�� �� ��
                    break;

                Object obj = hit.collider.gameObject.GetComponent<Object>();
                // ���⼭ hit.collider.gameObject�� Ŭ���� ��ü�� ��Ÿ���ϴ�.
                if (obj != null && mouseStartObject == hit.collider.gameObject) // ������Ʈ�� Ŭ���ߴٸ�
                {
                    ClickObject(obj);
                    mouseStartObject = null;
                    return;
                }
            }
            foreach (RaycastHit hit in hits)
            {
                Touch touchScript = hit.collider.gameObject.GetComponent<Touch>();
                // ���⼭ hit.collider.gameObject�� Ŭ���� ��ü�� ��Ÿ���ϴ�.
                if (touchScript != null && touchScript == mouseStartTouchCube)
                {
                    MouseEnd(touchScript); // ���� ���� Ŭ���� ���
                    return;
                }
            }

            // �׳� ����� ���콺�� �� ���
            if (mouseStartTouchCube != null)
            {
                int index = mouseStartTouchCube.Direction(startPosition);
                Turn(mouseStartTouchCube.TouchColors[index], mouseStartTouchCube.TouchInts[index], true);
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
        startPosition = Input.mousePosition; // �ٵ� �̰� mouseEnd���� �ʱ�ȭ �� �ص� �ǳ�?
        mouseStartTouchCube = script;
    }
    public void MouseEnd(Touch script)
    {
        ObjectType type = script.ObjType;
        if (type == ObjectType.NULL)
        {
            if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
            {
                if (StageManager.Instance.GetStageTextValue(StageText.MOVE) > 0 && GetComponent<ColorCheckManager>().Move(script.Color, script.Index, true))
                    StageManager.Instance.SetStageTextValue(StageText.MOVE, -1);
                else
                    ChangeToNormal();
            }
            if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
            {
                StageManager.Instance.SummonsFriend(script.Color, script.Index, itemID);
                playerTurnStatus = PlayerTurnStatus.NORMAL;
            }
        }
        else
        {
            ClickObject(script.Obj);
            mouseStartObject = null;
        }
        return;
    }
    public void ChangeToNormal()
    {
        gameObject.GetComponent<ColorCheckManager>().CharacterSelectCancel(null, true);
        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }
    private void Turn(Colors color, int direction, bool isTime)
    {
        if (color == Colors.NULL || playerTurnStatus != PlayerTurnStatus.NORMAL
            || (StageManager.Instance.GetStageTextValue(StageText.ROTATE) <= 0 && StageManager.Instance.StatusOfStage == StageStatus.PLAYER)) return;

        if (StageManager.Instance.StatusOfStage == StageStatus.PLAYER) StageManager.Instance.SetStageTextValue(StageText.ROTATE, -1);

        if (isTime) playerTurnStatus = PlayerTurnStatus.TURN;

        GameObject turnPoint = turnPoints[color.ToInt()];
        GameObject[] array = StageCube.Instance.colorArray[color.ToInt()];
        Vector3 rotation = Vector3.zero;
        switch (color.ToInt())
        {
            case WHITE:
            case WY:
                rotation.y += direction * 90; break;
            case RED:
            case RO:
                rotation.x += direction * 90; break;
            case BLUE:
            case BG:
                rotation.z += direction * 90; break;
            case GREEN:
                rotation.z -= direction * 90; break;
            case ORANGE:
                rotation.x -= direction * 90; break;
            case YELLOW:
                rotation.y -= direction * 90; break;
            default:
                rotation = Vector3.zero; break;
        }
        foreach (GameObject position in array) // ť���� �θ� turnPoint�� �����Ͽ� �Բ� ȸ����Ų��
        {
            position.GetComponent<CubePosition>().FindNewChild();
            position.transform.GetChild(0).parent = turnPoint.transform;
        }

        if (isTime) StartCoroutine(TurnEffect(turnPoint, rotation, array));
        else DiscreteTurnEffect(turnPoint, rotation, array);
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
        
        // ������ ���� ���� ȸ�� ������ ����
        turnPoint.transform.localRotation = endRotation;

        yield return new WaitForFixedUpdate();

        foreach (GameObject position in array) position.GetComponent<CubePosition>().FindNewChild();

        yield return new WaitForFixedUpdate(); // �̰� ������ check cube�� layer�� �ٲ�� ���� ���� üũ��

        gameObject.GetComponent<ColorCheckManager>().BingoTextChange(-1);

        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }
    private void DiscreteTurnEffect(GameObject turnPoint, Vector3 rotation, GameObject[] array)
    {
        Quaternion startRotation = turnPoint.transform.localRotation;
        Quaternion endRotation = Quaternion.Euler(rotation) * startRotation;

        turnPoint.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, value);
        
        foreach (GameObject position in array) position.GetComponent<CubePosition>().FindPriorChild();

        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }

    public void StartRandomTurn(int randomCount)
    {
        StartCoroutine(RandomTurn(randomCount));
    }
    private IEnumerator RandomTurn(int randomCount)
    {
        yield return new WaitForFixedUpdate(); // �ݵ�� �ʿ�!

        duration /= 2;

        for (int i = 0; i < randomCount; i++)
        {
            while (playerTurnStatus == PlayerTurnStatus.TURN)
                yield return new WaitForFixedUpdate();

            int direction = Random.Range(0, 2);
            direction = direction == 1 ? 1 : -1;
            int value = Random.Range(0, 9);

            Turn(value.ToColor(), direction, true);
        }
        while (playerTurnStatus == PlayerTurnStatus.TURN)
            yield return new WaitForFixedUpdate();

        duration *= 2;

        // �������� ����
        StartCoroutine(StageManager.Instance.StartStage());
    }

    public void SwitchPlayerTurnStatus(int itemIndex, ItemType itemType)
    {
        switch (itemType)
        {
            case ItemType.SCROLL:
                if (playerTurnStatus == PlayerTurnStatus.NORMAL && StageManager.Instance.StatusOfStage == StageStatus.PLAYER)
                {
                    playerTurnStatus = PlayerTurnStatus.SUMMONS_SELECTED;
                    itemID = itemIndex;
                    Debug.Log("summons btn selected!");
                }
                else if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
                    playerTurnStatus = PlayerTurnStatus.NORMAL;
                break;
            case ItemType.PORTION:
                if (playerTurnStatus == PlayerTurnStatus.NORMAL && StageManager.Instance.StatusOfStage == StageStatus.PLAYER)
                {
                    playerTurnStatus = PlayerTurnStatus.PORTION_SELECTED;
                    itemID = itemIndex;
                    Debug.Log("portion selected!");
                }
                else if (playerTurnStatus == PlayerTurnStatus.PORTION_SELECTED)
                    playerTurnStatus = PlayerTurnStatus.NORMAL;
                break;


        }
    }

    private void ClickObject(Object obj)
    {
        switch (playerTurnStatus)
        {
            case PlayerTurnStatus.CHARACTER_SELECTED:
                switch (obj.Type)
                {
                    case ObjectType.MERCHANT:
                        if (GetComponent<ColorCheckManager>().Move(obj.Color, obj.Index, false))
                            ObjectManager.Instance.ChangeShop();
                        break;
                    case ObjectType.TREASURE:
                        if (GetComponent<ColorCheckManager>().Move(obj.Color, obj.Index, true))
                            StageManager.Instance.StagePlayLogic.OpenTreasureBox(obj.gameObject);
                        break;
                    case ObjectType.PORTAL:
                        if (GetComponent<ColorCheckManager>().Move(obj.Color, obj.Index, true))
                            StageManager.Instance.NextStage();
                        break;
                    case ObjectType.PLAYER:
                    case ObjectType.FRIEND:
                        playerTurnStatus = GetComponent<ColorCheckManager>().CharacterSelectCancel(obj.gameObject, false)
                            ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.CHARACTER_SELECTED;
                        break;
                    case ObjectType.ENEMY:
                        ChangeToNormal();
                        break;
                }
                break;
            case PlayerTurnStatus.NORMAL:
                if (obj.Type == ObjectType.PLAYER || obj.Type == ObjectType.FRIEND)
                {
                    if (StageManager.Instance.GetStageTextValue(StageText.MOVE) > 0)
                    {
                        playerTurnStatus = PlayerTurnStatus.CHARACTER_SELECTED;
                        gameObject.GetComponent<ColorCheckManager>().CharacterSelect(obj.gameObject);
                    }
                }
                break;
            case PlayerTurnStatus.PORTION_SELECTED:
                if (obj.Type == ObjectType.PLAYER || obj.Type == ObjectType.FRIEND)
                {
                    StageManager.Instance.StagePlayLogic.UsePortion(itemID, obj.gameObject);
                    ObjectManager.Instance.UseItem(ItemType.PORTION, itemID);

                    playerTurnStatus = PlayerTurnStatus.NORMAL;
                }
                break;
            case PlayerTurnStatus.SUMMONS_SELECTED:
            case PlayerTurnStatus.TURN:
                break;
        }
    }
}