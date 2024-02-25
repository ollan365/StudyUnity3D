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
    [SerializeField] private Weapon[] allPlayerWeapons;
    [SerializeField] private GameObject[] inventorySlotButton;
    [SerializeField] private List<Weapon> inventoryWeaponList;
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
        // 죽은 애들은 현재 그 개수 맞추기를 위해서 일단은 다른 곳에 옮겨둔다
        // 나중에 공격 시스템만 어째 바꾸면 그냥 destroy 해도 될듯
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
        for (int i = 0; i < inventoryWeaponList.Count; i++)
            inventorySlotButton[i].SetActive(true);
    }
    public void ChangePlayerWeapon(int index)
    {
        if (cubeManager.CanChangeWeapon())
            player.ChangeWeapon(inventoryWeaponList[index]);
    }
}
