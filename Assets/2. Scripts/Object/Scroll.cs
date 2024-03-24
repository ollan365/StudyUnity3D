using UnityEngine;

[CreateAssetMenu(fileName = "New Scroll", menuName = "Items/Scroll", order = 2)]
public class Scroll : ItemObject
{
    [SerializeField] private int friendIndex;
    public int FriendIndex { get => friendIndex; }
}
