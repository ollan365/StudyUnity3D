using UnityEngine;

public class Object : MonoBehaviour
{
    public Touch position; // ¿œ¥‹¿∫ public
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Touch>() != null)
        {
            position = other.GetComponent<Touch>();
        }
    }
    public Touch GetPosition()
    {
        return position;
    }
}
