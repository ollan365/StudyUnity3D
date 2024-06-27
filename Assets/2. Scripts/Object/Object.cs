using UnityEngine;
using UnityEngine.UI;
using static Constants;
using static Excel;

public class Object : MonoBehaviour
{
    public ObjectManager objectManager;
    public Slider bottomHP;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject goldText;
    public GameObject GoldText { get => goldText; }

    [SerializeField] private int id;
    [SerializeField] private string objName;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private ObjectType type;
    [SerializeField] private WeaponType weaponType;

    public EventEffect eventEffect;

    public Touch touchCube;
    
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

    [SerializeField] private float maxHp;
    [SerializeField] private float hp;
    public float HP
    {
        get => hp;
        private set
        {
            hp = Mathf.Clamp(value, 0, maxHp);

            if (hpSlider != null) hpSlider.value = hp / maxHp;
            if (bottomHP != null) bottomHP.value = hp / maxHp;
        }
    }
    public Colors Color { get => touchCube.Color; }
    public int Index { get => touchCube.Index; }
    public void Init(ObjectType objType, string[] datas, Touch touchCube)
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

        hp = maxHp;
        HP = hp;

        this.touchCube = touchCube;
        Debug.Log($"{this.touchCube} / {touchCube}");
    }
    public void OnHit(StatusEffect effect, int damage)
    {
        if (effect == StatusEffect.HP) HP -= damage;
        else if (effect == StatusEffect.HP_PERCENT) HP -= MAX_HP * damage / 100;

        if (HP <= 0)
        {
            objectManager.ObjectDie(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void RotateCube()
    {
        StartCoroutine(StageManager.Instance.CubeRotate(Color));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Touch>() != null) touchCube = other.GetComponent<Touch>();
    }
}
