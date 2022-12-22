using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Flank))]
public class SkeletonAIController : MonoBehaviour
{
    enum State { DoingFlank, Attacking, WaitingPlayer }
    State currentState = State.WaitingPlayer;

    public GameObject animatedObject;

    Rigidbody2D rb;
    Animator animator;

    Transform playerTransform;

    Flank flank;
    AIMeleeAttack melee;

    public float damage = 10;

    public float attackCooldown= 1f;
    public float attackCooldownCounter;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = FindObjectOfType<PlayerController>().transform;

        melee = GetComponent<AIMeleeAttack>();
        flank = GetComponent<Flank>();
        rb = GetComponent<Rigidbody2D>();

        flank.InitBehaviourData();

        flank.maxDistToPlayer = melee.attackDistance + GetComponent<CircleCollider2D>().radius * transform.localScale.x;
    }


    private void UpdateIdleAnimation()
    {
        Vector2 enemyToPlayer = playerTransform.position - transform.position;

        if (Mathf.Abs(enemyToPlayer.x) > Mathf.Abs(enemyToPlayer.y))
        {
            if (enemyToPlayer.x > 0) animator.SetFloat("Horizontal", 1);
            else animator.SetFloat("Horizontal", -1);
            animator.SetFloat("Vertical", 0);
        }
        else
        {
            if (enemyToPlayer.y > 0) animator.SetFloat("Vertical", 1);
            else animator.SetFloat("Vertical", -1);
            animator.SetFloat("Horizontal", 0);
        }
    }


    private void UpdateAnimator()
    {
        float horizontalVelocity = rb.velocity.x;
        float verticalVelocity = rb.velocity.y;

        if (horizontalVelocity != 0 || verticalVelocity != 0) animator.SetFloat("Speed", 1);
        else
        {
            animator.SetFloat("Speed", 0);
            UpdateIdleAnimation();
        }

        if (horizontalVelocity > 0)
        {
            animator.SetFloat("Horizontal", 1);
            animator.SetFloat("Vertical", 0);
        }
        else if (horizontalVelocity < 0)
        {
            animator.SetFloat("Horizontal", -1);
            animator.SetFloat("Vertical", 0);
        }

        if (verticalVelocity > 2.8)
        {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", 1);
        }
        else if (verticalVelocity < -2.8)
        {
            animator.SetFloat("Horizontal", 0);
            animator.SetFloat("Vertical", -1);
        }
    }


    void Update()
    {
        if (animator != null) UpdateAnimator();
        else animator = animatedObject.GetComponent<Animator>();

        flank.UpdateDistanceToPlayer();

        switch (currentState)
        {
            case State.DoingFlank:
                DoFlank();
                break;
            case State.Attacking:
                Attack();
                break;
        }
    }

    void DoFlank()
    {
        flank.UpdateBehaviour();

        if (PositionUtils.AroundPlayer(flank.distanceToPlayer, flank.maxDistToPlayer, flank.minDistToPlayer))
        {
            currentState = State.Attacking;

            flank.StopBehaviour();
            melee.StartBehaviour();
        }
    }

    void Attack()
    {
        attackCooldownCounter += Time.deltaTime;

        if (attackCooldownCounter >= attackCooldown)
        {
            melee.StartBehaviour();
            attackCooldownCounter = 0f;
        }

        if (!PositionUtils.AroundPlayer(flank.distanceToPlayer, flank.maxDistToPlayer, flank.minDistToPlayer))
        {
            currentState = State.DoingFlank;

            flank.StartBehaviour();
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == State.WaitingPlayer && collision.gameObject.tag == "Player")
        {
            currentState = State.DoingFlank;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (currentState != State.WaitingPlayer && collision.gameObject.tag == "Player")
        {
            currentState = State.WaitingPlayer;

            animator.SetFloat("Speed", 0);
            UpdateIdleAnimation();
        }
    }
}
