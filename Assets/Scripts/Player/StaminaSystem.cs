using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{

    public static StaminaSystem instance;
    public float maxStamina = 100f;
    public float currentStamina;
    public float recovery = 5f;

    public float recoverCooldown = 1f;
    public float recoverTimer = 1f;

    [HideInInspector]
    public float staminaDebuff = 1f;

    public StaminaBar staminaBar;

    private void Start()
    {
        instance = this;

        currentStamina = maxStamina;
        staminaBar.SetMaxStamina(maxStamina);
    }

    private void Update()
    {
        if (PlayerController.instance.currentState == PlayerController.state.Normal && !PlayerController.instance.animator.GetBool("Attacking"))
        {
            if (recoverTimer >= recoverCooldown) 
            {
                currentStamina = Mathf.Min(maxStamina, currentStamina + recovery * Time.deltaTime);
                staminaBar.setStamina(currentStamina);
            }
            else
                recoverTimer = Mathf.Min(recoverCooldown, recoverTimer + Time.deltaTime);

        }
        else
            recoverTimer = 0f;

        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            PlayerController.instance.exhausted = true;
        }

        if (currentStamina == maxStamina)
            PlayerController.instance.exhausted = false;
    }

    public void loseStamina(float stamina) {
        currentStamina -= stamina;
        staminaBar.setStamina(currentStamina);
    }
}
