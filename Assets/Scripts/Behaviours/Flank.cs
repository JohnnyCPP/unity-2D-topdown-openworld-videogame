using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flank : AIBehaviour
{   // INITIAL DATA
    AstarPath pathController;
    Rigidbody2D rb;
    Pathfinding.Seeker seeker;
    // COROUTINES
    Coroutine seekPathRoutine;
    // PUBLIC ATTRIBUTES
    public float speed = 3f;
    public float minDistToPlayer = 4f;
    public float maxDistToPlayer = 8f;
    public Vector2 distanceToPlayer;
    public bool lookingAPath = false;
    public bool followingPath;
    // PRIVATE ATTRIBUTES
    public Transform playerTransform;
    float timeFollowingPathCount;
    Vector2 lastPathNodePos;


    Vector2 GetPositionToApproachPlayer()
    {
        float offset = 1f;
        float maxOffset = maxDistToPlayer - minDistToPlayer;

        Vector2 target;
        Vector2 target2;
        Vector2 playerToTarget;
        Vector2 playerToTarget2;

        bool cast1 = true;
        bool cast2 = true;

        do
        {
            target = - distanceToPlayer.normalized * ( minDistToPlayer + offset ) + (Vector2) playerTransform.position;
            target2 = target;
            playerToTarget = target - (Vector2) playerTransform.position;
            playerToTarget2 = playerToTarget;

            int it = 0;

            while ( cast1 && cast2 )
            {
                cast1 = Physics2D.CircleCast( target, GetComponent<CircleCollider2D>().radius, Vector2.right, LayerMask.GetMask( "Obstacle" ) );
                cast2 = Physics2D.CircleCast( target2, GetComponent<CircleCollider2D>().radius, Vector2.right, LayerMask.GetMask( "Obstacle" ) );

                if ( it ++ > 180 / 15 ) break;

                playerToTarget = PositionUtils.RotateVector2( playerToTarget, 15f );
                target = (Vector2) playerTransform.position + playerToTarget;

                playerToTarget2 = PositionUtils.RotateVector2( playerToTarget2, - 15f );
                target2 = (Vector2) playerTransform.position + playerToTarget2;
            }

            offset *= 1.1f;

        } while ( cast1 && cast2 && offset < maxOffset );
        
        if ( cast1 && !cast2 ) return target2;

        if ( !cast1 && cast2 ) return target;

        return Vector2.Distance( transform.position, target ) < Vector2.Distance( transform.position, target2 ) ? target : target2;
    }

    public bool onRange() {
        return PositionUtils.AroundPlayer(distanceToPlayer, maxDistToPlayer, minDistToPlayer);
    }


    Vector2 GetPositionToAvoidPlayer()
    {
        float offset = 2f;
        float maxOffset = maxDistToPlayer - minDistToPlayer;

        Vector2 target;
        Vector2 target2;

        Vector2 aiToTarget;
        Vector2 aiToTarget2;

        bool cast1 = true;
        bool cast2 = true;

        do
        {
            target = - distanceToPlayer.normalized * ( minDistToPlayer + offset ) + (Vector2) playerTransform.position;
            target2 = target;

            aiToTarget = target - (Vector2) transform.position;
            aiToTarget2 = aiToTarget;

            int it = 0;

            while ( cast1 && cast2 )
            {

                cast1 = Physics2D.OverlapCircle( target, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask( "Obstacle" ) );
                cast2 = Physics2D.OverlapCircle( target2, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask( "Obstacle" ) );

                if ( it ++ > 180 / 15 ) break;

                aiToTarget = PositionUtils.RotateVector2( aiToTarget, 15f );

                target = (Vector2) transform.position + aiToTarget;

                aiToTarget2 = PositionUtils.RotateVector2( aiToTarget2, -15f );
                target2 = (Vector2) transform.position + aiToTarget2;

            }

            var distanceTargetToPlayer = Vector2.Distance( playerTransform.position, target );
            var distanceTarget2ToPlayer = Vector2.Distance( playerTransform.position, target2 );

            if ( distanceTargetToPlayer < minDistToPlayer || distanceTarget2ToPlayer < minDistToPlayer )
            {
                cast1 = true;
                cast2 = true;
            }

            offset *= 1.1f;

        } while ( cast1 && cast2 && offset < maxOffset );

        if ( cast1 && !cast2 ) return target2;

        if ( !cast1 && cast2 ) return target;

        return Vector2.Distance( transform.position, target ) < Vector2.Distance( transform.position, target2 ) ? target : target2;
    }


    void StartPath( Vector2 start, Vector2 end, Pathfinding.OnPathDelegate callback )
    {
        lookingAPath = true;

        seeker.StartPath( start, end, ( Pathfinding.Path path ) => {

            lookingAPath = false;

            callback( path );
        });
    }


    public void UpdateDistanceToPlayer()
    {
        if(Time.timeScale != 0 && playerTransform != null)
            distanceToPlayer = playerTransform.position - transform.position;
    }


    public void FollowPlayerLogicUpdate()
    {
        UpdateDistanceToPlayer();

        float distanceToPlayer = Vector2.Distance( lastPathNodePos, playerTransform.position);

        Vector2 target;

        if ( followingPath && timeFollowingPathCount >= 3f && !PositionUtils.PositionIsAroundTarget( distanceToPlayer, maxDistToPlayer, minDistToPlayer ) )
        {
            try { StopCoroutine( seekPathRoutine ); } catch ( System.NullReferenceException ) {}

            target = GetPositionToAvoidPlayer();
            StartPath( transform.position, target, OnPathFound );
        }
        else if ( this.distanceToPlayer.magnitude < minDistToPlayer && !lookingAPath && !followingPath )
        {
            target = GetPositionToAvoidPlayer();
            Debug.DrawLine(transform.position, target, Color.cyan, 1f);
            StartPath( transform.position, target, OnPathFound );
        }
        else if ( !lookingAPath && !followingPath )
        {
            target = GetPositionToApproachPlayer();
            StartPath( transform.position, target, OnPathFound );
        }
    }


    void StopSeekingPath()
    {
        if ( rb.velocity != Vector2.zero )
        {
            rb.velocity = Vector2.zero;
            StopCoroutine( seekPathRoutine );
        }
    }


    void FinishFollowingPath()
    {
        followingPath = false;
        rb.velocity = Vector2.zero;
    }


    IEnumerator FollowPath( List<Vector3> path )
    {
        timeFollowingPathCount = 0f;

        const float minToReachNode = 0.1f;
        followingPath = true;
        rb.velocity = ( path[0] - transform.position ).normalized * speed;

        foreach ( var node in path )
        {
            while ( Vector2.Distance( transform.position, node ) > minToReachNode )
            {
                timeFollowingPathCount += Time.deltaTime;
                Vector2 newVelocity = node - transform.position;
                newVelocity.Normalize();
                rb.velocity = newVelocity * speed;
                yield return null;
            }
        }

        FinishFollowingPath();
    }


    void OnPathFound( Pathfinding.Path path )
    {
        lastPathNodePos = path.vectorPath[ path.vectorPath.Count - 1 ];

        if ( seekPathRoutine != null ) StopSeekingPath();

        seekPathRoutine = StartCoroutine( FollowPath( path.vectorPath ) );
    }


    public override void InitBehaviourData()
    {
        pathController = FindObjectOfType<AstarPath>();

        rb = GetComponent<Rigidbody2D>();

        seeker = GetComponent<Pathfinding.Seeker>();

        playerTransform = PlayerController.instance.gameObject.transform;
    }


    public override void StartBehaviour()
    {
        playerTransform = FindObjectOfType<PlayerController>().gameObject.transform;

        if (playerTransform != null)
            FollowPlayerLogicUpdate();
    }


    public override void StopBehaviour()
    {
        FinishFollowingPath();
        try { StopCoroutine( seekPathRoutine ); } catch ( System.NullReferenceException ) { Debug.Log( "Flank.cs: The coroutine was null" ); };
    }


    public override void UpdateBehaviour()
    {
        FollowPlayerLogicUpdate();
    }


    void Update()
    {
        if (playerTransform == null && PlayerController.instance != null) playerTransform = PlayerController.instance.gameObject.transform;
    }
}