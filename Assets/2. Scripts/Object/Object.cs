using UnityEngine;
using static Constants;

public class Object : MonoBehaviour
{
    [SerializeField] private ColorCheckCube position; // 일단은 보이게...
    [SerializeField] private float height;
    [SerializeField] private ObjectType type;
    public ObjectType Type { get => type; }

    [SerializeField] private int hp; // 일단은 object가 플레이어와 적 밖에 없다고 가정 -> 모두 hp를 가짐
    [SerializeField] private Weapon weapon; // 일단은 object가 플레이어와 적 밖에 없다고 가정 -> 모두 공격 패턴을 가짐
    
    public ColorCheckCube GetPosition()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.up, height * 10);

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
        hp -= damage;
        if (hp <= 0) gameObject.SetActive(false);
        else Debug.Log($"hp: {hp}");
    }
}
