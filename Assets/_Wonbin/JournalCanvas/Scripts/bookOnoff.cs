using UnityEngine;

public class bookOnoff : MonoBehaviour
{
    private bool onOff = false; // �ʱ� ���¸� false�� ����
    private journalBook journal; // journalBook �ν��Ͻ� ����

    private void Start()
    {
        // "journal" �±׸� ���� GameObject���� journalBook ������Ʈ�� ã���ϴ�.
        GameObject playerBook = GameObject.FindWithTag("journal");
        if (playerBook != null)
        {
            journal = playerBook.GetComponent<journalBook>();
        }
    }

    private void Update()
    {
        // "J" Ű�� ���� �� journalBook�� Ȱ��ȭ ���¸� ����մϴ�.
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    private void ToggleJournal()
    {
        if (journal != null)
        {
            onOff = !onOff; // ���� ���
            journal.gameObject.SetActive(onOff); // journalBook�� Ȱ��ȭ ���� ����
            Debug.Log(onOff ? "å Ȱ��ȭ" : "å ��Ȱ��ȭ");
        }
    }
}