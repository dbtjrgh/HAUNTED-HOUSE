using Infrastructure;
using UnityEngine;
using System;
using System.Collections;
using Infrastructure.Services;
using UnityEngine.InputSystem.XR;

namespace Wonbin
{
    public class playerMove : MonoBehaviour
    {
        //ī�޶� ȸ�� �ӵ�
        [SerializeField]
        private float turnSpeed = 4f;

        //ī�޶� ���� ȸ��
        [SerializeField]
        private float xRotation = 0f;

        //�̵� �ӵ�
        [SerializeField]
        private float moveSpeed = 4f;

        //�߷�
        [SerializeField]
        private float gravity = 3f;

        private void Update()
        {
            moveInput();
        }

        private void moveInput()
        {
            ///���콺�� �̵���
            float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
            //���� ȸ������ ���ο� ȸ������ ������
            float y_Rotate = transform.eulerAngles.y + y_RotateSize;

            //���콺 �����̵��� ���� �̵��� ���
            float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;

            xRotation = Mathf.Clamp(xRotation + x_RotationSize, 0, 0);

            //ī�޶� ȸ������ ī�޶� �ݿ�(x, y�� ȸ��)
            transform.eulerAngles = new Vector3(xRotation, y_Rotate, 0);

            //getAxis�� �̵��� ����

            Vector3 move = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

            //�̵����� ���� ��ǥ�� �ݿ�
            transform.position += move * moveSpeed * Time.deltaTime;

        }

    }
}

