using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : AIBehaviour
{   // INITIAL DATA
    // PUBLIC ATTRIBUTES
    public float shootReloadTime;
    public float animationTime;
    public float maximumAnimationTime;
    public GameObject projectile;
    public Vector2 distanceToPlayer;
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public string[] effects;
    // PRIVATE ATTRIBUTES
    Transform playerTransform;


    bool CanShotToPlayer()
    {
        Vector2 toPlayer = ( playerTransform.position - transform.position ).normalized;
        Vector2 perp = PositionUtils.GetPerpendicular( toPlayer ) * projectile.GetComponent<CircleCollider2D>().radius;

        bool firstCast = !Physics2D.Linecast( (Vector2) transform.position + perp, (Vector2) playerTransform.position + perp, LayerMask.GetMask( "Obstacle" ) );
        bool secondCast = !Physics2D.Linecast( (Vector2) transform.position - perp, (Vector2) playerTransform.position - perp, LayerMask.GetMask( "Obstacle" ) );

        return firstCast && secondCast;
    }


    public void ShootLogicUpdate()
    {
        UpdateDistanceToPlayer();

        if ( CanShotToPlayer() && shootReloadTime <= 0f)
        {
            
            if ( animationTime >= maximumAnimationTime )
            {
                var toPlayer = distanceToPlayer.normalized;

                var projectile = Instantiate(this.projectile, (Vector2)transform.position + toPlayer * 1f, Quaternion.identity);

                Vector3 playerPosition = playerTransform.position;
                Vector2 playerVelocity = playerTransform.GetComponent<Rigidbody2D>().velocity;
                Vector3 aiPosition = transform.position;

                float projectileSpeed = PlayerController.instance.speed * 2f;

                var target = PositionUtils.GetPlayerPredictiveTarget(playerPosition, playerVelocity, aiPosition, projectileSpeed);

                projectile.GetComponent<Rigidbody2D>().velocity = (target - (Vector2)transform.position).normalized * projectileSpeed;

                ProyectileBehaviour pb =  projectile.GetComponent<ProyectileBehaviour>();
                pb.damage = damage;
                pb.effects = effects;

                shootReloadTime = 3f;
                animationTime = 0f;
            }
            else animationTime += Time.deltaTime;
        }

        shootReloadTime -= Time.deltaTime;
    }


    public void UpdateDistanceToPlayer()
    {
        if (Time.timeScale != 0)
            distanceToPlayer = playerTransform.position - transform.position;
    }


    public override void InitBehaviourData()
    {      
        animationTime = 0f;
    }


    public override void StartBehaviour()
    {
        ShootLogicUpdate();
    }


    public override void StopBehaviour()
    {
        
    }


    public override void UpdateBehaviour()
    {

        if (Time.timeScale != 0)
            ShootLogicUpdate();
    }

    void Update()
    {
        if (playerTransform == null && PlayerController.instance != null) playerTransform = PlayerController.instance.transform;
    }


}
