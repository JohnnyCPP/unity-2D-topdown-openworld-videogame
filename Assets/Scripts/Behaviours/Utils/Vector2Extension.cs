using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extension
{
    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        //Vector2 diference = vec2 - vec1;
        //float sign = (vec2.y < vec1.y) ? -1.0f : 1.0f;
        //return Vector2.Angle(Vector2.right, diference) * sign;

        Vector2 vec1Rotated90 = new Vector2(-vec1.y, vec1.x);
        float sign = (Vector2.Dot(vec1Rotated90, vec2) < 0) ? -1.0f : 1.0f;
        return Vector2.Angle(vec1, vec2) * sign;
    }
}