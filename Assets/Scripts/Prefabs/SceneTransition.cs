using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public Vector2 playerStartingPosition;
    public Vector2Value storedPlayerPosition;


    public void OnTriggerEnter2D( Collider2D triggeringObject )
    {
        if ( triggeringObject.CompareTag( "Player" ) && !triggeringObject.isTrigger )
        {
            storedPlayerPosition.startingPositionOnLoad = playerStartingPosition;
            SceneManager.LoadScene( sceneToLoad );
        }
    }
}