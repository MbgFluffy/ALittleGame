using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float jumpForce = 400f;                             // Amount of force added when the player jumps.
    [SerializeField] private float wallJumpForce = 400f;                         // Amount of force added when the player jumps from a wall.
    [SerializeField] private float movementSpeed = 10f;                          // Movement speed of the player.
    [SerializeField] private float limitFallSpeed = 25f;                         // Limit fall speed
    [Range(0f, 0.99f)] [SerializeField] private float wallGrabSmoothing = 0.7f; // Smoothens the stop on a wall while wallsliding.                 
    [Range(0, .3f)] [SerializeField] private float movementSmoothingMidAir = .05f;     // How much to smooth out the movement while in air.
    [SerializeField] private bool airControl = true;                             // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask whatIsGround;                             // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                              // A position marking where to check if the player is grounded.
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform wallGrabCheck;
    [SerializeField] private float dashForce = 25f;
    [SerializeField] private int jumpCount = 1;                                  // How many times the player can jump mid-air.

    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool grounded;            // Whether or not the player is grounded.
    private Rigidbody2D rb2D;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Vector2 velocity = Vector2.zero;
    private Vector2 jumpDirection = new Vector2(0, 1); // Direction in which the jump force will be added.
    private Vector2 wallJumpDirection = new Vector2(1, 1); // Direction in which the wall jump force will be added.

    private bool canMove = true; //If player can move
    private bool canDash = true;
    private bool canWallGrab = true;
    private bool isDashing = false; //If player is dashing
    private bool isWall = false; //If there is a wall in front of the player
    private bool isWallGrabbing = false; //If player is sliding on a wall
    private bool isSliding = false; //If slipping down a wall
    private bool oldWallGrab = false; //If player is grabbing on a wall in the previous frame
    private bool canCheck = false; //For check if player is wallsliding
    private float wallPosition; //determins if the wall is left or right to the player
    private int jumpsLeft;

    private Coroutine crWallGrabbing; //Coroutine handling the wallsliding time
    private Coroutine crWallGrabCooldown; //Time until WallGrab is possible again

    // Start is called before the first frame update
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        /*Debug.Log("is wall sliding: " + isWallGrabbing);
        Debug.Log("wallslide detect: " + canWallGrab);
        Debug.Log("oldwallgrab: " + oldWallGrab);*/
        Debug.Log("grounded: " + grounded);

        HandleWallGrabbing();
    }

    private void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        CheckWallGrab();
    }

    private void CheckWall()
    {
            isWall = false;

            Collider2D[] collidersWall = Physics2D.OverlapCircleAll(wallCheck.position, groundedRadius, whatIsGround);
            for (int i = 0; i < collidersWall.Length; i++)
            {
                if (collidersWall[i].gameObject != null)
                {
                    isDashing = false;
                    isWall = true;
                }
            }
    }

    private void CheckWallGrab()
    {
        canWallGrab = false;

        Collider2D[] collidersWall = Physics2D.OverlapCircleAll(wallGrabCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < collidersWall.Length; i++)
        {
            if (collidersWall[i].gameObject != null)
            {
                canWallGrab = true;
            }
        }
    }

    private void CheckGround()
    {
        grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                jumpsLeft = jumpCount;
            }
        }
    }

    public void Move(float move, bool jump, bool dash)
    {
        rb2D.gravityScale = 4;

        if(rb2D.velocity.y < -limitFallSpeed)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, -limitFallSpeed);
        }

        if (canMove)
        {
            if (move * transform.localScale.x < 0 && !isWallGrabbing)
            {
                Flip();
            }

            if (isWallGrabbing && wallPosition * move < 0)
                EndWallGrab();

            if (grounded && !(isWall && move * transform.localScale.x > 0)) // movement on ground
            {
                rb2D.velocity = new Vector2(move * movementSpeed, rb2D.velocity.y);
            }

            if(airControl && !grounded && move != 0) //movement while in air, when airControll is on
            {
                Vector2 targetVelocity = new Vector2(move * movementSpeed, rb2D.velocity.y);
                rb2D.velocity = Vector2.SmoothDamp(rb2D.velocity, targetVelocity, ref velocity, movementSmoothingMidAir);
            }

            if (isWallGrabbing)
            {
                rb2D.gravityScale = 0;
                rb2D.velocity = new Vector2(0, rb2D.velocity.y * wallGrabSmoothing);
            }

            if (isSliding)
            {
                //rb2D.AddForce(new Vector2(0, -100));
            }

            if (jump)
            {
                if (grounded)
                {
                    Jump(jumpDirection, jumpForce);
                }
                else if (isWallGrabbing)
                {
                    EndWallGrab();
                    Jump(new Vector2(wallJumpDirection.x * -transform.localScale.x, wallJumpDirection.y), wallJumpForce);
                    StartCoroutine(WaitToMove(.2f));
                    Flip();
                }
                else if (jumpsLeft > 0)
                {
                    Jump(jumpDirection, jumpForce);
                    jumpsLeft--;
                }
            }
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void Jump(Vector2 direction, float force)
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, 0);
        rb2D.AddForce(direction * force);
    }

    private void HandleWallGrabbing()
    {
        if (oldWallGrab && (grounded || (isWall && wallPosition != transform.localScale.x)))
        {
                oldWallGrab = false;
                StopCoroutine(crWallGrabCooldown);          
        }

        if (transform.localScale.x * Input.GetAxisRaw("Horizontal") > 0 && !grounded && isWall && canWallGrab && !oldWallGrab && !isWallGrabbing)
        {
            wallPosition = transform.localScale.x;
            StartWallGrab();
        }
    }

    private void StartWallGrab()
    {
        jumpsLeft = jumpCount;
        crWallGrabbing = StartCoroutine(WallGrab());
    }

    private void EndWallGrab()
    {
        StopCoroutine(crWallGrabbing);
        isSliding = false;
        isWallGrabbing = false;
        crWallGrabCooldown = StartCoroutine(WallGrabCooldown());
    }

    IEnumerator WaitToMove(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    IEnumerator WallGrab()
    {
        isWallGrabbing = true;
        yield return new WaitForSeconds(2);
        isSliding = true;
        yield return new WaitForSeconds(2);
        isSliding = false;
        isWallGrabbing = false;
        crWallGrabCooldown = StartCoroutine(WallGrabCooldown());
    }

    IEnumerator WallGrabCooldown()
    {
        oldWallGrab = true;
        yield return new WaitForSeconds(1f);
        oldWallGrab = false;
    }
}