using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : AIBehaviour
{
    Transform playerTransform;
    Transform iaTransform;
    Rigidbody2D rb;
    public float offset;


    public void TeleportToPLayer()
    {
        rb.velocity = Vector3.zero;

        Vector3 target = Vector3.zero;

        bool cast;
        int i=0;

        do
        {
            var x = 0f;
            var y = 0f;

            var dice = Random.Range(0, 4);
            switch (dice)
            {
                case 0:
                    x = offset;
                    break;

                case 1:
                    x = offset;
                    break;

                case 2:
                    y = offset;
                    break;

                case 3:
                    y = offset;
                    break;
            }

            Vector3 playerPosition = playerTransform.position;

            target = new Vector2(x + playerPosition.x, y + playerPosition.y);

            Vector2 targetToPlayer = playerPosition - target;

            bool targetOutOfBounds = target.x >= 18 || target.x <= -18 || target.y >= 22 || target.y <= -22;
            cast = targetOutOfBounds || Physics2D.CircleCast(target, GetComponent<CircleCollider2D>().radius, targetToPlayer, targetToPlayer.magnitude,  LayerMask.GetMask("Obstacle"));

        } while (cast && i++<10);

        if(!cast)
            iaTransform.position = target;

        rb.velocity = Vector3.zero;
    }


    public override void InitBehaviourData()
    {
        iaTransform = gameObject.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    public override void StartBehaviour()
    {
        TeleportToPLayer();
    }

    public override void StopBehaviour()
    {

    }

    public override void UpdateBehaviour()
    {
        
    }

    void Update()
    {
        if (playerTransform == null && PlayerController.instance != null) playerTransform = PlayerController.instance.transform;
    }
}
