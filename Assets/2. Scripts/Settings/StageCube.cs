using UnityEngine;

public class StageCube : MonoBehaviour
{
    public static StageCube Instance { get; private set; }

    [Header("Color Arrays")]
    [SerializeField] private GameObject[] whiteArray;
    [SerializeField] private GameObject[] redArray;
    [SerializeField] private GameObject[] blueArray;
    [SerializeField] private GameObject[] greenArray;
    [SerializeField] private GameObject[] orangeArray;
    [SerializeField] private GameObject[] yellowArray;

    [SerializeField] private GameObject[] wyArray;
    [SerializeField] private GameObject[] roArray;
    [SerializeField] private GameObject[] bgArray;
    public GameObject[][] colorArray;

    [Header("Touch Arrays")]
    [SerializeField] private Touch[] whiteTouch;
    [SerializeField] private Touch[] redTouch;
    [SerializeField] private Touch[] blueTouch;
    [SerializeField] private Touch[] greenTouch;
    [SerializeField] private Touch[] orangeTouch;
    [SerializeField] private Touch[] yellowTouch;
    public Touch[][] touchArray;

    [Header("Cover Arrays")]
    [SerializeField] private GameObject[] whiteCoverArray;
    [SerializeField] private GameObject[] redCoverArray;
    [SerializeField] private GameObject[] blueCoverArray;
    [SerializeField] private GameObject[] greenCoverArray;
    [SerializeField] private GameObject[] orangeCoverArray;
    [SerializeField] private GameObject[] yellowCoverArray;
    public GameObject[][] coverArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            colorArray = new GameObject[][] { whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray, wyArray, roArray, bgArray };
            coverArray = new GameObject[][] { whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray };
            touchArray = new Touch[][] { whiteTouch, redTouch, blueTouch, greenTouch, orangeTouch, yellowTouch };
        }
        else
            Destroy(gameObject);
    }

    public bool[] Cross(int index)
    {
        bool[] array = new bool[9];

        for (int i = 0; i < array.Length; i++)
            array[i] = false;

        switch (index)
        {
            case 0:
                array[1] = true;
                array[3] = true;
                break;
            case 1:
                array[0] = true;
                array[2] = true;
                array[4] = true;
                break;
            case 2:
                array[1] = true;
                array[5] = true;
                break;
            case 3:
                array[0] = true;
                array[4] = true;
                array[6] = true;
                break;
            case 4:
                array[1] = true;
                array[3] = true;
                array[5] = true;
                array[7] = true;
                break;
            case 5:
                array[2] = true;
                array[4] = true;
                array[8] = true;
                break;
            case 6:
                array[3] = true;
                array[7] = true;
                break;
            case 7:
                array[4] = true;
                array[6] = true;
                array[8] = true;
                break;
            case 8:
                array[5] = true;
                array[7] = true;
                break;
        }

        return array;
    }
}
