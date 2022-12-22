using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMeleeAttack : AIBehaviour
{
    public float damage;
    public float attackDistance;
    public float thrust = 10f;
    public override void InitBehaviourData()
    {

    }

    public override void StartBehaviour()
    {
        Rigidbody2D playerRb = PlayerController.instance.GetComponent<Rigidbody2D>();

        //StartCoroutine(knockback(playerRb));

        PlayerController.instance.GetComponent<AILifeSystem>().TakeDamage(damage);
    }

    private IEnumerator knockback(Rigidbody2D enemy)
    {
        Debug.DrawLine(transform.position, enemy.transform.position, Color.black, 1f);
        Vector2 forceDirection = enemy.transform.position - transform.position;
        Vector2 force = forceDirection.normalized * thrust;

        Debug.Log(force);

        enemy.velocity = force;

        yield return new WaitForSeconds(0.1f);


        enemy.velocity = Vector2.zero;

    }
    public override void StopBehaviour()
    {
        
    }

    public override void UpdateBehaviour()
    {
        
    }
}
