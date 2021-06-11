using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private int comboCount = 3;
    [SerializeField] private string animationName;
    [SerializeField] private float inputOffset = 0.2f; // time offset until the next input counts as new attack

    private bool[] attacks;
    private string[] attackAnimations;

    private bool canAttack = true; // if the player can attack again
    private bool attackComplete = true;
    public bool isAttacking = false;
    private int attackCounter; //counts the attack which the player just did
    private float attackDuration;

    public Animator animator;
    private AnimationManager AM;

    private void Start()
    {
        AM = GetComponent<AnimationManager>();
        animator = GetComponent<Animator>();

        attackCounter = 0;

        attacks = new bool[comboCount];
        attackAnimations = new string[comboCount];

        for (int i = 0; i < comboCount; i++)
        {
            attacks[i] = false;

            attackAnimations[i] = animationName + i;
        }
    }

    private void Update()
    {
        NextAttack();
        FirstAttack();
    }

    public void DoAttack()
    {
        if (canAttack)
        {
            for(int i = 0; i < comboCount; i++ )
            {
                if(!attacks[i])
                {
                    attacks[i] = true;
                    StartCoroutine(WaitToAttack());
                    return;
                }
            }
        }
    }

    private void NextAttack()
    {
        for(int i = 0; i < comboCount; i++)
        {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnimations[i]) && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                attacks[i] = false;
                if(attacks[i + 1])
                {
                    AM.ChangeAnimationState(attackAnimations[i + 1]);
                }
            }
        }
    }

    private void FirstAttack()
    {
        if(attacks[0] && !isAttacking)
        {
            AM.ChangeAnimationState(attackAnimations[0]);
            attackDuration = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            StartCoroutine(IsAttacking(attackDuration));

        }
    }

    IEnumerator IsAttacking(float time)
    {
        isAttacking = true;
        yield return new WaitForSeconds(time);
        isAttacking = false;
    }

    IEnumerator WaitToAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(inputOffset);
        canAttack = true;
    }
}
