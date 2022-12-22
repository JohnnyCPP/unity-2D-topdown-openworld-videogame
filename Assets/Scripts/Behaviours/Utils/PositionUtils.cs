using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PositionUtils
{


    public static bool PositionIsAroundTarget( float distanceToTarget, float maxDistToTarget, float minDistToTarget )
    {
        return distanceToTarget < maxDistToTarget && distanceToTarget > minDistToTarget;
    }


    public static Vector2 RotateVector2( Vector2 v, float angle )
    {
        return Quaternion.AngleAxis(angle, Vector3.forward) * v;
    }


    static bool NotTooClose( Vector2 distanceToPlayer, float minDistToPlayer )
    {
        return distanceToPlayer.magnitude > minDistToPlayer;
    }


    static bool NotTooFarAway( Vector2 distanceToPlayer, float maxDistToPlayer )
    {
        return distanceToPlayer.magnitude < maxDistToPlayer;
    }


    public static bool AroundPlayer( Vector2 distanceToPlayer, float maxDistToPlayer, float minDistToPlayer )
    {
        return NotTooFarAway( distanceToPlayer, maxDistToPlayer ) && NotTooClose( distanceToPlayer, minDistToPlayer );
    }


    public static Vector2 GetPlayerPredictiveTarget( Vector2 targetPosition, Vector2 targetVelocity, Vector2 shootPosition, float projectileSpeed )
    {
        var dist = ( targetPosition - shootPosition ).magnitude;
        var time = dist / projectileSpeed;

        return targetPosition + ( targetVelocity * time );
    }


    public static Vector2 GetPerpendicular( Vector2 vector )
    {
        Vector2 result;
        result.x = vector.y;
        result.y = - vector.x;

        return result;
    }
}