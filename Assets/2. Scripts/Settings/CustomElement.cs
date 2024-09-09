[System.Serializable]
public class CustomElement
{
    public string displayName;  // 인스펙터에 표시할 텍스트
    public int value;

    public CustomElement(string name, int value)
    {
        displayName = name;
        this.value = value;
    }
}
