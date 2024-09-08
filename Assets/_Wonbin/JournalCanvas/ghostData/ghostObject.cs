using UnityEngine;

[CreateAssetMenu(fileName = "NewGhostData", menuName = "Ghost Object", order = 51)]
public class ghostObject : ScriptableObject
{
    public string ghostName;         // �ͽ��� �̸�
    [TextArea] public string description;  // �ͽ��� ����
    public string relatedItem;       // Ư�� �� �� �ִ� ������
}