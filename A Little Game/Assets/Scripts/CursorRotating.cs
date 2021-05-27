using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorRotating : MonoBehaviour
{

    public Camera cam;
    private float angle;
    public float offset = 0f;
    public float movementSpeed = 10;
    public float sensitivity = 1f;
    public Transform Limit;
    private Rigidbody2D rb;
    private Vector3 mousePos;
    private FixedJoint2D FJ;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition) * sensitivity;
        Vector3 lookDir = mousePos - transform.position;
        angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - offset;
    }

    void FixedUpdate()
    { 
        var desiredRotQ = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, angle);
            rb.MoveRotation(Quaternion.Lerp(transform.rotation, desiredRotQ, movementSpeed));        
    }
}
