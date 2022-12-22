using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILifeSystem : MonoBehaviour
{
    public float maxHp = 1000f;
    public float hp;
    public int souls;

    public HealthBar healthBar;

    void Start()
    {
        hp = maxHp;        
        healthBar.SetMaxHealth(maxHp);
    }

    void Update()
    {
        if (hp <= 0) {
            Destroy(gameObject);
        }
    }

    public bool HasFullHp() {
        return hp == maxHp;
    }

    public void TakeDamage(float attackStrength) 
    {
        //Debug.Log("Me dieron");
        hp -= attackStrength;
        healthBar.SetHealth(hp);
    }

    public void GetHealed(float healPower) {
        hp = Mathf.Min(maxHp, hp + healPower);
        healthBar.SetHealth(hp);
        //Debug.Log("Me curaron");
    }

    private void OnDestroy()
    {
        SoulsController.instance.addSouls(souls);
    }
}
