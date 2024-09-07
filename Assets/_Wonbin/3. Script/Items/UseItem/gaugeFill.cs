using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gaugeFill : MonoBehaviour
{
    mentalGaugeManager playerMentalGauge;

    [SerializeField]
    private GameObject _Player;

    private float ToAdd = 30;
    static bool getFill;
    public static bool isInItemSlot;
    private Transform itemSlotTransform;
    static bool useFill; // ��� ������ ���¸� �ǹ��ϴ� ����

    private void Start()
    {
        // Player ������Ʈ�� mentalGaugeManager ������Ʈ�� �����ϰ� ������
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerMentalGauge = playerObject.GetComponent<mentalGaugeManager>();

            if (playerMentalGauge == null)
            {
                Debug.LogError("Player ������Ʈ�� mentalGaugeManager ������Ʈ�� �����ϴ�!");
            }
        }
        else
        {
            Debug.LogError("Player ������Ʈ�� ã�� �� �����ϴ�! 'Player' �±װ� ����� �����Ǿ����� Ȯ���ϼ���.");
        }

        getFill = false;
        isInItemSlot = false;
        useFill = true;

        // ItemSlot ������Ʈ ã��
        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;

        if (itemSlotTransform == null)
        {
            Debug.LogError("ItemSlot ������Ʈ�� ã�� �� �����ϴ�!");
        }
    }

   

    private void Update()
    {
        // itemSlotTransform�� null�� �ƴ��� Ȯ��
        if (itemSlotTransform != null)
        {
            isInItemSlot = transform.IsChildOf(itemSlotTransform);

            if (Input.GetKeyDown(KeyCode.R))
            {
                // MentalGauge�� �̹� �ִ�ġ��� ��� �Ұ���
                if (isInItemSlot && playerMentalGauge != null && playerMentalGauge.MentalGauge >= playerMentalGauge.maxMentalGauge)
                {
                    useFill = false;
                    Debug.Log("���ŷ� �������� �ִ�ġ�Դϴ�.");
                }
                else if (isInItemSlot && useFill == true)
                {
                    fillUse();
                    Debug.Log("���ŷ� �������� ä�������ϴ�.");
                }
            }
        }
    }

    public void fillUse() // ��Ż ������ �� ���
    {
        if (playerMentalGauge != null)
        {
            playerMentalGauge.AddMentalGauge(ToAdd);
            Destroy(this.gameObject);
        }
    }
}
