using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileBehaviour : MonoBehaviour
{
    public float lifeSpan;
    [HideInInspector]
    internal string[] effects;
    [HideInInspector]
    public float damage;
    
    void Update()
    {
        if (lifeSpan <= 0f)
            Destroy(gameObject);

        lifeSpan -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            if(effects.Length != 0)
            {
                foreach (string effect in effects)
                {
                    switch (effect)
                    {
                        case "Toxic":
                            Poison p = GetComponent<Poison>();
                            PlayerController.instance.statusEffects.PoisonPlayer(p.poison, p.duration, p.slow);
                            break;
                    }
                }
            }

            Shake.instance.ShakeIt();
            PlayerController.instance.lifeSystem.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
