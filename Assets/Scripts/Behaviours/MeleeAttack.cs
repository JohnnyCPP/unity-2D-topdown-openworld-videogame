using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class MeleeAttack : MonoBehaviour
{
    Transform playerTransform;
    public static MeleeAttack instance;
    public float attack = 100;
    public float attackDistance = 1f;
    PlayerController playerController;
    Vector2 attackDir;
    public float thrust;

    void Start()
    {
        instance = this;
        playerTransform = GetComponent<Transform>();
        playerController = GetComponent<PlayerController>();
        attackDir = Vector2.zero;
    }

    public void Attack()
    {
        Vector2 playerPosition = playerTransform.position;

        Vector2 newAttackDir = playerController.GetPlayerDirection();
        attackDir = newAttackDir== Vector2.zero? playerController.FacingDirection(): newAttackDir;

        Debug.DrawLine(playerPosition, playerPosition+attackDir * attackDistance, Color.green, 1f);
        float maxAngle = 45f;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(playerPosition, attackDistance, LayerMask.GetMask("Enemy"));
        enemies = enemies.Where(c => !c.isTrigger).ToArray();
        if (enemies.Length != 0)
        {
            foreach (Collider2D enemy in enemies)
            {
                Vector2 enemyPosition = enemy.gameObject.GetComponent<Transform>().position;
                Vector2 playerToEnemy = enemyPosition - playerPosition;

                var angle = Vector2Extension.AngleBetweenVector2(attackDir, playerToEnemy.normalized);

                if (angle < maxAngle && angle > -maxAngle)
                {
                    enemy.GetComponent<AILifeSystem>().TakeDamage(attack);

                    enemy.gameObject.GetComponent<Flank>().StopBehaviour();
                    if (enemy.gameObject.tag == "Teleport")
                    {
                        enemy.gameObject.GetComponent<SkullAIController>().InterruptDash();
                    }

                    if (enemy.gameObject.activeInHierarchy)
                    {
                        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                        StartCoroutine(knockback(enemyRb));
                    }

                    FindObjectOfType<AudioManager>().Play("Stab");
                }
            }
        }
        else
            FindObjectOfType<AudioManager>().Play("Swing");
        
    }

    private IEnumerator knockback(Rigidbody2D enemy)
    {
        Vector2 forceDirection = enemy.transform.position - transform.position;
        Vector2 force = forceDirection.normalized * thrust;

        enemy.velocity += force;

        yield return new WaitForSeconds(0.1f);


        enemy.velocity = new Vector2();

    }
}