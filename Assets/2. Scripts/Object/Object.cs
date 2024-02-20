using UnityEngine;
using static Constants;

public class Object : MonoBehaviour
{
    [SerializeField] private ColorCheckCube position; // �ϴ��� ���̰�...
    [SerializeField] private float height;
    [SerializeField] private ObjectType type;
    public ObjectType Type { get => type; }

    [SerializeField] private int hp; // �ϴ��� object�� �÷��̾�� �� �ۿ� ���ٰ� ���� -> ��� hp�� ����
    [SerializeField] private Weapon weapon; // �ϴ��� object�� �÷��̾�� �� �ۿ� ���ٰ� ���� -> ��� ���� ������ ����
    
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
