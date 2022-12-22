using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileMovement : MonoBehaviour
{
    public float lifeSpan;
    void Start()
    {
    }

    
    // Update is called once per frame
    void Update()
    {
        if (lifeSpan <= 0f)
            Destroy(gameObject);

        lifeSpan -= Time.deltaTime;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
       Destroy(gameObject);
        if (collision.collider.tag == "Player") {
            Shake.instance.ShakeIt();
        }
       
    }
}
