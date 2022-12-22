using UnityEngine;

[ RequireComponent( typeof(Flank) ) ]
[ RequireComponent( typeof(Shoot) ) ]


public class SlimeAIController : MonoBehaviour
{
    [HideInInspector]
    public enum State { DoingFlank, Shooting, WaitingPlayer}
    public State currentState = State.WaitingPlayer;

    public GameObject animatedObject;

    Rigidbody2D rb;
    Animator animator;

    Flank flank;
    Shoot shoot;

    public float damage = 25;

    public float reloadTime = 3f;
    public float reloadTimeCounter;

    [HideInInspector]
    public enum SlimeType{ Normal, Toxic }
    public SlimeType slimeType;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        flank = GetComponent<Flank>();
        shoot = GetComponent<Shoot>();


        flank.InitBehaviourData();
        shoot.InitBehaviourData();

        shoot.damage = damage;

        switch (slimeType)
        {
            case SlimeType.Normal:
                shoot.effects = new string[0];
                break;
            case SlimeType.Toxic:
                string[] effects = { "Slow", "Toxic" };
                shoot.effects = effects;
                break;
        }

    }


    void Shoot()
    {
        shoot.UpdateBehaviour();

        if ( shoot.shootReloadTime <= 0f ) animator.SetBool( "Attacking", true );
        else if ( animator.GetBool( "Attacking" ) ) animator.SetBool( "Attacking", false );

        if ( !PositionUtils.AroundPlayer( flank.distanceToPlayer, flank.maxDistToPlayer, flank.minDistToPlayer ) )
        {
            currentState = State.DoingFlank;

            shoot.StopBehaviour();
            flank.StartBehaviour();
        }
    }


    private void UpdateAnimator()
    {
        float horizontalVelocity = rb.velocity.x;
        float verticalVelocity = rb.velocity.y;

        if ( horizontalVelocity != 0 || verticalVelocity != 0 ) animator.SetFloat( "Speed", 1 );
        else animator.SetFloat( "Speed", 0 );
    }


    void DoFlank()
    {
        flank.UpdateBehaviour();

        if ( PositionUtils.AroundPlayer( flank.distanceToPlayer, flank.maxDistToPlayer, flank.minDistToPlayer ) )
        {
            currentState = State.Shooting;

            flank.StopBehaviour();
            shoot.StartBehaviour();
        }
    }


    void Update()
    {
        if ( animator == null ) animator = animatedObject.GetComponent<Animator>();

        if ( animator != null ) UpdateAnimator();

        flank.UpdateDistanceToPlayer();

        switch ( currentState )
        {
            case State.DoingFlank:

                DoFlank();

                break;

            case State.Shooting:

                Shoot();

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
        }
    }
}