using UnityEngine;

[ RequireComponent( typeof(Flank) ) ]
[ RequireComponent( typeof(ZicZac) ) ]


public class BirdAIController : MonoBehaviour
{
    public const float MAX_TIME_TO_ATTACK_AGAIN = 10f;
    public const float MIN_TIME_TO_ATTACK_AGAIN = 3f;

    public float timeCounterPostAttack = 0f;
    public float timeToAttackAgain = 3f;

    public float preAttackTime = 0f;
    public float waitingTime;

    [HideInInspector]
    public enum State { DoingFlank, StartingAttack, DoingZicZac, WaitingPlayer}

    public State currentState = State.WaitingPlayer;

    public GameObject animatedObject;

    Rigidbody2D rb;
    Animator animator;

    Transform playerTransform;

    Flank flank;
    ZicZac zicZac;

    public float damage;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = FindObjectOfType<PlayerController>().transform;


        flank = GetComponent<Flank>();
        zicZac = GetComponent<ZicZac>();


        flank.InitBehaviourData();
        zicZac.InitBehaviourData();

    }


     private void OnCollisionEnter2D( Collision2D collision )
    {
        if ( collision.collider.tag == "Player" )
        {
            Shake.instance.ShakeIt();
            PlayerController.instance.lifeSystem.TakeDamage( damage );
        }
    }


    private void DoZicZac()
    {
        zicZac.UpdateBehaviour();

        if ( zicZac.Finished() )
        {
            timeToAttackAgain = Random.Range( MIN_TIME_TO_ATTACK_AGAIN, MAX_TIME_TO_ATTACK_AGAIN );
            timeCounterPostAttack = 0f;

            currentState = State.DoingFlank;
            zicZac.StopBehaviour();
            flank.StartBehaviour();
        }
    }


    private void StartAttack()
    {
        animator.SetFloat( "Flying", 1 );

        preAttackTime += Time.deltaTime;

        if ( preAttackTime >= waitingTime )
        {
            preAttackTime = 0f;
            currentState = State.DoingZicZac;
            zicZac.StartBehaviour();
        }
    }


    private void UpdateIdleAnimation()
    {
        Vector2 enemyToPlayer = playerTransform.position - transform.position;

        if ( Mathf.Abs( enemyToPlayer.x ) > Mathf.Abs( enemyToPlayer.y ) )
        {
            if ( enemyToPlayer.x > 0 ) animator.SetFloat( "Horizontal", 1 );
            else animator.SetFloat( "Horizontal", -1 );
            animator.SetFloat( "Vertical", 0 );
        }
        else
        {
            if ( enemyToPlayer.y > 0 ) animator.SetFloat( "Vertical", 1 );
            else animator.SetFloat( "Vertical", -1 );
            animator.SetFloat( "Horizontal", 0 );
        }
    }


    private void UpdateAnimator()
    {
        float horizontalVelocity = rb.velocity.x;
        float verticalVelocity = rb.velocity.y;

        if ( horizontalVelocity != 0 || verticalVelocity != 0 ) animator.SetFloat( "Speed", 1 );
        else
        {
            animator.SetFloat( "Speed", 0 );
            UpdateIdleAnimation();
        }

        if ( horizontalVelocity > 0 )
        {
            animator.SetFloat( "Horizontal", 1 );
            animator.SetFloat( "Vertical", 0 );
        }
        else if ( horizontalVelocity < 0 )
        {
            animator.SetFloat( "Horizontal", -1 );
            animator.SetFloat( "Vertical", 0 );
        }

        if ( verticalVelocity > 2.8 )
        {
            animator.SetFloat( "Horizontal", 0 );
            animator.SetFloat( "Vertical", 1 );
        }
        else if ( verticalVelocity < -2.8 )
        {
            animator.SetFloat( "Horizontal", 0 );
            animator.SetFloat( "Vertical", -1 );
        }
    }


    private void DoFlank()
    {
        if ( animator.GetFloat( "Flying" ) == 1 ) animator.SetFloat( "Flying", 0 );
        flank.UpdateBehaviour();

        timeCounterPostAttack += Time.deltaTime;

        if ( timeCounterPostAttack >= timeToAttackAgain && zicZac.CanExecuteBehaviour() )
        {
            currentState = State.StartingAttack;
            flank.StopBehaviour();
        }
    }


    void Update()
    {
        if (animator == null) animator = animatedObject.GetComponent<Animator>();

        if (animator != null) UpdateAnimator();

        flank.UpdateDistanceToPlayer();

        switch (currentState)
        {
            case State.DoingFlank:

                DoFlank();

                break;

            case State.StartingAttack:

                StartAttack();

                break;

            case State.DoingZicZac:

                DoZicZac();

                break;
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