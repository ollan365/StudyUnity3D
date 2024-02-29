using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
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

    [SerializeField] private Object player;
    [SerializeField] private GameObject[] inventorySlotButton;
    [SerializeField] private List<ItemObject> inventoryList;
    [SerializeField] private CubeManager cubeManager;
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

    public void OpenTreasureBox(GameObject obj)
    {
        Debug.Log("Open treasure box!");
        obj.transform.position = dieObject.position;
        obj.transform.parent = dieObject;
    }

    public void ChangePlayerInventory()
    {
        for (int i = 0; i < inventoryList.Count; i++)
            inventorySlotButton[i].SetActive(true);
    }
    public void ClickInventoryBTN(int index)
    {
        switch (inventoryList[index].itemType)
        {
            case ItemType.WEAPON:
                if (cubeManager.CanChangeWeapon()) player.ChangeWeapon((Weapon)inventoryList[index]);
                break;
            case ItemType.PORTION:
                break;
            case ItemType.ETC:
                break;
        }
    }
}
