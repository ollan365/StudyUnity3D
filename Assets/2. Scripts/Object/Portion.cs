using UnityEngine;

[CreateAssetMenu(fileName = "New Portion", menuName = "Items/Portion", order = 1)]
public class Portion : ItemObject
{
    [SerializeField] private int value;
    [SerializeField] private Material objectMaterial; // 오브젝트의 이미지 등을 담는다 지금은 Material
    public Material ObjectMaterial { get => objectMaterial; }

}
