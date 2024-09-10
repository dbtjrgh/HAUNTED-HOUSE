using UnityEngine;

public class bookOnoff : MonoBehaviour
{
    private bool onOff = false; // 초기 상태를 false로 설정
    private journalBook journal; // journalBook 인스턴스 변수

    private void Start()
    {
        // "journal" 태그를 가진 GameObject에서 journalBook 컴포넌트를 찾습니다.
        GameObject playerBook = GameObject.FindWithTag("journal");
        if (playerBook != null)
        {
            journal = playerBook.GetComponent<journalBook>();
        }
    }

    private void Update()
    {
        // "J" 키가 눌릴 때 journalBook의 활성화 상태를 토글합니다.
        if (Input.GetKeyDown(KeyCode.J))
        {
            ToggleJournal();
        }
    }

    private void ToggleJournal()
    {
        if (journal != null)
        {
            onOff = !onOff; // 상태 토글
            journal.gameObject.SetActive(onOff); // journalBook의 활성화 상태 설정
            Debug.Log(onOff ? "책 활성화" : "책 비활성화");
        }
    }
}