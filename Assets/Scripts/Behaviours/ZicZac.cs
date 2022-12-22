using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ZicZac : AIBehaviour
{// PUBLIC ATTRIBUTES
    public int movCount;
    public float speed;
    public float distToLine;
 // PRIVATE ATTRIBUTES
    public bool finished = false;


    public bool Finished()
    {
        return finished;
    }

    
    public bool CanExecuteBehaviour()
    {
        var targets = GetTargets();

        var originPos = transform.position;

        foreach( var target in targets )
        {
            if ( Physics2D.Linecast( originPos, target, LayerMask.GetMask( "Obstacle" ) ) )
            {
                //Debug.DrawLine(originPos, target, Color.red, 100f);
                return false;
            }

            originPos = target;
        }

        if ( Physics2D.Linecast( originPos, PlayerController.instance.transform.position, LayerMask.GetMask( "Obstacle" ) ) )
        {
            //Debug.DrawLine(originPos, PlayerController.instance.transform.position, Color.red, 100f);
            return false;
        }

        return true;
    }


    Vector2[] GetTargets()
    {
        Vector2[] targets = new Vector2[ movCount ];

        Vector2 meToPlayer = PlayerController.instance.transform.position - transform.position;
        Vector2 perp = PositionUtils.GetPerpendicular( meToPlayer ).normalized;

        float distBetweenMovs = meToPlayer.magnitude / ( movCount + 1f );


        if ( Random.Range( 0f, 100f ) < 50f ) perp *= - 1;

        for ( int i = 0 ; i < movCount ; i ++ )
        {
            Vector2 linePoint = (Vector2) transform.position + meToPlayer.normalized * ( distBetweenMovs * (float) ( i + 1 ) );
            Vector2 resultPoint = linePoint + perp * distToLine;
            perp *= - 1f;

            targets[ i ] = resultPoint;
        }

        return targets;
    }


    IEnumerator ZicZacAttack()
    {
        Vector2[] targets = GetTargets();

        int currentTargetIndex = 0;

        do
        {
            var meToTarget = targets[ currentTargetIndex ] - (Vector2) transform.position;
            //Debug.DrawLine(transform.position, (Vector2)transform.position + meToTarget);
            meToTarget.Normalize();

            GetComponent<Rigidbody2D>().velocity = meToTarget * speed;

            if ( Vector2.Distance( transform.position, targets[ currentTargetIndex ] ) <= GetComponent<CircleCollider2D>().radius * transform.localScale.x)
            {
                currentTargetIndex ++;

                if ( currentTargetIndex >= targets.Length ) break;
            }

            yield return null;

        } while ( true );


        Vector2 playerPos = PlayerController.instance.transform.position;

        while ( Vector2.Distance( transform.position, playerPos ) > GetComponent<CircleCollider2D>().radius * transform.localScale.x )
        {
            yield return null;

            var meToTarget = playerPos - (Vector2) transform.position;
            //Debug.DrawLine(transform.position, (Vector2)transform.position + meToTarget);
            meToTarget.Normalize();

            GetComponent<Rigidbody2D>().velocity = meToTarget * speed;
        }

        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        finished = true;
    }


    public override void InitBehaviourData()
    {

    }


    public override void StartBehaviour()
    {
        finished = false;
        StartCoroutine( ZicZacAttack() );
    }


    public override void StopBehaviour()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
    }


    public override void UpdateBehaviour()
    {
        
    }
}