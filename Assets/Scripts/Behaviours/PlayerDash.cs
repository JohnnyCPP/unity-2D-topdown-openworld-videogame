using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : AIBehaviour
{
    public static Dash instance;
    Rigidbody2D rb;
    new Transform transform;

    public bool dashing;

    IEnumerator MakeDash(float distance, float maxSpeed, float acceleration)
    {
        float traveledDistance = 0.0f;
        var initPosition = transform.position;
        float initSpeed = rb.velocity.magnitude;
        if (initSpeed == 0)
             rb.velocity = PlayerController.instance.FacingDirection() * PlayerController.instance.speed;
        

        dashing = true;

        while (traveledDistance < distance)
        {
            var currentSpeed = rb.velocity.magnitude;
            var newSpeed = Mathf.Min(maxSpeed, currentSpeed + acceleration * Time.deltaTime);

            rb.velocity = rb.velocity.normalized * newSpeed;

            var actualPosition = transform.position;
            traveledDistance = (actualPosition - initPosition).magnitude;
            yield return null;
        }

        while (rb.velocity.magnitude > initSpeed)
        {
            var currentSpeed = rb.velocity.magnitude;
            var newSpeed = Mathf.Max(0f, currentSpeed - acceleration * 2f * Time.deltaTime);

            rb.velocity = rb.velocity.normalized * newSpeed;
            yield return null;
        }

        dashing = false;
    }

    public override void InitBehaviourData()
    {
        rb = GetComponent<Rigidbody2D>();
        transform = GetComponent<Transform>();
        
    }

    public override void StartBehaviour()
    {
        float playerSpeed = PlayerController.instance.speed;
        float distance = playerSpeed / 4;
        float maxDashSpeed = playerSpeed * 6;
        float acceleration = maxDashSpeed * 4;

        StartCoroutine(MakeDash(distance, maxDashSpeed, acceleration));
    }

    public override void StopBehaviour()
    {
        
    }

    public override void UpdateBehaviour()
    {
        
    }
}
