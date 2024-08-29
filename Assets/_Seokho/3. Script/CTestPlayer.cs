using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestPlayer : MonoBehaviour
{
    public float speed = 10f;
    Rigidbody playerRigidbody; // public �� ����

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>(); // Rigidbody ������Ʈ�� �پ� �ִٸ� ã�Ƽ� ���� ����.
    }


    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");

        float inputZ = Input.GetAxis("Vertical");

        float fallSpeed = playerRigidbody.velocity.y;

        Vector3 velocity = new Vector3(inputX, 0, inputZ);
        velocity = velocity * speed;

        velocity.y = fallSpeed;

        playerRigidbody.velocity = velocity;
    }
}
