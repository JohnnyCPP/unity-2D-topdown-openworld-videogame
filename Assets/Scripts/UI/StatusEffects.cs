using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    public StatusBar statusBar;

    public void PoisonPlayer(float poison, float duration, float slow)
    {
        StopAllCoroutines();

        StartCoroutine(PosionPlayer(poison, duration, slow));
    }

    IEnumerator PosionPlayer(float poison, float duration, float slow)
    {
        float timer = 0.0f;

        AILifeSystem lifeSystem= PlayerController.instance.lifeSystem;
        Rigidbody2D playerRb = PlayerController.instance.rigidBody2D;

        statusBar.SetDuration(duration);

        while (timer < duration)
        {
            playerRb.velocity *= slow;
            lifeSystem.TakeDamage(poison * Time.deltaTime);

            statusBar.UpdateTime();

            timer += Time.deltaTime;

            yield return null;
        }
    }
}
