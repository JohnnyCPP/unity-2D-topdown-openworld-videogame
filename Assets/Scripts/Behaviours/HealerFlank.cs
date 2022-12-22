using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class HealerFlank : AIBehaviour
{   // INITIAL DATA
    AstarPath pathController;
    Rigidbody2D rb;
    Pathfinding.Seeker seeker;
    // COROUTINES
    Coroutine seekPathRoutine;
    // PUBLIC ATTRIBUTES
    public float speed = 3f;
    public float maxDistToEnemy = 8f;
    public float minDistToPlayer = 4f;
    Transform enemyTransform;
    public Vector2 distanceToEnemy;
    public Vector2 distanceToPlayer;
    // PRIVATE ATTRIBUTES
    public bool lookingAPath = false;
    public bool followingPath;
    float timeFollowingPathCount;
    Vector2 lastPathNodePos;
    public bool hasEnemiesAround = false;

    Vector2 GetPositionToApproachEnemy()
    {
        float offset = 1f;
        float maxOffset = maxDistToEnemy;

        Vector2 target;
        Vector2 target2;
        Vector2 playerToTarget;
        Vector2 playerToTarget2;

        bool cast1 = true;
        bool cast2 = true;

        do
        {
            target = -distanceToEnemy.normalized * offset + (Vector2)enemyTransform.position;
            target2 = target;
            playerToTarget = target - (Vector2)enemyTransform.position;
            playerToTarget2 = playerToTarget;

            int it = 0;

            while (cast1 && cast2)
            {
                cast1 = Physics2D.CircleCast(target, GetComponent<CircleCollider2D>().radius, Vector2.right, LayerMask.GetMask("Obstacle"));
                cast2 = Physics2D.CircleCast(target2, GetComponent<CircleCollider2D>().radius, Vector2.right, LayerMask.GetMask("Obstacle"));

                if (it++ > 180 / 15) break;

                playerToTarget = PositionUtils.RotateVector2(playerToTarget, 15f);
                target = (Vector2)enemyTransform.position + playerToTarget;

                playerToTarget2 = PositionUtils.RotateVector2(playerToTarget2, -15f);
                target2 = (Vector2)enemyTransform.position + playerToTarget2;
            }

            offset *= 1.1f;

        } while (cast1 && cast2 && offset < maxOffset);

        if (cast1 && !cast2) return target2;

        if (!cast1 && cast2) return target;

        return Vector2.Distance(transform.position, target) < Vector2.Distance(transform.position, target2) ? target : target2;
    }


    Vector2 GetPositionToAvoidPlayer()
    {
        float offset = 1f;

        Vector2 target;
        Vector2 target2;

        Vector2 aiToTarget;
        Vector2 aiToTarget2;

        bool cast1 = true;
        bool cast2 = true;

        do
        {
            target = -distanceToPlayer.normalized * (maxDistToEnemy + offset) + (Vector2)PlayerController.instance.transform.position;
            target2 = target;

            aiToTarget = target - (Vector2)transform.position;
            aiToTarget2 = aiToTarget;

            int it = 0;

            while (cast1 && cast2)
            {
                cast1 = Physics2D.OverlapCircle(target, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Obstacle"));
                cast2 = Physics2D.OverlapCircle(target2, GetComponent<CircleCollider2D>().radius, LayerMask.GetMask("Obstacle"));

                if (it++ > 180 / 15) break;

                aiToTarget = PositionUtils.RotateVector2(aiToTarget, 15f);
                target = (Vector2)transform.position + aiToTarget;

                aiToTarget2 = PositionUtils.RotateVector2(aiToTarget2, -15f);
                target2 = (Vector2)transform.position + aiToTarget2;
            }

            Vector2 playerPosition = PlayerController.instance.transform.position;

            var distanceTargetToPlayer = Vector2.Distance(playerPosition, target);
            var distanceTarget2ToPlayer = Vector2.Distance(playerPosition, target2);

            if (distanceTargetToPlayer < minDistToPlayer || distanceTarget2ToPlayer < minDistToPlayer)
            {
                cast1 = true;
                cast2 = true;
            }

            offset *= 1.1f;

        } while (cast1 && cast2);

        if (cast1 && !cast2) return target2;

        if (!cast1 && cast2) return target;

        return Vector2.Distance(transform.position, target) < Vector2.Distance(transform.position, target2) ? target : target2;
    }


    void StartPath(Vector2 start, Vector2 end, Pathfinding.OnPathDelegate callback)
    {
        lookingAPath = true;

        seeker.StartPath(start, end, (Pathfinding.Path path) => {

            lookingAPath = false;

            callback(path);
        });
    }


    public void UpdateDistances()
    {
        if (hasEnemiesAround) {
            distanceToEnemy = enemyTransform.position - transform.position;
        }
        

        distanceToPlayer = (Vector2) (PlayerController.instance.transform.position - transform.position);
    }

    public bool onRange()
    {
        bool aroundPlayer = PositionUtils.AroundPlayer(distanceToPlayer, 100, minDistToPlayer);
        if (hasEnemiesAround) {
            bool aroundEnemy = PositionUtils.AroundPlayer(distanceToEnemy, maxDistToEnemy, 0);
            return aroundEnemy && aroundPlayer;
        }
        return aroundPlayer;
    }


    public void FollowPlayerLogicUpdate()
    {
        UpdateDistances();

        float distanceToPlayer = Vector2.Distance(lastPathNodePos, PlayerController.instance.transform.position);

        Vector2 target;

        if (followingPath && timeFollowingPathCount >= 3f && !PositionUtils.PositionIsAroundTarget(distanceToPlayer, 100f, minDistToPlayer))
        {
            try { StopCoroutine(seekPathRoutine); } catch (System.NullReferenceException) { }

            target = GetPositionToAvoidPlayer();
            StartPath(transform.position, target, OnPathFound);
        }
        else if (!lookingAPath && !followingPath && this.distanceToPlayer.magnitude < minDistToPlayer)
        {
            target = GetPositionToAvoidPlayer();
            Debug.DrawLine(transform.position, target, Color.red, 1f);
            StartPath(transform.position, target, OnPathFound);
        }
        else if (!lookingAPath && !followingPath && hasEnemiesAround)
        {
            target = GetPositionToApproachEnemy();
            StartPath(transform.position, target, OnPathFound);
        }
    }


    void StopSeekingPath()
    {
        if (rb.velocity != Vector2.zero)
        {
            rb.velocity = Vector2.zero;
            StopCoroutine(seekPathRoutine);
        }
    }


    void FinishFollowingPath()
    {
        followingPath = false;
        rb.velocity = Vector2.zero;
    }


    IEnumerator FollowPath(List<Vector3> path)
    {
        timeFollowingPathCount = 0f;

        const float minToReachNode = 0.1f;
        followingPath = true;
        rb.velocity = (path[0] - transform.position).normalized * speed;

        foreach (var node in path)
        {
            while (Vector2.Distance(transform.position, node) > minToReachNode)
            {
                timeFollowingPathCount += Time.deltaTime;
                Vector2 newVelocity = node - transform.position;
                newVelocity.Normalize();
                rb.velocity = newVelocity * speed;
                yield return null;
            }
        }

        FinishFollowingPath();
    }

    void UpdateClosestTransform()
    {
        Collider2D[] enemiesAround = Physics2D.OverlapCircleAll(transform.position, 10f, LayerMask.GetMask("Enemy"));
        if (enemiesAround.Length == 0)
        {
            hasEnemiesAround = false;
        }
        else
        {
            float closestDistance = enemiesAround.Min(e => (e.transform.position - transform.position).magnitude);
            Collider2D closestEnemy = enemiesAround.First(e => (e.transform.position - transform.position).magnitude == closestDistance);

            enemyTransform = closestEnemy.gameObject.GetComponent<Transform>();

            hasEnemiesAround = true;
        }

    }


    void OnPathFound(Pathfinding.Path path)
    {
        lastPathNodePos = path.vectorPath[path.vectorPath.Count - 1];

        if (seekPathRoutine != null) StopSeekingPath();

        seekPathRoutine = StartCoroutine(FollowPath(path.vectorPath));
    }


    public override void InitBehaviourData()
    {
        pathController = FindObjectOfType<AstarPath>();

        rb = GetComponent<Rigidbody2D>();

        seeker = GetComponent<Pathfinding.Seeker>();

        UpdateClosestTransform();
    }



    public override void StartBehaviour()
    {
        FollowPlayerLogicUpdate();
    }


    public override void StopBehaviour()
    {
        FinishFollowingPath();
        try { StopCoroutine(seekPathRoutine); } catch (System.NullReferenceException) { Debug.Log("Flank.cs: The coroutine was null"); };
    }


    public override void UpdateBehaviour()
    {
        UpdateDistances();

        if (!onRange())
            FollowPlayerLogicUpdate();

    }

    private void FixedUpdate()
    {
        if (enemyTransform == null)
            UpdateClosestTransform();
    }

}