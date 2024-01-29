using UnityEngine;
using static Constants;

public class ColorCheckCube : MonoBehaviour
{
    [SerializeField] private Colors color;
    private bool isColorMatch;
    public bool IsColorMatch { get => isColorMatch; }
    private void Awake()
    {
        isColorMatch = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        isColorMatch = other.gameObject.layer == gameObject.layer;
    }
}
