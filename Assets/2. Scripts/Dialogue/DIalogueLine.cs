public class DialogueLine
{
    public string SpeakerID { get; private set; }
    public string ScriptID { get; private set; }
    public string ImageID { get; private set; }
    public string BackgroundID { get; private set; }
    public string Next { get; private set; }

    // initialize function
    public DialogueLine(string speakerID, string scriptID, string imageID, string backgroundID, string next)
    {
        SpeakerID = speakerID;
        ScriptID = scriptID;
        ImageID = imageID;
        BackgroundID = backgroundID;
        Next = next;
    }
}