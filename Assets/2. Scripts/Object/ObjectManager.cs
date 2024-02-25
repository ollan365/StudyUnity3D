using UnityEngine;
using UnityEngine.UI;
using static Constants;
public class ObjectManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject friendPrefab;
    [SerializeField] private GameObject treasurePrefab;
    [SerializeField] private GameObject merchantPrefab;
    [SerializeField] private GameObject portalPrefab;

    [SerializeField] private GameObject objectStatusParent;
    [SerializeField] private GameObject objectStatusPrefab;

    [SerializeField] private Transform dieObject;

    [SerializeField] private GameObject merchantInventory;
    [SerializeField] private GameObject playerInventory;

    public GameObject Summons(ColorCheckCube cube, ObjectType objectType)
    {
        GameObject newObject;
        GameObject objectStatus;

        switch (objectType) {
            case ObjectType.ENEMY:
                newObject = Instantiate(enemyPrefab);
                objectStatus = Instantiate(objectStatusPrefab);

                objectStatus.GetComponent<Image>().color = Color.blue;
                objectStatus.transform.SetParent(objectStatusParent.transform, false);
                newObject.GetComponent<Object>().objectStatus = objectStatus;
                break;
            case ObjectType.FRIEND:
                newObject = Instantiate(friendPrefab);
                objectStatus = Instantiate(objectStatusPrefab);

                objectStatus.GetComponent<Image>().color = Color.green;
                objectStatus.transform.SetParent(objectStatusParent.transform, false);
                newObject.GetComponent<Object>().objectStatus = objectStatus;
                break;
            case ObjectType.TREASURE:
                newObject = Instantiate(treasurePrefab);
                break;
            case ObjectType.MERCHANT:
                newObject = Instantiate(merchantPrefab);
                break;
            case ObjectType.PORTAL:
                newObject = Instantiate(portalPrefab);
                break;
            default:
                newObject = null;
                break;
        }

        newObject.transform.parent = cube.colorPointCube.transform.GetChild(0);
        newObject.transform.position = cube.colorPointCube.transform.GetChild(0).position;
        newObject.transform.rotation = cube.colorPointCube.transform.GetChild(0).rotation;

        newObject.GetComponent<Object>().objectManager = this;

        return newObject;
    }

    public void ObjectDie(GameObject obj)
    {
        // ���� �ֵ��� ���� �� ���� ���߱⸦ ���ؼ� �ϴ��� �ٸ� ���� �Űܵд�
        // ���߿� ���� �ý��۸� ��° �ٲٸ� �׳� destroy �ص� �ɵ�
        obj.transform.position = dieObject.position;
        obj.transform.parent = dieObject;
    }

    public void OpenMerchantInventory()
    {
        merchantInventory.SetActive(true);
    }
    public void OpenTreasureBox()
    {
        Debug.Log("Open treasure box!");
    }
}
