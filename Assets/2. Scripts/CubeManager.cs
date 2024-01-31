using UnityEngine;
using System.Collections;
using static Constants;

public class CubeManager : MonoBehaviour
{
    // 2���� �迭�� inspector���� �Ҵ��� �� �ż� ���� array��
    [SerializeField] private GameObject[] whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray;
    [SerializeField] private GameObject[] wyArray, roArray, bgArray;
    private GameObject[][] colorArray;

    [SerializeField] private GameObject[] turnPoints;

    [SerializeField] private float duration; // ȸ���� �ɸ��� �ð�
    private bool isTurning; // ť�갡 ���ư��� �ִ°�
    public bool isDraging; // ���콺 �巡�� ���ΰ�
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
                if (script != null) // Ŭ���� ��ü�� �� Touch ������Ʈ�� ���� ��ü�� ������
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
                // ���⼭ hit.collider.gameObject�� Ŭ���� ��ü�� ��Ÿ���ϴ�.
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
            // �� ���� Ŭ������ ��
            // �װ��� ĳ���Ͱ� �ִٸ� �����ϴ� �������� ������ �Ƹ�...?
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
        foreach (GameObject position in array) // ť���� �θ� turnPoint�� �����Ͽ� �Բ� ȸ����Ų��
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

        // ������ ���� ���� ȸ�� ������ ����
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