using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PrintUtils
{


    public static void PrintPath( List<Vector3> path, Transform originObjectTransform )
    {
        #if UNITY_EDITOR
        Vector2 lastPos = originObjectTransform.position;

        foreach ( var node in path )
        {
            Vector2 current = new Vector2( node.x, node.y );
            Debug.DrawLine( lastPos, current, Color.green, 1f );
            lastPos = current;
        }
        #endif
    }
}