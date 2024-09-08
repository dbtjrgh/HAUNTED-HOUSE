using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.ProBuilder.MeshOperations;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(PhotonRigidbodyView))]
public class flashLight : MonoBehaviourPun
{
    bool playerGetLight; // �÷��̾ �������� on�� �������� Ȯ��
    static bool getLight; // ������ ȹ�� ���� Ȯ��
    Light myLight; // Light ������Ʈ ����
    public static bool isInItemSlot; // �������� ItemSlot�� �ִ��� ���θ� Ȯ��
    private Transform itemSlotTransform;

    PlayerInventory Inventory;

    private void Start()
    {
        playerGetLight = false;
        getLight = false;
        isInItemSlot = false; // �ʱ� ���´� ItemSlot�� ����
        myLight = GetComponent<Light>(); // flashLight�� Light ������Ʈ�� ������
        myLight.intensity = 0; // ���� �� �������� ���� �ֵ��� ����
        myLight.enabled = false;
        itemSlotTransform = GameObject.Find("ItemSlot")?.transform;
    }

    private void Update()
    {
        if (itemSlotTransform == null)
        {
            // ItemSlot�� �������� ���� ��
            myLight.enabled = false;
            return;
        }

        // �������� ItemSlot�� �ڽ����� Ȯ��
        bool isInItemSlot = transform.IsChildOf(itemSlotTransform);

        if (isInItemSlot)
        {
            lightOnOFF(); // �������� ItemSlot�� ���� ���� ȣ��
        }
        else
        {
            myLight.enabled = false; // �������� ItemSlot�� ���� �� ��Ȱ��ȭ
        }

    }

    static internal void lightEquip()
    {
        getLight = true;
        GameObject flashLightObject = GameObject.FindGameObjectWithTag("Items");

        if (flashLightObject != null)
        {
            GameObject itemSlot = GameObject.Find("ItemSlot");
            if (itemSlot != null)
            {
                flashLightObject.transform.SetParent(itemSlot.transform);
                flashLightObject.transform.localPosition = Vector3.zero; // ��ġ �ʱ�ȭ
                flashLightObject.transform.localRotation = Quaternion.identity; // ȸ�� �ʱ�ȭ

                isInItemSlot = true; // ItemSlot�� �߰��Ǿ����� ǥ��

            }
        }
    }

    void lightOnOFF()
    {
        getLight = true;
        if (getLight)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                playerGetLight = !playerGetLight; // ������ on/off
                myLight.intensity = playerGetLight ? 10 : 0; // ������ ��� ����
                myLight.enabled = playerGetLight; // ������ Ȱ��ȭ/��Ȱ��ȭ
            }
        }
    }

    public IEnumerator Blink()
    {

        if (Ghost.instance.state == changwon.GhostState.HUNTTING)
        {

            float ghostBlinkTargetDistance = Vector3.Distance(Ghost.instance.target.transform.position, Ghost.instance.transform.position);
            if (ghostBlinkTargetDistance < 30)
            {
                myLight.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);
                myLight.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                myLight.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.5f);
                myLight.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                myLight.gameObject.SetActive(false);
            }
        }
        else
        {

            myLight.gameObject.SetActive(true);
        }
        yield return null;
    }


}