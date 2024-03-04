using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class Object : MonoBehaviour
{
    public ObjectManager objectManager;
    public Weapon weapon;
    public Slider hpSlider;

    private ColorCheckCube position; // 일단은 보이게...
    [SerializeField] private ObjectType type;
    public ObjectType Type { get => type; }

    [SerializeField] private float hp; // 일단은 object가 플레이어와 적 밖에 없다고 가정 -> 모두 hp를 가짐
    public float HP
    {
        get => hp;
        private set
        {
            hp = value;
            if (hpSlider != null) // 보물상자나 상인은 이게 없음
                hpSlider.value = hp / weapon.MaxHP;
        }
    }
    private void Start()
    {
        hp = weapon.MaxHP;
        HP = hp;
        GetComponent<MeshRenderer>().material = weapon.ObjectMaterial;
    }
    //private void FixedUpdate()
    //{
    //    Debug.DrawRay(transform.position, transform.up * height, Color.red, 1f);
    //}
    public ColorCheckCube GetPosition()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.up, 30);
        foreach (RaycastHit hit in hits)
        {
            ColorCheckCube touchComponent = hit.collider.gameObject.GetComponent<ColorCheckCube>();
            if (touchComponent != null)
            {
                position = touchComponent;
                break;
            }
        }
        return position;
    }

    public WeaponType GetWeaponType()
    {
        return weapon.WeaponType;
    }
    public int GetDamage()
    {
        return weapon.WeaponDamage;
    }

    public void OnHit(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            if (hpSlider != null) hpSlider.gameObject.SetActive(false);
            objectManager.ObjectDie(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void ChangeWeapon(Weapon newWeapon)
    {
        if (type == ObjectType.PLAYER) // 플레이어만 무기를 바꿀 수 있음
            weapon = newWeapon;
    }
}
