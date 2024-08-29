using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestPlayer : MonoBehaviour
{
    public float speed = 10f;
    Rigidbody playerRigidbody; // public 뗀 상태

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>(); // Rigidbody 컴포넌트가 붙어 있다면 찾아서 리턴 해줌.
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
