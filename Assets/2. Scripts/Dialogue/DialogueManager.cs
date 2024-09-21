using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // Dialogue UI
    [Header("Dialogue UI")]
    private const int DIALOGUE_NORMAL = 0, DIALOGUE_POPUP = 1, DIALOGUE_LINE = 2;
    private int dialogueType = DIALOGUE_NORMAL;
    public GameObject[] dialogueSet;
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI[] scriptText;
    public Image backgroundImage;
    public Image characterImage;
    public Transform[] choicesContainer;
    public GameObject choicePrefab;
    private string imagePath = "ScriptImages/";

    [Header("teddyBearIcons")] public GameObject[] teddyBearIcons;
    [Header("Blocking Panels")] public Image[] blockingPanels;

    // 타자 효과 속도
    [Header("Typing Speed")]
    public float typeSpeed = 0.05f;

    // 자료 구조
    public Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    public Dictionary<string, Script> scripts = new Dictionary<string, Script>();
    private Dictionary<string, Choice> choices = new Dictionary<string, Choice>();
    private List<string> events = new List<string>();

    // 상태 변수
    private string currentDialogueID = "";
    public bool isDialogueActive = false;
    private bool isTyping = false;
    private string fullSentence;

    // Dialogue Queue
    private Queue<string> dialogueQueue = new Queue<string>();

    void Awake()
    {
        if (Instance == null)
        {
            DialoguesParser dialoguesParser = new DialoguesParser();
            dialogues = dialoguesParser.ParseDialogues();
            scripts = dialoguesParser.ParseScripts();
            choices = dialoguesParser.ParseChoices();
            events = dialoguesParser.ParseEvents();

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        dialogueSet[dialogueType].SetActive(false);
    }

    // ---------------------------------------------- Dialogue methods ----------------------------------------------
    public void StartDialogue(string dialogueID)
    {
        if (isDialogueActive)  // 이미 대화가 진행중이면 큐에 넣음
        {
            Debug.Log($"dialogue ID: {dialogueID} queued!");

            dialogueQueue.Enqueue(dialogueID);
            return;
        }

        isDialogueActive = true;

        dialogues[dialogueID].SetCurrentLineIndex(0);
        currentDialogueID = dialogueID;
        DialogueLine initialDialogueLine = dialogues[dialogueID].Lines[0];
        DisplayDialogueLine(initialDialogueLine);
    }

    private void DisplayDialogueLine(DialogueLine dialogueLine)
    {
        dialogueType = DIALOGUE_NORMAL;
        if (events.Contains(scripts[dialogueLine.ScriptID].ScriptEvent))
        {
            CallEvent(scripts[dialogueLine.ScriptID].ScriptEvent);
        }

        foreach (Transform child in choicesContainer[dialogueType])
        {
            Destroy(child.gameObject);
        }

        // 타자 효과 적용
        isTyping = true;
        var sentence = scripts[dialogueLine.ScriptID].KorScript;

        // 사용할 대화창을 제외한 다른 대화창을 꺼둔다
        foreach (GameObject canvas in dialogueSet) if (canvas != null) canvas.SetActive(false);
        dialogueSet[dialogueType].SetActive(true);

        StartCoroutine(TypeSentence(sentence));

        // 배경화면 표시
        var backgroundID = dialogueLine.BackgroundID;

        if (string.IsNullOrWhiteSpace(backgroundID)) backgroundImage.color = new Color(1, 1, 1, 0);
        else
        {
            var backgroundSprite = Resources.Load<Sprite>(imagePath + backgroundID);
            backgroundImage.sprite = backgroundSprite;
            backgroundImage.color = new Color(1, 1, 1, 1);
        }


        // 화자 이미지 표시
        var imageID = dialogueLine.ImageID;
        if (string.IsNullOrWhiteSpace(imageID)) characterImage.gameObject.SetActive(false);
        else
        {
            var characterSprite = Resources.Load<Sprite>(imagePath + imageID);

            characterImage.color = new Color(1, 1, 1, 1);
            characterImage.sprite = characterSprite;
            characterImage.SetNativeSize();
            characterImage.gameObject.SetActive(true);
        }
    }

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogueSet[dialogueType].SetActive(false);
            characterImage.gameObject.SetActive(false);
        if (dialogueQueue.Count > 0)  // 큐에 다이얼로그가 들어있으면 다시 대화 시작
        {
            string queuedDialogueID = dialogueQueue.Dequeue();
            StartDialogue(queuedDialogueID);

            return;
        }
    }

    // ---------------------------------------------- Script methods ----------------------------------------------
    private void ProceedToNext()
    {
        int currentDialogueLineIndex = dialogues[currentDialogueID].CurrentLineIndex;
        string next = dialogues[currentDialogueID].Lines[currentDialogueLineIndex].Next;

        if (events.Contains(next))  // Event인 경우
        {
            EndDialogue();
            CallEvent(next);
        }
        else if (dialogues.ContainsKey(next))  // Dialogue인 경우
        {
            EndDialogue();
            StartDialogue(next);
        }
        else if (string.IsNullOrWhiteSpace(next))  // 빈칸인 경우 다음 줄(대사)로 이동
        {
            currentDialogueLineIndex++;

            if (currentDialogueLineIndex >= dialogues[currentDialogueID].Lines.Count)
            {
                EndDialogue();  // 더 이상 DialogueLine이 존재하지 않으면 대화 종료
                return;
            }
            dialogues[currentDialogueID].SetCurrentLineIndex(currentDialogueLineIndex);
            DialogueLine nextDialogueLine = dialogues[currentDialogueID].Lines[currentDialogueLineIndex];
            DisplayDialogueLine(nextDialogueLine);
        }
        else if (choices.ContainsKey(next)) // Choice인 경우
        {
            DisplayChoices(next);
        }
    }
    IEnumerator TypeSentence(string sentence)
    {
        teddyBearIcons[dialogueType].SetActive(false);
        scriptText[dialogueType].text = "";
        fullSentence = sentence;

        // <color=red> 같은 글씨 효과들은 출력되지 않도록 변수 설정
        var isEffect = false;
        var effectText = "";

        foreach (char letter in sentence.ToCharArray())
        {
            if (letter == '<')
            {
                effectText = ""; // effectText 초기화
                isEffect = true;
            }
            else if (letter == '>') // > 가 나오면 scriptText에 한번에 붙인다
            {
                effectText += letter;
                scriptText[dialogueType].text += effectText;
                isEffect = false;
                continue;
            }

            if (isEffect) // < 가 나온 이후부터는 effectText에 붙인다
            {
                effectText += letter;
                continue;
            }

            scriptText[dialogueType].text += letter;
            // SoundPlayer.Instance.UISoundPlay(Sound_Typing); // 타자 소리 한번씩만
            yield return new WaitForSeconds(typeSpeed);
        }
        isTyping = false;
        teddyBearIcons[dialogueType].SetActive(true);
    }

    public void OnDialoguePanelClick()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            CompleteSentence();
        }
        else
        {
            ProceedToNext();
        }
    }

    private void CompleteSentence()
    {
        StopAllCoroutines();
        scriptText[dialogueType].text = fullSentence;
        isTyping = false;
        teddyBearIcons[dialogueType].SetActive(true);
    }

    // ---------------------------------------------- Choice methods ----------------------------------------------
    private void DisplayChoices(string choiceID)
    {
        blockingPanels[dialogueType].color = new Color(0, 0, 0, 0.7f);

        foreach (Transform child in choicesContainer[dialogueType])
        {
            Destroy(child.gameObject);
        }

        List<ChoiceLine> choiceLines = choices[choiceID].Lines;

        foreach (ChoiceLine choiceLine in choiceLines)
        {
            var choiceButton = Instantiate(choicePrefab, choicesContainer[dialogueType]).GetComponent<Button>();
            var choiceText = choiceButton.GetComponentInChildren<TextMeshProUGUI>();

            choiceText.text = scripts[choiceLine.ScriptID].KorScript;
            choiceButton.onClick.AddListener(() => OnChoiceSelected(choiceLine.Next));
        }
    }

    private void OnChoiceSelected(string next)
    {
        if (dialogues.ContainsKey(next))
        {
            EndDialogue();
            StartDialogue(next);
        }
        else if (events.Contains(next))
        {
            CallEvent(next);
        }

        foreach (Transform child in choicesContainer[dialogueType])
        {
            Destroy(child.gameObject);
        }

        blockingPanels[dialogueType].color = new Color(0, 0, 0, 0);
    }


    public void CallEvent(string eventID)
    {
        if (eventID.StartsWith("Event_Dialogue"))
        {
            StartDialogue(eventID.Substring(14));
        }
        else if (eventID.StartsWith("Dialogue_Canvas"))
        {
            switch (eventID.Substring(15))
            {
                case "Popup":
                    dialogueType = DIALOGUE_POPUP;
                    break;
                case "Line":
                    dialogueType = DIALOGUE_LINE;
                    break;
            }
        }
        else
        {
            switch (eventID)
            {
                case "Inventory_Click_1":
                    TutorialManager.Instance.Inventory_Click_1();
                    break;

                case "Inventory_Click_2":
                    TutorialManager.Instance.Inventory_Click_2();
                    break;

                case "Event_Bright":
                    ScreenEffect.Instance.Fade(1, 0, 1);
                    break;

                case "Event_TutorialStageStart":
                    StartCoroutine(TutorialManager.Instance.StartStage());
                    break;

                case "Event_Dark":
                    StartCoroutine(EventDark());
                    break;

                case "Event_Dark_002":
                    StartCoroutine(Event_Dark_002());
                    break;

                case "Cube_Mix":
                    StartCoroutine(TutorialManager.Instance.CubeMix());
                    break;
            }
        }
    }

    private IEnumerator EventDark()
    {
        ScreenEffect.Instance.Fade(0, 1, 1);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(0);
    }
    private IEnumerator Event_Dark_002()
    {
        ScreenEffect.Instance.Fade(0, 1, 1);
        yield return new WaitForSeconds(1);
        StartDialogue("Dialogue_002");
    }
}
