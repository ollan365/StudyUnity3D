using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;
using static Excel;
using DG.Tweening;

public class Object : MonoBehaviour
{
    public ObjectManager objectManager;
    public Slider bottomHP;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject popTextObj;
    public GameObject PopTextObject { get => popTextObj; }
    private Text popText;

    [SerializeField] private int id;
    [SerializeField] private string objName;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private ObjectType type;
    [SerializeField] private WeaponType weaponType;

    public EventEffect eventEffect;

    public Touch touchCube;

    private Sequence sequence;


    public int ID { get => id; }
    public string Name { get => objName; }
    public int MinDamage { get => minDamage; }
    public int MaxDamage { get => maxDamage; }
    public ObjectType Type { get => type; }
    public WeaponType AttackType { get => weaponType; }

    public void SetWeapon(int min, int max, WeaponType weaponType)
    {
        minDamage = min;
        maxDamage = max;
        this.weaponType = weaponType;
    }
    public float Damage { get => Random.Range(minDamage, maxDamage + 1) * eventEffect.Dealt(); }


    [SerializeField] private float maxHp;
    [SerializeField] private float hp;
    public float MaxHp { get => maxHp; }
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

    private void Start()
    {
        sequence = DOTween.Sequence();
    }

    public void OnHit(StatusEffect effect, float damage)
    {
        float dmg = 0f; //�������� �ؽ�Ʈ�� ����ϱ� ���� �ʿ��� ����

        //ȿ�� �� ������ ����
        if (effect == StatusEffect.HP)
            dmg = damage * eventEffect.Received();
        else if (effect == StatusEffect.HP_PERCENT)
            dmg = maxHp * damage / 100 * eventEffect.Received();

        //������ ���
        HP -= dmg;

        //�÷��̾�, ��, ���� �̰�, ����ִٸ� ������ ���
        if (popTextObj != null && gameObject.activeSelf)
            StartCoroutine(PoppingText($"-{dmg}", new Color(0, 0, 1, 1)));

        if(HP <= 0)
            StartCoroutine(Death(HP, gameObject));
    }

    //death ����� �ڷ�ƾ���� �����ϰ�, 1�� ������ ����, ������ ����Ʈ�� ��� ������ ������Ʈ ��Ȱ��ȭ
    public IEnumerator Death(float HP, GameObject gameObject)
    {
        yield return new WaitForSeconds(1.0f);
        objectManager.ObjectDie(gameObject);
        gameObject.SetActive(false);
    }

    public IEnumerator PoppingText(string text, Color color)
    {
        //set value
        popText = popTextObj.GetComponent<Text>();
        popText.text = text;
        Color textColor = color;

        RectTransform rectTransform = popTextObj.GetComponent<RectTransform>();
        rectTransform.localPosition = Vector3.zero;

        textColor.a = 1;
        sequence.Append(rectTransform.DOLocalMoveY(0.5f, 1.0f)).SetEase(Ease.OutCubic)
                .Join(popText.DOColor(textColor, 1.0f));

        yield return new WaitForSeconds(1.0f);

        textColor.a = 0;
        sequence.Append(rectTransform.DOLocalMoveY(1f, 1.0f)).SetEase(Ease.Linear)
                .Join(popText.DOColor(textColor, 1.0f));

        yield return new WaitForFixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Touch>() != null) touchCube = other.GetComponent<Touch>();
    }




}
