using UnityEngine;

[CreateAssetMenu(fileName = "New Portion", menuName = "Items/ETC", order = 2)]
public class ETC : ItemObject
{
    [SerializeField] private int value;
    [SerializeField] private Material objectMaterial; // 오브젝트의 이미지 등을 담는다 지금은 Material
    public Material ObjectMaterial { get => objectMaterial; }

}
