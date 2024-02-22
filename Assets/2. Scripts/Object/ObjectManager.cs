using UnityEngine;
using UnityEngine.UI;
using static Constants;
public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject friendPrefab;

    [SerializeField] private GameObject objectStatusParent;
    [SerializeField] private GameObject objectStatusPrefab;

    public GameObject Summons(ColorCheckCube cube, ObjectType objectType)
    {
        GameObject newObject;
        GameObject objectStatus = Instantiate(objectStatusPrefab);

        switch (objectType) {
            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                objectStatus.GetComponent<Image>().color = Color.blue;
                break;
            case ObjectType.FRIEND:
                newObject = Instantiate(friendPrefab);
                objectStatus.GetComponent<Image>().color = Color.green;
                break;
            default:
                newObject = null;
                break;
        }

        newObject.transform.parent = cube.colorPointCube.transform.GetChild(0);
        newObject.transform.position = cube.colorPointCube.transform.GetChild(0).position;
        newObject.transform.rotation = cube.colorPointCube.transform.GetChild(0).rotation;

        objectStatus.transform.SetParent(objectStatusParent.transform, false);
        newObject.GetComponent<Object>().objectStatus = objectStatus;

        return newObject;
    }
}
