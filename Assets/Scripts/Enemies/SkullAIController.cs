using UnityEngine;

[ RequireComponent( typeof(Teleport) ) ]
[ RequireComponent( typeof(Dash) ) ]
[ RequireComponent( typeof(Flank) ) ]


public class SkullAIController : MonoBehaviour
{
    public static SkullAIController instance;

    public GameObject animatedObject;

    Animator animator;
    Transform playerTransform;
    Teleport teleport;
    Dash dash;
    Flank flank;

    public float damage = 30f;
    public float attackCooldown = 2f;
    public float attackCooldownTimer;

    public int consecutiveAttacks = 4;
    public int consecutiveAttacksCounter;

    public bool bonked;

    [HideInInspector]
    public enum State
    {
        Dashing,
        Waiting,
        Attack,
        FollowingPlayer,
        Resting,
        WaitingPlayer
    }
    public State currentState = State.WaitingPlayer;


    void Start()
    {
        animator = GetComponent<Animator>();
        instance = this;

        teleport = GetComponent<Teleport>();
        dash = GetComponent<Dash>();
        flank = GetComponent<Flank>();

        teleport.InitBehaviourData();
        dash.InitBehaviourData();
        flank.InitBehaviourData();

        attackCooldownTimer = 0f;
        consecutiveAttacksCounter = consecutiveAttacks;

        currentState = State.WaitingPlayer;
    }


    void Update()
    {
        if ( animator == null ) animator = animatedObject.GetComponent<Animator>();

        if ( playerTransform == null && PlayerController.instance != null ) playerTransform = PlayerController.instance.transform;

        switch (currentState)
        {
            case State.Dashing:
                DashingUpdateLogic();
                break;
            case State.Resting:
                RestingUpdateLogic();
                break;
            case State.Attack:
                Attack();
                break;
            case State.FollowingPlayer:
                FollowPlayer();
                break;
            default:
                break;
        }
    }


    private void FollowPlayer()
    {
        UpdateIdleAnimation();

        flank.UpdateBehaviour();

        if (attackCooldownTimer <= 0f && flank.onRange()) {
            flank.StopBehaviour();

            currentState = State.Resting;
        }
        else
            attackCooldownTimer -= Time.deltaTime;
    }


    void RestingUpdateLogic()
    {
        UpdateIdleAnimation();

        flank.UpdateDistanceToPlayer();

        if (!flank.onRange() && !bonked)
        {
            currentState = State.FollowingPlayer;
        }
        else if (attackCooldownTimer <= 0f)
        {
            bonked = false;

            attackCooldownTimer = attackCooldown;
            consecutiveAttacksCounter = consecutiveAttacks;
            currentState = State.Attack;
        }
        else
            attackCooldownTimer -= Time.deltaTime;
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


    void Attack()
    {
        consecutiveAttacksCounter--;

        Vector2 initialPosition = transform.position;

        teleport.StartBehaviour();

        UpdateIdleAnimation();

        if ((Vector2)transform.position != initialPosition)
            dash.StartBehaviour();

        currentState = State.Dashing;
    }


    void DashingUpdateLogic()
    {
        if (!dash.dashing)
        {
            UpdateIdleAnimation();

            if (flank.distanceToPlayer.magnitude > flank.maxDistToPlayer)
                currentState = State.FollowingPlayer;
            else if (consecutiveAttacksCounter == 0)
                currentState = State.Resting;
            else
                currentState = State.Attack;
        }
    }


    public void InterruptDash()
    {
        dash.StopBehaviour();

        attackCooldownTimer = 0.5f;
        consecutiveAttacksCounter = 0;

        currentState = State.Resting;

        bonked = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player" && dash.dashing) {
            PlayerController.instance.lifeSystem.TakeDamage(damage);

            Shake.instance.ShakeIt();

            dash.dashing = false;
            dash.StopBehaviour();
        }

        if (collision.gameObject.layer == 3 && dash.dashing) {
            InterruptDash();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (currentState == State.WaitingPlayer && collision.gameObject.tag == "Player")
        {
            currentState = State.FollowingPlayer;
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