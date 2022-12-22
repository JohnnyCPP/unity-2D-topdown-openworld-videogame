using UnityEngine;


public class LoadInterior : MonoBehaviour
{
    

    void Start()
    {
        Debug.Log( "START EXECUTED" );
        transform.GetChild( 0 ).GetComponent<Animator>().SetFloat( "Vertical", 1 );
    }
}