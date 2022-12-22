using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraMovement : MonoBehaviour
{   // PUBLIC ATTRIBUTES
    public Vector2 downLeftMapPosition;
    public Vector2 upRightMapPosition;
    // PRIVATE ATTRIBUTES
    float speedScale = 10;
    float maxVerticalDistanceToPlayer = 1.5f;
    float maxHorizontalDistanceToPlayer = 3f;
    Transform playerTransform;
    Rigidbody2D rigidBody2D;
    Vector2 cameraToPlayer;


    void DrawRedDiagonalLine()
    {
        var upLeft = (Vector2) playerTransform.transform.position + Vector2.left * maxHorizontalDistanceToPlayer + Vector2.up * maxVerticalDistanceToPlayer;
        var rightDown = (Vector2) playerTransform.transform.position + Vector2.right * maxHorizontalDistanceToPlayer + Vector2.down * maxVerticalDistanceToPlayer;

        Debug.DrawLine( upLeft, rightDown, Color.red );
    }


    Vector2 LimitPositionInsideMap( Vector2 position )
    {
        position.x = Mathf.Clamp( position.x, downLeftMapPosition.x, upRightMapPosition.x );
        position.y = Mathf.Clamp( position.y, downLeftMapPosition.y, upRightMapPosition.y );
        return position;
    }


    void FollowAroundPlayer( Vector2 cameraToPlayer )
    {
        Vector2 newPosition = transform.position;

        if ( cameraToPlayer.y > maxVerticalDistanceToPlayer ) newPosition.y = playerTransform.position.y - maxVerticalDistanceToPlayer;
        if ( cameraToPlayer.x > maxHorizontalDistanceToPlayer ) newPosition.x = playerTransform.position.x - maxHorizontalDistanceToPlayer;
        if ( cameraToPlayer.y < - maxVerticalDistanceToPlayer ) newPosition.y = playerTransform.position.y + maxVerticalDistanceToPlayer;
        if ( cameraToPlayer.x < - maxHorizontalDistanceToPlayer ) newPosition.x = playerTransform.position.x + maxHorizontalDistanceToPlayer;

        transform.position = LimitPositionInsideMap( newPosition );
    }


    // USES "speedScale" TO FOLLOW WHERE THE PLAYER IS. CURRENTLY UNUSED. IT'S HERE FOR DIDACTIC PURPOSES.
    void FollowPlayer( Vector2 cameraToPlayer )
    {
        rigidBody2D.velocity = cameraToPlayer * speedScale * Time.deltaTime * 100;
    }

    
    void Update()
    {
        if ( playerTransform == null && PlayerController.instance != null ) playerTransform = PlayerController.instance.transform;

        if (Time.timeScale != 0)
        {
            cameraToPlayer = playerTransform.position - transform.position;

            // FOLLOWS THE PLAYER LEAVING SOME DISTANCE RELATIVE TO THE LIMITS DEFINED.
            FollowAroundPlayer(cameraToPlayer);

            // DRAWS A LINE FROM THE POSITIONS THAT THE CAMERA FOLLOWS THE PLAYER. FOR DEBUGGING PURPOSES.
            DrawRedDiagonalLine();
        }
    }


    void Start()
    {
        rigidBody2D = GetComponent<Rigidbody2D>();
    }
}