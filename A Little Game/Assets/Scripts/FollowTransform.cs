using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [Header("Transform")]
    [Space]

    public bool changeTransform;
    public Camera cam;
    public float transformMoveSpeed = 0.1f;
    public Vector3 transformOffset;
    public bool moveX = false;
    public bool moveY = false;

    [Header("Rotation")]
    [Space]

    public bool changeRotation;
    public float rotationOffset = 0f;
    public float rotationMovementSpeed = 0.1f;

    private float angle;
    private Rigidbody2D rb;
    private Vector2 position = new Vector2(0f, 0f);
    private Vector3 destination;
    private Vector3 mousePosition;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam != null)
        {
            mousePosition = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector3 lookDir = mousePosition - transform.position;
            angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - rotationOffset;

            if (moveX && moveY)
            {
                destination = mousePosition + transformOffset;
            }

            else if (!moveY && moveX)
            {
                destination = new Vector3(mousePosition.x, transform.position.y, transform.position.z) + transformOffset;
            }

            else if (!moveX && moveY)
            {
                destination = new Vector3(transform.position.x, mousePosition.y, transform.position.z) + transformOffset;
            }

            position = Vector2.Lerp(transform.position, destination, transformMoveSpeed);
        }
    }

    private void FixedUpdate()
    {
        if(changeTransform)
        {
            rb.MovePosition(position);
        }

        if (changeRotation)
        {
            var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, desiredRotQ, rotationMovementSpeed));
        }
    }

    public void SetCamera(Camera camera)
    {
        cam = camera;
    }
}
