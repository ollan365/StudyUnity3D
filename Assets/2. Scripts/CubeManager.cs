using UnityEngine;
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
    private bool isTurning; // 큐브가 돌아가고 있는가
    public bool isDraging; // 마우스 드래그 중인가
    private GameObject mouseStartObject;

    private void Start()
    {
        isTurning = false;
        colorArray = new GameObject[][] { whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray, wyArray, roArray, bgArray };
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                Touch script = hit.collider.gameObject.GetComponent<Touch>();
                if (script != null) // 클릭된 객체들 중 Touch 컴포넌트를 가진 객체가 있으면
                {
                    MouseStart(hit.collider.gameObject);
                    break;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits;
            hits = Physics.RaycastAll(ray);

            foreach (RaycastHit hit in hits)
            {
                Touch script = hit.collider.gameObject.GetComponent<Touch>();
                // 여기서 hit.collider.gameObject는 클릭된 객체를 나타냅니다.
                if (script != null)
                {
                    MouseEnd(hit.collider.gameObject);
                    break;
                }
            }
        }
    }
    public void MouseStart(GameObject mouseStartObject)
    {
        if (isTurning) return;

        isDraging = true;
        this.mouseStartObject = mouseStartObject;
    }
    public void MouseEnd(GameObject mouseEndObject)
    {
        if (!isDraging) return;
        isDraging = false;

        if(mouseStartObject == mouseEndObject)
        {
            // 한 곳을 클릭했을 때
            // 그곳에 캐릭터가 있다면 선택하는 로직으로 가겠지 아마...?
            mouseEndObject.GetComponent<Touch>().PrintInfo();
            return;
        }

        Touch start = mouseStartObject.GetComponent<Touch>();
        Touch end = mouseEndObject.GetComponent<Touch>();

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
    private void Turn(Colors color, int direction)
    {
        if (color == Colors.NULL || isTurning) return;
        isTurning = true;

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
            yield return null;
        }

        // 보정을 위해 최종 회전 각도로 설정
        turnPoint.transform.localRotation = endRotation;

        foreach (GameObject position in array)
            position.GetComponent<CubePosition>().FindChild();

        gameObject.GetComponent<ColorCheckManager>().BingoCheck();
        isTurning = false;
    }

    public void StartRandomTurn(int randomCount)
    {
        StartCoroutine(RandomTurn(randomCount));
    }
    private IEnumerator RandomTurn(int randomCount)
    {
        for (int i = 0; i < randomCount; i++)
        {
            while (isTurning)
                yield return null;

            int direction = Random.Range(0, 2);
            direction = direction == 1 ? 1 : -1;
            int value = Random.Range(0, 9);

            Turn(value.ToColor(), direction);
        }
    }
}