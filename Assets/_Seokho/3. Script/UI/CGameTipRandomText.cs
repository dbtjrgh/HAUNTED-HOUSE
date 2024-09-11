using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CGameTipRandomText : MonoBehaviour
{
    public Text tipText;
    private int random;

    private void Awake()
    {
        tipText = GetComponent<Text>();
        random = Random.Range(0, 3);
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        switch (random)
        {
            case 0:
                tipText.text = "�� : �������϶� ���� ���Ƿ� ���߾��մϴ�.";
                return;
            case 1:
                tipText.text = "�� : �ǳ��� ������ ���ŷ��� �����մϴ�.";
                return;
            case 2:
                tipText.text = "�� : ������ �Բ� �ٴϼ���.";
                return;
        }

    }




}
