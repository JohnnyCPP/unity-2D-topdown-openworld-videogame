using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : AIBehaviour
{
    Transform playerTransform;
    public static Dash instance;
    Rigidbody2D rb;
    Coroutine dashRoutine;

    public float dashFactor;

    public bool dashing = false;

    IEnumerator MakeDash(Vector2 direction, float distance, float speed, float acceleration)
    {
        float traveledDistance = 0.0f;
        var initPosition = transform.position;

        rb.velocity = direction.normalized;

        dashing = true;
        while (traveledDistance < distance)
        {
            var currentSpeed = rb.velocity.magnitude;
            var newSpeed = Mathf.Min(speed, currentSpeed + acceleration * Time.deltaTime);

            rb.velocity = rb.velocity.normalized * newSpeed;

            var actualPosition = transform.position;
            traveledDistance = (actualPosition - initPosition).magnitude;

            yield return null;
        }

        while (rb.velocity.magnitude > 0f)
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
    }

    public override void StartBehaviour()
    {
        Vector2 toTarget = playerTransform.position - transform.position;
        dashRoutine =  StartCoroutine(MakeDash(toTarget, 4f, 12f*dashFactor, 16f*dashFactor));
    }

    public override void StopBehaviour()
    {
        try { StopCoroutine(dashRoutine); dashing = false; } catch (System.NullReferenceException) { Debug.Log("Dash.cs: The coroutine was null"); };
    }

    public override void UpdateBehaviour()
    {
        
    }

    void Update()
    {
        if (playerTransform == null && PlayerController.instance != null) playerTransform = PlayerController.instance.transform;
    }
}
