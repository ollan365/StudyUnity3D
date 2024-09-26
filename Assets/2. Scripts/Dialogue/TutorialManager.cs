using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Constants;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }
    [SerializeField] private CubeManager cubeManager;
    [SerializeField] private GameObject[] steps;
    public bool IsTutorial { set; get; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        IsTutorial = false;

        return;

        if (StaticManager.Instance.Stage == 1)
        {
            IsTutorial = true;
            DialogueManager.Instance.StartDialogue("Prologue_001");
        }
    }
    public IEnumerator StartStage()
    {
        // 화면 밝아짐
        ScreenEffect.Instance.Fade(1, 0, 1);
        yield return new WaitForSeconds(2f);

        //포탈에서 플레이어 생성
        GameObject portal = ObjectManager.Instance.Summons(StageCube.Instance.touchArray[WHITE][3], ObjectType.PORTAL, 0);
        ObjectEffect.Instance.MakeBig(portal);
        yield return new WaitForSeconds(1f);
        ObjectManager.Instance.Summons(StageCube.Instance.touchArray[WHITE][3], ObjectType.PLAYER, 0);
        yield return new WaitForSeconds(1f);
        StageManager.Instance.Player.gameObject.SetActive(true);
        ColorCheckManager.Instance.CharacterSelect(StageManager.Instance.Player.gameObject);
        StartCoroutine(ColorCheckManager.Instance.MoveCoroutine(WHITE, 4, true));
        yield return new WaitForSeconds(1f);
        ColorCheckManager.Instance.CharacterSelectCancel(null, true);
        ObjectEffect.Instance.MakeSmall(portal);
        yield return new WaitForSeconds(2f);

        // 대화 시작
        DialogueManager.Instance.StartDialogue("Prologue_002");

    }
    public IEnumerator CubeMix()
    {
        // 큐브를 섞는다
        yield return new WaitForSeconds(5);

        // 플레이어 쪽으로 회전
        StartCoroutine(StageManager.Instance.CubeRotate(StageManager.Instance.Player.Color));
        yield return new WaitForSeconds(1.2f);

        // 적 소환 (1. 소환할 큐브 블록을 미리 정함 -> 2. 파티클 재생 -> 3. 적 소환)
        StageManager.Instance.SummonStageEnemy();
        yield return new WaitForSeconds(1.2f);

        // UI 및 체력바 활성화
        yield return new WaitForSeconds(1f);

        // 대화 시작
        DialogueManager.Instance.StartDialogue("Tutorial_004");
    }
    public void Inventory_Click_1()
    {
        steps[0].SetActive(true);
    }
    public void Inventory_Click_2()
    {
        steps[1].SetActive(true);
    }
    public void OnClick_Step_0()
    {
        DialogueManager.Instance.StartDialogue("Tutorial_002");
    }
    public void OnClick_Step_1()
    {
        DialogueManager.Instance.StartDialogue("Tutorial_003");
    }
}
