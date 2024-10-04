using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using static Constants;

public class CubeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] turnPoints;

    [SerializeField] private float duration; // 회전에 걸리는 시간
    [SerializeField] private float rotateSpeed; //Cube Object Rotation Speed
    private Stack<TurnData> turnDataSave;
    private float currentRotateSpeed = 0;
    private Touch mouseStartTouchCube;
    private GameObject mouseStartObject;
    private Vector3 startPosition;
    private enum PlayerTurnStatus { NORMAL,TURN, CHARACTER_SELECTED, PORTION_SELECTED, SUMMONS_SELECTED }
    [SerializeField] private PlayerTurnStatus playerTurnStatus;
    private int itemID;

    [SerializeField] private float scrollSpeed;
    private float maxValue = 75f;
    private float minValue = 40f;

    public bool IgnoreClick { get; set; }
    public bool IgnoreWheel { get; set; }
    public bool IsEvent { get; set; }

    private void Awake()
    {
        playerTurnStatus = PlayerTurnStatus.NORMAL;
        IsEvent = false;
        IgnoreClick = false;
        IgnoreWheel = false;
        turnDataSave = new();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            ObjectManager.Instance.EnemyInfoPanel.SetActive(false);
        }

        if(StageManager.Instance.StatusOfStage == StageStatus.PLAYER || StageManager.Instance.StatusOfStage == StageStatus.END)
        {
            if (IgnoreWheel || IsEvent) return;
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if(Camera.main.fieldOfView >= maxValue)
            {
                //minZoom
                Camera.main.fieldOfView = maxValue;
                scrollWheel = Mathf.Max(0, scrollWheel);
            }else if(Camera.main.fieldOfView <= minValue)
            {
                //maxZoom
                Camera.main.fieldOfView = minValue;
                scrollWheel = Mathf.Min(0, scrollWheel);
            }
            //Debug.Log(Camera.main);
            //Debug.Log(scrollWheel);
            Camera.main.fieldOfView -= scrollWheel * Time.deltaTime * scrollSpeed;
        }

        if (Input.GetMouseButtonDown(2))
        {
            if (IgnoreWheel) return;
            if (StageManager.Instance.StatusOfStage != StageStatus.PLAYER && StageManager.Instance.StatusOfStage != StageStatus.END) return;
            currentRotateSpeed = 0;
        }
        
        if (Input.GetMouseButton(2))
        {
            if (IgnoreWheel) return;
            if (StageManager.Instance.StatusOfStage != StageStatus.PLAYER && StageManager.Instance.StatusOfStage != StageStatus.END) return;

            transform.Rotate(0f, -Input.GetAxis("Mouse X") * currentRotateSpeed, 0f, Space.World);
            transform.Rotate(Input.GetAxis("Mouse Y") * currentRotateSpeed, 0f, Input.GetAxis("Mouse Y") * currentRotateSpeed, Space.World);
            
            currentRotateSpeed = Mathf.Clamp(currentRotateSpeed + Time.deltaTime * 10, 0, rotateSpeed);
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 초기화
            mouseStartObject = null;
            mouseStartTouchCube = null;

            if (IgnoreClick || IsEvent) return;
            
            if (StageManager.Instance.StatusOfStage != StageStatus.PLAYER && StageManager.Instance.StatusOfStage != StageStatus.END) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) break; // 큐브 너머의 오브젝트는 확인 안 함

                Object script = hit.collider.gameObject.GetComponent<Object>();
                if (script != null) // 클릭된 객체들 중 Object 컴포넌트를 가진 객체가 있으면
                {
                    mouseStartObject = hit.collider.gameObject;
                    break;
                }
            }

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) break; // 큐브 너머의 오브젝트는 확인 안 함

                Touch script = hit.collider.gameObject.GetComponent<Touch>();
                if (script != null) // 클릭된 객체들 중 Touch 컴포넌트를 가진 객체가 있으면
                {
                    MouseStart(script);
                    break;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (IgnoreClick || IsEvent) return;
            if (StageManager.Instance.StatusOfStage != StageStatus.PLAYER && StageManager.Instance.StatusOfStage != StageStatus.END) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject.layer == 6) // 큐브 너머의 오브젝트는 확인 안 함
                    break;

                Object obj = hit.collider.gameObject.GetComponent<Object>();
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                if (obj != null && mouseStartObject == hit.collider.gameObject) // 오브젝트를 클릭했다면
                {
                    ClickObject(obj);
                    return;
                }
            }
            foreach (RaycastHit hit in hits)
            {
                Touch touchScript = hit.collider.gameObject.GetComponent<Touch>();
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                if (touchScript != null && touchScript == mouseStartTouchCube)
                {
                    MouseEnd(touchScript); // 같은 곳을 클릭한 경우
                    return;
                }
            }

            // 그냥 허공에 마우스를 뗀 경우
            if (mouseStartTouchCube != null)
            {
                int index = mouseStartTouchCube.Direction(startPosition);
                Turn(mouseStartTouchCube.TouchColors[index], mouseStartTouchCube.TouchInts[index]);
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
        startPosition = Input.mousePosition;
        mouseStartTouchCube = script;
    }
    public void MouseEnd(Touch script)
    {
        ObjectType type = script.ObjType;
        if (type == ObjectType.NULL)
        {
            if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
            {
                if (StageManager.Instance.GetStageTextValue(StageText.MOVE) > 0 && ColorCheckManager.Instance.Move(script.Color, script.Index, true, false))
                    StageManager.Instance.SetStageTextValue(StageText.MOVE, -1);
                else
                    ChangeToNormal();
            }
            if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
            {
                StageManager.Instance.SummonsFriend(script, itemID);
                playerTurnStatus = PlayerTurnStatus.NORMAL;
            }
        }
        else
        {
            ClickObject(script.Obj);
        }
        return;
    }
    public void ChangeToNormal()
    {
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);
        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }
    private void Turn(Colors color, int direction, bool isMaze = false, bool isInverse = false)
    {
        if (color == Colors.NULL || playerTurnStatus != PlayerTurnStatus.NORMAL
            || (StageManager.Instance.GetStageTextValue(StageText.ROTATE) <= 0 && StageManager.Instance.StatusOfStage == StageStatus.PLAYER)) return;

        if (!isMaze && StageManager.Instance.StatusOfStage == StageStatus.PLAYER) StageManager.Instance.SetStageTextValue(StageText.ROTATE, -1);

        playerTurnStatus = PlayerTurnStatus.TURN;

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
        foreach (GameObject position in array) // 큐브의 부모를 turnPoint로 설정하여 함께 회전시킨다
        {
            position.GetComponent<CubePosition>().FindNewChild();
            position.transform.GetChild(0).parent = turnPoint.transform;
        }

        if(!isInverse) turnDataSave.Push(new TurnData(color, direction));
        StartCoroutine(TurnEffect(turnPoint, rotation, array, isMaze));
    }
    private IEnumerator TurnEffect(GameObject turnPoint, Vector3 rotation, GameObject[] array, bool isMaze)
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

        foreach (GameObject position in array) position.GetComponent<CubePosition>().FindNewChild();

        yield return new WaitForFixedUpdate(); // 이게 없으면 check cube의 layer가 바뀌기 전에 빙고 체크함
       

        if (StageManager.Instance.StatusOfStage == StageStatus.PLAYER && !isMaze)
        {
            EventManager.Instance.BingoCheck(); 
        }

        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }
    public void StartRandomTurn(int randomCount, bool isMaze)
    {
        StartCoroutine(RandomTurn(randomCount, isMaze));
    }
    private IEnumerator RandomTurn(int randomCount, bool isMaze)
    {
        yield return new WaitForFixedUpdate(); // 반드시 필요!

        duration /= 2;

        for (int i = 0; i < randomCount; i++)
        {
            while (playerTurnStatus == PlayerTurnStatus.TURN)
                yield return new WaitForFixedUpdate();

            int direction = Random.Range(0, 2);
            direction = direction == 1 ? 1 : -1;
            int value = Random.Range(0, 9);

            Turn(value.ToColor(), direction, isMaze);
        }
        while (playerTurnStatus == PlayerTurnStatus.TURN)
            yield return new WaitForFixedUpdate();

        duration *= 2;
    }
    public void InverseTurn()
    {
        StartCoroutine(InverseTurnCoroutine());
    }
    private IEnumerator InverseTurnCoroutine()
    {
        yield return new WaitForFixedUpdate();

        duration = 3 / turnDataSave.Count;

        while (turnDataSave.Count != 0)
        {
            while (playerTurnStatus == PlayerTurnStatus.TURN)
                yield return new WaitForFixedUpdate();

            TurnData turn = turnDataSave.Pop();
            Turn(turn.color, turn.direction * -1, false, true);
        }
        while (playerTurnStatus == PlayerTurnStatus.TURN)
            yield return new WaitForFixedUpdate();
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
                if ((playerTurnStatus == PlayerTurnStatus.NORMAL || playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED) && StageManager.Instance.StatusOfStage == StageStatus.PLAYER)
                {
                    ColorCheckManager.Instance.CharacterSelectCancel(null, true);
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
                        if (ColorCheckManager.Instance.Move(obj.Color, obj.Index, false))
                        {
                            ObjectManager.Instance.OpenShop();
                            ChangeToNormal();
                        }
                            
                        break;
                    case ObjectType.TRIGGER:
                        Debug.Log(obj.gameObject);
                        if (ColorCheckManager.Instance.Move(obj.Color, obj.Index, false, false))
                            StageManager.Instance.StagePlayLogic.Trigger(obj.gameObject);
                        break;
                    case ObjectType.PORTAL:
                        if (ColorCheckManager.Instance.Move(obj.Color, obj.Index))
                        {
                            StageManager.Instance.NextStage(obj.gameObject);
                        }
                        break;
                    case ObjectType.PLAYER:
                    case ObjectType.FRIEND:
                        playerTurnStatus = ColorCheckManager.Instance.CharacterSelectCancel(obj.gameObject, false)
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
                        ColorCheckManager.Instance.CharacterSelect(obj.gameObject);
                    }
                }
                if (obj.Type == ObjectType.ENEMY)
                {
                    ObjectManager.Instance.SetObjectInfo(obj);
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

public struct TurnData
{
    public Colors color;
    public int direction;

    public TurnData(Colors color, int direction)
    {
        this.color = color;
        this.direction = direction;
    }
}