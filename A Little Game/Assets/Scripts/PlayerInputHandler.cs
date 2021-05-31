using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerController2D controller;
    public Attack attack;

    public float runSpeed = 40f;

    float doubleTapTime = 0.3f;
    float lastTapD;
    float lastTapA;
    float horizontalMove = 0f;
    bool jump = false;
    bool dash = false;

    private Vector3 position;

    // Update is called once per frame
    private void Start()
    {
        position = transform.position;
    }

    void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
        {
            jump = true;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            float timeSinceLastClick = Time.time - lastTapD;

            if (timeSinceLastClick <= doubleTapTime)
            {
                dash = true;
            }

            lastTapD = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            float timeSinceLastClick = Time.time - lastTapA;

            if (timeSinceLastClick <= doubleTapTime)
            {
                dash = true;
            }

            lastTapA = Time.time;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = position;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            attack.Melee();
        }

    }

    void FixedUpdate()
    {
        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump, dash);
        jump = false;
        dash = false;
    }
}
