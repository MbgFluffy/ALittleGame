using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Transform attackPoint;
    public float attackRange = 1;

    public void Melee()
    {
        Debug.Log("Attacking...");
        Physics2D.Raycast(attackPoint.position, new Vector2(transform.localScale.x, 0), attackRange);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(attackPoint.position, new Vector3(transform.localScale.x, 0, 0) * attackRange);
    }
}
