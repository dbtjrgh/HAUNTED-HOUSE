using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewGhostData", menuName = "Ghost Object", order = 51)]
public class ghostObject : ScriptableObject
{
    public string ghostName;         // 귀신의 이름
    [TextArea] public string description;  // 귀신의 설명
    public string relatedItem;       // 특정 할 수 있는 아이템
    public Sprite ghostImage;         // 귀신의 이미지
}