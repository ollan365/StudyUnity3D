using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguesParser : MonoBehaviour
{
    // CSV 파일
    private TextAsset dialoguesCSV = Resources.Load<TextAsset>("CSV/Dialogues");
    private TextAsset scriptsCSV = Resources.Load<TextAsset>("CSV/Scripts");
    private TextAsset choicesCSV = Resources.Load<TextAsset>("CSV/Choices");
    private TextAsset eventsCSV = Resources.Load<TextAsset>("CSV/Events");

    // 자료 구조
    private Dictionary<string, Dialogue> dialogues = new Dictionary<string, Dialogue>();
    private Dictionary<string, Script> scripts = new Dictionary<string, Script>();
    private Dictionary<string, Choice> choices = new Dictionary<string, Choice>();
    private List<string> events = new List<string>();

    private string Escaper(string originalString)
    {
        string modifiedString = originalString.Replace("\\n", "\n");
        modifiedString = modifiedString.Replace("`", ",");
        modifiedString = modifiedString.Replace("+", ",");

        return modifiedString;
    }

    public Dictionary<string, Dialogue> ParseDialogues()
    {
        string[] lines = dialoguesCSV.text.Split('\n');

        string lastDialogueID = "";

        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');

            if ((string.IsNullOrWhiteSpace(lines[i])) || (fields[0] == "" && fields[1] == "")) continue;

            string dialogueID = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(dialogueID)) dialogueID = lastDialogueID;
            else lastDialogueID = dialogueID;

            string speakerID = fields[1].Trim();
            string scriptID = fields[2].Trim();
            string imageID = fields[3].Trim();
            string backgroundID = fields[4].Trim();
            string next = fields[5].Trim();

            if (!dialogues.ContainsKey(dialogueID))
            {
                Dialogue dialogue = new Dialogue(dialogueID);
                dialogues[dialogueID] = dialogue;
            }

            dialogues[dialogueID].AddLine(speakerID, scriptID, imageID, backgroundID, next);
        }

        return dialogues;
    }

    public Dictionary<string, Script> ParseScripts()
    {
        string[] lines = scriptsCSV.text.Split("EOL");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string scriptID = fields[0].Trim().Trim('\n');
            string korScript = Escaper(fields[1].Trim());
            string scriptEvent = Escaper(fields[2].Trim());

            Script script = new Script(
                scriptID,
                korScript,
                scriptEvent
            );
            scripts[scriptID] = script;
        }

        return scripts;
    }

    public Dictionary<string, Choice> ParseChoices()
    {
        string[] lines = choicesCSV.text.Split('\n');

        string lastChoiceID = "";

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string[] fields = lines[i].Split(',');

            string choiceID = fields[0].Trim();
            if (string.IsNullOrWhiteSpace(choiceID)) choiceID = lastChoiceID;
            else lastChoiceID = choiceID;

            string scriptID = fields[1].Trim();
            string next = fields[2].Trim();

            if (!choices.ContainsKey(choiceID))
            {
                choices[choiceID] = new Choice(choiceID);
            }

            choices[choiceID].AddLine(scriptID, next);
        }

        return choices;
    }
    public List<string> ParseEvents()
    {
        string[] lines = eventsCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) continue;

            string eventID = lines[i].Split(',')[0].Trim();

            if (!events.Contains(eventID))
            {
                events.Add(eventID);
            }
        }

        return events;
    }
}
