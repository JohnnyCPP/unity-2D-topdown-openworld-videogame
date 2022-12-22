using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HealerFlank))]
[RequireComponent(typeof(Heal))]
[RequireComponent(typeof(AILifeSystem))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]

public class GoblinAIController : MonoBehaviour
{
    public GameObject animatedObject;

    Rigidbody2D rb;
    Animator animator;

    Transform playerTransform;

    Heal heal;
    HealerFlank healerFlank;

    public float healingCooldown = 5f;
    public float lastHealingTime = 0f;

    [HideInInspector]
    public enum State
    {
        FollowingEnemy,
        Waiting,
        Healing,
    }
    public State currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = FindObjectOfType<PlayerController>().transform;

        heal = GetComponent<Heal>();
        healerFlank = GetComponent<HealerFlank>();

        heal.InitBehaviourData();
        healerFlank.InitBehaviourData();

        currentState = State.FollowingEnemy;
        healerFlank.StartBehaviour();
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
        if ( animator == null ) animator = animatedObject.GetComponent<Animator>();

        if ( animator != null ) UpdateAnimator();

        switch (currentState)
        {
            case State.FollowingEnemy:
                FollowEnemyLogic();
                break;
            case State.Waiting:
                UpdateLogic();
                break;
            case State.Healing:
                Heal();
                break;
        }
    }

    private void FollowEnemyLogic()
    {
        healerFlank.UpdateBehaviour();

        if (healerFlank.followingPath && healerFlank.onRange())
        {
            healerFlank.StopBehaviour();

            currentState = State.Waiting;
        }
        else
            lastHealingTime -= Time.deltaTime;
    }

    void Heal()
    {
        heal.StartBehaviour();
        lastHealingTime = 0f;

        currentState = State.Waiting;
    }

    void UpdateLogic()
    {
        healerFlank.UpdateDistances();

        if (!healerFlank.onRange())
        {
            currentState = State.FollowingEnemy;
            healerFlank.StartBehaviour();
        }

        if (lastHealingTime >= healingCooldown)
            currentState = State.Healing;
        else
            lastHealingTime += Time.deltaTime;
    }
}
