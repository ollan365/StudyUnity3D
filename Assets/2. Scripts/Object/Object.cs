using UnityEngine;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class Object : MonoBehaviour
{
    public ObjectManager objectManager;
    public Slider hpSlider;

    private int id;
    private string objName;
    private int minDamage;
    private int maxDamage;
    private ObjectType type;
    private WeaponType weaponType;

    public void init(ObjectType objType, string[] datas)
    {
        type = objType;
        if(objType == ObjectType.PLAYER)
        {
            maxHp = int.Parse(datas[0]);
        }
        else if(objType == ObjectType.ENEMY)
        {
            id = int.Parse(datas[ENEMY_ID]);
            name = datas[ENEMY_NAME];
            maxHp = int.Parse(datas[ENEMY_HP]);
            SetWeapon(int.Parse(datas[ENEMY_MIN]), int.Parse(datas[ENEMY_MAX]), datas[ENEMY_WEAPON_TYPE].ToEnum());
        }
        else if(objType == ObjectType.FRIEND)
        {
            id = int.Parse(datas[FRIEND_ID]);
            maxHp = int.Parse(datas[FRIEND_HP]);
            SetWeapon(int.Parse(datas[FRIEND_MIN]), int.Parse(datas[FRIEND_MAX]), datas[FRIEND_WEAPON_TYPE].ToEnum());
        }

        hp = maxHp;
        HP = hp;
    }
    public int ID { get => id; }
    public string Name { get => objName; }
    public void SetWeapon(int min, int max, WeaponType weaponType)
    {
        minDamage = min;
        maxDamage = max;
        this.weaponType = weaponType;
    }
    public int Damage { get => Random.Range(minDamage, maxDamage + 1); }
    public ObjectType Type { get => type; }
    public WeaponType AttackType { get => weaponType; }

    private float maxHp;
    private float hp;
    public float HP
    {
        get => hp;
        set
        {
            hp = Mathf.Clamp(value, 0, maxHp);
            if (hpSlider != null) // 보물상자나 상인은 이게 없음
                hpSlider.value = hp / maxHp;
        }
    }
    
    public Colors Color { get => GetPosition().Color; }
    public int Index { get => GetPosition().Index; }
    private ColorCheckCube GetPosition()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.up, 30);
        foreach (RaycastHit hit in hits)
        {
            ColorCheckCube touchComponent = hit.collider.gameObject.GetComponent<ColorCheckCube>();
            if (touchComponent != null)
                return touchComponent;
        }
        return null;
    }

    public void OnHit(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            objectManager.ObjectDie(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void HP_Percent(int percent)
    {
        if(percent < 0)
            HP -= maxHp * percent / 100;
        else
            HP += maxHp * percent / 100;

        Debug.Log($"{HP}");
    }
}
