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
        //카메라 회전 속도
        [SerializeField]
        private float turnSpeed = 4f;

        //카메라 상하 회전
        [SerializeField]
        private float xRotation = 0f;

        //이동 속도
        [SerializeField]
        private float moveSpeed = 4f;

        //중력
        [SerializeField]
        private float gravity = 3f;

        private void Update()
        {
            moveInput();
        }

        private void moveInput()
        {
            ///마우스의 이동량
            float y_RotateSize = Input.GetAxis("Mouse X") * turnSpeed;
            //현재 회전값에 새로운 회전값을 더해줌
            float y_Rotate = transform.eulerAngles.y + y_RotateSize;

            //마우스 상하이동에 따른 이동값 계산
            float x_RotationSize = -Input.GetAxis("Mouse Y") * turnSpeed;

            xRotation = Mathf.Clamp(xRotation + x_RotationSize, 0, 0);

            //카메라 회전값을 카메라에 반영(x, y축 회전)
            transform.eulerAngles = new Vector3(xRotation, y_Rotate, 0);

            //getAxis로 이동량 측정

            Vector3 move = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");

            //이동량을 현재 좌표에 반영
            transform.position += move * moveSpeed * Time.deltaTime;

        }

    }
}

