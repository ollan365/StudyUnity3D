using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static Constants;
using static Excel;
using DG.Tweening;
using TMPro;

public class Object : MonoBehaviour
{
    public ObjectManager objectManager;
    public Slider bottomHP;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private GameObject popTextObj;
    public GameObject PopTextObject { get => popTextObj; }
    private TMP_Text popText;

    [SerializeField] private int id;
    [SerializeField] private string objName;
    [SerializeField] private int minDamage;
    [SerializeField] private int maxDamage;
    [SerializeField] private ObjectType type;
    [SerializeField] private WeaponType weaponType;
    [SerializeField] private GameObject indicator;
    [SerializeField] private float textFontSize;

    public Touch touchCube;
    private Sequence sequence;
    private RectTransform rectTransform;
    

    public int ID { get => id; }
    public string Name { get => objName; }
    public int MinDamage { get => minDamage; }
    public int MaxDamage { get => maxDamage; }
    public ObjectType Type { get => type; }
    public WeaponType AttackType { get => weaponType; }
    public GameObject Indicator { get => indicator; }

    public void SetWeapon(int min, int max, WeaponType weaponType)
    {
        minDamage = min;
        maxDamage = max;
        this.weaponType = weaponType;
    }
    public float Damage { get => Random.Range(minDamage, maxDamage + 1) * EventManager.Instance.Effect.Dealt(this); }

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

    private void Awake()
    {
        if (popTextObj != null)
        {
            popText = popTextObj.GetComponent<TMP_Text>();
            rectTransform = popTextObj.GetComponent<RectTransform>();
        }
    }

    public void Init(ObjectType objType, string[] datas, Touch touchCube)
    {
        type = objType;

        switch (objType)
        {
            case ObjectType.PLAYER:
                maxHp = int.Parse(datas[0]);
                break;
            case ObjectType.ENEMY:
                id = int.Parse(datas[ENEMY_ID]);
                name = datas[ENEMY_NAME];
                maxHp = int.Parse(datas[ENEMY_HP]);
                SetWeapon(int.Parse(datas[ENEMY_MIN]), int.Parse(datas[ENEMY_MAX]), datas[ENEMY_WEAPON_TYPE].ToEnum());
                break;
            case ObjectType.FRIEND:
                id = int.Parse(datas[FRIEND_ID]);
                maxHp = int.Parse(datas[FRIEND_HP]);
                SetWeapon(int.Parse(datas[FRIEND_MIN]), int.Parse(datas[FRIEND_MAX]), datas[FRIEND_WEAPON_TYPE].ToEnum());
                break;
            case ObjectType.TRIGGER:
                name = datas[0];
                break;
        }

        hp = maxHp;
        HP = hp;

        this.touchCube = touchCube;
        Debug.Log($"{this.touchCube} / {touchCube}");

    }

    public void OnHit(StatusEffect effect, float damage)
    {
        float dmg = 0f; //데미지를 텍스트로 출력하기 위해 필요한 변수

        //효과 별 데미지 설정
        if (effect == StatusEffect.HP)
            dmg = damage * EventManager.Instance.Effect.Received(this);
        else if (effect == StatusEffect.HP_PERCENT)
            dmg = maxHp * damage / 100 * EventManager.Instance.Effect.Received(this);

        //데미지를 반올림 한 후, HP에 반영한다.
        dmg = Mathf.Round(dmg);
        HP -= dmg;
        if (HP <= 0)
            StartCoroutine(Death(HP, gameObject));

        //플레이어, 적, 동료 이고, 살아있다면 데미지 출력
        if (popTextObj != null && gameObject.activeSelf)
        {
            string text;
            if (dmg > 0)
                text = $"-{Mathf.Abs(dmg)}";
            else if(dmg < 0)
                text = $"+{Mathf.Abs(dmg)}";
            else
                text = $"{Mathf.Abs(dmg)}";
            DamageText(text);
        }
            

    }

    //death 기능을 코루틴으로 설정하고, 1초 딜레이 시켜, 데미지 이펙트가 모두 나오고 오브젝트 비활성화
    public IEnumerator Death(float HP, GameObject gameObject)
    {
        if (type == ObjectType.PLAYER) StageManager.Instance.GameOver();
        if (type == ObjectType.ENEMY)
        {
            StageManager.Instance.SetStageTextValue(StageText.MONSTER, -1);
            StageManager.Instance.CheckStageClear();
        }
        yield return new WaitForSeconds(1.0f);
        objectManager.ObjectDie(gameObject);
        gameObject.SetActive(false);
        if (type == ObjectType.ENEMY) 
            StageManager.Instance.SetStageTextValue(StageText.MONSTER, -1);
    }

    public void PoppingText(string text, Color color)
    {
        //기본적인 텍스트 띄우기
        //set value
        popText.text = text;
        popText.color = color;

        rectTransform.localPosition = Vector3.zero;

        sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMoveY(0.5f, 1.0f)).SetEase(Ease.OutCubic)
                .Join(popText.DOFade(1.0f, 1.0f))
                .Append(rectTransform.DOLocalMoveY(1f, 1.0f)).SetEase(Ease.Linear)
                .Join(popText.DOFade(0.0f, 1.0f));

        rectTransform.localPosition = Vector3.zero;

    }

    private void DamageText(string text, bool isCritical = false)
    {
        //초기 값 설정
        popText.text = text;
        popText.fontSize = 25;

        if (isCritical)
        {
            popText.color = UnityEngine.Color.red;
        }

        sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DOLocalMoveY(0.5f, 0.1f)).SetDelay(0.15f)
                .Append(popText.DOFontSize(textFontSize * 1.5f, 0.5f))
                .Join(popText.DOFade(1.0f, 0.5f))
                .Append(popText.DOFontSize(textFontSize, 0.5f))
                .Append(popText.DOFade(0.0f, 0.5f))
                .Append(rectTransform.DOLocalMoveY(0.0f, 0.1f));

        popText.fontSize = textFontSize;
        rectTransform.localPosition = Vector3.zero;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (StageManager.Instance.StatusOfStage != StageStatus.FIGHT && other.GetComponent<Touch>() != null) touchCube = other.GetComponent<Touch>();
    }

}
