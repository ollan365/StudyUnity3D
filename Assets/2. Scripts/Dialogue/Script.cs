public class Script
{
    public string ScriptID { get; private set; }
    public string KorScript { get; private set; }
    public string ScriptEvent { get; private set; }

    public Script(string id, string kor, string placeholder)
    {
        ScriptID = id;
        KorScript = kor;
        ScriptEvent = placeholder;
    }
}