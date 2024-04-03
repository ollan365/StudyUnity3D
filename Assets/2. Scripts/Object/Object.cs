using UnityEngine;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class Object : MonoBehaviour
{
    public ObjectManager objectManager;
    public Slider bottomHP;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Text goldText;

    private int id;
    private string objName;
    private int minDamage;
    private int maxDamage;
    private ObjectType type;
    private WeaponType weaponType;

    public void Init(ObjectType objType, string[] datas)
    {
        type = objType;
        if (objType == ObjectType.PLAYER)
        {
            maxHp = int.Parse(datas[0]);
        }
        else if (objType == ObjectType.ENEMY)
        {
            id = int.Parse(datas[ENEMY_ID]);
            name = datas[ENEMY_NAME];
            maxHp = int.Parse(datas[ENEMY_HP]);
            SetWeapon(int.Parse(datas[ENEMY_MIN]), int.Parse(datas[ENEMY_MAX]), datas[ENEMY_WEAPON_TYPE].ToEnum());
        }
        else if (objType == ObjectType.FRIEND)
        {
            id = int.Parse(datas[FRIEND_ID]);
            maxHp = int.Parse(datas[FRIEND_HP]);
            SetWeapon(int.Parse(datas[FRIEND_MIN]), int.Parse(datas[FRIEND_MAX]), datas[FRIEND_WEAPON_TYPE].ToEnum());
        }
        else
            type = objType;

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
            if (hpSlider != null)
                hpSlider.value = hp / maxHp;
        }
    }
    public Colors Color { get => GetPosition().Color; }
    public int Index { get => GetPosition().Index; }
    private Touch GetPosition()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, -transform.up, 10);
        foreach (RaycastHit hit in hits)
        {
            Touch touchComponent = hit.collider.gameObject.GetComponent<Touch>();
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
