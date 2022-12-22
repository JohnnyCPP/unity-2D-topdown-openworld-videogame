using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shake : MonoBehaviour
{
    public static Shake instance;
    new public Transform camera;


    private void Awake()
    {
        instance = this;
    }


    IEnumerator ShakeCamera(float duration, float magnitude)
    {
        float elapsedTime = 0.0f;
        Vector3 originalPosition = camera.transform.localPosition;


        while (elapsedTime < duration)
        {
            if(Time.timeScale != 0)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                camera.transform.localPosition = new Vector3(x, y, originalPosition.z);
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }


        camera.transform.localPosition = originalPosition;
    }


    public void ShakeIt()
    {
        StartCoroutine(ShakeCamera(0.1f, 0.5f));
    }
}