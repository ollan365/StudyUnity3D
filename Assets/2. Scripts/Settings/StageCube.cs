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

}
