using UnityEngine;
using System.Collections;
using static Constants;

public class CubeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] turnPoints;

    [SerializeField] private float duration; // 회전에 걸리는 시간
    [SerializeField] private float rotateSpeed; //Cube Object Rotation Speed
    
    private Touch mouseStartTouchCube;
    private GameObject mouseStartObject;
    private enum PlayerTurnStatus { NORMAL, TURN, CHARACTER_SELECTED, PORTION_SELECTED, SUMMONS_SELECTED }
    private PlayerTurnStatus playerTurnStatus;
    private int itemID;

    //Camera Zoon In and Out
    [SerializeField] private float scrollSpeed; // Camera Object Scroll Speed
    private float maxValue = 90.0f;
    private float minValue = 40.0f;

    private void Awake()
    {
        playerTurnStatus = PlayerTurnStatus.NORMAL;
    }

    private void Update()
    {
        //240325 Camera Zoom in&out
        if(StageManager.Instance.StatusOfStage != StageStatus.INIT && StageManager.Instance.StatusOfStage != StageStatus.FIGHT){
            
            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (Camera.main.fieldOfView >= maxValue) 
            {
                //Min zoom
                Camera.main.fieldOfView = maxValue;
                scrollWheel = Mathf.Max(0, scrollWheel);
            }
            else if (Camera.main.fieldOfView <= minValue)
            {   
                //Max zoom
                Camera.main.fieldOfView = minValue;
                scrollWheel = Mathf.Min(0, scrollWheel);
            }
            Camera.main.fieldOfView -= scrollWheel * Time.deltaTime * scrollSpeed;
            
        }
        //Camera Zoom in&out     



        if (Input.GetMouseButton(2) &&(StageManager.Instance.StatusOfStage != StageStatus.INIT && StageManager.Instance.StatusOfStage != StageStatus.FIGHT))
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
        if (Input.GetMouseButtonUp(0) && (StageManager.Instance.StatusOfStage == StageStatus.PLAYER || StageManager.Instance.StatusOfStage == StageStatus.END))
        {
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
            ObjectType type = script.ObjType;
            if (type == ObjectType.NULL)
            {
                if (playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
                {
                    if (StageManager.Instance.StageTextChange(false, StageText.MOVE, -1) && GetComponent<ColorCheckManager>().Move(script.Color, script.Index, true))
                        StageManager.Instance.StageTextChange(true, StageText.MOVE, -1);

                    DisableMoveableBlock(StageManager.Instance.Player.gameObject);
                }
                    

                if (playerTurnStatus == PlayerTurnStatus.SUMMONS_SELECTED)
                    playerTurnStatus = StageManager.Instance.SummonsFriend(script.Color, script.Index, itemID)
                        ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.SUMMONS_SELECTED;
            }
            else
            {
                ClickObject(script.Obj);
                mouseStartObject = null;
            }
            return;
        }
        else if(script.ObjType == ObjectType.NULL && playerTurnStatus == PlayerTurnStatus.CHARACTER_SELECTED)
        {
            DisableMoveableBlock(StageManager.Instance.Player.gameObject);
            return;
        }
        else if (playerTurnStatus != PlayerTurnStatus.NORMAL)
            return;

        Touch start = mouseStartTouchCube;
        Touch end = script;

        for (int i =0;i<start.TouchColors.Length;i++)
            for(int j=0;j<end.TouchColors.Length;j++)
                if(start.TouchColors[i] == end.TouchColors[j])
                {
                    int direction = end.TouchInts[j] - start.TouchInts[i];
                    if (direction < 0) Turn(end.TouchColors[j], -1);
                    else if (direction > 0) Turn(end.TouchColors[j], 1);
                    return;
                }
    }
    private void Turn(Colors color, int direction)
    {
        if (color == Colors.NULL || playerTurnStatus != PlayerTurnStatus.NORMAL || (!StageManager.Instance.StageTextChange(false, StageText.ROTATE, -1) && StageManager.Instance.StatusOfStage == StageStatus.PLAYER)) return;
        if (StageManager.Instance.StatusOfStage == StageStatus.PLAYER)
            if(!StageManager.Instance.StageTextChange(true, StageText.ROTATE, -1)) return;

        playerTurnStatus = PlayerTurnStatus.TURN;

        GameObject turnPoint = turnPoints[color.ToInt()];
        GameObject[] array = StageCube.Instance.colorArray[color.ToInt()];
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

        for (int i = 0; i < 6; i++)
            gameObject.GetComponent<ColorCheckManager>().BingoCheck(i, false);

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
                        playerTurnStatus = gameObject.GetComponent<ColorCheckManager>().CharacterSelectCancel(mouseStartObject)
                            ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.CHARACTER_SELECTED;
                        break;
                    case ObjectType.ENEMY:
                        //여기서 DisableMoveableBlock을 해야하는데, 지금 선택된건 적아니면 NULL이라 플레이어의 게임 오브젝트를 받아와야함 
                        DisableMoveableBlock(StageManager.Instance.Player.gameObject);
                        break;
                }
                break;
            case PlayerTurnStatus.NORMAL:
                if (obj.Type == ObjectType.PLAYER || obj.Type == ObjectType.FRIEND)
                {
                    playerTurnStatus = PlayerTurnStatus.CHARACTER_SELECTED;
                    gameObject.GetComponent<ColorCheckManager>().CharacterSelect(mouseStartObject);
                }
                break;
            case PlayerTurnStatus.PORTION_SELECTED:
                if (obj.Type == ObjectType.PLAYER || obj.Type == ObjectType.FRIEND)
                {
                    StageManager.Instance.StagePlayLogic.UsePortion(itemID, obj.gameObject);
                    ObjectManager.Instance.UseItem(ItemType.PORTION, itemID);
                }
                break;
            case PlayerTurnStatus.SUMMONS_SELECTED:
            case PlayerTurnStatus.TURN:
                break;
        }
    }
    public void DisableMoveableBlock(GameObject character)
    {
        playerTurnStatus = gameObject.GetComponent<ColorCheckManager>().CharacterSelectCancel(character)
            ? PlayerTurnStatus.NORMAL : PlayerTurnStatus.CHARACTER_SELECTED;
    }
}