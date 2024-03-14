using UnityEngine;

public class StageCube : MonoBehaviour
{
    public static StageCube Instance { get; private set; }

    [SerializeField] private GameObject[] whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray;
    [SerializeField] private GameObject[] wyArray, roArray, bgArray;
    public GameObject[][] colorArray;

    [SerializeField] private GameObject[] whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray;
    public GameObject[][] coverArray;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            colorArray = new GameObject[][] { whiteArray, redArray, blueArray, greenArray, orangeArray, yellowArray, wyArray, roArray, bgArray };
            coverArray = new GameObject[][] { whiteCoverArray, redCoverArray, blueCoverArray, greenCoverArray, orangeCoverArray, yellowCoverArray };
        }
        else
            Destroy(gameObject);
    }

}
