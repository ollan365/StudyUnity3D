using UnityEngine;

public class Object : MonoBehaviour
{
    [SerializeField] private Touch position; // 일단은 보이게...
    [SerializeField] private float height;

    public Touch GetPosition()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, height))
        {
            if (position == null || position != hit.collider.gameObject.GetComponent<Touch>())
                position = hit.collider.gameObject.GetComponent<Touch>();
        }

        return position;
    }
}
